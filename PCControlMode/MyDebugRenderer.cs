//By Jason Zelsnack, All rights reserved

using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;


using NovodexWrapper;


namespace Simulator
{
	public class MyDebugRenderer
	{
		public static readonly int MAX_NUM_POINTS=1000000;
		public static readonly int MAX_NUM_LINES=1000000;
		public static readonly int MAX_NUM_TRIANGLES=1000000;
		
		private Direct3D.Device renderDevice=null;
		private CustomVertex.PositionColored[] pointVert=null;
		private CustomVertex.PositionColored[] lineVert=null;
		private CustomVertex.PositionColored[] triangleVert=null;
		private int pointCapacity,lineCapacity,triangleCapacity;
		private bool ZBufferEnabledFlag=false;
		private bool drawLineShadows=false;
		private NxVec3 shadowOffset=new NxVec3(0,0,0);

		
		public MyDebugRenderer(Direct3D.Device renderDevice)
		{
			setRenderDevice(renderDevice);
			initPrimitiveCapacities(5000,5000,5000);
		}

		protected MyDebugRenderer(Direct3D.Device renderDevice,int startNumPoints,int startNumLines,int startNumTriangles)
		{
			setRenderDevice(renderDevice);
			initPrimitiveCapacities(startNumPoints,startNumLines,startNumTriangles);
		}

		public void setRenderDevice(Direct3D.Device newRenderDevice)
			{renderDevice=newRenderDevice;}

		private void initPrimitiveCapacities(int numPoints,int numLines,int numTriangles)
		{
			pointVert=new CustomVertex.PositionColored[Math.Min(numPoints,MAX_NUM_POINTS)];
			lineVert=new CustomVertex.PositionColored[Math.Min(numLines,MAX_NUM_LINES)*2];
			triangleVert=new CustomVertex.PositionColored[Math.Min(numTriangles,MAX_NUM_TRIANGLES)*3];
			pointCapacity=pointVert.Length;
			lineCapacity=lineVert.Length/2;
			triangleCapacity=triangleVert.Length/3;
		}

		protected void setPointCapacity(int numPoints)
		{
			if(numPoints>pointCapacity && pointCapacity<MAX_NUM_POINTS)
				{pointVert=new CustomVertex.PositionColored[Math.Min(numPoints,MAX_NUM_POINTS)];}
			pointCapacity=pointVert.Length;
		}

		protected void setLineCapacity(int numLines)
		{
			if(numLines>lineCapacity && lineCapacity<MAX_NUM_LINES)
				{lineVert=new CustomVertex.PositionColored[Math.Min(numLines,MAX_NUM_LINES)*2];}
			lineCapacity=lineVert.Length/2;	
		}

		protected void setTriangleCapacity(int numTriangles)
		{
			if(numTriangles>triangleCapacity && triangleCapacity<MAX_NUM_TRIANGLES)
				{triangleVert=new CustomVertex.PositionColored[Math.Min(numTriangles,MAX_NUM_TRIANGLES)*3];}
			triangleCapacity=triangleVert.Length/3;
		}

		protected void setPrimitiveCapacities(int numPoints,int numLines,int numTriangles)
		{
			setPointCapacity(numPoints);
			setLineCapacity(numLines);
			setTriangleCapacity(numTriangles);
		}
		
		public bool DrawLineShadows
		{
			get{return drawLineShadows;}
			set{drawLineShadows=value;}
		}
		
		public NxVec3 ShadowOffset
		{
			get{return shadowOffset;}
			set{shadowOffset=value;}
		}

		public bool ZBufferEnabled
		{
			get{return ZBufferEnabledFlag;}
			set{ZBufferEnabledFlag=value;}
		}
		
