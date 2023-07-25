using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swiper : MonoBehaviour
{

    public GameObject rightbtn;
    public GameObject leftbtn;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void clickright() {
        rightbtn.SetActive(false);
        leftbtn.SetActive(true);
    }
    // Update is called once per frame
    public void clickleft() {
        leftbtn.SetActive(false);
        rightbtn.SetActive(true);

    }
}
