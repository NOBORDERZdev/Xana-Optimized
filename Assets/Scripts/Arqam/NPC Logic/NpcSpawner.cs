using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcSpawner : MonoBehaviour
{
    public static NpcSpawner npcSpawner;
    [HideInInspector]
    public int npcCounter = 0;
    [HideInInspector]
    public List<GameObject> npcModel;

    [SerializeField] private int aiStrength = 5;
    
    private GameObject maleAiPrefabs;
    private GameObject femmaleAiPrefabs;

    public List<string> Names;

    private void Awake()
    {
        if (npcSpawner is null)
            npcSpawner = this;
        else if (npcSpawner != null && npcSpawner != this)
            Destroy(this.gameObject);
    }
    private void OnEnable()
    {
        //NpcChatSystem.npcNameAction += UpdateNpcName;

    }
    private void OnDisable()
    {
        //NpcChatSystem.npcNameAction -= UpdateNpcName;
    }

    void Start()
    {
        //npcModel = new List<GameObject>();

        maleAiPrefabs = Resources.Load("NPCMale") as GameObject;
        femmaleAiPrefabs = Resources.Load("NPCFemale") as GameObject;
        for (int i = 0; i < aiStrength; i++)
        {
             GameObject npc;
            if (Random.Range(0,2) == 0)
            {
               npc = Instantiate(maleAiPrefabs);
            }else{ 
               npc = Instantiate(femmaleAiPrefabs);
             }
          
            Vector3 temp = RandomNavMeshPoint();
            npc.transform.position = temp;
            npc.transform.rotation = Quaternion.identity;

            npcCounter++;
            npcModel.Add(npc);
        }
        UpdateNpcName();
        StartCoroutine(ReactScreen.Instance.getAllReactions());
    }

    //private void UpdateNpcName(NpcChatSystem npcChatSystem)
    //{
    //    for (int i = 0; i < npcModel.Count; i++)
    //    {
    //        npcModel[i].GetComponent<NpcBehaviourSelector>().SetAiName(npcChatSystem.npcDB[i].aiNames);
    //    }
    //}

     private void UpdateNpcName()
    {
        for (int i = 0; i < npcModel.Count; i++)
        {
            npcModel[i].GetComponent<NpcBehaviourSelector>().SetAiName(Names[i]);
        }
    }

    Vector3 RandomNavMeshPoint()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        // Get a random triangle from the NavMesh
        int randomTriangleIndex = Random.Range(0, navMeshData.indices.Length / 3);
        int index = randomTriangleIndex * 3;
        // Get the vertices of the random triangle
        Vector3 vertex1 = navMeshData.vertices[navMeshData.indices[index]];
        Vector3 vertex2 = navMeshData.vertices[navMeshData.indices[index + 1]];
        Vector3 vertex3 = navMeshData.vertices[navMeshData.indices[index + 2]];
        // Get a random point within the triangle
        float r1 = Random.Range(0f, 1f);
        float r2 = Random.Range(0f, 1f);

        if (r1 + r2 > 1)
        {
            r1 = 1 - r1;
            r2 = 1 - r2;
        }
        Vector3 randomPoint = vertex1 + r1 * (vertex2 - vertex1) + r2 * (vertex3 - vertex1);
        return randomPoint;
    }

}