		public void renderData(NxDebugRenderable data)
		{
			if(renderDevice==null)
				{return;}

			if(data.getNbPoints()>pointCapacity)
				{setPointCapacity(pointCapacity*2);}
			if(data.getNbLines()>lineCapacity)
				{setLineCapacity(lineCapacity*2);}
			if(data.getNbTriangles()>triangleCapacity)
				{setTriangleCapacity(triangleCapacity*2);}

			NxDebugPoint[] pointArray=data.getPoints();
			NxDebugLine[] lineArray=data.getLines();
			NxDebugTriangle[] triangleArray=data.getTriangles();

			int numPoints=Math.Min(pointArray.Length,pointVert.Length);
			for(int i=0;i<numPoints;i++)
			{
				pointVert[i].Position=NovodexUtil.convertNxVec3ToVector3(pointArray[i].p);
				pointVert[i].Color=(int)pointArray[i].color;
			}

			int numLines=Math.Min(lineArray.Length,lineVert.Length/2);
			for(int i=0;i<numLines;i++)
			{
                lineVert[(i * 2) + 0].Position = NovodexUtil.convertNxVec3ToVector3(lineArray[i].p0);
				lineVert[(i*2)+0].Color=(int)lineArray[i].color;
                lineVert[(i * 2) + 1].Position = NovodexUtil.convertNxVec3ToVector3(lineArray[i].p1);
				lineVert[(i*2)+1].Color=(int)lineArray[i].color;
			}

			int numTriangles=Math.Min(triangleArray.Length,triangleVert.Length/3);
			for(int i=0;i<numTriangles;i++)
			{
                triangleVert[(i * 3) + 0].Position = NovodexUtil.convertNxVec3ToVector3(triangleArray[i].p0);
				triangleVert[(i*3)+0].Color=(int)triangleArray[i].color;
                triangleVert[(i * 3) + 1].Position = NovodexUtil.convertNxVec3ToVector3(triangleArray[i].p1);
				triangleVert[(i*3)+1].Color=(int)triangleArray[i].color;
                triangleVert[(i * 3) + 2].Position = NovodexUtil.convertNxVec3ToVector3(triangleArray[i].p2);
				triangleVert[(i*3)+2].Color=(int)triangleArray[i].color;
			}


			//Cache zBuffer and lighting states
			bool last_ZBufferEnabled=renderDevice.RenderState.ZBufferEnable;
			bool last_lighting=renderDevice.RenderState.Lighting;

			renderDevice.RenderState.ZBufferEnable=ZBufferEnabledFlag;

            //renderDevice.SetRenderState(RenderStates.CullMode, true); // ????

			if(drawLineShadows)
			{
				//Create a squished matrix at the ground
                renderDevice.Transform.World = NovodexUtil.convertNxMat34ToMatrix(NovodexUtil.createMatrix(new NxVec3(1, 0, 0), new NxVec3(0, 0.001f, 0), new NxVec3(0, 0, 1), shadowOffset));
				//Setting lighting to true with a squished matrix will make the shadow lines black
				renderDevice.RenderState.Lighting=true;
				renderDevice.VertexFormat=CustomVertex.PositionColored.Format;
				renderDevice.DrawUserPrimitives(PrimitiveType.LineList,numLines,lineVert);
			}

			renderDevice.RenderState.Lighting=false;

			//Set matrix to identity because the data is in worldspace
			renderDevice.Transform.World=Matrix.Identity;

			//Render triangles first
            renderDevice.VertexFormat = CustomVertex.PositionColored.Format;
            //renderDevice.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            renderDevice.DrawUserPrimitives(PrimitiveType.TriangleList, numTriangles, triangleVert);

			//Draw lines over triangles
            renderDevice.VertexFormat = CustomVertex.PositionColored.Format;
            //renderDevice.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            renderDevice.DrawUserPrimitives(PrimitiveType.LineList, numLines, lineVert);

			//Draw points over lines and triangles	(this needs to be better because 1 pixel points suck)
			renderDevice.VertexFormat=CustomVertex.PositionColored.Format;
			renderDevice.DrawUserPrimitives(PrimitiveType.PointList,numPoints,pointVert);

			//put zBuffer and lighting back the way they were
			renderDevice.RenderState.ZBufferEnable=last_ZBufferEnabled;
			renderDevice.RenderState.Lighting=last_lighting;
		}




		public void drawTrianglesFromVertexTriplets(NxVec3[] triangleTripletArray,int color)
		{
			int lineVertIndex=0;
			int numTris=triangleTripletArray.Length/3;
			numTris=Math.Min(numTris,MAX_NUM_LINES/3);
			if(numTris*3>lineCapacity)
				{setLineCapacity(numTris*3*2);}

			int numLines=numTris*3;
			for(int i=0;i<numTris;i++)
			{
				for(int j=0;j<3;j++)
				{
                    lineVert[lineVertIndex].Position = NovodexUtil.convertNxVec3ToVector3(triangleTripletArray[(i * 3) + j]);
					lineVert[lineVertIndex].Color=color;
					lineVertIndex++;
                    lineVert[lineVertIndex].Position = NovodexUtil.convertNxVec3ToVector3(triangleTripletArray[(i * 3) + ((j + 1) % 3)]);
					lineVert[lineVertIndex].Color=color;
					lineVertIndex++;
				}
			}


			//Cache zBuffer and lighting states
			bool last_ZBufferEnabled=renderDevice.RenderState.ZBufferEnable;
			bool last_lighting=renderDevice.RenderState.Lighting;

			renderDevice.RenderState.ZBufferEnable=ZBufferEnabledFlag;

			if(drawLineShadows)
			{
				//Create a squished matrix at the ground
                renderDevice.Transform.World = NovodexUtil.convertNxMat34ToMatrix(NovodexUtil.createMatrix(new NxVec3(1, 0, 0), new NxVec3(0, 0.001f, 0), new NxVec3(0, 0, 1), shadowOffset));
				//Setting lighting to true with a squished matrix will make the shadow lines black
				renderDevice.RenderState.Lighting=true;
				renderDevice.VertexFormat=CustomVertex.PositionColored.Format;
				renderDevice.DrawUserPrimitives(PrimitiveType.LineList,numLines,lineVert);
			}

			renderDevice.RenderState.Lighting=false;

			//Set matrix to identity because the data is in worldspace
			renderDevice.Transform.World=Matrix.Identity;

			//Draw lines over triangles
			renderDevice.VertexFormat=CustomVertex.PositionColored.Format;
			renderDevice.DrawUserPrimitives(PrimitiveType.LineList,numLines,lineVert);

			//put zBuffer and lighting back the way they were
			renderDevice.RenderState.ZBufferEnable=last_ZBufferEnabled;
			renderDevice.RenderState.Lighting=last_lighting;
		}

        protected CustomVertex.PositionColored[] vertices;

        public void drawline(Vector3 src, Vector3 dest, System.Drawing.Color colour) //, Matrix viewProjection)
        {
            vertices = new CustomVertex.PositionColored[2];

            vertices[0] = new CustomVertex.PositionColored(src, colour.ToArgb());
            vertices[1] = new CustomVertex.PositionColored(dest, colour.ToArgb());

            //Cache zBuffer and lighting states
            bool last_ZBufferEnabled = renderDevice.RenderState.ZBufferEnable;
            bool last_lighting = renderDevice.RenderState.Lighting;

            renderDevice.RenderState.Lighting = false;


            // Setup a material for the teapot
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



        //static private VertexBuffer vbuf;
        
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
    }	
}	


