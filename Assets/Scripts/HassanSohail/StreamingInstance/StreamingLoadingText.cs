using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class StreamingLoadingText : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text TmpText;
    [SerializeField] string loadingScreenTxt;
    [SerializeField] List<GameObject> OffObjs;
    [SerializeField] GameObject barparent;
    [SerializeField] Image bar;
    [SerializeField] GameObject bufferText;
    [SerializeField] TMP_Text percentageTxt;
    private void Start()
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
          StartCoroutine( ResetLoadingBar());
        }
    }
    public void UpdateLoadingText(bool movingToWorld)
    {
        //if (XanaConstants.xanaConstants.isCameraMan)
        //{
        //        if (movingToWorld)
        //    TmpText.text= loadingScreenTxt+XanaConstants.xanaConstants.JjWorldTeleportSceneName.ToString();
        //        else
        //    TmpText.text = "Switching world";
        //}
        //else
        //{
        //    gameObject.SetActive(false);
        //}
    }

    public void FullFillBar(){ 
         bar.DOFillAmount(1f,0.1f);
    }

    public IEnumerator ResetLoadingBar()
    {
        yield return new WaitForSeconds(0);
        bufferText.SetActive(true);
        percentageTxt.text ="0%";
        bar.DOFillAmount(0,.001f);
        bar.gameObject.SetActive(true);
        bar.DOFillAmount(0.95f,20);
        TmpText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (percentageTxt!= null)
        {
            percentageTxt.text = (int) (bar.fillAmount *100f) +"%";
        }
    }
}
