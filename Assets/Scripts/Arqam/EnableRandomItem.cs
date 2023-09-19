using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomItem : MonoBehaviour
{
    [SerializeField]
    private GameObject[] items;
    private int rand = 0;


    void OnEnable()
    {
        
    }

    private void Start()
    {
        rand = Random.Range(0, items.Length);
        foreach (GameObject obj in items) obj.SetActive(false); 
        items[rand].SetActive(true);
    }


}
