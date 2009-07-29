﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using TCVertex = Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored;	// simply our code
using PCVertex = Microsoft.DirectX.Direct3D.CustomVertex.PositionColored;


namespace RobobuilderLib
{
    public partial class Form5 : Form
    {
        public int[] servo_pos = new int[20];
        Vector3 cameraRot;
        Vector3 cameraPos;
        protected CustomVertex.PositionColored[] vertices;

        Device renderDevice = null;
        Matrix cameraMatrix;
        bool[] keyStates = new bool[256];

        float verticalFOV = 60.0f;
        float nearClipDistance = 0.01f;
        float farClipDistance = 2000.0f;

        Mesh cylinder = null;
        

        private Microsoft.DirectX.Direct3D.Mesh cubeMesh;
        private Material servoMat;

        public struct ModelData
        {
            public int object_type; //0=servo
            public string name;     //servo
            public Matrix pose;
            public Vector3 scale;
            public Mesh modelsmesh;
            public Material[] mat;
            public Texture[] txtr;
        };

        Simulator.IniManager IniData;

        ModelData[] models;
        int MaxModels = 10;

        ServoModel[] servos;

        public Form5()
        {
            InitializeComponent();
            cameraRot = new Vector3(4, 8, -14); 
            cameraPos = new Vector3(2, 10, -23);

            InitializeGraphics();

            init(); 
            
            initSetup();

            Tx.Text = cameraPos.X.ToString();
            Ty.Text = cameraPos.Y.ToString();
            Tz.Text = cameraPos.Z.ToString();

            Vx.Text = cameraRot.X.ToString();
            Vy.Text = cameraRot.Y.ToString();
            Vz.Text = cameraRot.Z.ToString();

            Tx.Focus();

        }


        public bool InitializeGraphics()
        {
            try
            {
                PresentParameters presentParams = new PresentParameters();
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;
                presentParams.AutoDepthStencilFormat = DepthFormat.D16;
                presentParams.EnableAutoDepthStencil = true;

                renderDevice = new Device(0, DeviceType.Hardware, viewPort, CreateFlags.HardwareVertexProcessing, presentParams);
                
                return true;
            }
            catch (DirectXException e)
            {
                MessageBox.Show(e.Message, "InitializeGraphics Error");
                return false;
            }
        }

        public void render()
        {
            if (renderDevice == null)
            { return; }

            renderDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            //renderDevice.Clear(ClearFlags.ZBuffer | ClearFlags.Target, System.Drawing.Color.CornflowerBlue, 1.0f, 0);
            renderDevice.BeginScene();

            drawScene();

            renderDevice.EndScene();
            renderDevice.Present();
        }

        public void drawScene()
        {
            //ensure rotation speed is independent of computer speed
            //float rotationAngle = (float)((2 * Math.PI) *
            //        (double)((Environment.TickCount % 1000) / 1000f));
            //renderDevice.Transform.World = Matrix.RotationY(rotationAngle);

            drawline(new Vector3(-100f, 0f, 0f), new Vector3(100f, 0f, 0f), Color.Red);
            drawline(new Vector3(0f, -100f, 0f), new Vector3(0f, 100f, 0f), Color.Blue);
            drawline(new Vector3(0f, 0f, -100f), new Vector3(0f, 0f, 100f), Color.White);
            
            drawplane();

            for (int s = 0; s < 30; s++)
            {
                if (servos[s] != null)
                {
                    drawModel(servos[s].mod_no, servos[s].loc, servos[s].rot);
                }
            }
            //drawModel(2, new Vector3(0, 0, 9));   
            for (int s = 30; s < 60; s++)
            {
                if (servos[s] != null)
                {
                    drawline(servos[s].loc, servos[s].rot,servos[s].mod_no==0? Color.Red:Color.Yellow);
                }
            }
            setupView();
        }

