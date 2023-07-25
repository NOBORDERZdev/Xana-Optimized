using UnityEngine;


public  class CreateFrameHelper : MonoBehaviour
{
    public static CreateFrameHelper instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [HideInInspector]
    public GameObject Frame;
    [HideInInspector]
    public GameObject FrameCube;

    public Material Materials;

    public  GameObject CreatePlane(float width,float height,Material mat,bool colldier)
    {

        GameObject go = new GameObject("Frame");

        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh m = new Mesh();

        m.vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(width,0,0),
            new Vector3(width,height,0),
            new Vector3(0,height,0),


        };

        m.uv = new Vector2[]
       {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),

       };

        m.triangles = new int[] {0,1,2,0,2,3};

        mf.mesh = m;

        if (colldier)
        {
            (go.AddComponent(typeof(MeshCollider)) as MeshCollider).sharedMesh = m;
        }

        mr.material = mat;
        mr.material.SetInt("_Cull", 0);
        m.RecalculateBounds();
        m.RecalculateNormals();

        Frame = go;

        return go;

    }


    public void CreateCube(float width, float height, Material mat, bool colldier)
    {
        GameObject go = new GameObject("CubeFrame");

        MeshFilter mf = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh m = new Mesh();

        #region vertcies

        Vector3[] vertcies = new Vector3[24];

        // front face vertcies
        vertcies[0] = new Vector3(0,0,0);
        vertcies[1] = new Vector3(width, 0, 0);
        vertcies[2] = new Vector3(0, height, 0); 
        vertcies[3] = new Vector3(width, height, 0);


        // top face vertcies
        vertcies[4] = new Vector3(0, height, 0);
        vertcies[5] = new Vector3(width, height, 0);
        vertcies[6] = new Vector3(0, height, width);
        vertcies[7] = new Vector3(width, height, width);

        // back face vertcies
        vertcies[8] = new Vector3(0, 0, width);
        vertcies[9] = new Vector3(width, 0, width);
        vertcies[10] = new Vector3(0, height, width);
        vertcies[11] = new Vector3(width, height, width);


        // bottom face vertcies
        vertcies[12] = new Vector3(0, 0, 0);
        vertcies[13] = new Vector3(width, 0, 0);
        vertcies[14] = new Vector3(0, 0, width);
        vertcies[15] = new Vector3(width, 0, width);


        // left face vertcies
        vertcies[16] = new Vector3(0, 0, 0);
        vertcies[17] = new Vector3(0, 0, width);
        vertcies[18] = new Vector3(0, height, 0);
        vertcies[19] = new Vector3(0, height, width);

        // Right face vertcies
        vertcies[20] = new Vector3(width, 0, 0);
        vertcies[21] = new Vector3(width, 0, width);
        vertcies[22] = new Vector3(width, height, 0);
        vertcies[23] = new Vector3(width, height, width);

        #endregion

        #region Triangles
        // front face triangles

        int[] tringles = new int[36];
        tringles[0] = 0;    // first tringle
        tringles[1] = 2;
        tringles[2] = 1;

        tringles[3] = 2;    // second tringle
        tringles[4] = 3;
        tringles[5] = 1;


        // Top face triangles

        tringles[6] = 4;    //first  tringle
        tringles[7] = 6;
        tringles[8] = 5;

        tringles[9] = 6;    // second tringle
        tringles[10] = 7;
        tringles[11] = 5;


        // back face triangles

        tringles[12] = 8;    //first  tringle
        tringles[13] = 10;
        tringles[14] = 9;

        tringles[15] = 10;    // second tringle
        tringles[16] = 11;
        tringles[17] = 9;


        // bottom face triangles

        tringles[18] = 12;    //first  tringle
        tringles[19] = 14;
        tringles[20] = 13;

        tringles[21] = 14;    // second tringle
        tringles[22] = 15;
        tringles[23] = 13;


        // left face triangles

        tringles[24] = 16;    //first  tringle
        tringles[25] = 18;
        tringles[26] = 17;

        tringles[27] = 18;    // second tringle
        tringles[28] = 19;
        tringles[29] = 17;

        // right face triangles

        tringles[30] = 20;    //first  tringle
        tringles[31] = 22;
        tringles[32] = 21;

        tringles[33] = 22;    // second tringle
        tringles[34] = 23;
        tringles[35] = 21;

        #endregion

        #region Normals

        // create and assign normal
        Vector3[] normals= new Vector3[24];

        // front face normals
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;


        // Top face normals
        normals[4] = -Vector3.forward;
        normals[5] = -Vector3.forward;
        normals[6] = -Vector3.forward;
        normals[7] = -Vector3.forward;


        // back face normals
        normals[8] = -Vector3.forward;
        normals[9] = -Vector3.forward;
        normals[10] = -Vector3.forward;
        normals[11] = -Vector3.forward;

        // bototm face normals
        normals[12] = -Vector3.forward;
        normals[13] = -Vector3.forward;
        normals[14] = -Vector3.forward;
        normals[15] = -Vector3.forward;

        // left face normals
        normals[16] = -Vector3.forward;
        normals[17] = -Vector3.forward;
        normals[18] = -Vector3.forward;
        normals[19] = -Vector3.forward;

        // right face normals
        normals[20] = -Vector3.forward;
        normals[21] = -Vector3.forward;
        normals[22] = -Vector3.forward;
        normals[23] = -Vector3.forward;


        #endregion

        #region UVs
        //create and assign UVs

        //Vector2[] uvs = new Vector2[12];
        ////uv for front face
        //uvs[0] = new Vector2(0, 0);
        //uvs[1] = new Vector2(1, 0);
        //uvs[2] = new Vector2(0, 1);
        //uvs[3] = new Vector2(1, 1);

        ////uv for top face

        //uvs[4] = new Vector2(0, 1);
        //uvs[5] = new Vector2(1, 1);
        //uvs[6] = new Vector2(0, 1);
        //uvs[7] = new Vector2(1, 1);

        #endregion

        m.vertices = vertcies;
        m.triangles = tringles;
        m.normals = normals;

        mf.mesh = m;
        mr.material = mat;

        mr.material.SetInt("_Cull",0);
        //m.uv= uvs;

        m.Optimize();

        m.RecalculateBounds();
        m.RecalculateNormals();

        FrameCube = go;
    }


}
