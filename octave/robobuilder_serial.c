#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <termios.h>
#include <stdio.h>
#include <pthread.h>

#include "robobuilder.h"
#include "macro.h"
#define MAX_wCK 31
#define PGM_P char*

#define	DBO(x) {if (dbg) {x}}
int dbg=0;

int fd = -1;

struct termios oldtio,newtio;

char devicename[20];

char *defdevice = "/dev/ttyUSB0";

int response[32];


static pthread_mutex_t cs_mutex = PTHREAD_MUTEX_INITIALIZER;

/* misc */
void delay_ms(int x)  					
{
	usleep(x*1000);
}

void setdevname(char *n) 
{ 
	strncpy(devicename,n,19); 
	defdevice=&devicename[0];
}

void initserial(int f)
{
	int br;

	DBO(printf ("ROBO: init i/o [%s]\n",((f==1)?"Fast":"Normal"));)
	switch(br)
	{
		case 0: br=115200; break; // default
		case 1: br=230400; break; // high speed
		case 2: br=9600;   break; // low speed (BT connections)
	}
	if (openport(defdevice, br)) 	// set up I/O
		return;

    	//check DCMP version

	wck_t *r = wckReadPos(30,0);
	printf ("DCMP v=%d.%d\n", r->b1, r->b2);

	if (!((r->b1)==3 && (r->b2)>10 ))
	{
		printf ("Not connected \n");
		closeport();
	}
	free(r);
}

int openport(char *device, int baudrate)
{
	long Baud = B115200;

	fd = open(device, O_RDWR | O_NOCTTY | O_NDELAY);
	if(fd == -1) {
	  printf( "failed to open port %s %d\n", device, baudrate );
          return -1;
	}

     	switch(baudrate)
	{
        case 9600:   Baud=B9600;   break;
        case 115200: Baud=B115200; break;
        case 230400: Baud=B230400; break;
	}

	tcgetattr(fd,&oldtio); /* save current serial port settings */
	bzero(&newtio, sizeof(newtio)); /* clear struct for new port settings */

	newtio.c_cflag = Baud | CS8  | CLOCAL ; //| CREAD;
	newtio.c_oflag = 0;
	newtio.c_lflag = 0;
	newtio.c_iflag = IGNPAR;

	newtio.c_cc[VTIME]    = 0;   /* inter-character timer unused */
	newtio.c_cc[VMIN]     = 0;   /* blocking read until 5 chars received */

	tcflush(fd, TCIFLUSH);
	tcsetattr(fd,TCSANOW,&newtio);
        return 0;
}

void closeport()
{
	if(fd == -1) {
	  //printf( "open port failed\n" );
	  return;
	}

	/* restore the old port settings */
	tcsetattr(fd,TCSANOW,&oldtio);
	close(fd);
}

void writebyte(int b)
{
	char buf[1];
	if(fd == -1) {
	  printf( "write: open port failed [%d]\n",b );
	  return;
	}

	buf[0] = b;
	write(fd, buf, 1);
}

int readbyte()
{
	int buf[1];
	if(fd == -1) {
	  printf( "read: open port failed\n" );
	  return -1;
	}

	int cnt=0;

	while (1)
	{
		int b = read(fd, buf, 1);
		if (b>0)
			return buf[0]&0xFF;

		if (b<0)
			return -1;

		if (cnt++>10000) // timeout
			return -1;
	}
}


/**************************************************************************************/



/**************************************************************************************/





/* wck commands */
void wckPosSend(unsigned char ServoID, char Torque, unsigned char Position)
{
	DBO(printf ("ROBO: Servo Send %d [%d] -> %d\n", ServoID, Torque, Position);)
	wckMovePos(ServoID, Position, Torque);
}

int  wckPosRead(char ServoID)
{
	int n;
	wck_t *p = wckReadPos(ServoID, 0);

	n = p->b2;
	free(p);

	DBO(printf ("ROBO: Servo Read %d=%d\n", ServoID, n); )
	return n;
}

void wckSetPassive(char ServoID)
{
	DBO(printf ("ROBO: Servo Passive %d\n", ServoID); )
	wckPassive(ServoID);
}

