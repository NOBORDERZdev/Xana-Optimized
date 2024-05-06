using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPanelNFT : MonoBehaviour
{
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
            this.transform.parent.GetComponent<PointerDown>().timer = 0;
            // GameManager.Instance.WorldBool = false;
            //Invoke("SetFalse", 0.15f);
            // GameManager.Instance.UiManager.ShowFooter(true);//rik
        }
        Invoke("SetFalse", 0f);
    }

    private void OnDisable()
    {
        //if (GameManager.Instance.UiManager.HomePage.activeInHierarchy)
        //    GameManager.Instance.UiManager.HomePage.SetActive(true);


        reverse = false;
        m_scaleToZero = false;
        this.transform.localScale = Vector3.one;
        GetComponent<CanvasGroup>().alpha = 1;

    }

    // Update is called once per frame

    private void Update()
    {
        if (rectInterpolate)
        {
            //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, factor * Time.deltaTime);
            //transform.GetComponent<RectTransform>().offsetMin = Vector2.Lerp(transform.GetComponent<RectTransform>().offsetMin, Vector2.zero, factor * Time.deltaTime);
            //transform.GetComponent<RectTransform>().offsetMax = Vector2.Lerp(transform.GetComponent<RectTransform>().offsetMax, Vector2.zero, factor * Time.deltaTime);
            //transform.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(transform.GetComponent<RectTransform>().anchorMin, Vector2.zero, factor * Time.deltaTime);
            //transform.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(transform.GetComponent<RectTransform>().anchorMax, Vector2.one, factor * Time.deltaTime);
            //transform.GetComponent<RectTransform>().pivot = Vector2.Lerp(transform.GetComponent<RectTransform>().pivot, new Vector2(0.5f, 1f), factor * Time.deltaTime);
        }
        if (reverse)
        {
            SetRect(this.transform.GetComponent<RectTransform>());
        }
    }
    void SetRect(RectTransform rec)
    {
        //transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.4f, 0.4f, 0.4f), closeFactor * Time.deltaTime);
        //GetComponent<CanvasGroup>().alpha = Mathf.Lerp(GetComponent<CanvasGroup>().alpha, 0, closeFactor * Time.deltaTime);
        //transform.GetComponent<RectTransform>().offsetMin = Vector2.Lerp(transform.GetComponent<RectTransform>().offsetMin, Vector2.zero, closeFactor * Time.deltaTime);
        //transform.GetComponent<RectTransform>().offsetMax = Vector2.Lerp(transform.GetComponent<RectTransform>().offsetMax, Vector2.zero, closeFactor * Time.deltaTime);
        //transform.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(transform.GetComponent<RectTransform>().anchorMin, Vector2.zero, closeFactor * Time.deltaTime);
        //transform.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(transform.GetComponent<RectTransform>().anchorMax, Vector2.one, closeFactor * Time.deltaTime);
        //transform.GetComponent<RectTransform>().pivot = Vector2.Lerp(transform.GetComponent<RectTransform>().pivot, new Vector2(0.5f, 0.5f), closeFactor * Time.deltaTime);
    }

    void SetFalse()
    {
        reverse = false;
        // m_scaleToZero = true;
        this.gameObject.SetActive(false);
        // Invoke("SetDisable", 0.1f);
    }

}
