using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Text;
using RobobuilderLib;
using Microsoft.Kinect;

namespace RoboKinnect
{
    class RoboKinectAPI
    {
        public Boolean bRBConnected = false;
        wckMotion rbWCK = null;
        PCremote rbPCR = null;

        //Define maximum thresholds for relating Kinect to Shoulder 
        //Single sMaxKinectHandY = (Single)0.25;
        //Single sMinKinectHandY = (Single)0.7;
        //SByte sbKinectAngle;
        Byte bMaxShoulder = 185;
        Byte bMinShoulder = 47;

        public bool simple = true;
        const byte atMINwck = 0, atMAXwck = 1;
        const int X = 0, Y = 1, Z = 2;

        byte[,] bServoRange = new byte[16, 2];
        double[,] dDegreesRange = new double[16, 2];

        public RoboKinectAPI() { init(); }

        void init()
        {
            //            ' define maximum ranges for servos
            bServoRange[14, atMINwck] = 125; dDegreesRange[14, atMINwck] = -90; //' when at 125 this corresponds to -90 deg
            bServoRange[14, atMAXwck] = 207; dDegreesRange[14, atMAXwck] = 0; //' and when at 207 this corresponds to 0

            bServoRange[13, atMINwck] = 0; dDegreesRange[13, atMINwck] = 80;
            bServoRange[13, atMAXwck] = 208; dDegreesRange[13, atMAXwck] = -120;

            bServoRange[15, atMINwck] = 140; dDegreesRange[15, atMINwck] = 0;
            bServoRange[15, atMAXwck] = 242; dDegreesRange[15, atMAXwck] = 135;


            bServoRange[11, atMINwck] = 48; dDegreesRange[11, atMINwck] = 0;
            bServoRange[11, atMAXwck] = 130; dDegreesRange[11, atMAXwck] = -90;

            bServoRange[10, atMINwck] = 48; dDegreesRange[10, atMINwck] = 120;
            bServoRange[10, atMAXwck] = 255; dDegreesRange[10, atMAXwck] = -80;

            bServoRange[12, atMINwck] = 13; dDegreesRange[12, atMINwck] = 135;
            bServoRange[12, atMAXwck] = 115; dDegreesRange[12, atMAXwck] = 0;
        }

        public void Connect(string sCOMMNum)
        {
            try
            {
                SerialPort s = new SerialPort(sCOMMNum, 115200);
                s.Open();
                rbPCR = new RobobuilderLib.PCremote(s);
                rbWCK = new RobobuilderLib.wckMotion(rbPCR);

                if (rbWCK.wckReadPos(30))
                {
                    //DCMP is true 
                    Console.WriteLine("V={0}.{1}\n\n", rbWCK.respnse[0], rbWCK.respnse[1]);
                    rbWCK.DCMP = true;
                    rbWCK.PlayPose(1000, 10, wckMotion.basic16, true);
                }
                else
                {
                    Console.WriteLine("required DCMP firmware\n\n");
                }
                bRBConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to Connect to Robobuilder\n\n" + e.Message);
                rbPCR = null;
                bRBConnected = false;
            }
        }

        public void Disconnect()
        {
            if (rbWCK != null)
                rbWCK.close();

            if (rbPCR != null)
                rbPCR.Close();
            rbWCK = null;
            rbPCR = null;
            bRBConnected = false;
        }

        public void SendArmPosition(Byte bServo10, Byte bServo11, Byte bServo12, Byte bServo13, Byte bServo14, Byte bServo15)
        {
            try
            {
                Console.WriteLine("MOVE {0},{1},{2},{3},{4},{5}", bServo10, bServo11, bServo12, bServo13, bServo14, bServo15);
                if (!bRBConnected)
                {
                    return;
                }
                rbWCK.wckMovePos(10, bServo10, 1); // 3rd parameter is Torque 0~4: 0=maximum 
                rbWCK.wckMovePos(11, bServo11, 1);
                rbWCK.wckMovePos(12, bServo12, 1);
                rbWCK.wckMovePos(13, bServo13, 1);
                rbWCK.wckMovePos(14, bServo14, 1);
                rbWCK.wckMovePos(15, bServo15, 1);
            }
            catch
            {
                //MessageBox.Show("Unable to Update Servo Position " + e.Message); 
            }
        }

