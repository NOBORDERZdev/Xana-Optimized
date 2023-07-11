using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResetPrefferedWidthScript : MonoBehaviour
{
    public TextMeshProUGUI textObj;
    public ContentSizeFitter contentSizeFitter;
    public LayoutElement layoutElement;

    public void SetupObjectWidth()
    {
        layoutElement.enabled = false;
        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 0);
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        if (this.gameObject.activeSelf)
        {
            StartCoroutine(WaitToReset());
        }
    }

    IEnumerator WaitToReset()
    {
        yield return new WaitForSeconds(0.02f);
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        yield return new WaitForSeconds(0.01f);
        if (this.GetComponent<RectTransform>().sizeDelta.x > 720f)
        {
            layoutElement.enabled = true;
            layoutElement.preferredWidth = 720;
        }

        textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, 1);
    }
}