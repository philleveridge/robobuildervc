﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using NovodexWrapper;


namespace RobobuilderLib
{
    class RoboPhysx
    {
        protected List<NxJoint> joints = new List<NxJoint>();
        protected List<NxActor> actors = new List<NxActor>();

        NxPhysicsSDK physicsSDK = null;
        NxScene physicsScene = null;
        Render physics3D = null;				
        Vector3 physicsGravity = new Vector3(0, -9.8f, 0);
        float visualizeScale = 1; //0.5f;

        //There's an array which is used to smooth out the framerate for the physics timestep
        float lastTime = 0;
        float lastTimeStep = 0;
        int timeStepIndex = 0;
        float[] timeStepArray = null;

        Hashtable bfalgs = new Hashtable();

        NxScene scene;

        Simulator.MyDebugRenderer dbr = null;

        public bool debug_render_on;

        public RoboPhysx(Render r)
        {
            dbr = new Simulator.MyDebugRenderer(r.Device);
            debug_render_on = false;

            physics3D = r; 


            //Initialize the array to a reasonable target frame rate of 60 FPS.
            timeStepArray = new float[32];
            for (int i = 0; i < timeStepArray.Length; i++)
            { 
                timeStepArray[i] = 1 / 20.0f; 
            }
        }

        public bool startPhysics()
        {
            physicsSDK = NxPhysicsSDK.Create();
            if (physicsSDK == null)
            {
                MessageBox.Show("Failed to start physics", "Error", MessageBoxButtons.OK);
                this.Close();
                return false;
            }

            //There are several createScene() methods. In this case the simplest is used.
            physicsScene = physicsSDK.createScene(physicsGravity);

            if (physicsScene == null)
            {
                MessageBox.Show("Failed to create scene", "Error", MessageBoxButtons.OK);
                this.Close();
                return false;
            }

            scene = physicsScene;

            //These need to be set for the debugRenderer to have anything to render
            physicsSDK.setParameter(NxParameter.NX_VISUALIZATION_SCALE, visualizeScale);	//Things like actor axes and joints will be drawn to the size of visualizeScale
            physicsSDK.setParameter(NxParameter.NX_VISUALIZE_ACTOR_AXES, 1);				//Set to non-zero to visualize
            physicsSDK.setParameter(NxParameter.NX_VISUALIZE_JOINT_LIMITS, 1);
            physicsSDK.setParameter(NxParameter.NX_VISUALIZE_BODY_JOINT_LIST, 1);
            physicsSDK.setParameter(NxParameter.NX_VISUALIZE_COLLISION_SHAPES, 1);

            //Set the skin width to 0.01 meters
            physicsSDK.setParameter(NxParameter.NX_SKIN_WIDTH, 0.01f);
            physicsSDK.setParameter(NxParameter.NX_VISUALIZE_ACTIVE_VERTICES, 1);

            return true;
        }


        public void killPhysics()
        {
            if (physicsSDK != null)
            {
                if (physicsScene != null)
                { physicsSDK.releaseScene(physicsScene); }
                physicsSDK.release();
                physicsSDK = null;
                physicsScene = null;
            }
        }

        //This uses an array to cache and average timeSteps to maintain a smooth simulation.
        public float getTimeStep()
        {
            float time = NovodexUtil.getTimeInSeconds();
            float deltaTime = time - lastTime;
            deltaTime = UTIL.clampFloat(deltaTime, 1 / 200.0f, 1 / 30.0f);	//Clamp deltaTime between 200 FPS and 30 FPS. This disallow freakish numbers to effect the smoothing. (Like the first deltaTime and any large time caused by pausing the physics)
            lastTime = time;

            timeStepArray[timeStepIndex] = deltaTime;
            timeStepIndex = (timeStepIndex + 1) % timeStepArray.Length;

            float sum = 0;
            for (int i = 0; i < timeStepArray.Length; i++)
            { sum += timeStepArray[i]; }

            float timeStep = sum / timeStepArray.Length;	//Return the average of timeStepArray
            lastTimeStep = timeStep;
            return timeStep;
        }

        public float getLastTimeStep()
        { return lastTimeStep; }

