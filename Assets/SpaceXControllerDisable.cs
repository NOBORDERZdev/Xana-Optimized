using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XanaNFT;

public class SpaceXControllerDisable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnEnable()
    {
       ReferencesForGamePlay.instance.workingCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        ReferencesForGamePlay.instance.workingCanvas.SetActive(true);
    }
}
