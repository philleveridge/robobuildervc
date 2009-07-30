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

using TCVertex = Microsoft.DirectX.Direct3D.CustomVertex.TransformedColored;	// simply our code
using PCVertex = Microsoft.DirectX.Direct3D.CustomVertex.PositionColored;


namespace RobobuilderLib
{
    public partial class Form5 : Form
    {
        Vector3 cameraRot;
        Vector3 cameraPos;
        protected CustomVertex.PositionColored[] vertices;

        Microsoft.DirectX.DirectInput.Device keyb;
        Microsoft.DirectX.Direct3D.Device renderDevice = null;

        Matrix cameraMatrix;

        float verticalFOV = 60.0f;
        float nearClipDistance = 0.01f;
        float farClipDistance = 2000.0f;

        Mesh cylinder = null;

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

        List<ServoModel> servos;

        public Form5()
        {
            InitializeComponent();

            InitializeGraphics();

            InitializeKeyboard();

            init();

            //initSetup("config-t.txt");
            initSetup("config.txt");
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

                renderDevice = new Microsoft.DirectX.Direct3D.Device(0, Microsoft.DirectX.Direct3D.DeviceType.Hardware, viewPort, CreateFlags.HardwareVertexProcessing, presentParams);

                return true;
            }
            catch (DirectXException e)
            {
                MessageBox.Show(e.Message, "InitializeGraphics Error");
                return false;
            }
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
        
        }

        public void render()
        {
            if (renderDevice == null)
            { return; }

            renderDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            renderDevice.BeginScene();

            drawScene();

            renderDevice.EndScene();
            renderDevice.Present();

            ReadKeyboard();
        }

        public void drawScene()
        {
            // draw axis
            drawline(new Vector3(-100f, 0f, 0f), new Vector3(100f, 0f, 0f), Color.Red, Matrix.Identity);
            drawline(new Vector3(0f, -100f, 0f), new Vector3(0f, 100f, 0f), Color.Blue, Matrix.Identity);
            drawline(new Vector3(0f, 0f, -100f), new Vector3(0f, 0f, 100f), Color.White, Matrix.Identity);

            // ground plan
            drawplane();

            //object and joints
            foreach (ServoModel s in servos)
            {
                if (s.joint)
                    drawline(s.loc, s.rot, s.mod_no == 0 ? Color.Red : Color.Yellow, Matrix.Identity);
                else
                    drawModel(s.mod_no, s.loc, s.rot);
            }
            //set camera
            setupView();
        }

        public void setupView()
        {
            Vector3 upDir = new Vector3(0, 1, 0);

            float aspectRatio = ((float)viewPort.ClientSize.Width) / ((float)viewPort.ClientSize.Height);
            cameraMatrix = Matrix.Translation(cameraPos) * Matrix.RotationYawPitchRoll(cameraRot.Y * (float)(Math.PI / 180), -cameraRot.X * (float)(Math.PI / 180), 0);

            UTIL.setMatrixPos(ref cameraMatrix, cameraPos);

            Vector3 dir = UTIL.getMatrixZaxis(ref cameraMatrix);
            Vector3 target = cameraPos + dir;

            renderDevice.Transform.View = Matrix.LookAtLH(target, cameraPos, upDir);
            renderDevice.Transform.Projection = Matrix.PerspectiveFovLH(verticalFOV * (float)(Math.PI / 180), aspectRatio, nearClipDistance, farClipDistance);
        }



        void drawModel(int n, Vector3 loc, Vector3 rot)
        {
            Mesh m;
            if (cylinder == null) cylinder = Mesh.Cylinder(renderDevice, 0.25f, 0.25f, 1, 16, 4);

            if (checkBox1.Checked == true && (n == 5 || n == 6)) return;

            if (models[n].modelsmesh != null)
            {
                Matrix locrot = Matrix.RotationYawPitchRoll(0f, 0f, 0f);

                m = models[n].modelsmesh;

                if (n == 1)
                {
                    locrot = Matrix.RotationYawPitchRoll(0, 0, (float)(Math.PI / 2)); 
                    locrot *= Matrix.RotationYawPitchRoll(UTIL.DegToRads(rot.X ), UTIL.DegToRads(rot.Y), UTIL.DegToRads(rot.Z));
                }

                Matrix wp;

                if (checkBox1.Checked == true && n == 1)
                {
                    m = cylinder;
                    drawBoxOutline(loc.X,loc.Y,loc.Z, 1f, 1.5f, 1f, Color.White, locrot);
                    renderDevice.Transform.World = locrot * Matrix.Translation(loc.X, loc.Y, loc.Z) ;
                }
                else
                {
                    wp = Matrix.Scaling(models[n].scale) * models[n].pose * locrot;
                    renderDevice.Transform.World = wp * Matrix.Translation(loc);
                }


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

            cameraPos = new Vector3(7.5f, 10, -11);
            cameraRot = new Vector3(30, 170, -14);
        }

        // process Ini file
        void initSetup(string fn)
        {
            float[] t1;

            IniData = new Simulator.IniManager();
            IniData.Load(fn); // use default

            loadmodels();

            string mb;

            if ((mb = IniData.getParameter("CAMERA")) != "")
            {
                string[] t = mb.Split(',');

                cameraRot = new Vector3(float.Parse(t[4]), float.Parse(t[5]), float.Parse(t[6]));
                cameraPos = new Vector3(float.Parse(t[0]), float.Parse(t[1]), float.Parse(t[2]));
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
                        float.Parse(n[5].Trim()) * (float)(Math.PI / 180),
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        void drawBoxOutline(float x, float y, float z, float h, float w, float d, Color c, Matrix wp)
        {
            Vector3 loc = new Vector3(x, y, z);
            x = -w/2; y = -h/2; z = -d/2;

            wp *= Matrix.Translation(loc);

            drawline(new Vector3(x, y, z), new Vector3(x + w, y, z), c, wp);
            drawline(new Vector3(x, y, z), new Vector3(x, y + h, z), c, wp);
            drawline(new Vector3(x, y, z), new Vector3(x, y, z + d), c, wp);

            drawline(new Vector3(x + w, y + h, z), new Vector3(x + w, y + h, z + d), c, wp);
            drawline(new Vector3(x + w, y + h, z), new Vector3(x + w, y, z), c, wp);
            drawline(new Vector3(x + w, y + h, z), new Vector3(x, y + h, z), c, wp);

            drawline(new Vector3(x + w, y, z + d), new Vector3(x + w, y, z), c, wp);
            drawline(new Vector3(x + w, y, z + d), new Vector3(x, y, z + d), c, wp);
            drawline(new Vector3(x + w, y, z + d), new Vector3(x + w, y + h, z + d), c, wp);

            drawline(new Vector3(x, y + h, z + d), new Vector3(x + w, y + h, z + d), c, wp);
            drawline(new Vector3(x, y + h, z + d), new Vector3(x, y, z + d), c, wp);
            drawline(new Vector3(x, y + h, z + d), new Vector3(x, y + h, z), c, wp);
        }

        public void drawline(Vector3 src, Vector3 dest, Color colour, Matrix wp) //, Matrix viewProjection)
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

            renderDevice.Transform.World = wp;

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
            return (float)(Math.PI / 180)*x;
        }
    }

    class ServoModel
    {
        public Vector3 loc;
        public Vector3 rot;
        public string id;
        public int mod_no;
        public bool joint = false;
        public int pos;
    }
}