void wckSyncPosSend(char LastID, char SpeedLevel, char *TargetArray, char Index)
{
	char CheckSum;
	int i=0;
	DBO(printf ("ROBO: Servo Synch Send  %d [%d]\n", LastID, SpeedLevel);)

    pthread_mutex_lock( &cs_mutex );

	CheckSum = 0;
	writebyte(0xFF);
	writebyte((SpeedLevel<<5)|0x1f);
	writebyte(LastID+1);
	while(1)
	{
		if(i>LastID)
			break;
		writebyte(TargetArray[Index*(LastID+1)+i]);
		CheckSum = CheckSum ^ TargetArray[Index*(LastID+1)+i];
		i++;
	}
	CheckSum = CheckSum & 0x7f;
	writebyte(CheckSum);

	pthread_mutex_unlock( &cs_mutex );
}

void wckSendSetCommand(char Data1, char Data2, char Data3, char Data4)
{
	char CheckSum;
	CheckSum = (Data1^Data2^Data3^Data4)&0x7f;
	writebyte(0xFF);
	writebyte(Data1);
	writebyte(Data2);
	writebyte(Data3);
	writebyte(Data4);
	writebyte(CheckSum);
}

void wckWriteIO(unsigned char ServoID, unsigned char IO)
{
	DBO(printf ("ROBO: Servo write IO %d=%d\n", ServoID, IO); )

    pthread_mutex_lock( &cs_mutex );

	wckSendSetCommand((7<<5)|ServoID, 0x64, IO, IO);
	response[0] = readbyte();
	response[1] = readbyte();

	pthread_mutex_unlock( &cs_mutex );

	return;
}


void  I2C_read    (int addr, int ocnt, BYTE * outbuff, int icnt, BYTE * inbuff)
{
	printf ("ROBO: I2C read %d\n", addr);
	//tbc
	//wckReadPos(30,13) + addr + nbytes +[bytes]
	//response cnt [buf]
	//
}

int   I2C_write   (int addr, int ocnt, BYTE * outbuff)
{
	printf ("ROBO: I2C write %d\n", addr);
	//tbc
	//wckReadPos(30,14); + addr + nbytes +[bytes]
	//response (b1 & b2)
	return 0;
}

//i.e. 10,33,50,66,90
void  blights(lights_t l)
{
	wck_t *w;
	DBO(printf ("ROBO: Lights %d [%d,%d,%d,%d,%d]\n",l.value, l.range[0], l.range[1], l.range[2], l.range[3], l.range[4]);)
	BYTE a=0x20,b=0x30;

	if (l.value > l.range[0])   b |= 1;
	if (l.value > l.range[1])   a |= 2;
	if (l.value > l.range[2])   a |= 1;
	if (l.value > l.range[3])   a |= 4;
	if (l.value > l.range[4])   a |= 8;

	w = wckReadPos(30,9); //set mic sampling off
	free(w);

	w = wckReadPos(30,a); //set lights 1-4
	free(w);

	w= wckReadPos(30,b); //set lights 5-8
	free(w);
}

WORD send_hex_str(char *bus_str, int n)
{
		BYTE b1,b2;
		WORD r=0;
		char *eos = bus_str+n;

		while  ((bus_str<eos) && *bus_str != 0)
		{
			b1=0;
			if ((*bus_str>='0') && (*bus_str<='9')) b1 = *bus_str-'0' ;
			if ((*bus_str>='A') && (*bus_str<='Z')) b1 = *bus_str-'A' + 10 ;
			bus_str++;
			b1 <<=4;
			if ((*bus_str>='0') && (*bus_str<='9')) b1 += *bus_str-'0' ;
			if ((*bus_str>='A') && (*bus_str<='Z')) b1 += *bus_str-'A' + 10 ;
			bus_str++;
			DBO(printf ("ROBO: SEND WCK HEX %x\n",b1);)
			writebyte(b1);
		}

		b1=readbyte();
		b2=readbyte();
		r = ((0xFF&b1)<<8) | (0xFF&b2);

		return r;
}



