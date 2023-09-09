using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    public TMP_InputField sizeInputFieldX, sizeInputFieldY, sizeInputFieldZ;
    [Space(5)]
    public TMP_InputField countInputField;
    [Space(5)]
    public TMP_Dropdown type;
    [Space(10)]
    public GameObject[] objectsForSpawn;

    private int numOfObjectToSpawn;

    private int sizeX, sizeY, sizeZ;

    private int objectType;

    private int objectPolyCount = 1000;

    private int maxDistance = 48;

    private GameObject spawnedObject;

    // Start is called before the first frame update
    void Start()
    {
        sizeX = sizeY = sizeZ = 1;
        sizeInputFieldX.text = sizeInputFieldY.text = sizeInputFieldZ.text = "1";
        numOfObjectToSpawn = 100;
        countInputField.text = "100";
        objectType = 3;
        type.value = objectType;
    }

    public void UpdateCountInputField()
    {
        string data = countInputField.text;
        if (int.TryParse(data, out numOfObjectToSpawn))
        {
            Debug.Log("numOfObjectToSpawn: " + numOfObjectToSpawn);
        }
    }

    public void UpdateSizeXInputField()
    {
        string data = sizeInputFieldX.text;
        if (int.TryParse(data, out sizeX))
        {
            Debug.Log("SizeX: " + sizeX);
        }
    }

    public void UpdateSizeYInputField()
    {
        string data = sizeInputFieldY.text;
        if (int.TryParse(data, out sizeY))
        {
            Debug.Log("SizeY: " + sizeY);
        }
    }

    public void UpdateSizeZInputField()
    {
        string data = sizeInputFieldZ.text;
        if (int.TryParse(data, out sizeZ))
        {
            Debug.Log("SizeZ: " + sizeZ);
        }
    }


    public void UpdateDropdown()
    {
        objectType = type.value;
        switch (objectType)
        {
            case 0:
                Debug.Log("Polygoles are 100");
                objectPolyCount = 100;
                break;
            case 1:
                Debug.Log("Polygoles are 250");
                objectPolyCount = 250;
                break;
            case 2:
                Debug.Log("Polygoles are 500");
                objectPolyCount = 500;
                break;
            case 3:
                Debug.Log("Polygoles are 1000");
                objectPolyCount = 1000;
                break;
            case 4:
                Debug.Log("Polygoles are 10000");
                objectPolyCount = 10000;
                break;
        }
    }

    public void SpawnObject()
    {
        if(TestWorldCanvasManager.Instance.parentObj == null)
        {
            Debug.Log("<color=red> Parent object is null </color>");
            TestWorldCanvasManager.Instance.parentObj = Instantiate(new GameObject("TestObjectsParent"), Vector3.zero, Quaternion.identity);
            TestWorldCanvasManager.Instance.parentObj.transform.SetParent(null);
        }
        //sizeY / 2
        
        for (int i=0; i< numOfObjectToSpawn; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-maxDistance, maxDistance), sizeY / 2, Random.Range(-maxDistance, maxDistance));
            spawnedObject = Instantiate(objectsForSpawn[objectType]); //, randPos, Quaternion.identity
            spawnedObject.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
            spawnedObject.transform.position = randPos;
            float rotationAngle = UnityEngine.Random.Range(0f, 360f);
            spawnedObject.transform.rotation = Quaternion.Euler(-90f, rotationAngle, 0f);
            spawnedObject.transform.parent = TestWorldCanvasManager.Instance.parentObj.transform;
        }
        TestWorldCanvasManager.Instance.AddPolyCount(numOfObjectToSpawn * objectPolyCount);
        Debug.Log($"<color=red> Object spawned </color>");
    }
}
