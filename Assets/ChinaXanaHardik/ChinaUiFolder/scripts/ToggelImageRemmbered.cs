using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggelImageRemmbered : MonoBehaviour
{
    // Start is called before the first frame update
    public Toggle toggle;
public GameObject imageToChange;


public void ChangeImage()
{

    if (toggle.isOn)
    {
       
        imageToChange.SetActive(true);
    }
    else
    {
        imageToChange.SetActive(false);
    }
}

private void OnEnable()
{
    //  ChangeImage();
}
}