/**************************************************************************************

  DCMP commands

 **************************************************************************************/

   int CB2I(int x) {if (x<128) return x; else return x-256;}


   result_t *readXYZ()
   {
	result_t *v = (result_t *)malloc(3*sizeof(result_t));

	wck_t *w = wckReadPos(30,1);
	v->x = CB2I(w->b1);
	v->z = CB2I(w->b2);
    	free(w);
	w = wckReadPos(30,2);
	v->y = CB2I(w->b1);
	free(w);
	return v;
   }

   int readPSD()
   {
	   int n;
	   wck_t *w = wckReadPos(30,5);
           n=CB2I(w->b1);
	   return n;
   }

   int readIR()
   {
	   int t;
	   wck_t *w = wckReadPos(30,7);
	   t = CB2I(w->b1);
	   //t = CB2I(w->b2);
	   //PINA = t;
	   free(t);

	   return t;
   }


   wck_t *wckReadPos(int id, int d1)
   {
	   wck_t *v = (wck_t *)malloc(sizeof(wck_t));

	   pthread_mutex_lock( &cs_mutex );

	   writebyte(0xFF);
	   id = (5 << 5 | (id % 31));
	   writebyte(id);
	   writebyte(d1);
	   writebyte((id ^ d1) &0x7f);


	   v->b1 = readbyte();
	   v->b2 = readbyte();

	   pthread_mutex_unlock( &cs_mutex );
	   return v;
   }

   wck_t *wckPassive(int id)
   {
	   wck_t *v = (wck_t *)malloc(sizeof(wck_t));
	   pthread_mutex_lock( &cs_mutex );

	   writebyte(0xFF);
	   id = (6 << 5 | (id % 31));
	   writebyte(id);
	   writebyte(0x10);
	   writebyte((id ^ 0x10) &0x7f);

	   v->b1 = readbyte();
	   v->b2 = readbyte();

	   pthread_mutex_unlock( &cs_mutex );
	   return v;
   }

   wck_t *wckMovePos(int id, int pos, int torq)
   {
	   wck_t *v = (wck_t *)malloc(sizeof(wck_t));
	   pthread_mutex_lock( &cs_mutex );

	   writebyte(0xFF);
	   id  = ((torq % 5) << 5) | (id % 31);
	   pos = pos % 254;
	   writebyte (id);
	   writebyte (pos);
	   writebyte ((id ^ pos) & 0x7f);

	   v->b1 = readbyte();
	   v->b2 = readbyte();

	   pthread_mutex_unlock( &cs_mutex );
	   return v;
   }

	void SyncPosSend(int SpeedLevel, servos_t s)
	{
		int i = 0;
		int CheckSum = 0;
   		int LastID=(s.nos)-1;
		char *TargetArray= &(s.values[0]);
		int Index=0;

		pthread_mutex_lock( &cs_mutex );

		writebyte(0xFF);
	    	writebyte((SpeedLevel << 5) | 0x1f);
	    	writebyte(LastID + 1);

		while (1)
		{
			if (i > LastID) break;

			printf("%02x ", TargetArray[Index * (LastID + 1) + i]);

			writebyte(TargetArray[Index * (LastID + 1) + i]);
			CheckSum ^= TargetArray[Index * (LastID + 1) + i];
			i++;
		}
		writebyte(CheckSum & 0x7f);
			
		printf("\n");

		pthread_mutex_unlock( &cs_mutex );

		return;
	}


	BYTE cpos[32];
	int offset[32];
	BYTE nos=0;


	servos_t *readservos(int n)
	{
		BYTE i;

	   	servos_t *v = (servos_t *)malloc(sizeof(servos_t));

	        if (n==0) n=31;
		for (i=0; i<n; i++)
		{
			int p = wckPosRead(i);
			if (p<0 || p>255) break;
			v->values[i]=cpos[i]=p;
		}
		nos=v->nos=i;
		return v;
	}

	enum { AccelDecel=0, Accel, Decel, Linear };

	int PP_mtype=Linear;

	double CalculatePos_Accel(int Distance, double FractionOfMove)
	{
		return FractionOfMove * (Distance * FractionOfMove);
	}

	double CalculatePos_Decel(int Distance, double FractionOfMove)
	{
		FractionOfMove = 1 - FractionOfMove;
		return Distance - (FractionOfMove * (Distance * FractionOfMove));
	}

	double CalculatePos_Linear(int Distance, double FractionOfMove)
	{
		return (Distance * FractionOfMove);
	}

	double CalculatePos_AccelDecel(int Distance, double FractionOfMove)
	{
		if ( FractionOfMove < 0.5 )     // Accel:
			return CalculatePos_Accel(Distance /2, FractionOfMove * 2);
		else if (FractionOfMove > 0.5 ) //'Decel:
			return CalculatePos_Decel(Distance/2, (FractionOfMove - 0.5) * 2) + (Distance * 0.5);
		else                            //'= .5! Exact Middle.
			return Distance / 2;
	}

	double GetMoveValue(int mt, int StartPos, int EndPos, double FractionOfMove)
	{
		int Offset,Distance;
		if (StartPos > EndPos)
		{
			Distance = StartPos - EndPos;
			Offset = EndPos;
			switch (mt)
			{
				case Accel:
					return Distance - CalculatePos_Accel(Distance, FractionOfMove) + Offset;
				case AccelDecel:
					return Distance - CalculatePos_AccelDecel(Distance, FractionOfMove) + Offset;
				case Decel:
					return Distance - CalculatePos_Decel(Distance, FractionOfMove) + Offset;
				case Linear:
					return Distance - CalculatePos_Linear(Distance, FractionOfMove) + Offset;
			}
		}
		else
		{
			Distance = EndPos - StartPos;
			Offset = StartPos;
			switch (mt)
			{
				case Accel:
					return CalculatePos_Accel(Distance, FractionOfMove) + Offset;
				case AccelDecel:
					return CalculatePos_AccelDecel(Distance, FractionOfMove) + Offset;
				case Decel:
					return CalculatePos_Decel(Distance, FractionOfMove) + Offset;
				case Linear:
					return CalculatePos_Linear(Distance, FractionOfMove) + Offset;
			}
		}
		return 0.0;
	}

	void PlayPoseA(int d, int f, int tq, int flag, servos_t s)
	{
		BYTE *spod;

		spod=&(s.values[0]);
		PlayPose(d, f, tq, spod, flag);
	}

	void PlayPose(int d, int f, int tq, BYTE spod[], int flag)
	// Play d ms per step, f frames, from current -> spod
	{
		int i;


		DBO(printf ("ROBO: Playpose  [d=%d , f=%d]\n", d,f);)

		if (flag!=0)
		{
			readservos(0);	// set nos and reads cpos
			nos=flag;
		}

		int dur=d/f;
		if (dur<25) dur=25; //25ms is quickest

		for (i=0; i<f; i++)
		{
			BYTE temp[nos];
			int j;
			for (j=0; j<nos; j++)
			{
				//temp[j] = cpos[j] + (float)((i)*intervals[j]+0.5);
				temp[j] = (BYTE)GetMoveValue(PP_mtype, cpos[j], spod[j], (double)i / (double)f);

				DBO(printf ("[%d] ", temp[j]);)

			}
			wckSyncPosSend(nos-1, tq, temp, 0);
			delay_ms(dur);

			DBO(printf ("\n");)
		}

		for (i=0; i<nos; i++)
		{
			cpos[i]=spod[i];
		}

		wckSyncPosSend(nos-1, tq, cpos, 0);
		delay_ms(dur);
	}

	const BYTE basic18[] = { 143, 179, 198, 83, 106, 106, 69, 48, 167, 141, 47, 47, 49, 199, 192, 204, 122, 125};
	const BYTE basic16[] = { 125, 179, 199, 88, 108, 126, 72, 49, 163, 141, 51, 47, 49, 199, 205, 205 };
	const BYTE basicdh[] = { 143, 179, 198, 83, 105, 106, 68, 46, 167, 140, 77, 70, 152, 165, 181, 98, 120, 124, 99};

	int dm=0;
	void setdh(int n) {dm=n;}
	void standup (int n)
	{
		if (n<18)
			PlayPose(1000, 10, 4, basic16, 16); //huno basic
		else
		{
			if (dm)
			    PlayPose(1000, 10, 4, basicdh, 18); //huno with hip
			else
			    PlayPose(1000, 10, 4, basic18, 18); //huno with hip
		}
	}

	/**********************************************************************************/

	// Data structure for the Motion Data-------------------------------------------------------------
	//      - hierarchy is : wCK < Frame < Scene < Motion < Action
	//      - data is sent to the wCK for each frame on the timer
	//      - frames are created from scenes by interpolation
	//      - a motion comprises a sequence of scenes
	//      - an action can invoke motions

	struct TwCK_in_Motion{      // Structure for each wCK in a motion
		BYTE	Exist;			// 1 if wCK exists
		BYTE	RPgain;			// Runtime P setting
		BYTE	RDgain;			// Runtime D setting
		BYTE	RIgain;			// Runtime I setting
		BYTE	PortEn;			// (0 = disable, 1 = enable)
		BYTE	InitPos;		// Initial wCK position (apparently ignored)
	};

	struct TwCK_in_Scene{		// Structure for each wCK in a scene
		BYTE	Exist;			// 1 if wCK exists
		BYTE	SPos;			// wCK starting position
		BYTE	DPos;			// wCK destination position
		BYTE	Torq;			// Torque
		BYTE	ExPortD;		// External Port Data(1~3)
	};

	struct TMotion{			    // Structure for a motion
		BYTE	PF;				//  ? (not used)
		BYTE	RIdx;			//  ? (not used)
		DWORD	AIdx;			//  ? (not used)
		WORD	NumOfScene;		// number of scenes in motion
		WORD	NumOfwCK;		// number of wCK included
		struct	TwCK_in_Motion  wCK[MAX_wCK];	// Motion data for each wCK
		WORD	FileSize;		// overall file size
	}Motion;

	struct TScene{			    // Structure for a scene
		WORD	Idx;			// index of scene (0~65535)
		WORD	NumOfFrame;		// number of frames in scene
		WORD	RTime;			// running time of scene[msec]
		struct	TwCK_in_Scene   wCK[MAX_wCK];	// scene data for each wCK
	}Scene;

	struct FlashMotionData {
		PGM_P TT;
		PGM_P ET;
		PGM_P PT;
		PGM_P DT;
		PGM_P IT;
		PGM_P FT;
		PGM_P RT;
		PGM_P PoT;
		int NoS;
		int Now;
	};

