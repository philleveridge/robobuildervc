xof 0303txt 0032
template XSkinMeshHeader {
 <3cf169ce-ff7c-44ab-93c0-f78f62d172e2>
 WORD nMaxSkinWeightsPerVertex;
 WORD nMaxSkinWeightsPerFace;
 WORD nBones;
}

template VertexDuplicationIndices {
 <b8d65549-d7c9-4995-89cf-53a9a8b031e3>
 DWORD nIndices;
 DWORD nOriginalVertices;
 array DWORD indices[nIndices];
}

template SkinWeights {
 <6f0d123b-bad2-4167-a0d0-80224f25fabb>
 STRING transformNodeName;
 DWORD nWeights;
 array DWORD vertexIndices[nWeights];
 array FLOAT weights[nWeights];
 Matrix4x4 matrixOffset;
}


Frame SCENE_ROOT {
 

 FrameTransformMatrix {
  1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
 }

 Frame pCube1 {
  

  FrameTransformMatrix {
   1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000,0.000000,0.000000,0.000000,0.000000,1.000000;;
  }

  Mesh {
   24;
   -0.500000;-0.500000;0.500000;,
   0.500000;-0.500000;0.500000;,
   -0.500000;0.500000;0.500000;,
   0.500000;0.500000;0.500000;,
   -0.500000;0.500000;-0.500000;,
   0.500000;0.500000;-0.500000;,
   -0.500000;-0.500000;-0.500000;,
   0.500000;-0.500000;-0.500000;,
   -0.500000;0.500000;0.500000;,
   0.500000;0.500000;0.500000;,
   -0.500000;0.500000;-0.500000;,
   0.500000;0.500000;-0.500000;,
   -0.500000;-0.500000;-0.500000;,
   0.500000;-0.500000;-0.500000;,
   0.500000;-0.500000;0.500000;,
   -0.500000;-0.500000;0.500000;,
   0.500000;-0.500000;0.500000;,
   0.500000;-0.500000;-0.500000;,
   0.500000;0.500000;-0.500000;,
   0.500000;0.500000;0.500000;,
   -0.500000;-0.500000;-0.500000;,
   -0.500000;-0.500000;0.500000;,
   -0.500000;0.500000;0.500000;,
   -0.500000;0.500000;-0.500000;;
   6;
   4;0,1,3,2;,
   4;8,9,5,4;,
   4;10,11,7,6;,
   4;12,13,14,15;,
   4;16,17,18,19;,
   4;20,21,22,23;;

   MeshNormals {
    24;
    0.000000;0.000000;1.000000;,
    0.000000;0.000000;1.000000;,
    0.000000;0.000000;1.000000;,
    0.000000;0.000000;1.000000;,
    0.000000;1.000000;0.000000;,
    0.000000;1.000000;0.000000;,
    0.000000;0.000000;-1.000000;,
    0.000000;0.000000;-1.000000;,
    0.000000;1.000000;0.000000;,
    0.000000;1.000000;0.000000;,
    0.000000;0.000000;-1.000000;,
    0.000000;0.000000;-1.000000;,
    0.000000;-1.000000;0.000000;,
    0.000000;-1.000000;0.000000;,
    0.000000;-1.000000;0.000000;,
    0.000000;-1.000000;0.000000;,
    1.000000;0.000000;0.000000;,
    1.000000;0.000000;0.000000;,
    1.000000;0.000000;0.000000;,
    1.000000;0.000000;0.000000;,
    -1.000000;0.000000;0.000000;,
    -1.000000;0.000000;0.000000;,
    -1.000000;0.000000;0.000000;,
    -1.000000;0.000000;0.000000;;
    6;
    4;0,1,3,2;,
    4;8,9,5,4;,
    4;10,11,7,6;,
    4;12,13,14,15;,
    4;16,17,18,19;,
    4;20,21,22,23;;
   }

   MeshTextureCoords {
    24;
    0.000000;0.000000;,
    1.000000;0.000000;,
    0.000000;-1.000000;,
    1.000000;-1.000000;,
    0.000000;-2.000000;,
    1.000000;-2.000000;,
    0.000000;-3.000000;,
    1.000000;-3.000000;,
    0.000000;-1.000000;,
    1.000000;-1.000000;,
    0.000000;-2.000000;,
    1.000000;-2.000000;,
    0.000000;-3.000000;,
    1.000000;-3.000000;,
    1.000000;-4.000000;,
    0.000000;-4.000000;,
    1.000000;0.000000;,
    2.000000;0.000000;,
    2.000000;-1.000000;,
    1.000000;-1.000000;,
    -1.000000;0.000000;,
    0.000000;0.000000;,
    0.000000;-1.000000;,
    -1.000000;-1.000000;;
   }

   MeshMaterialList {
    1;
    6;
    0,
    0,
    0,
    0,
    0,
    0;

    Material obj_diff_no_texture {
     0.400000;0.400000;0.400000;1.000000;;
     0.000000;
     0.000000;0.000000;0.000000;;
     0.000000;0.000000;0.000000;;
    }
   }

   VertexDuplicationIndices {
    24;
    8;
    0,
    1,
    2,
    3,
    4,
    5,
    6,
    7,
    2,
    3,
    4,
    5,
    6,
    7,
    1,
    0,
    1,
    7,
    5,
    3,
    6,
    0,
    2,
    4;
   }
  }
 }
}