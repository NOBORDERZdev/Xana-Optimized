using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ProfilesManager : MonoBehaviour
{
    public Image TargetSprite;
    public Button[] ProfileButtons;


    private void Start()
    {
        if (ProfileSelector._instance.currentProfile != -1)
        {
      //      TargetSprite.rectTransform.anchoredPosition = ProfileButtons[ProfileSelector._instance.currentProfile].GetComponent<RectTransform>().anchoredPosition;
            TargetSprite.rectTransform.position = ProfileButtons[ProfileSelector._instance.currentProfile].transform.position;
            TargetSprite.transform.parent = ProfileButtons[ProfileSelector._instance.currentProfile].transform;
            TargetSprite.gameObject.SetActive(true);
        }
    }

    public void ProfileSelected(int i) {
        ProfileSelector._instance.currentProfile = i;
        //  TargetSprite.rectTransform.anchoredPosition = ProfileButtons[ProfileSelector._instance.currentProfile].GetComponent<RectTransform>().anchoredPosition;
        TargetSprite.rectTransform.position = ProfileButtons[ProfileSelector._instance.currentProfile].transform.position;
        TargetSprite.transform.parent = ProfileButtons[ProfileSelector._instance.currentProfile].transform;
        TargetSprite.gameObject.SetActive(true);
    }

}
