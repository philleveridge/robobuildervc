using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;

namespace RobobuilderLib
{
    public partial class Form5 : Form
    {
        Vector3 cameraRot;
        Vector3 cameraPos;

        Microsoft.DirectX.DirectInput.Device keyb;

        bool simulation_running;
        bool simulation_paused;

        Render g3D;

        Simulator.IniManager IniData;

        List<ServoModel> servos;

        public Form5()
        {
            InitializeComponent();

            g3D = new Render(viewPort);

            InitializeKeyboard();

            initSetup("config-20dof.txt");

            //initSetup("config.txt");
            //selectServo(1, true);
        }

        public void InitializeKeyboard()
        {
            keyb = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
            keyb.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            keyb.Acquire();
        }


        private void ReadKeyboard()
        {
            KeyboardState keys = keyb.GetCurrentKeyboardState();

            float driveScale = 0.2f;
            float rotateScale = 2.0f;
            Vector3 driveInput = new Vector3(0, 0, 0);
            Vector3 rotateInput = new Vector3(0, 0, 0);

            if (keys[Key.W])
            { driveInput.Z -= 1; }
            if (keys[Key.S])
            { driveInput.Z += 1; }
            if (keys[Key.A])
            { driveInput.X += 1; }
            if (keys[Key.D])
            { driveInput.X -= 1; }
            if (keys[Key.F])
            { driveInput.X -= 1; }
            if (keys[Key.R])
            { driveInput.X += 1; }
            if (keys[Key.Up])
            { rotateInput.X -= 1; }
            if (keys[Key.Down])
            { rotateInput.X += 1; }

            if (keys[Key.Q] || keys[Key.Left])
            { rotateInput.Y -= 1; }
            if (keys[Key.E] || keys[Key.Right])
            { rotateInput.Y += 1; }

            if (keys[Key.Escape])
                simulation_running = false;

            if (keys[Key.Space])
                simulation_paused = !simulation_paused;

            // update camera view

            driveInput *= driveScale;
            rotateInput *= rotateScale;

            cameraRot.X = UTIL.clampFloat(cameraRot.X + rotateInput.X, -85, 85);	//clamp angle between -85 and 85 degrees
            cameraRot.Y += rotateInput.Y;

            Vector3 dir = new Vector3((float)Math.Sin((float)(Math.PI / 180) * cameraRot.Y), 0, (float)Math.Cos((float)(Math.PI / 180) * cameraRot.Y));
            Vector3 perpDir = new Vector3(dir.Z, 0, -dir.X);
            Vector3 upDir = new Vector3(0, 1, 0);

            cameraPos = cameraPos + (dir * driveInput.Z) + (perpDir * driveInput.X) + (upDir * driveInput.Y);

            Tx.Text = cameraPos.X.ToString();
            Ty.Text = cameraPos.Y.ToString();
            Tz.Text = cameraPos.Z.ToString();

            Vx.Text = cameraRot.X.ToString();
            Vy.Text = cameraRot.Y.ToString();
            Vz.Text = cameraRot.Z.ToString();

            g3D.updateCamera(cameraPos, cameraRot);

        }

        public void render()
        {
            if (g3D.Device == null)
            { return; }

            g3D.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            g3D.Device.BeginScene();

            drawScene();

            g3D.Device.EndScene();
            g3D.Device.Present();

            ReadKeyboard();
        }

        public void drawScene()
        {
            // draw axis
            g3D.drawline(new Vector3(-100f, 0f, 0f), new Vector3(100f, 0f, 0f), Color.Red, Matrix.Identity);
            g3D.drawline(new Vector3(0f, -100f, 0f), new Vector3(0f, 100f, 0f), Color.Blue, Matrix.Identity);
            g3D.drawline(new Vector3(0f, 0f, -100f), new Vector3(0f, 0f, 100f), Color.White, Matrix.Identity);

            // ground plan
            g3D.drawplane();

            //object and joints
            foreach (ServoModel s in servos)
            {
                if (s.joint)
                    g3D.drawline(s.loc, s.rot, s.mod_no == 0 ? Color.Red : Color.Yellow, Matrix.Identity);
                else
                    g3D.drawModel(s.mod_no, s.loc, s.rot, s.select, checkBox1.Checked);
            }
            //set camera
            g3D.setupView();
        }

