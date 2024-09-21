using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapOnOff : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject otherButton;

    private void OnEnable()
    {
        if (ConstantsHolder.xanaConstants.minimap == 1)
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
        if (ConstantsHolder.xanaConstants.minimap == 1)
        {
            ReferencesForGamePlay.instance.minimap.SetActive(false);
            PlayerPrefs.SetInt("minimap", 0);
            ConstantsHolder.xanaConstants.minimap = PlayerPrefs.GetInt("minimap");
            ReferencesForGamePlay.instance.SumitMapStatus(false);

            if (XanaChatSystem.instance.chatButton.GetComponent<UnityEngine.UI.Image>().enabled)
            {
                //XanaChatSystem.instance.chatDialogBox.SetActive(true);
                XanaChatSystem.instance.chatDialogBox.GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        else
        {
            if (GameplayEntityLoader.instance != null && GameplayEntityLoader.instance.IsJoinSummitWorld)
            {
                Debug.Log("Minimap is not allowed in Summit World");
                return;
            }


            ReferencesForGamePlay.instance.minimap.SetActive(true);
            PlayerPrefs.SetInt("minimap", 1);
            ConstantsHolder.xanaConstants.minimap = PlayerPrefs.GetInt("minimap");
            ReferencesForGamePlay.instance.SumitMapStatus(true);

            //XanaChatSystem.instance.chatDialogBox.SetActive(false);
            XanaChatSystem.instance.chatDialogBox.GetComponent<CanvasGroup>().alpha = 0;
        }
        OnEnable();
    }

   
}
