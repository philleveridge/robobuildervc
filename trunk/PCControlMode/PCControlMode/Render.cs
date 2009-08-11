using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;

namespace RobobuilderLib
{
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

    class Render
    {
        public Microsoft.DirectX.Direct3D.Device Device;
        Vector3 cameraRot;
        Vector3 cameraPos;
        Matrix cameraMatrix;
        protected CustomVertex.PositionColored[] vertices;

        float verticalFOV = 60.0f;
        float nearClipDistance = 0.01f;
        float farClipDistance = 2000.0f; 
        Panel viewPort;

        ModelData[] models;
        int MaxModels = 10;
        Mesh cylinder = null;


        public Render(Panel vp)
        {
            viewPort = vp;
            InitializeGraphics();
            init();
        }

        void init()
        {
            Device.RenderState.CullMode = Cull.None;
            Device.RenderState.Lighting = true;
            Device.RenderState.ZBufferEnable = true;
            Device.Lights[0].Type = LightType.Directional;
            Device.Lights[0].Direction = new Vector3(0.5f, -0.8f, 0.7f);
            Device.Lights[0].Diffuse = Color.White;
            Device.Lights[0].Specular = Color.White;
            Device.Lights[0].Enabled = true;

            cameraPos = new Vector3(7.5f, 10, -11);
            cameraRot = new Vector3(30, 170, -14);
        }

