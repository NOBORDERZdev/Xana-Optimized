using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPanel : MonoBehaviour
{
    public CanvasScaler MainCanvas;
    // Start is called before the first frame update
    public bool rectInterpolate;
    public Image fadeImage;
    bool reverse;
    bool m_scaleToZero;
    Transform parent;
    int factor = 10;
    int closeFactor = 20;

    private void Awake()
    {
        parent = this.transform.parent;
    }

    public void DisablePanel()
    {
        if (rectInterpolate)
        {

            rectInterpolate = false;
            this.transform.SetParent(parent);
            reverse = true;
            try
            {
                fadeImage.GetComponent<Animator>().SetBool("FadeIn", true);
            }
            catch (System.Exception e)
            {
                Debug.Log("Exception thrown");
            }
            GameManager.Instance.WorldBool = false;
            Invoke("SetFalse", 0f);
            GameManager.Instance.UiManager.ShowFooter(true);//rik
        }
    }
    private void OnDisable()
    {
        if ( GameManager.Instance.UiManager.HomePage && GameManager.Instance.UiManager.HomePage.activeInHierarchy)
            GameManager.Instance.UiManager.HomePage.SetActive(true);


        reverse = false;
        m_scaleToZero = false;
        this.transform.localScale = Vector3.one;
        GetComponent<CanvasGroup>().alpha = 1;

    }
    void SetFalse()
    {
        reverse = false;
        this.gameObject.SetActive(false);
    }

}