        void drawModel(int n, Vector3 loc, Vector3 rot)
        {
            Mesh m;
            if (cylinder == null) cylinder = Mesh.Cylinder(renderDevice, 0.5f, 0.5f, 2, 16, 4);

            if (checkBox1.Checked == true && (n == 5 || n == 6)) return;

            if (models[n].modelsmesh != null)
            {
                Matrix locrot = Matrix.RotationYawPitchRoll(0f, 0f, 0f);

                if (checkBox1.Checked == true && n == 1)
                {
                    m = cylinder;
                }
                else
                {
                    m = models[n].modelsmesh;
                }
                

                if (n == 1)
                {
                    locrot = Matrix.RotationYawPitchRoll(0, 0, (float)(Math.PI / 2)); //Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(y), MathHelper.ToRadians(p), MathHelper.ToRadians(r));
                    locrot *= Matrix.RotationYawPitchRoll(rot.X * (float)Math.PI / 180f, rot.Y * (float)Math.PI / 180f, rot.Z * (float)Math.PI / 180f);
                }

                Matrix wp = Matrix.Scaling(models[n].scale) * models[n].pose * locrot;

                renderDevice.Transform.World = wp * Matrix.Translation(loc) ;

                for (int i = 0; i < models[n].mat.Length; ++i)
                {
                    if (models[n].txtr[i] != null)
                    {
                        renderDevice.SetTexture(0, models[n].txtr[i]);
                    }

                    renderDevice.Material = models[n].mat[i];
                    m.DrawSubset(i);
                }
            }
        }

        void init()
        {
            renderDevice.RenderState.CullMode = Cull.None;
            renderDevice.RenderState.Lighting = true;
            renderDevice.RenderState.ZBufferEnable = true;
            renderDevice.Lights[0].Type = LightType.Directional;
            renderDevice.Lights[0].Direction = new Vector3(0.5f, -0.8f, 0.7f);
            renderDevice.Lights[0].Diffuse = Color.White;
            renderDevice.Lights[0].Specular = Color.White;
            renderDevice.Lights[0].Enabled = true;
        }

        // process Ini file
        void initSetup()
        {
            IniData = new Simulator.IniManager();
            IniData.Load("config.txt"); // use default

            loadmodels();

            string mb;

            if ((mb = IniData.getParameter("CAMERA")) != "")
            {
                string[] t = mb.Split(',');

                cameraRot = new Vector3(float.Parse(t[4]), float.Parse(t[5]), float.Parse(t[6]));
                cameraPos = new Vector3(float.Parse(t[0]), float.Parse(t[1]), float.Parse(t[2]));
            }

            servos = new ServoModel[60];

            for (int id = 0; id < 20; id++)
            {
                if (IniData.getParameter("S" + id) != "")
                {
                    //S0=  $L,  4.6,     0.2,     0,  0,  90     #Hip
                    float[] t = IniData.getParameterEvalArray("S" + id);
                    servos[id] = new ServoModel();
                    servos[id].mod_no = 1; // use servo mesh for display
                    servos[id].id = "S" + id;
                    servos[id].loc = new Vector3(t[0], t[1], t[2]);
                    servos[id].rot = new Vector3(t[3], t[4], t[5]);
                }
            }
            if (IniData.getParameter("FootL") != "")
            {
                servos[20] = new ServoModel();
                servos[20].id = "FootL";
                servos[20].mod_no = 4;
                float[] t = IniData.getParameterEvalArray("FootL");
                servos[20].loc = new Vector3(t[0], t[1], t[2]);
                servos[20].rot = new Vector3(0,0,0);
            }
            if (IniData.getParameter("FootR") != "")
            {
                servos[21] = new ServoModel();
                servos[21].id = "FootR";
                servos[21].mod_no = 4;
                float[] t = IniData.getParameterEvalArray("FootR");
                servos[21].loc = new Vector3(t[0], t[1], t[2]);
                servos[21].rot = new Vector3(0, 0, 0);
            }
            if (IniData.getParameter("HandL") != "")
            {
                servos[22] = new ServoModel();
                servos[22].id = "HandL";
                servos[22].mod_no = 2;
                float[] t = IniData.getParameterEvalArray("HandL");
                servos[22].loc = new Vector3(t[0], t[1], t[2]);
                servos[22].rot = new Vector3(0, 0, 0);
            }
            if (IniData.getParameter("HandR") != "")
            {
                servos[23] = new ServoModel();
                servos[23].id = "HandR";
                servos[23].mod_no = 3;
                float[] t = IniData.getParameterEvalArray("HandR");
                servos[23].loc = new Vector3(t[0], t[1], t[2]);
                servos[23].rot = new Vector3(0, 0, 0);
            }
            if (IniData.getParameter("Body") != "")
            {
                servos[26] = new ServoModel();
                servos[26].id = "Body";
                servos[26].mod_no = 5;
                float[] t = IniData.getParameterEvalArray("Body");
                servos[26].loc = new Vector3(t[0], t[1], t[2]);
                servos[26].rot = new Vector3(0, 0, 0);
            }
            if (IniData.getParameter("KneeL") != "")
            {
                servos[24] = new ServoModel();
                servos[24].id = "KneeL";
                servos[24].mod_no = 6;
                float[] t = IniData.getParameterEvalArray("KneeL");
                servos[24].loc = new Vector3(t[0], t[1], t[2]);
                servos[24].rot = new Vector3(0, 0, 0);
            }
            if (IniData.getParameter("KneeR") != "")
            {
                servos[25] = new ServoModel();
                servos[25].id = "KneeR";
                servos[25].mod_no = 6;
                float[] t = IniData.getParameterEvalArray("KneeR");
                servos[25].loc = new Vector3(t[0], t[1], t[2]);
                servos[25].rot = new Vector3(0, 0, 0);
            }
            for (int id = 0; id < 30; id++)
            {
                if (IniData.getParameter("Joint" + id) != "")
                {
                    //S0=  $L,  4.6,     0.2,     0,  0,  90     #Hip
                    float[] t = IniData.getParameterEvalArray("Joint" + id);
                    servos[30+id] = new ServoModel();
                    servos[30 + id].mod_no = (int)t[2]; // use servo mesh for display
                    servos[30+id].id = "Joint" + id;
                    servos[30+id].loc = servos[(int)t[0]].loc;
                    servos[30+id].rot = servos[(int)t[1]].loc;
                }
            }
        }

