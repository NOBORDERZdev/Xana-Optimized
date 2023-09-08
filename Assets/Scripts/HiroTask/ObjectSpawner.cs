using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    public Transform parentObj;
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

    private GameObject spawnedObject;

    // Start is called before the first frame update
    void Start()
    {
        sizeX = sizeY = sizeZ = 1;
        numOfObjectToSpawn = objectType = 0;
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
                break;
            case 1:
                Debug.Log("Polygoles are 250");
                break;
            case 2:
                Debug.Log("Polygoles are 500");
                break;
            case 3:
                Debug.Log("Polygoles are 1000");
                break;
            case 4:
                Debug.Log("Polygoles are 10000");
                break;
        }
    }

    public void SpawnObject()
    {
        for (int i=0; i< numOfObjectToSpawn; i++)
        {
            Vector3 randPos = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            spawnedObject = Instantiate(objectsForSpawn[objectType], randPos, Quaternion.identity);
            spawnedObject.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
            spawnedObject.transform.parent = parentObj;
        }
        Debug.Log("<color=red> Object spawned </color>");
    }



}
