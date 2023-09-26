using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamingLoadingText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text TmpText;
    [SerializeField] string loadingScreenTxt;
    [SerializeField] List<GameObject> OffObjs;
    private void OnEnable()
    {
        if (!XanaConstants.xanaConstants.isCameraMan)
        {
            gameObject.SetActive(false);
        }
        else
        {
            foreach (var item in OffObjs)
            {
                item.SetActive(false);
            }
        }
    }
    public void UpdateLoadingText(bool movingToWorld)
    {
       
            if (XanaConstants.xanaConstants.isCameraMan)
            {
                 if (movingToWorld)
                TmpText.text= loadingScreenTxt+XanaConstants.xanaConstants.JjWorldTeleportSceneName.ToString();
                 else
                TmpText.text = "Switching world";
            }
            else
            {
                gameObject.SetActive(false);
            }
    }

    

}