        void loadmodels()
        {
            string mb;
           
            models = new ModelData[MaxModels];

            for (int id = 0; id < MaxModels; id++)
            {
                if ((mb = IniData.getParameter("M" + id)) != "")
                {
                    /*
                    #Model, scale vec, rot vec, translate
                    M1=servo,servo6,0.55, 0.55, 0.55, 0, 0, 0
                    */
                    string[] n = mb.Split(',');

                    models[id].name = n[0];
                    models[id].pose = Matrix.RotationYawPitchRoll(
                        float.Parse(n[5].Trim()) * (float)(Math.PI/180),
                        -float.Parse(n[6].Trim()) * (float)(Math.PI / 180),
                        float.Parse(n[7].Trim()) * (float)(Math.PI / 180));
                    models[id].scale = new Vector3(
                        float.Parse(n[2].Trim()),
                        float.Parse(n[3].Trim()),
                        float.Parse(n[4].Trim()));

                    if (n.Length > 8)
                    {
                        models[id].pose *= Matrix.Translation(
                            float.Parse(n[8].Trim()),
                            float.Parse(n[9].Trim()),
                            float.Parse(n[10].Trim()));
                    }

                    ExtendedMaterial[] exMaterials;
                    models[id].modelsmesh = Mesh.FromFile("Models\\" + n[1] + ".x", MeshFlags.Managed, renderDevice, out exMaterials);

                    if (models[id].txtr != null)
                    {
                        //DisposeTextures();
                    }

                    models[id].txtr = new Texture[exMaterials.Length];
                    models[id].mat = new Material[exMaterials.Length];

                    for (int i = 0; i < exMaterials.Length; ++i)
                    {
                        if (exMaterials[i].TextureFilename != null)
                        {
                            string texturePath =
                             Path.Combine(Path.GetDirectoryName(n[1]), exMaterials[i].TextureFilename);
                            models[id].txtr[i] = TextureLoader.FromFile(renderDevice, texturePath);
                        }
                        models[id].mat[i] = exMaterials[i].Material3D;
                        models[id].mat[i].Ambient = models[id].mat[i].Diffuse;
                    }
                }
            }
        }

