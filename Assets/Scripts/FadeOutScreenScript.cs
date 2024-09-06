using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;


public class FadeOutScreenScript : MonoBehaviour
{
    public Image blackScreen;
   // float delay = 1.5f;
    float alpha = 1f;
    // Start is called before the first frame update
    private void OnEnable()
    {
        UserRegisterationManager.instance.BlackScreen.SetActive(true);
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        while (alpha > 0.0f)
        {
            yield return new WaitForSeconds(0.04f);
            alpha -= 0.05f;
            blackScreen.color = new Color(1f, 1f, 1f, alpha);
        }
       UserRegisterationManager.instance.BlackScreen.SetActive(false);
        alpha = 1f;
        blackScreen.color = new Color(1f, 1f, 1f, alpha);
    }
}