        public void tickPhysics()
        {
            physicsScene.simulate(getTimeStep());	//Run the physics for X seconds
            physicsScene.flushStream();				//Flush any commands that haven't been run yet
            physicsScene.fetchResults(NxSimulationStatus.NX_RIGID_BODY_FINISHED, true);	//Get the results of the simulation which is required before the next call to simulate()
        }

        public void Close()
        {
            killPhysics();
        }

        //This will delete all the actors in the scene and then rebuild the start scene and reset the camera
        public void resetScene()
        {
            //Get a list of all the triangleMeshes, convexMeshes, and heightFields associated with this scene so I can release them
            System.Collections.ArrayList meshList = NovodexUtil.getAllMeshesAssociatedWithScene(physicsScene, true, true, true);

            ControllerManager.purgeControllers();	//Put this above releaseAllActorsFromScene() because it deletes the actors associated with the controllers. If it is put below Novodex will complain about releasing the actors in the controllers twice. It won't crash, it just complains.
            NovodexUtil.releaseAllActorsFromScene(physicsScene); //When an actor is released any joint attached to it will be released. Releasing all actors will release all joints.
            NovodexUtil.releaseAllClothsFromScene(physicsScene);
            NovodexUtil.releaseAllMaterialsFromScene(physicsScene);

            //Just to be safe wait until after the actors were released before releasing the triangleMeshes, convexMeshes, and heightFields.
            NovodexUtil.releaseMeshes(physicsSDK, meshList);
            NovodexUtil.seedRandom(03082009);

            createPhysicsStuff();
            tickPhysics(); //Call tick once so the renderer looks correct even if the physics is paused
        }

        //This manually wakes up every object in the scene. When you turn gravity back on it is possible for motionless actors to remain floating in the air. That's why when you hit the gravity checkBox it will call this
        public void wakeUpScene()
        {
            NxActor[] actorArray = physicsScene.getActors();
            foreach (NxActor actor in actorArray)
            { actor.wakeUp(); }
        }

        public void createPhysicsStuff()
        {
            //Make it so the default material isn't frictionless
            physicsScene.setDefaultMaterial(new NxMaterialDesc(0.5f, 0.6f, 0.0f));

            //Create static ground plane using descriptors
            NxPlaneShapeDesc planeDesc = NxPlaneShapeDesc.Default;	//The default plane is the ground plane so no parameters need to be changed
            NxBodyDesc bodyDesc = null;								//using a null bodyDesc makes the actor static
            NxActorDesc planeActorDesc = new NxActorDesc(planeDesc, bodyDesc, 1, Matrix.Identity);
            NxActor planeActor = physicsScene.createActor(planeActorDesc);
            planeActor.Name = "Ground_Plane";
        }

        public void render()
        {
            if (physics3D.Device == null)  return;
            physics3D.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            physics3D.Device.BeginScene();

            physics3D.setupView();
            if (dbr != null && debug_render_on == true)
            {
                dbr.renderData(physicsScene.getDebugRenderable());
            }
            else
            {
                RenderActors();
                RenderJoints();
            }
            physics3D.Device.EndScene();
            physics3D.Device.Present();
        }

        void RenderJoints()
        {
            // Render all the actors in the scene 
            NxJoint[] jts = scene.getJoints();
            //Console.WriteLine("N=" + jts.Length);              

            foreach (NxJoint j in jts)
            {
                Color c = Color.Blue;
                if (j.getJointType() == NxJointType.NX_JOINT_FIXED) 
                    c = Color.Red;

                if (j.getJointType() == NxJointType.NX_JOINT_REVOLUTE) 
                    c = Color.Yellow;

                NxActor a;
                NxActor b;
                j.getActors(out a, out b);

                physics3D.drawline(a.getGlobalPosition(), b.getGlobalPosition(), c, Matrix.Identity);

                if (j.getJointType() == NxJointType.NX_JOINT_REVOLUTE)
                {
                    Matrix r = a.getShape(0).getLocalOrientation(); 
                    physics3D.drawCylinder(j.getGlobalAnchor(), new Vector3(0.25f, 0.25f, 1), r, (bool)bfalgs[a.UserData.ToInt32()], true);
                }
            }
        }