        public void updateCamera(Vector3 p, Vector3 r)
        {
            cameraPos = p; cameraRot = r;
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

                Device = new Microsoft.DirectX.Direct3D.Device(0, Microsoft.DirectX.Direct3D.DeviceType.Hardware, viewPort, CreateFlags.HardwareVertexProcessing, presentParams);

                return true;
            }
            catch (DirectXException e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "InitializeGraphics Error");
                return false;
            }
        }

        public void setupView()
        {
            Vector3 upDir = new Vector3(0, 1, 0);

            float aspectRatio = ((float)viewPort.ClientSize.Width) / ((float)viewPort.ClientSize.Height);
            cameraMatrix = Matrix.Translation(cameraPos) * Matrix.RotationYawPitchRoll(cameraRot.Y * (float)(Math.PI / 180), -cameraRot.X * (float)(Math.PI / 180), 0);

            UTIL.setMatrixPos(ref cameraMatrix, cameraPos);

            Vector3 dir = UTIL.getMatrixZaxis(ref cameraMatrix);
            Vector3 target = cameraPos + dir;

            Device.Transform.View = Matrix.LookAtLH(target, cameraPos, upDir);
            Device.Transform.Projection = Matrix.PerspectiveFovLH(verticalFOV * (float)(Math.PI / 180), aspectRatio, nearClipDistance, farClipDistance);
        }

        public void drawBoxOutline(float x, float y, float z, float h, float w, float d, Color c, Matrix wp)
        {
            Vector3 loc = new Vector3(x, y, z);
            x = (-w / 2) + 0.25f; y = (-h / 2); z = (-d / 2);

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
            bool last_ZBufferEnabled = Device.RenderState.ZBufferEnable;
            bool last_lighting = Device.RenderState.Lighting;

            Device.RenderState.Lighting = false;

            Material m = new Material();
            m.Diffuse = colour;
            m.Emissive = colour;

            Device.Transform.World = wp;

            //Set our VertexFormat so the device knows the format of the array we pass in
            Device.VertexFormat = CustomVertex.PositionColored.Format;


            Device.Material = m;
            Device.DrawUserPrimitives(PrimitiveType.LineList,   //The type of primitive we're rendering
                                        1,                      //The number of primitives we're rendering
                                        vertices);              //The data to render

            //put zBuffer and lighting back the way they were
            Device.RenderState.ZBufferEnable = last_ZBufferEnabled;
            Device.RenderState.Lighting = last_lighting;

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
            bool last_ZBufferEnabled = Device.RenderState.ZBufferEnable;
            bool last_lighting = Device.RenderState.Lighting;

            Material m = new Material();
            m.Diffuse = color;
            m.Emissive = color;

            Device.RenderState.Lighting = true;
            Device.RenderState.CullMode = Cull.None;    // Or this one...
            Device.RenderState.Ambient = Color.Crimson;

            Device.Transform.World = Matrix.Identity;
            Device.VertexFormat = CustomVertex.PositionColored.Format;
            Device.Material = m;
            Device.DrawUserPrimitives(PrimitiveType.TriangleFan, 4, vertices);

            //put zBuffer and lighting back the way they were
            Device.RenderState.ZBufferEnable = last_ZBufferEnabled;
            Device.RenderState.Lighting = last_lighting;
        }

        public void drawCylinder(Vector3 loc, Matrix pose, bool sel, bool hide)
        {
            if (cylinder == null) cylinder = Mesh.Cylinder(Device, 0.25f, 0.25f, 1, 16, 4);
            Device.Transform.World = pose * Matrix.Translation(loc.X, loc.Y, loc.Z);

            Device.SetTexture(0, models[1].txtr[0]);
            Material t = models[1].mat[0];

            if (sel)
            {
                t.Diffuse = Color.Yellow;
            }

            Device.Material = t;
            cylinder.DrawSubset(0);
        }

        public void drawMesh(int n, Vector3 loc, Matrix pose, bool sel, bool hide)
        {
            Mesh m;

            if (models[n].modelsmesh != null)
            {
                Matrix locrot = pose;

                m = models[n].modelsmesh;              

                Matrix wp = Matrix.Scaling(models[n].scale) * models[n].pose * locrot;
                Device.Transform.World = wp * Matrix.Translation(loc);

                for (int i = 0; i < models[n].mat.Length; ++i)
                {
                    if (models[n].txtr[i] != null)
                    {
                        Device.SetTexture(0, models[n].txtr[i]);
                    }

                    Material t = models[n].mat[i];

                    if (sel)
                    {
                        t.Diffuse = Color.Yellow;
                    }

                    Device.Material = t;
                    m.DrawSubset(i);
                }
            }
        }

        public void drawModel(int n, Vector3 loc, Vector3 rot, bool sel, bool hide)
        {
            Matrix locrot = Matrix.RotationYawPitchRoll(0f, 0f, 0f);

            if (hide)
            {
                if (n == 1)
                {
                    locrot = Matrix.RotationYawPitchRoll(0, 0, (float)(Math.PI / 2));
                    locrot *= Matrix.RotationYawPitchRoll(UTIL.DegToRads(rot.X), UTIL.DegToRads(rot.Y), UTIL.DegToRads(rot.Z));
                    drawCylinder(loc, locrot, sel, hide);
                    drawBoxOutline(loc.X, loc.Y, loc.Z, 1f, 1.5f, 1f, (sel) ? Color.Red : Color.White, locrot);
                }
                else
                {
                    if (n == 5 || n == 6) return;
                    drawMesh(n, loc, locrot, sel, hide);
                }               
            }
            else
            {
                if (n == 1)
                {
                    locrot = Matrix.RotationYawPitchRoll(0, 0, (float)(Math.PI / 2));
                    locrot *= Matrix.RotationYawPitchRoll(UTIL.DegToRads(rot.X), UTIL.DegToRads(rot.Y), UTIL.DegToRads(rot.Z));
                }
                drawMesh(n, loc, locrot, sel, hide);
            }

        }


        public void loadmodels(Simulator.IniManager IniData)
        {
            string mb;

            models = new ModelData[MaxModels];

            for (int id = 0; id < MaxModels; id++)
            {
                if ((mb = IniData.getParameter("M" + id)) != "")
                {
                    /*
                    #Model, scale vec, rot vec, translate
                    Eg:  M1=servo,servo6,0.55, 0.55, 0.55, 0, 0, 0
                    */
                    string[] n = mb.Split(',');

                    models[id].name = n[0];
                    models[id].pose = Matrix.RotationYawPitchRoll(
                        UTIL.DegToRads(float.Parse(n[5].Trim())),
                        UTIL.DegToRads(-float.Parse(n[6].Trim())),
                        UTIL.DegToRads(float.Parse(n[7].Trim())));

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
                    models[id].modelsmesh = Mesh.FromFile("Models\\" + n[1] + ".x", MeshFlags.Managed, Device, out exMaterials);

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
                            System.IO.Path.Combine(System.IO.Path.GetDirectoryName(n[1]), exMaterials[i].TextureFilename);
                            models[id].txtr[i] = TextureLoader.FromFile(Device, texturePath);
                        }
                        models[id].mat[i] = exMaterials[i].Material3D;
                        models[id].mat[i].Ambient = models[id].mat[i].Diffuse;
                    }
                }
            }
        }
    }

}