        void CalculateVector(double[] vectorXYZ, Microsoft.Kinect.Skeleton playerSkeleton, Microsoft.Kinect.JointType jEndJoint, Microsoft.Kinect.JointType jStartJoint)
        {
            //' the vector is passed By Reference so we will write the results directly to the original argument passed.
            vectorXYZ[0] = playerSkeleton.Joints[jEndJoint].Position.X - playerSkeleton.Joints[jStartJoint].Position.X;
            vectorXYZ[1] = playerSkeleton.Joints[jEndJoint].Position.Y - playerSkeleton.Joints[jStartJoint].Position.Y;
            vectorXYZ[2] = playerSkeleton.Joints[jEndJoint].Position.Z - playerSkeleton.Joints[jStartJoint].Position.Z;
        }

        double CalculateVectorSize(double[] vector)
        {
            return Math.Sqrt(vector[0] * vector[0] + vector[1] * vector[1] + vector[2] * vector[2]);
        }

        double RadToDeg(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public int ScaleTo(double val, int imin, int imax, double dmin, double dmax)
        {
            int r;
            if (val < dmin) val = dmin;
            if (val > dmax) val = dmax;
            val = (val - dmin) / (dmax - dmin);
            r = (int)((double)(imax - imin) * val) + imin;

            System.Console.WriteLine("DEBUG: " + val + ": " + r);
            return r;
        }

        void UpdateRobobuilderArms(Byte bServo11, Byte bservo14)
        {
            //left arm
            Byte bServo10 = 127, bServo12 = 127;
            try
            {
                double dServo11 = Convert.ToDouble(bServo11);
                //               5E-05x3 - 0.0213x2 + 2.9618x - 46.011
                bServo10 = Convert.ToByte(0.00005 * Math.Pow(dServo11, 3) - 0.0213 * Math.Pow(dServo11, 2) + 2.9618 * dServo11 - 46.011);

                //             8E-05x3 - 0.0339x2 + 4.7877x - 108.15
                bServo12 = Convert.ToByte(0.00008 * Math.Pow(dServo11, 3) - 0.0339 * Math.Pow(dServo11, 2) + 4.7877 * dServo11 - 108.15);
            }
            catch (Exception) { }

            // right arm
            Byte bServo13 = 127, bServo15 = 127;
            try
            {
                double dServo14 = Convert.ToDouble(bservo14);
                //               5E-05x3 - 0.0176x2 + 2.0147x + 87.278
                bServo13 = Convert.ToByte(0.00005 * Math.Pow(dServo14, 3) - 0.0176 * Math.Pow(dServo14, 2) + 2.0147 * dServo14 + 87.27);
                //               8E-05x3 - 0.0272x2 + 3.0598x + 24.756
                bServo15 = Convert.ToByte(0.00008 * Math.Pow(dServo14, 3) - 0.0272 * Math.Pow(dServo14, 2) + 3.0598 * dServo14 + 24.756);

            }
            catch (Exception) { }
            SendArmPosition(bServo10, bServo11, bServo12, bServo13, bservo14, bServo15);
        }

        public void updaterobot(Skeleton playerSkeleton)
        {
            if (simple)
            {
                //Figure out values for the shoulder based on Hand position.
                // Kinect returns positive values if the hand is above the head and negative values if it's bellow the head
                int ServoRange = bMaxShoulder - bMinShoulder;

                //Left Arm
                //Dim scaledLeftArm = playerSkeleton.Joints(JointType.HandRight).ScaleTo(1, ServoRange, 0.5F, 0.3F);
                int sla = ScaleTo(playerSkeleton.Joints[JointType.HandRight].Position.Y, 0, ServoRange, 0.0, 0.3);
                Byte bServo11 = Convert.ToByte(255 - (ServoRange - sla + bMinShoulder));

                // Right Arm
                //Dim scaledRightArm = playerSkeleton.Joints(JointType.HandLeft).ScaleTo(1, ServoRange, 0.5F, 0.3F);
                int sra = ScaleTo(playerSkeleton.Joints[JointType.HandLeft].Position.Y, 0, ServoRange, 0.0, 0.3);
                Byte bServo14 = Convert.ToByte(255 - (255 - (ServoRange - sra + bMinShoulder)));
                UpdateRobobuilderArms(bServo11, bServo14);
            }
            else
            {
                byte[] r = new byte[3];
                byte[] l = new byte[3];
                r = UpdateRightArm(playerSkeleton);
                l = UpdateLeftArm(playerSkeleton);
                SendArmPosition(r[0], r[1], r[2], l[0], l[1], l[2]);
            }
        }

        byte[] UpdateRightArm(Microsoft.Kinect.Skeleton playerSkeleton)
        {
            byte[] res = new byte[3];
            //''''''''''''''''''''''''''''''''''''''''
            //' RIGHT ARM
            //''''''''''''''''''''''''''''''''''''''''

            //' Calculate Angles and values for Right Arm (Robobuilder's Left Arm).
            double[] dVectorRS_RElbowShoulder = new double[3];
            double[] dVectorRS_RWristElbow = new double[3];

            CalculateVector(dVectorRS_RElbowShoulder, playerSkeleton, JointType.ElbowRight, JointType.ShoulderRight);

            System.Console.WriteLine("{0} {1} {2}", dVectorRS_RElbowShoulder[X], dVectorRS_RElbowShoulder[Y], dVectorRS_RElbowShoulder[Z]);

            double[] dVectorRobobuilder = new double[3];
            dVectorRobobuilder[X] = 0 - dVectorRS_RElbowShoulder[Z];
            dVectorRobobuilder[Y] = 0 - dVectorRS_RElbowShoulder[Y];
            dVectorRobobuilder[Z] = 0 - dVectorRS_RElbowShoulder[X];

            double dRadius_LElbowShoulder = CalculateVectorSize(dVectorRobobuilder);

            double dAzimuth_RElbowShoulder = Math.Atan(dVectorRobobuilder[Y] / dVectorRobobuilder[X]);
            double dElevation_RElbowShoulder = Math.PI / 2 - Math.Acos(dVectorRobobuilder[Z] / dRadius_LElbowShoulder);
            if (dVectorRobobuilder[0] < 0)
            {  //' correction to prevent going fron +90 to -90 when the arm is upright
                if (dVectorRobobuilder[1] > 0)
                {
                    dAzimuth_RElbowShoulder = Math.PI + dAzimuth_RElbowShoulder;
                }
                else
                {
                    dAzimuth_RElbowShoulder = -Math.PI + dAzimuth_RElbowShoulder;
                }
            }

            // ' Now calculate the Vector for the Elbow/Wrist
            CalculateVector(dVectorRS_RWristElbow, playerSkeleton, JointType.WristRight, JointType.ElbowRight);

            double dAngle_RWristElbow = Math.Acos((dVectorRS_RWristElbow[X] * dVectorRS_RElbowShoulder[X]
                + dVectorRS_RWristElbow[Y] * dVectorRS_RElbowShoulder[Y]
                + dVectorRS_RWristElbow[Z] * dVectorRS_RElbowShoulder[Z]) / (CalculateVectorSize(dVectorRS_RWristElbow) * CalculateVectorSize(dVectorRS_RElbowShoulder)));

            System.Console.WriteLine("{0}", RadToDeg(dAngle_RWristElbow));


            //' Write Angles to Servos
            //' the trigonometric functions may return NaN when a skeleton is not in a realistic position.
            if (!Double.IsNaN(dAzimuth_RElbowShoulder) &&
                !Double.IsNaN(dElevation_RElbowShoulder) &&
                !Double.IsNaN(dAngle_RWristElbow))
            {

                //' convert the angles to degress
                dAzimuth_RElbowShoulder = RadToDeg(dAzimuth_RElbowShoulder);
                dElevation_RElbowShoulder = RadToDeg(dElevation_RElbowShoulder);
                dAngle_RWristElbow = RadToDeg(dAngle_RWristElbow);

                System.Console.WriteLine("{0} {1}", dAzimuth_RElbowShoulder, dElevation_RElbowShoulder);

                res[0] = AngleToWckPosition(dAzimuth_RElbowShoulder, 10);
                res[1] = AngleToWckPosition(dElevation_RElbowShoulder, 11);
                res[2] = AngleToWckPosition(dElevation_RElbowShoulder, 12);

                return res;
            }
            return null;
        }


        byte[] UpdateLeftArm(Microsoft.Kinect.Skeleton playerSkeleton)
        {
            //''''''''''''''''''''''''''''''''''''''''
            //' LEFT ARM
            //''''''''''''''''''''''''''''''''''''''''
            byte[] res = new byte[3];

            //' Calculate Angles and values for Left Arm (Robobuilder's Right Arm).
            double[] dVectorRS_LElbowShoulder = new double[3];
            double[] dVectorRS_LWristElbow = new double[3];

            CalculateVector(dVectorRS_LElbowShoulder, playerSkeleton, JointType.ElbowLeft, JointType.ShoulderLeft);

            //'HEADKinectX.Text = CStr(dVectorRS_LElbowShoulder(X))
            //'HEADKinectY.Text = CStr(dVectorRS_LElbowShoulder(Y))
            //'HEADKinectZ.Text = CStr(dVectorRS_LElbowShoulder(Z))

            double[] dVectorRobobuilder = new double[3];

            dVectorRobobuilder[0] = 0 - dVectorRS_LElbowShoulder[2];
            dVectorRobobuilder[1] = dVectorRS_LElbowShoulder[1];
            dVectorRobobuilder[2] = dVectorRS_LElbowShoulder[0];

            double dRadius_LElbowShoulder = CalculateVectorSize(dVectorRobobuilder);

            double dAzimuth_LElbowShoulder = Math.Atan(dVectorRobobuilder[1] / dVectorRobobuilder[0]);
            double dElevation_LElbowShoulder = Math.PI / 2 - Math.Acos(dVectorRobobuilder[2] / dRadius_LElbowShoulder);

            if (dVectorRobobuilder[0] < 0)
            { //' correction to prevent going fron +90 to -90 when the arm is upright
                if (dVectorRobobuilder[1] > 0)
                {
                    dAzimuth_LElbowShoulder = Math.PI + dAzimuth_LElbowShoulder;
                }
                else
                {
                    dAzimuth_LElbowShoulder = -Math.PI + dAzimuth_LElbowShoulder;
                }
            }
            else
            {
                //' don't touch azimuth
            }

            //' Now calculate the Vector for the Elbow/Wrist
            CalculateVector(dVectorRS_LWristElbow, playerSkeleton, JointType.WristLeft, JointType.ElbowLeft);

            double dAngle_LWristElbow = Math.Acos((dVectorRS_LWristElbow[0] * dVectorRS_LElbowShoulder[0]
                + dVectorRS_LWristElbow[1] * dVectorRS_LElbowShoulder[1]
                + dVectorRS_LWristElbow[2] * dVectorRS_LElbowShoulder[2]) / (CalculateVectorSize(dVectorRS_LWristElbow) * CalculateVectorSize(dVectorRS_LElbowShoulder)));
            //'Me.HLKinectZ.Text = CStr(RadToDeg(dAngle_LWristElbow))


            //' Write Angles to Servos
            //' the trigonometric functions may return NaN when a skeleton is not in a realistic position.
            if (!Double.IsNaN(dAzimuth_LElbowShoulder) && !Double.IsNaN(dElevation_LElbowShoulder) && !Double.IsNaN(dAngle_LWristElbow))
            {
                //' convert the angles to degress
                dAzimuth_LElbowShoulder = RadToDeg(dAzimuth_LElbowShoulder);
                dElevation_LElbowShoulder = RadToDeg(dElevation_LElbowShoulder);
                dAngle_LWristElbow = RadToDeg(dAngle_LWristElbow);

                res[0] = AngleToWckPosition(dAzimuth_LElbowShoulder, 13);
                res[1] = AngleToWckPosition(dElevation_LElbowShoulder, 14);
                res[2] = AngleToWckPosition(dElevation_LElbowShoulder, 15);

                return res;
            }
            return null;
        }


        byte AngleToWckPosition(double dAngle, byte iServoNumber)
        {
            double dAngleRange = Math.Abs(dDegreesRange[iServoNumber, atMAXwck] - dDegreesRange[iServoNumber, atMINwck]);
            byte bWckRange = (byte)(bServoRange[iServoNumber, atMAXwck] - bServoRange[iServoNumber, atMINwck]);
            byte bPos;
            double dNormalizedAngle;
            double dClippedAngle;

            if (dDegreesRange[iServoNumber, atMINwck] > dDegreesRange[iServoNumber, atMAXwck])
            {
                //' in this case the servo and angle grow in an inverse relation
                //' clip actual angle to alowed limits
                dClippedAngle = Math.Min(dAngle, dDegreesRange[iServoNumber, atMINwck]);
                dClippedAngle = Math.Max(dClippedAngle, dDegreesRange[iServoNumber, atMAXwck]);

                //' normalize angle to range:
                dNormalizedAngle = dClippedAngle - dDegreesRange[iServoNumber, atMAXwck];

                bPos = (byte)Math.Floor(Math.Abs((double)(bWckRange) * (dNormalizedAngle / dAngleRange)));

                //' normalize position to range:
                bPos = (byte)(bServoRange[iServoNumber, atMAXwck] - bPos);
            }
            else
            {
                //' clip actual angle to alowed limits
                dClippedAngle = Math.Min(dAngle, dDegreesRange[iServoNumber, atMAXwck]);
                dClippedAngle = Math.Max(dClippedAngle, dDegreesRange[iServoNumber, atMINwck]);

                //' normalize angle to range:
                dNormalizedAngle = dClippedAngle - dDegreesRange[iServoNumber, atMINwck];

                bPos = (byte)(Math.Floor(Math.Abs((double)(bWckRange) * (dNormalizedAngle / dAngleRange))));

                //' normalize position to range:
                bPos = (byte)(bPos + bServoRange[iServoNumber, atMINwck]);
            }
            return bPos;
        }
    }
}
