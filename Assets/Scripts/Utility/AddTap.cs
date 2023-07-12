using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddTap : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] data;
    void Start()
    {
        //Invoke("init_btns", 1f);
    }
    public void init_btns()
    {
        data = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {


            data[i] = gameObject.transform.GetChild(i).gameObject;
        
        }
        foreach(GameObject obj in data)
        {
           
            obj.GetComponent<Button>().onClick.AddListener(() => {

                CustomNavigation.Instance.addData(obj);
            });
        }
    }
    
}
