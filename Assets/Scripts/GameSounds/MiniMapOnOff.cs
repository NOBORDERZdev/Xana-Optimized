using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;

    private void OnEnable()
    {
        if (XanaConstants.xanaConstants.minimap == 1)
        {
            if (this.gameObject.name == "OffButton")
            {
                otherButton.SetActive(true);
                this.gameObject.SetActive(false);
            }
            else if (this.gameObject.name == "OnButton")
            {
                this.gameObject.SetActive(true);
                otherButton.SetActive(false);
            }
        }
        else
        {
            if (this.gameObject.name == "OffButton")
            {
                this.gameObject.SetActive(true);
                otherButton.SetActive(false);
            }
            else if (this.gameObject.name == "OnButton")
            {
                otherButton.SetActive(true);
                this.gameObject.SetActive(false);
            }
        }
        //XanaVoiceChat.instance.UpdateMicButton();
    }

    public void ClickMicMain()
    {
        if (XanaConstants.xanaConstants.minimap == 1)
        {
            ReferrencesForDynamicMuseum.instance.minimap.SetActive(false);
            PlayerPrefs.SetInt("minimap", 0);
            XanaConstants.xanaConstants.minimap = PlayerPrefs.GetInt("minimap");
        }
        else
        {
            ReferrencesForDynamicMuseum.instance.minimap.SetActive(true);
            PlayerPrefs.SetInt("minimap", 1);
            XanaConstants.xanaConstants.minimap = PlayerPrefs.GetInt("minimap");
        }
        OnEnable();
    }
}
