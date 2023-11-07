using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcSpawner : MonoBehaviour
{
    public static NpcSpawner npcSpawner;
    [HideInInspector]
    public int npcCounter = 0;
    public List<GameObject> npcModel;

    [SerializeField] private int aiStrength = 5;
    [SerializeField] List<string> aiNames;
    
    private GameObject aiPrefabs;
    private int rand = 0;

    private void Awake()
    {
        if (npcSpawner is null)
            npcSpawner = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        npcModel = new List<GameObject>();

        aiPrefabs = Resources.Load("NPC") as GameObject;
        for (int i = 0; i < aiStrength; i++)
        {
            GameObject npc = Instantiate(aiPrefabs);
            Vector3 temp = RandomNavMeshPoint();
            npc.transform.position = temp;
            npc.transform.rotation = Quaternion.identity;

            rand = Random.Range(0, aiNames.Count);
            npc.GetComponent<NpcBehaviourSelector>().SetAiName(aiNames[rand]);       // Set npc names
            aiNames.RemoveAt(rand);
            
            npcCounter++;
            npcModel.Add(npc);
        }

        StartCoroutine(ReactScreen.Instance.getAllReactions());
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
