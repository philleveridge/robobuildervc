using System;
using System.Collections.Generic;
using System.IO.Ports;

// enable download of basic programs direct to Robos
// will need new version of Robos to support

/*
The ability to create simple actions in an elemntry basic language

Language Spec:
VAR    A-Z  INTEGER
OPER   +-*\()=<>
CMD    LET:FOR:NEXT:GOTO:IF:THEN:ELSE:PRINT:END:SET
STRING " ... "
EXPR1  VAR | LITERAL
EXPR2  EXPR1 | STRING
LIST   EXPR2 [,EXPR2]
EXPR   EXPR1 OPER EXPR1  

SYNTAX:
[LINE no] LET  VAR '=' EXPR 
[LINE no] GOTO [Line No]
[LINE no] PRINT LIST [;]
[LINE No] END
[LINE no] IF  EXPR THEN LINE no ELSE Line No
[LINE No] FOR VAR '=' EXPR 'To' EXPR
[LINE No] NEXT
[LINE No] XACT EXPR
[LINE No] WAIT EXPR
[Line No] GOSUB [Line No]
[Line No] RETURN
[Line No] SERVO VAR '=' EXPR | '@'
[LINE No] SCENE LIST
[LINE No] MOVE LIST
--------------------- UNDER TEST -----------------------

[LINE No] PUT [Special] = [Expr]

--------------------------TBD --------------------------

[LINE No] SENDOP   expr, expr
[Line No] SENDSET  expr, expr, expr,  expr

Rbas Cmd		 Description
==============   ==============
XACT       		 Call any Experimental action using literal code i.e. XACT 0, would do basic pose, XACT 17 a wave
PUT PF1=1	   	 access to PORTS/SPECIAL REGISTER, This would set PF1_led1 on
SERVO ID=POS     set servo id to positon POS / @
LET A=$SERVO:id  let A get position of servo id 
SCENE            set up a Scene - 16 Servo Positions
MOVE             sends a loaded Scene  (time ms, no frames)

Special register access ($)
LET A=$IR  		 //get char from IR and transfer to A (also $ADC. $PSD, $X, $Y...)
LET A=$PORT:A:6  //Read Bit 6 of Port A
LET A=$ROM:10    // read byte 10 of ROM

POKE 10,A         // Put A into Byte 10
PUT PORT:A:8 = 3 //set DDR of Port A = 3 (PIN0,PIN1 readable)
PUT PORT:A:2 = 1 //set Port A bit 1 =1 (assuming writeable)

Example Programs are now available from examples folder


*/

namespace RobosMode
{
    class Basic
    {
        byte[] code;
        int codeptr;

        public int errno;
        public int lineno;

        /**********************************************************/

        string[] error_msgs = new string[] {
	        "",
	        "Syntax error",
	        "Invalid Command",
	        "Illegal var",
	        "Bad number",
	        "Next without for",
	        };
	
        string[] specials = new string[] { "PF", "MIC", "X", "Y", "Z", "PSD", "VOLT", "IR", "KBD", "RND", "SERVO", "TICK", 
		        "PORT", "ROM" };
        
        enum KEY {
	        LET=0, FOR, IF, THEN, 
	        ELSE, GOTO, PRINT, GET, 
	        PUT, END, SCENE, XACT, 
	        WAIT, NEXT, SERVO, MOVE,
	        GOSUB, RETURN, POKE
	        };
	
        string[] tokens = new string[] {
            "LET", "FOR", "IF","THEN", 
            "ELSE","GOTO","PRINT","GET",
            "PUT", "END", "SCENE", "XACT",
            "WAIT", "NEXT", "SERVO", "MOVE",
            "GOSUB", "RETURN", "POKE"
        };


        enum SKEY {sPF1=0, sMIC, sGX, sGY, sGZ, sPSD, sVOLT, sIR, sKBD, sRND, sSERVO, sTICK, sPORT, sROM};
							
        struct basic_line {
            int lineno;
	        byte token;
	        byte var;
	        int value;
	        string text; // rest of line - unproceesed
        };

        public Basic()
        {
            code = new byte[4096];
            codeptr = 0;
        }

        /*-------------------------------------

        -------------------------------------*/

        bool IsNumber(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] < '0' || s[i] > '9') return false;
            }
            return true;
        }

        int IsToken(string w)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                if (w == tokens[i]) return i;
            }
            retun - 1;
        }


        public bool Compile(string prog)
        {
            errno = 0;
            lineno = 0;

            string[] lines = prog.Split('\n');
            foreach (string s in lines)
            {
                lineno++;
                Console.Write(s);
                string[] words = s.Split(' ');
            }

            return false;
        }

        public bool Download(SerialPort s1)
        {
            if (s1 == null || !s1.IsOpen) return false;

            // transfer code --> robot
            return false;
        }

        public string Dump()
        {
            string m = "";
            for (int i = 0; i < 32; i+=8)
            {
                m += (i.ToString("X4") + " - ");
                for (int j = 0; j < 8; j++)
                {
                    m += (code[i].ToString("X2") + "  ");
                }
                m += "\r\n" ;
            }
            Console.Write(m);
            return m;
        }
    }
}
