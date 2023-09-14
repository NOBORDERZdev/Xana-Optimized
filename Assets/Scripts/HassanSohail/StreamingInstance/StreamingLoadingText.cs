using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamingLoadingText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text TmpText;
    [SerializeField]  string loadingScreenTxt;

    private void OnEnable()
    {

    }

    public void UpdateLoadingText()
    {
         if (XanaConstants.xanaConstants.isCameraMan)
        {
            TmpText.text= loadingScreenTxt+XanaConstants.xanaConstants.JjWorldTeleportSceneName.ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }



}
