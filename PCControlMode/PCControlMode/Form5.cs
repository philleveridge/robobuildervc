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

        Vector3[] ModelSize = new Vector3[] {
            new Vector3(0.25f, 0.25f, 1),           //Cylinder
            new Vector3(1, 1.5f, 1),                //Servo box
            new Vector3(0.5f, 0.5f, 0.5f),          //hand box
            new Vector3(0.5f, 0.5f, 0.5f),          //hand box
            new Vector3(2.4f, 0.5f, 2f),            //foor box
            new Vector3(1.2f, 1f, 0.8f),            //body box
            new Vector3(1, 1, 1)                    //knee box
        };

        Microsoft.DirectX.DirectInput.Device keyb;

        bool simulation_running;
        bool simulation_paused;

        Render g3D;

        int sel_servo = 0;
        int mindex = 100;
        int jindex = 100;

        Simulator.IniManager IniData;

        KeyboardState keys;

        string hook = "";

        List<ServoModel> servos;
        List<JointModel> skeleton;

        ServoModel temp;
        bool temp_focus;


        public Form5()
        {
            InitializeComponent();

            g3D = new Render(viewPort);

            InitializeKeyboard();

            initSetup("config-20dof.txt");

            //initSetup("c-test.txt");
            //selectServo(1, true);


            foreach (ServoModel s in servos)
            {
                servo_listBox1.Items.Add(s.id);
            }
            servo_listBox1.SelectedIndex = 0;
            temp_focus = false;
        }

        public void InitializeKeyboard()
        {
            keyb = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
            keyb.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
            keyb.Acquire();
        }


        private void ReadKeyboard()
        {
            keys = keyb.GetCurrentKeyboardState();

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
            {
                simulation_paused = !simulation_paused;
                pause_msg.Visible = simulation_paused;

            }

            if (keys[Key.LeftBracket])
            {
                selectServo(sel_servo, false);
                sel_servo -= 1; if (sel_servo < 0) sel_servo = 17;
                selectServo(sel_servo, true);

            }

            if (keys[Key.RightBracket])
            {
                selectServo(sel_servo, false);
                sel_servo += 1; if (sel_servo > 17) sel_servo = 0;
                selectServo(sel_servo, true);
            }

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
                g3D.drawModel(s.mod_no, s.loc, s.rot, s.size, s.select, checkBox1.Checked);
            }

            foreach (JointModel j in skeleton)
            {
                if (j.from != null && j.to != null)
                {
                    g3D.drawline(j.from.loc, j.to.loc, j.jtype == 0 ? Color.Red : Color.Yellow, Matrix.Identity);
                }
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

            hook = IniData.getParameter("HOOK");


            servos = new List<ServoModel>();
            skeleton = new List<JointModel>();

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
                addJoint("FootLJ", "FootL", "S" + t1[3].ToString(), 0);
            }
            if ((t1 = IniData.getParameterEvalArray("FootR")).Length>2)
            {
                addModel("FootR", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 4);
                addJoint("FootRJ", "FootR", "S" + t1[3].ToString(), 0);
            }

            if ((t1 = IniData.getParameterEvalArray("HandL")).Length >2)
            {
                addModel("HandL", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 2);
                addJoint("HandLJ", "S" + t1[3].ToString(), "HandL", (int)t1[4]);
            }

            if ((t1 = IniData.getParameterEvalArray("HandR")).Length > 2)
            {
                addModel("HandR", new Vector3(t1[0], t1[1], t1[2]), new Vector3(0, 0, 0), 3);
                addJoint("HandRJ", "S" + t1[3].ToString(), "HandR", (int)t1[4]);
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
                    addJoint("Joint" + id, "S" + t1[0], "S" + t1[1], (int)t1[2]);
                }
            }

            if ((mb = IniData.getParameter("ZERO")) != "")
            {
                int n=0;
                foreach (string s in mb.Split(','))
                {
                    setZeroPos(n++, Convert.ToInt32(s));
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
            s.index = mindex++;
            s.size = ModelSize[t];
            servos.Add(s);
        }

        void addJoint(string n, string from, string to, int c)
        {
            ServoModel f = findServo(from);
            ServoModel t = findServo(to);

            JointModel j = new JointModel();
            j.id = n;
            j.jtype = c;
            j.from = f;
            j.to = t;
            j.index = jindex++;

            skeleton.Add(j);
        }

        JointModel findJoint(int servo_index)
        {
            foreach (JointModel j in skeleton)
            {
                if (j.from.index == servo_index)
                    return j;
            }
            return null;
        }

        ServoModel findServo(string n)
        {
            foreach (ServoModel r in servos)
            {
                if (r.id == n) return r;
            }
            return null;
        }

        ServoModel findServo(int n)
        {
            foreach (ServoModel r in servos)
            {
                if (r.index == n) return r;
            }
            return null;
        }

        public void  setServoPos(int n, int v)
        {
            ServoModel t = findServo("S"+n);
            if (t != null)
            {
                t.pos = v-t.zpos;
            }
        }

        public void setZeroPos(int n, int v)
        {
            ServoModel t = findServo("S" + n);
            if (t != null)
            {
                t.zpos = v;
                t.pos = 127;
            }
        }

        public void selectServo(int n, bool f)
        {
            ServoModel s = findServo("S" + n);
            if (s != null) s.select = f;
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
            ServoModel cur = null;

            RoboPhysx rp = new RoboPhysx(g3D);

            if (!rp.startPhysics())
                return;

            rp.resetScene();

            foreach (ServoModel s in servos)
            {
                rp.addServo(s);
            }

            foreach (JointModel j in skeleton) 
            {
                rp.addJoint(j);
            }

            if (hook != "") { foreach (string s in hook.Split(',')) rp.setHook(findServo(s).index, true);};

            simulation_running = true;
            simulation_paused = true; pause_msg.Visible = true;

            while (simulation_running)	    //Main loop
            {
                if (!simulation_paused) rp.tickPhysics();

                rp.render(); 
                
                ReadKeyboard();

                Tx.Focus();

                if (keys[Key.BackSpace]) rp.debug_render_on = !rp.debug_render_on;


                if (keys[Key.D] && keys[Key.LeftShift] && hook != "")
                { 
                    foreach (string s in hook.Split(',')) 
                        rp.setHook(findServo(s).index, false);
                    rp.wakeUpScene();
                };

                if (temp_focus) 
                {
                    if (cur != null) 
                        rp.selServo(cur.index, false);
                    rp.selServo(temp.index, true);
                    rp.turnServo(temp.index, temp.pos);
                    temp_focus = false;
                    cur = temp;
                }

                Application.DoEvents();
            }
            pause_msg.Visible = false;
            rp.killPhysics();	            //be nice and properly release the physics
        }

        private void sim_btn_Click(object sender, EventArgs e)
        {
            sim_btn.Enabled = false;

            Physx_loop();

            sim_btn.Enabled = true;
        }

        private void servo_listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (temp != null) temp.select = false;
            ServoModel s = findServo(servo_listBox1.SelectedItem.ToString());
            if (s != null)
            {
                sero_pos_sb.Value = s.pos;
                s.select = true;
                temp = s;
                temp_focus = true;
                sero_v_txt.Text = sero_pos_sb.Value.ToString();
            }
        }

        private void sero_pos_sb_Scroll(object sender, ScrollEventArgs e)
        {
            ServoModel s = findServo(servo_listBox1.SelectedItem.ToString());
            int n = sero_pos_sb.Value;
            if (n < 0) n = 0;
            if (n > 255) n = 255;
            if (s != null) s.pos = n;
            sero_pos_sb.Value = n;
            temp_focus = true;

            sero_v_txt.Text = sero_pos_sb.Value.ToString();
        }
    }

    public class ServoModel
    {
        public Vector3 loc;
        public Vector3 rot;
        public Vector3 size;

        public string id;
        public int mod_no;
        public int pos;
        public int zpos;
        public bool select = false;

        public JointModel[] conns;
        public int index;
    }

    public class JointModel
    {
        public string id;
        public ServoModel from;
        public ServoModel to;
        public int jtype;
        public int index;
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