        // process Ini file
        void initSetup(string fn)
        {
            float[] t1;

            IniData = new Simulator.IniManager();
            IniData.Load(fn); // use default

            g3D.loadmodels(IniData);

            string mb;

            if ((mb = IniData.getParameter("CAMERA")) != "")
            {
                string[] t = mb.Split(',');

                cameraRot = new Vector3(float.Parse(t[3]), float.Parse(t[4]), float.Parse(t[5]));
                cameraPos = new Vector3(float.Parse(t[0]), float.Parse(t[1]), float.Parse(t[2]));

                g3D.updateCamera(cameraPos, cameraRot);

            }

            servos = new List<ServoModel>();

            for (int id = 0; id < 32; id++)
            {
                if ((t1 = IniData.getParameterEvalArray("S" + id)).Length>5)
                {
                    addModel("S" + id, new Vector3(t1[0], t1[1], t1[2]), new Vector3(t1[3], t1[4], t1[5]), 1);
                }
            }

            if ((t1 = IniData.getParameterEvalArray("FootL")).Length>2)
            {
                addModel("FootL", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 4);
            }
            if ((t1 = IniData.getParameterEvalArray("FootR")).Length>2)
            {
                addModel("FootR", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 4);
            }

            if ((t1 = IniData.getParameterEvalArray("HandL")).Length >2)
            {
                addModel("HandL", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 2);
                addJoint("HandLJ", new Vector3(t1[0], t1[1], t1[2]), findServo("S" + t1[3].ToString()).loc, (int)t1[4]);
            }

            if ((t1 = IniData.getParameterEvalArray("HandR")).Length > 2)
            {
                addModel("HandR", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 3);
                addJoint("HandRJ", new Vector3(t1[0], t1[1], t1[2]), findServo("S" + t1[3].ToString()).loc, (int)t1[4]);
            }
            if ((t1 = IniData.getParameterEvalArray("Body")).Length > 2)
            {
                addModel("Body", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 5);
            }
            if ((t1 = IniData.getParameterEvalArray("KneeL")).Length >2)
            {
                addModel("KneeL", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 6);
            }
            if ((t1 = IniData.getParameterEvalArray("KneeR")).Length >2)
            {
                addModel("KneeR", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 6);
            }
            for (int id = 0; id < 50; id++)
            {
                if ((t1 = IniData.getParameterEvalArray("Joint" + id)).Length>2)
                {
                    addJoint("Joint" + id, findServo("S" + t1[0].ToString()).loc, findServo("S" + t1[1].ToString()).loc, (int)t1[2]);
                }
            }
        }

        void addModel(string n, Vector3 loc, Vector3 rot, int t)
        {
            ServoModel s = new ServoModel();
            s.id = n;
            s.mod_no = t;
            s.loc = loc;
            s.rot = rot;
            servos.Add(s);
        }

        void addJoint(string n, Vector3 from, Vector3 to, int c)
        {
            ServoModel s = new ServoModel();
            s.mod_no = c; // type 0/1
            s.id = n;
            s.loc = from;
            s.rot = to;
            s.joint = true;
            servos.Add(s);
        }

        ServoModel findServo(string n)
        {
            foreach (ServoModel r in servos)
            {
                if (r.id == n) return r;
            }
            return null;
        }

        public void  setServoPos(int n, int v)
        {
            findServo("S"+n).pos = v;
        }

        public void selectServo(int n, bool f)
        {
            findServo("S" + n).select = f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!simulation_running) this.Dispose();
        }

        private void viewport_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Click = " + e.X + e.Y + e.Button);
        }

        void Physx_loop()
        {
            RoboPhysx rp = new RoboPhysx(g3D);

            if (!rp.startPhysics())
                return;

            rp.resetScene();

            foreach (ServoModel s in servos)
            {
                if (s.joint==false && s.mod_no == 1)
                { rp.addServo(s); Console.WriteLine("servo = " + s.id); }
            }

            simulation_running = true;
            simulation_paused = false;

            while (simulation_running)	    //Main loop
            {
                if (!simulation_paused) rp.tickPhysics();

                rp.render(); ReadKeyboard();

                Application.DoEvents();
            }
            rp.killPhysics();	            //be nice and properly release the physics
        }

        private void sim_btn_Click(object sender, EventArgs e)
        {
            sim_btn.Enabled = false;

            Physx_loop();

            sim_btn.Enabled = true;
        }
    }

    public class ServoModel
    {
        public Vector3 loc;
        public Vector3 rot;
        public string id;
        public int mod_no;
        public bool joint = false;
        public int pos;
        public bool select = false;

        public Vector3 conn1;
        public Vector3 conn2;
        public Vector3 conn3;
    }


    static class UTIL
    {
        static public float clampFloat(float num, float min, float max)
        {
            if (num < min) { num = min; }
            if (num > max) { num = max; }
            return num;
        }

        static public void setMatrixPos(ref Matrix mat, Vector3 pos)
        {
            mat.M41 = pos.X;
            mat.M42 = pos.Y;
            mat.M43 = pos.Z;
        }

        static public Vector3 getMatrixZaxis(ref Matrix mat)
        {
            return new Vector3(mat.M31, mat.M32, mat.M33);
        }

        static public float DegToRads(float x)
        {
            return (float)(Math.PI / 180) * x;
        }

    }
}
