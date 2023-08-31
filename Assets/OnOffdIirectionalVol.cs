using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffdIirectionalVol : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;
    public void ClickOnDirVol()
    {
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().RangeVolSpeaker.SetActive(false);
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().VolSpeaker.SetActive(true);
        this.gameObject.SetActive(false);
        otherButton.SetActive(true);
     }
    public void ClickOffDirVol()
    {
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().RangeVolSpeaker.SetActive(true);
        ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<SpeakerRefrence>().VolSpeaker.SetActive(false);
        this.gameObject.SetActive(false);
        otherButton.SetActive(true);
     }
}
