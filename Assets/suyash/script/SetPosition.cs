using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPosition : MonoBehaviour
{

    public static SetPosition instance;
   /* [SerializeField] private float x;
    [SerializeField] private float y;
    [SerializeField] private float z;*/

    private void Awake()
    {
        instance = this;

        /*if (!instance)
        {
            instance = this;
          //  DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }

        *//* Scene currentScene = SceneManager.GetActiveScene();
         if (currentScene.name == "Home")
         {
             gameObject.transform.position = new Vector3(x, y, z);
         }*/
    }

    private void Update()
    {
        
    }





}