        public void setupView()
        {
            Vector3 tv = new Vector3(0, 0, -5);
            Vector3 hv = new Vector3(0, 0, 0);
            Vector3 upDir = new Vector3(0, 1, 0);

            try
            {
                tv = new Vector3(Convert.ToInt32(Tx.Text), Convert.ToInt32(Ty.Text), Convert.ToInt32(Tz.Text));
                hv = new Vector3(Convert.ToInt32(Vx.Text), Convert.ToInt32(Vy.Text), Convert.ToInt32(Vz.Text));
            }
            catch (Exception e1) { }

            renderDevice.Transform.View = Matrix.LookAtLH(
               tv,	 // target location
               hv,	 // eye/camera location
               upDir);	// “up” axis

            float aspectRatio = ((float)viewPort.ClientSize.Width) / ((float)viewPort.ClientSize.Height);
            cameraMatrix = Matrix.Translation(cameraPos) * Matrix.RotationYawPitchRoll(cameraRot.Y * (float)(Math.PI / 180), -cameraRot.X * (float)(Math.PI / 180), 0);
            Vector3 dir = new Vector3(0, 1, 0);
            Vector3 target = cameraPos + dir;

            //renderDevice.Transform.View = Matrix.LookAtLH(cameraPos, target, upDir);
            renderDevice.Transform.Projection = Matrix.PerspectiveFovLH(verticalFOV * (float)(Math.PI / 180), aspectRatio, nearClipDistance, farClipDistance);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void drawline(Vector3 src, Vector3 dest, Color colour) //, Matrix viewProjection)
        {
            vertices = new CustomVertex.PositionColored[2];
            vertices[0] = new CustomVertex.PositionColored(src, colour.ToArgb());
            vertices[1] = new CustomVertex.PositionColored(dest, colour.ToArgb());

            //Cache zBuffer and lighting states
            bool last_ZBufferEnabled = renderDevice.RenderState.ZBufferEnable;
            bool last_lighting = renderDevice.RenderState.Lighting;

            renderDevice.RenderState.Lighting = false;

            Material m = new Material();
            m.Diffuse = colour;
            m.Emissive = colour;

            renderDevice.Transform.World = Matrix.Identity;

            //Set our VertexFormat so the device knows the format of the array we pass in
            renderDevice.VertexFormat = CustomVertex.PositionColored.Format;


            renderDevice.Material = m;
            renderDevice.DrawUserPrimitives(PrimitiveType.LineList, //The type of primitive we're rendering
                                        1,         //The number of primitives we're rendering
                                        vertices);             //The data to render

            //put zBuffer and lighting back the way they were
            renderDevice.RenderState.ZBufferEnable = last_ZBufferEnabled;
            renderDevice.RenderState.Lighting = last_lighting;

        }

        public void drawplane()
        {
            Color color = Color.DarkOliveGreen;

            CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[4];

            vertices[0].Position = new Vector3(-100, 0, -100);
            vertices[0].Color = color.ToArgb();
            vertices[1].Position = new Vector3(100, 0, -100);
            vertices[1].Color = color.ToArgb();
            vertices[2].Position = new Vector3(100, 0, 100);
            vertices[2].Color = color.ToArgb();
            vertices[3].Position = new Vector3(-100, 0, 100);
            vertices[3].Color = color.ToArgb();

            //Cache zBuffer and lighting states
            bool last_ZBufferEnabled = renderDevice.RenderState.ZBufferEnable;
            bool last_lighting = renderDevice.RenderState.Lighting;

            Material m = new Material();
            m.Diffuse = color;
            m.Emissive = color;

            renderDevice.RenderState.Lighting = true;
            renderDevice.RenderState.CullMode = Cull.None;    // Or this one...
            renderDevice.RenderState.Ambient = Color.Crimson;

            renderDevice.Transform.World = Matrix.Identity;
            renderDevice.VertexFormat = CustomVertex.PositionColored.Format;
            renderDevice.Material = m;
            renderDevice.DrawUserPrimitives(PrimitiveType.TriangleFan, 4, vertices);

            //put zBuffer and lighting back the way they were
            renderDevice.RenderState.ZBufferEnable = last_ZBufferEnabled;
            renderDevice.RenderState.Lighting = last_lighting;
        }

        private void viewport_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Click = " + e.X + e.Y + e.Button);
        }

        private void clearKeyStates()
        {
            for (int i = 0; i < 256; i++)
            { keyStates[i] = false; }
        }

        public void processKeys()
        {
            if (this != Form.ActiveForm)	//Lost focus so reset the keys
            { clearKeyStates(); }

            if (keyStates[(int)Keys.NumPad1])
            {
                //int p = servos[current_servo].getPos();
                //setServoValue(current_servo, p + 1);
                keyStates[(int)Keys.NumPad1] = false;
            }


            if (keyStates[(int)Keys.V])
            {
                //bodyvis = !bodyvis;
                keyStates[(int)Keys.V] = false;
            }


            if (keyStates[(int)Keys.NumPad3])
            {
                //int p = servos[current_servo].getPos();
                //setServoValue(current_servo, p - 1);
                keyStates[(int)Keys.NumPad3] = false;
            }
            if (keyStates[(int)Keys.Insert])
            {
                // next servo in use (i.e. non-null)
                //do { current_servo = (current_servo + 1) % 16; } while (servos[current_servo] == null);
                //selectedActor = servos[current_servo].getActor();
                //setCS(current_servo);
                keyStates[(int)Keys.Insert] = false;
            }
            if (keyStates[(int)Keys.Delete])
            {
                //do
                //{
                //    current_servo = (current_servo - 1) % 16;
                //    if (current_servo < 0) current_servo = 15;
                //} while (servos[current_servo] == null);
                //selectedActor = servos[current_servo].getActor();
                //setCS(current_servo);

                keyStates[(int)Keys.Delete] = false;
            }

            driveObject(ref cameraRot, ref cameraPos, false);
        }

