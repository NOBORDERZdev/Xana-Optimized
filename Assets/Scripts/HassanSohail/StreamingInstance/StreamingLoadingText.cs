using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamingLoadingText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text TmpText;
    [SerializeField]  string loadingScreenTxt;

    private void OnEnable()
    {
        if (!XanaConstantsHolder.xanaConstants.isCameraMan)
        {
            gameObject.SetActive(false);
        }
    }
    public void UpdateLoadingText(bool movingToWorld)
    {
       
            if (XanaConstantsHolder.xanaConstants.isCameraMan)
            {
                 if (movingToWorld)
                TmpText.text= loadingScreenTxt+XanaConstantsHolder.xanaConstants.JjWorldTeleportSceneName.ToString();
                 else
                TmpText.text = "Switching world";
            }
            else
            {
                gameObject.SetActive(false);
            }
    }

    

}