        void RenderActors()
        {
            // Render all the actors in the scene 
            NxActor[] actors = scene.getActors();
            foreach (NxActor actor in actors)
            {
                foreach (NxShape s in actor.getShapes())
                {
                    if (s.getShapeType() == NxShapeType.NX_SHAPE_BOX)
                    {
                        int n = actor.UserData.ToInt32();

                        NxBoxShapeDesc t1 = (NxBoxShapeDesc)s.getShapeDesc();
                        physics3D.drawBoxOutline(0, 0, 0, t1.dimensions.Y * 2, t1.dimensions.X * 2, t1.dimensions.Z * 2, ((bool)bfalgs[n]) ? Color.Red : Color.White, s.getGlobalPose());
                    }
                    if (s.getShapeType() == NxShapeType.NX_SHAPE_PLANE)
                    {
                        //for (float stp = -100; stp < 100; stp += 5)
                        //{
                        //    physics3D.drawline(new Vector3(-100f, 0f, stp), new Vector3(100f, 0f, stp), Color.Red, Matrix.Identity);
                        //    physics3D.drawline(new Vector3(stp, 0f, -100f), new Vector3(stp, 0f, 100f), Color.White, Matrix.Identity);
                        //}
                        physics3D.drawline(new Vector3(-100f, 0f, 0f), new Vector3(100f, 0f, 0f), Color.Red, Matrix.Identity);
                        physics3D.drawline(new Vector3(0f, 0f, -100f), new Vector3(0f, 0f, 100f), Color.White, Matrix.Identity);
                        physics3D.drawline(new Vector3(0f, -100f, 0f), new Vector3(0f, 100f, 0f), Color.Blue, Matrix.Identity);
                        physics3D.drawplane();
                    }
                }
            }
        }

        public void addJoint(JointModel j)
        {
            Console.WriteLine("add j = " + j.index);
            switch (j.jtype)
            {
                case 1:
                    addRevJoint(j);
                    break;
                case 0:
                    addFixJoint(j);
                    break;
            }
        }

        void addRevJoint(JointModel j)
        {
            if (j == null)
            {
                Console.WriteLine("No such joint");
                return;
            }
            Console.WriteLine("R Joint - " + j.id + "type " + j.jtype );

            if (j.from == null || j.to == null)
            {
                Console.WriteLine("No such servo");
                return;
            } 
            
            int s = j.from.index;
            int f = j.to.index;        
            
            NxActor a = findActor(s);
            NxActor b = findActor(f);


            if (a == null || b == null)
            {
                Console.WriteLine("find servos failed");
                return;
            }

            NxRevoluteJointDesc rjd = new NxRevoluteJointDesc();
            rjd.actor[0] = a;
            rjd.actor[1] = b;

            Matrix m1 = a.getGlobalPose();
            Matrix m2 = a.getShape(0).getLocalPose();

            rjd.FlagMotorEnabled = true;
            rjd.FlagVisualization = true; 

            rjd.setGlobalAnchor(NovodexUtil.getMatrixPos(ref m1));
            rjd.setGlobalAxis(NovodexUtil.getMatrixZaxis(ref m2));

            rjd.userData = new IntPtr(j.index);
            NxRevoluteJoint t = (NxRevoluteJoint)scene.createJoint(rjd);

            turnServo(t, j.from.pos);

            if (t == null)
            {
                Console.WriteLine("failed");
            }
            else
                joints.Add(t);
        }

        void addFixJoint(JointModel j)
        {
            Console.WriteLine("F Joint - " + j.id + "type " + j.jtype);
            if (j.from == null || j.to == null)
            {
                Console.WriteLine("No missing from/to joint");
                return;
            } 
            int s = j.from.index;
            int f = j.to.index;

            NxActor a = findActor(s);
            NxActor b = findActor(f);

            if (a == null || b == null)
            {
                Console.WriteLine("find servos failed");
                return;
            }

            NxFixedJointDesc fjd = new NxFixedJointDesc();
            fjd.actor[0] = a;
            fjd.actor[1] = b;

            fjd.userData = new IntPtr(j.index);

            NxJoint t = scene.createJoint(fjd);
            if (t == null)
            {
                Console.WriteLine("create joint failed");
            }
            else joints.Add(t);
        }

