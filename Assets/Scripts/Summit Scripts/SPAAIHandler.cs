using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SPAAIHandler : MonoBehaviour
{
    public GameObject[] AIAvatarPrefabs;
    public Transform SpawnPoint;
    public GameObject CurrentAIPerformerRef;
    // Start is called before the first frame update
    void Start()
    {
        SpawnAIPerformer();
    }

    void SpawnAIPerformer()
    {
        if (CurrentAIPerformerRef)
        {
            CurrentAIPerformerRef.SetActive(true);
        }
        else
        {
            int _aiPrefabIndex = Random.Range(0, AIAvatarPrefabs.Length);
            CurrentAIPerformerRef = Instantiate(AIAvatarPrefabs[_aiPrefabIndex], SpawnPoint.position, Quaternion.identity);
        }
    }
}
