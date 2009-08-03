using System;
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

        NxScene scene;

        public RoboPhysx(Render r)
        {
            physics3D = r;
            //Initialize the array to a reasonable target frame rate of 60 FPS.
            timeStepArray = new float[32];
            for (int i = 0; i < timeStepArray.Length; i++)
            { 
                timeStepArray[i] = 1 / 60.0f; 
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

            RenderActors();
            RenderJoints();

            physics3D.setupView();
            physics3D.Device.EndScene();
            physics3D.Device.Present();
        }

        void RenderJoints()
        {
            // Render all the actors in the scene 
            foreach (NxJoint j in scene.getJoints())
            {
                Color c = Color.Blue;
                if (j.getJointType() == NxJointType.NX_JOINT_FIXED) c = Color.Blue;
                if (j.getJointType() == NxJointType.NX_JOINT_REVOLUTE) c = Color.Red;

                NxActor a;
                NxActor b;
                j.getActors(out a, out b);

                physics3D.drawline(a.getGlobalPosition(), b.getGlobalPosition(), c, Matrix.Identity);
            }
        }

        void RenderActors()
        {
            // Render all the actors in the scene 
            int nbActors = scene.getNbActors();
            Console.WriteLine("Actors = " + nbActors);

            NxActor[] actors = scene.getActors();
            foreach (NxActor actor in actors)
            {
                //if (actor.UserData != null) Console.WriteLine("Userdata=" + actor.UserData);

                foreach (NxShape s in actor.getShapes())
                {
                    if (s.getShapeType() == NxShapeType.NX_SHAPE_BOX)
                    {
                        //drawbox(s, actor.UserData.ToInt32());
                    }
                    if (s.getShapeType() == NxShapeType.NX_SHAPE_PLANE)
                    {
                        physics3D.drawline(new Vector3(-100f, 0f, 0f), new Vector3(100f, 0f, 0f), Color.Red, Matrix.Identity);
                        physics3D.drawline(new Vector3(0f, -100f, 0f), new Vector3(0f, 100f, 0f), Color.Blue, Matrix.Identity);
                        physics3D.drawline(new Vector3(0f, 0f, -100f), new Vector3(0f, 0f, 100f), Color.White, Matrix.Identity);
                        physics3D.drawplane();
                    }
                    if (s.getShapeType() == NxShapeType.NX_SHAPE_SPHERE)
                    {
                    }
                    if (s.getShapeType() == NxShapeType.NX_SHAPE_CAPSULE)
                    {
                        //physics3D.drawBoxOutline(s.getGlobalPosition().X,s.getGlobalPosition().Y,s.getGlobalPosition().Z,
                        physics3D.drawModel(1 , s.getGlobalPosition(), new Vector3(0,0,0), false, true);

                    }
                }
            }
        }

        public void addServo(ServoModel s)
        {
            NxActor actor;
            NxActorDesc actorDesc = new NxActorDesc();
            NxBodyDesc bodyDesc = new NxBodyDesc();

            float density = 1.0f;
            Vector3 boxes = new Vector3(0.6f, 0.3f, 0.45f);
            Vector3 rot = new Vector3(0, boxes.Y / 2, boxes.Z / 2);

            NxCapsuleShapeDesc capDesc = new NxCapsuleShapeDesc();
            capDesc.radius = 0.4f;
            capDesc.height = 1.0f;

            float y, p, r;
            y = s.rot.X; p = s.rot.Y; r = s.rot.Z;

            Matrix localpose = Matrix.RotationYawPitchRoll(0, 0, UTIL.DegToRads(90));
            localpose *= Matrix.RotationYawPitchRoll(UTIL.DegToRads(y), UTIL.DegToRads(p), UTIL.DegToRads(r));

            capDesc.localPose = localpose;

            actorDesc.addShapeDesc(capDesc);
            actorDesc.BodyDesc = bodyDesc;
            actorDesc.density = density;
            actorDesc.globalPose = Matrix.Translation(s.loc); 

            actor = scene.createActor(actorDesc);
            //actor.Name = s.id;
            actors.Add(actor);
        }

    }

}