        public void selServo(int index, bool f)
        {
            bfalgs[index] = f;
        }

        public void addServo(ServoModel s)
        {
            bfalgs.Add(s.index, s.select);

            if (s.mod_no == 1)
            {
                Console.WriteLine("Servo = " + s.id);
                addBoxShape(s.mod_no, s.index, s.loc, s.size, s.rot, 10);
            }
            if (s.mod_no == 2 || s.mod_no == 3)
            {
                Console.WriteLine("Hand = " + s.id);
                addBoxShape(s.mod_no, s.index, s.loc, s.size, s.rot, 10);
            }
            if (s.mod_no == 4)
            {
                Console.WriteLine("Foot = " + s.id);
                addBoxShape(s.mod_no, s.index, s.loc, s.size, s.rot, 10);
            }
            if (s.mod_no == 5)
            {
                Console.WriteLine("Body = " + s.id);
                addBoxShape(s.mod_no, s.index, s.loc, s.size, s.rot, 10);
            }
        }

        public void turnServo(int servo_index, int pos)
        {
            NxRevoluteJoint joint = null;
            foreach (NxJoint j in joints)
            {
                if (j.getJointType() == NxJointType.NX_JOINT_REVOLUTE && j.getJointDesc().actor[0].UserData.ToInt32() == servo_index)
                    joint = (NxRevoluteJoint)j;
            }

            if (joint != null) 
                turnServo(joint, pos);
        }

        public void turnServo(NxRevoluteJoint joint, int pos)
        {
            if (pos < 0) pos = 0;
            if (pos > 255) pos = 255;

            float angle = 270f * (float)(pos-127) / 255;

            Console.WriteLine(joint.UserData.ToInt32() + "] Angle=" + angle);
            NxSpringDesc ns = new NxSpringDesc(50000, 5000, angle * (float)Math.PI / 180);

            if (joint != null) joint.setSpring(ns);
        }

        NxActor findActor(int n)
        {
            NxActor[] actors = scene.getActors();
            foreach (NxActor actor in actors)
            {
                if (actor.UserData.ToInt32() == n) return actor; 
            }
            return null;
        }


        public void addBoxShape(int mtype, int n, Vector3 loc, Vector3 dim, Vector3 rot, float density)
        {         
            NxActor actor;
            NxActorDesc actorDesc = new NxActorDesc();
            NxBodyDesc bodyDesc = new NxBodyDesc();

            NxBodyDesc bodyD = new NxBodyDesc();
            NxBoxShapeDesc boxD = new NxBoxShapeDesc(dim.X / 2, dim.Y / 2, dim.Z / 2);
            boxD.density = density;

            float y, p, r;
            y = rot.X; p = rot.Y; r = rot.Z;

            Matrix locpose = Matrix.RotationYawPitchRoll(0f, 0f, 0f);

            if (mtype == 1)
            {
                //locpose = Matrix.RotationYawPitchRoll(0, 0, UTIL.DegToRads(90));
                locpose = Matrix.Translation(0, 0.25f, 0);
                locpose *= Matrix.RotationYawPitchRoll(UTIL.DegToRads(y), UTIL.DegToRads(p), UTIL.DegToRads(r));
            }
            //Matrix localpose = Matrix.RotationYawPitchRoll(UTIL.DegToRads(y), UTIL.DegToRads(p), UTIL.DegToRads(r));
            
            boxD.localPose = locpose; 

            actorDesc.addShapeDesc(boxD);

            actorDesc.BodyDesc = bodyDesc;
            actorDesc.density = density;
            actorDesc.globalPose = Matrix.Translation(loc); 

            actor = scene.createActor(actorDesc);

            actor.UserData = new IntPtr(n);

            actors.Add(actor);
        }

        public void setHook(int index, bool f)
        {
            NxActor t = findActor(index);
            if (t != null)
            {
                if (f)
                {
                    t.raiseBodyFlag(NxBodyFlag.NX_BF_KINEMATIC);
                }
                else
                {
                    t.clearBodyFlag(NxBodyFlag.NX_BF_KINEMATIC);
                }
            }
        }
    }

}
