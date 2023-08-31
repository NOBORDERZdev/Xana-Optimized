using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffdIirectionalVol : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;
    public void ClickOnDirVol()
    {
        SpeakerRefrence.instance.reftoSpeaker.SetActive(false); 
        this.gameObject.SetActive(false);
        otherButton.SetActive(true);
       
     
    }
    public void ClickOffDirVol()
    {
        SpeakerRefrence.instance.reftoSpeaker.SetActive(true);
        this.gameObject.SetActive(false);
        otherButton.SetActive(true);
       
      
    }
}
