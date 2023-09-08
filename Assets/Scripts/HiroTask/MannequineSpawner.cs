using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MannequineSpawner : MonoBehaviour
{
    public Transform parentObj;
    public GameObject objectsForSpawn;
    [Space(5)]
    public TMP_InputField countInputField;

    private int numOfObjectToSpawn;
    private GameObject spawnedObject;

    // Start is called before the first frame update
    void Start()
    {
        numOfObjectToSpawn = 0;
    }

    public void UpdateCountInputField()
    {
        string data = countInputField.text;
        if (int.TryParse(data, out numOfObjectToSpawn))
        {
            Debug.Log("numOfObjectToSpawn: " + numOfObjectToSpawn);
        }
    }

    public void SpawnMannequine()
    {
        for (int i = 0; i < numOfObjectToSpawn; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            spawnedObject = Instantiate(objectsForSpawn, randPos, Quaternion.identity);
            spawnedObject.transform.parent = parentObj;
        }
        Debug.Log("<color=red> Object spawned </color>");
    }

}
