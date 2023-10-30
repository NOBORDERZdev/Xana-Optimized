using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearClothes : MonoBehaviour
{
    [SerializeField] private GameObject[] attire;
    [SerializeField] private GameObject sourceGameobject;
    private Stitcher stitcher;

    private void Awake()
    {
        stitcher = new Stitcher();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(var item in attire)
        {
            var go = Instantiate(item);
            stitcher.Stitch(go, sourceGameobject);
            Destroy(go);
        }
    }


}
