using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenURL_ByClick : MonoBehaviour
{
    public string url_To_Open;

    void Start()
    {
        
    }

    public void UrlOpenedByClick()
    {
        Application.OpenURL(url_To_Open);
    }

}
