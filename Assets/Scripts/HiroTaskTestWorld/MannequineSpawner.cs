using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MannequineSpawner : MonoBehaviour
{
    public GameObject objectsForSpawn;
    [Space(5)]
    public TMP_InputField countInputField;

    private int numOfObjectToSpawn;
    private GameObject spawnedObject;
    private int maxDistance = 20;

    // Start is called before the first frame update
    void Start()
    {
        numOfObjectToSpawn = 5;
        countInputField.text = "5";
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
        if (TestWorldCanvasManager.Instance.parentObj == null)
        {
            Debug.Log("<color=red> Parent object is null </color>");
            TestWorldCanvasManager.Instance.parentObj = Instantiate(new GameObject("TestObjectsParent"), Vector3.zero, Quaternion.identity);
            TestWorldCanvasManager.Instance.parentObj.transform.SetParent(null);
        }
        for (int i = 0; i < numOfObjectToSpawn; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-maxDistance, maxDistance), 0, Random.Range(-maxDistance, maxDistance));
            spawnedObject = Instantiate(objectsForSpawn, randPos, Quaternion.identity);
            float rotationAngle = UnityEngine.Random.Range(0f, 360f);
            spawnedObject.transform.rotation = Quaternion.Euler(0f, rotationAngle, 0f);
            spawnedObject.transform.parent = TestWorldCanvasManager.Instance.parentObj.transform;
        }
        TestWorldCanvasManager.Instance.AddCharacterCount(numOfObjectToSpawn);
        Debug.Log("<color=red> Object spawned </color>");
    }

}