        void driveObject(ref Vector3 objectRot, ref Vector3 objectPos, bool alternateKeysFlag)
        {
            float driveScale = 0.2f;
            float rotateScale = 2.0f;
            Vector3 driveInput = new Vector3(0, 0, 0);
            Vector3 rotateInput = new Vector3(0, 0, 0);

            if (keyStates[(int)Keys.ShiftKey])
            {
                driveScale *= 3.0f;
                rotateScale *= 2.0f;
            }

            if (!alternateKeysFlag)
            {
                if (keyStates[(int)Keys.W])
                { driveInput.Z += 1; }
                if (keyStates[(int)Keys.S])
                { driveInput.Z -= 1; }
                if (keyStates[(int)Keys.A])
                { driveInput.X -= 1; }
                if (keyStates[(int)Keys.D])
                { driveInput.X += 1; }
                if (keyStates[(int)Keys.R])
                { driveInput.Y += 1; }
                if (keyStates[(int)Keys.F])
                { driveInput.Y -= 1; }
                if (keyStates[(int)Keys.Q] || keyStates[(int)Keys.Left])
                { rotateInput.Y -= 1; }
                if (keyStates[(int)Keys.E] || keyStates[(int)Keys.Right])
                { rotateInput.Y += 1; }
                if (keyStates[(int)Keys.Up])
                { rotateInput.X += 1; }
                if (keyStates[(int)Keys.Down])
                { rotateInput.X -= 1; }
            }
            else
            {
                if (keyStates[(int)Keys.I])
                { driveInput.Z += 1; }
                if (keyStates[(int)Keys.K])
                { driveInput.Z -= 1; }
                if (keyStates[(int)Keys.J])
                { driveInput.X -= 1; }
                if (keyStates[(int)Keys.L])
                { driveInput.X += 1; }
                if (keyStates[(int)Keys.Y])
                { driveInput.Y += 1; }
                if (keyStates[(int)Keys.H])
                { driveInput.Y -= 1; }
                if (keyStates[(int)Keys.U])
                { rotateInput.Y -= 1; }
                if (keyStates[(int)Keys.O])
                { rotateInput.Y += 1; }
            }

            driveInput *= driveScale;
            rotateInput *= rotateScale;

            /*
           objectRot.X  += rotateInput.X;	//clamp angle between -85 and 85 degrees
           objectRot.Y += rotateInput.Y;

           Vector3 dir = new Vector3(1, 0, 0); //((float)Math.Sin(NovodexUtil.DEG_TO_RAD * objectRot.Y), 0, (float)Math.Cos(NovodexUtil.DEG_TO_RAD * objectRot.Y));
           Vector3 perpDir = new Vector3(dir.Z, 0, -dir.X);
           Vector3 upDir = new Vector3(0, 1, 0);

           objectPos = objectPos + (dir * driveInput.Z) + (perpDir * driveInput.X) + (upDir * driveInput.Y);
           */

            Tx.Text = (Convert.ToInt32(Tx.Text) + driveInput.X).ToString();
            Ty.Text = (Convert.ToInt32(Ty.Text) + driveInput.Y).ToString();
            Tz.Text = (Convert.ToInt32(Tz.Text) + driveInput.Z).ToString();

            Vx.Text = (Convert.ToInt32(Vx.Text) + rotateInput.X).ToString();
            Vy.Text = (Convert.ToInt32(Vy.Text) + rotateInput.Y).ToString();
            Vz.Text = (Convert.ToInt32(Vz.Text) + rotateInput.Z).ToString();
        }

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        protected override bool ProcessKeyPreview(ref Message m)	//This is required because the viewport panel receives no key events.
        {
            //if(startedFlag)
            {
                switch (m.Msg)
                {
                    case WM_KEYDOWN:
                        if ((((uint)m.LParam) >> 30 & 1) == 0)
                        { keyStates[(int)m.WParam] = true; }
                        break;
                    case WM_KEYUP:
                        keyStates[(int)m.WParam] = false;
                        break;
                }
            }
            return base.ProcessKeyPreview(ref m);
        }
    }

    class ServoModel
    {
        public Vector3 loc;
        public Vector3 rot;
        public string id;
        public int mod_no;
    }
}