#define flash
#include "HunoBasic.h"
#include "e-motion.h"

	struct FlashMotionData mlist[] =
	{
		{ // 0. PunchLeft
			(PGM_P) HunoBasic_PunchLeft_Torque,
			(PGM_P) HunoBasic_PunchLeft_Port,
			(PGM_P) HunoBasic_PunchLeft_PGain,
			(PGM_P) HunoBasic_PunchLeft_DGain,
			(PGM_P) HunoBasic_PunchLeft_IGain,
			(PGM_P) HunoBasic_PunchLeft_Frames,
			(PGM_P) HunoBasic_PunchLeft_TrTime,
			(PGM_P) HunoBasic_PunchLeft_Position,
			HUNOBASIC_PUNCHLEFT_NUM_SCENES,
			HUNOBASIC_PUNCHLEFT_NUM_MOTORS
		},
		{ // 1. PunchRight
			(PGM_P) HunoBasic_PunchRight_Torque,
			(PGM_P) HunoBasic_PunchRight_Port,
			(PGM_P) HunoBasic_PunchRight_PGain,
			(PGM_P) HunoBasic_PunchRight_DGain,
			(PGM_P) HunoBasic_PunchRight_IGain,
			(PGM_P) HunoBasic_PunchRight_Frames,
			(PGM_P) HunoBasic_PunchRight_TrTime,
			(PGM_P) HunoBasic_PunchRight_Position,
			HUNOBASIC_PUNCHRIGHT_NUM_SCENES,
			HUNOBASIC_PUNCHRIGHT_NUM_MOTORS
		},
		// 2. SidewalkLeft
		{
			(PGM_P) HunoBasic_SidewalkLeft_Torque,
			(PGM_P) HunoBasic_SidewalkLeft_Port,
			(PGM_P) HunoBasic_SidewalkLeft_PGain,
			(PGM_P) HunoBasic_SidewalkLeft_DGain,
			(PGM_P) HunoBasic_SidewalkLeft_IGain,
			(PGM_P) HunoBasic_SidewalkLeft_Frames,
			(PGM_P) HunoBasic_SidewalkLeft_TrTime,
			(PGM_P) HunoBasic_SidewalkLeft_Position,
			HUNOBASIC_SIDEWALKLEFT_NUM_SCENES,
			HUNOBASIC_SIDEWALKLEFT_NUM_MOTORS
		},
		// 3. SidewalkRight
		{
			(PGM_P) HunoBasic_SidewalkRight_Torque,
			(PGM_P) HunoBasic_SidewalkRight_Port,
			(PGM_P) HunoBasic_SidewalkRight_PGain,
			(PGM_P) HunoBasic_SidewalkRight_DGain,
			(PGM_P) HunoBasic_SidewalkRight_IGain,
			(PGM_P) HunoBasic_SidewalkRight_Frames,
			(PGM_P) HunoBasic_SidewalkRight_TrTime,
			(PGM_P) HunoBasic_SidewalkRight_Position,
			HUNOBASIC_SIDEWALKRIGHT_NUM_SCENES,
			HUNOBASIC_SIDEWALKRIGHT_NUM_MOTORS
		},
		// 4. TurnLeft
		{
			(PGM_P) HunoBasic_TurnLeft_Torque,
			(PGM_P) HunoBasic_TurnLeft_Port,
			(PGM_P) HunoBasic_TurnLeft_PGain,
			(PGM_P) HunoBasic_TurnLeft_DGain,
			(PGM_P) HunoBasic_TurnLeft_IGain,
			(PGM_P) HunoBasic_TurnLeft_Frames,
			(PGM_P) HunoBasic_TurnLeft_TrTime,
			(PGM_P) HunoBasic_TurnLeft_Position,
			HUNOBASIC_TURNLEFT_NUM_SCENES,
			HUNOBASIC_TURNLEFT_NUM_MOTORS
		},
		// 5. TurnRight
		{
			(PGM_P) HunoBasic_TurnRight_Torque,
			(PGM_P) HunoBasic_TurnRight_Port,
			(PGM_P) HunoBasic_TurnRight_PGain,
			(PGM_P) HunoBasic_TurnRight_DGain,
			(PGM_P) HunoBasic_TurnRight_IGain,
			(PGM_P) HunoBasic_TurnRight_Frames,
			(PGM_P) HunoBasic_TurnRight_TrTime,
			(PGM_P) HunoBasic_TurnRight_Position,
			HUNOBASIC_TURNRIGHT_NUM_SCENES,
			HUNOBASIC_TURNRIGHT_NUM_MOTORS
		},
		// 6. GetupBack
		{
			(PGM_P) HunoBasic_GetupBack_Torque,
			(PGM_P) HunoBasic_GetupBack_Port,
			(PGM_P) HunoBasic_GetupBack_PGain,
			(PGM_P) HunoBasic_GetupBack_DGain,
			(PGM_P) HunoBasic_GetupBack_IGain,
			(PGM_P) HunoBasic_GetupBack_Frames,
			(PGM_P) HunoBasic_GetupBack_TrTime,
			(PGM_P) HunoBasic_GetupBack_Position,
			HUNOBASIC_GETUPBACK_NUM_SCENES,
			HUNOBASIC_GETUPBACK_NUM_MOTORS
		},
		// 7. GetupFront
		{
			(PGM_P) HunoBasic_GetupFront_Torque,
			(PGM_P) HunoBasic_GetupFront_Port,
			(PGM_P) HunoBasic_GetupFront_PGain,
			(PGM_P) HunoBasic_GetupFront_DGain,
			(PGM_P) HunoBasic_GetupFront_IGain,
			(PGM_P) HunoBasic_GetupFront_Frames,
			(PGM_P) HunoBasic_GetupFront_TrTime,
			(PGM_P) HunoBasic_GetupFront_Position,
			HUNOBASIC_GETUPFRONT_NUM_SCENES,
			HUNOBASIC_GETUPFRONT_NUM_MOTORS
		},
		// 8. WalkForward
			{
			(PGM_P) HunoBasic_WalkForward_Torque,
			(PGM_P) HunoBasic_WalkForward_Port,
			(PGM_P) HunoBasic_WalkForward_PGain,
			(PGM_P) HunoBasic_WalkForward_DGain,
			(PGM_P) HunoBasic_WalkForward_IGain,
			(PGM_P) HunoBasic_WalkForward_Frames,
			(PGM_P) HunoBasic_WalkForward_TrTime,
			(PGM_P) HunoBasic_WalkForward_Position,
			HUNOBASIC_WALKFORWARD_NUM_SCENES,
			HUNOBASIC_WALKFORWARD_NUM_MOTORS
		},
		// 9. WalkBackward
		{
			(PGM_P) HunoBasic_WalkBackward_Torque,
			(PGM_P) HunoBasic_WalkBackward_Port,
			(PGM_P) HunoBasic_WalkBackward_PGain,
			(PGM_P) HunoBasic_WalkBackward_DGain,
			(PGM_P) HunoBasic_WalkBackward_IGain,
			(PGM_P) HunoBasic_WalkBackward_Frames,
			(PGM_P) HunoBasic_WalkBackward_TrTime,
			(PGM_P) HunoBasic_WalkBackward_Position,
			HUNOBASIC_WALKBACKWARD_NUM_SCENES,
			HUNOBASIC_WALKBACKWARD_NUM_MOTORS
		},


		// 10. E-motion LSHOOT

		{
			(PGM_P) LSHOOT_Torque,
			(PGM_P) LSHOOT_Port,
			(PGM_P) LSHOOT_RuntimePGain,
			(PGM_P) LSHOOT_RuntimeDGain,
			(PGM_P) LSHOOT_RuntimeIGain,
			(PGM_P) LSHOOT_Frames,
			(PGM_P) LSHOOT_TrTime,
			(PGM_P) LSHOOT_Position,
			LSHOOT_NUM_OF_SCENES,
			LSHOOT_NUM_OF_WCKS
		},

		// 11. E-motion RSHOOT
		{
			(PGM_P) RSHOOT_Torque,
			(PGM_P) RSHOOT_Port,
			(PGM_P) RSHOOT_RuntimePGain,
			(PGM_P) RSHOOT_RuntimeDGain,
			(PGM_P) RSHOOT_RuntimeIGain,
			(PGM_P) RSHOOT_Frames,
			(PGM_P) RSHOOT_TrTime,
			(PGM_P) RSHOOT_Position,
			RSHOOT_NUM_OF_SCENES,
			RSHOOT_NUM_OF_WCKS
		},

		// 12. E-motion RSIDEWALK
		{
			(PGM_P) RSIDEWALK_Torque,
			(PGM_P) RSIDEWALK_Port,
			(PGM_P) RSIDEWALK_RuntimePGain,
			(PGM_P) RSIDEWALK_RuntimeDGain,
			(PGM_P) RSIDEWALK_RuntimeIGain,
			(PGM_P) RSIDEWALK_Frames,
			(PGM_P) RSIDEWALK_TrTime,
			(PGM_P) RSIDEWALK_Position,
			RSIDEWALK_NUM_OF_SCENES,
			RSIDEWALK_NUM_OF_WCKS
		},

		// 13. E-motion LSIDEWALK
		{
			(PGM_P) LSIDEWALK_Torque,
			(PGM_P) LSIDEWALK_Port,
			(PGM_P) LSIDEWALK_RuntimePGain,
			(PGM_P) LSIDEWALK_RuntimeDGain,
			(PGM_P) LSIDEWALK_RuntimeIGain,
			(PGM_P) LSIDEWALK_Frames,
			(PGM_P) LSIDEWALK_TrTime,
			(PGM_P) LSIDEWALK_Position,
			LSIDEWALK_NUM_OF_SCENES,
			LSIDEWALK_NUM_OF_WCKS
		},

		// 14. E-motion STANDUPR
		{
			(PGM_P) STANDUPR_Torque,
			(PGM_P) STANDUPR_Port,
			(PGM_P) STANDUPR_RuntimePGain,
			(PGM_P) STANDUPR_RuntimeDGain,
			(PGM_P) STANDUPR_RuntimeIGain,
			(PGM_P) STANDUPR_Frames,
			(PGM_P) STANDUPR_TrTime,
			(PGM_P) STANDUPR_Position,
			STANDUPR_NUM_OF_SCENES,
			STANDUPR_NUM_OF_WCKS
		},

		// 15. E-motion STANDUPF
		{
			(PGM_P) STANDUPF_Torque,
			(PGM_P) STANDUPF_Port,
			(PGM_P) STANDUPF_RuntimePGain,
			(PGM_P) STANDUPF_RuntimeDGain,
			(PGM_P) STANDUPF_RuntimeIGain,
			(PGM_P) STANDUPF_Frames,
			(PGM_P) STANDUPF_TrTime,
			(PGM_P) STANDUPF_Position,
			STANDUPF_NUM_OF_SCENES,
			STANDUPF_NUM_OF_WCKS
		},

		// 16. E-motion HUNODEMO_SITDOWN
		{
			(PGM_P) HUNODEMO_SITDOWN_Torque,
			(PGM_P) HUNODEMO_SITDOWN_Port,
			(PGM_P) HUNODEMO_SITDOWN_RuntimePGain,
			(PGM_P) HUNODEMO_SITDOWN_RuntimeDGain,
			(PGM_P) HUNODEMO_SITDOWN_RuntimeIGain,
			(PGM_P) HUNODEMO_SITDOWN_Frames,
			(PGM_P) HUNODEMO_SITDOWN_TrTime,
			(PGM_P) HUNODEMO_SITDOWN_Position,
			HUNODEMO_SITDOWN_NUM_OF_SCENES,
			HUNODEMO_SITDOWN_NUM_OF_WCKS
		},

		// 17. E-motion HUNODEMO_HI
		{
			(PGM_P) HUNODEMO_HI_Torque,
			(PGM_P) HUNODEMO_HI_Port,
			(PGM_P) HUNODEMO_HI_RuntimePGain,
			(PGM_P) HUNODEMO_HI_RuntimeDGain,
			(PGM_P) HUNODEMO_HI_RuntimeIGain,
			(PGM_P) HUNODEMO_HI_Frames,
			(PGM_P) HUNODEMO_HI_TrTime,
			(PGM_P) HUNODEMO_HI_Position,
			HUNODEMO_HI_NUM_OF_SCENES,
			HUNODEMO_HI_NUM_OF_WCKS
		},

		// 18. E-motion HUNODEMO_KICKLEFTFRONTTURN
		{
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_Torque,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_Port,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_RuntimePGain,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_RuntimeDGain,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_RuntimeIGain,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_Frames,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_TrTime,
			(PGM_P) HUNODEMO_KICKLEFTFRONTTURN_Position,
			HUNODEMO_KICKLEFTFRONTTURN_NUM_OF_SCENES,
			HUNODEMO_KICKLEFTFRONTTURN_NUM_OF_WCKS
		},

		// 19. E-motion HANDSTANDS1
		{
			(PGM_P) HANDSTANDS1_Torque,
			(PGM_P) HANDSTANDS1_Port,
			(PGM_P) HANDSTANDS1_RuntimePGain,
			(PGM_P) HANDSTANDS1_RuntimeDGain,
			(PGM_P) HANDSTANDS1_RuntimeIGain,
			(PGM_P) HANDSTANDS1_Frames,
			(PGM_P) HANDSTANDS1_TrTime,
			(PGM_P) HANDSTANDS1_Position,
			HANDSTANDS1_NUM_OF_SCENES,
			HANDSTANDS1_NUM_OF_WCKS
		}

	};


	void PlayMotion(int n)
	{
		DBO(printf ("ROBO:  Play %d\n", n);)
		int dur,frt, i,j;

		int ns = mlist[n].NoS;
		int nw = mlist[n].Now;

		PGM_P p = mlist[n].PoT;
		PGM_P f = mlist[n].FT;
		PGM_P t = mlist[n].RT;

		for (i=1; i<=ns; i++) //for each scene
		{
			BYTE temp[nw];
			for (j=0; j<nw; j++)
			{

				temp[j] = *(p+j+i*nw); //pgm_read_byte(p+j+i*nw);
				if (j<16)
				{
					temp[j] += offset[j];
				}
			}

			dur=*(t+(i-1)*4) + 256*(*(t+1+(i-1)*4));  	//pgm_read_word(t+(i-1)*2);
			frt=*(f+(i-1)*4) + 256*(*(f+1+(i-1)*4));  	//pgm_read_word(f+(i-1)*2);

			//printf("Dur=%d, frt=%d\n", dur,frt);
			//for (int x=0; x<nw; x++)
			//	printf ("%d - %d\n", x,temp[x]);

			PlayPose(dur, frt, 4, temp, (i==1)?16:0);
		}
	}




