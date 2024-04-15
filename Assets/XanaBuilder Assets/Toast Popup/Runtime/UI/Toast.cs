using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Toast : MonoBehaviour
{
    public Text msgText;
    public TextMeshProUGUI msgTextPro;

    public bool isToastAutoDisappear = true;

    static Toast _instance;

    public bool isPointerEnter = false;



    public Image successOrFailuerThumbnail;
    public Sprite[] successFailureSprites;


    static Toast instance
    {
        get
        {
            if (_instance == null)
            {
                FindObjectOfType<Toast>().GetComponent<Toast>().init();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    static Transform elements;
    float displayTime = 5;

    void OnEnable()
    {
        init();
    }

    public void init()
    {
        instance = this;
        elements = transform.GetChild(0);
        elements.gameObject.SetActive(false);
    }

    void Test()
    {
        Show("Updated Successfully");
    }

    public static void Show(string msg)
    {
        Debug.Log("Message " + msg);
        instance.ShowIns(msg, instance.displayTime, false, elements.position);
    }

    public static void ShowError(string msg)
    {
        Debug.Log("Message " + msg);
        instance.ShowIns(msg, instance.displayTime, false, elements.position, 1);
    }

    public static void Show(string msg, Vector3 position)
    {
        instance.ShowIns(msg, instance.displayTime, true, position);
    }

    public static void Show(string msg, float time, Vector3 position)
    {
        instance.ShowIns(msg, time, true, position);
    }

    void ShowIns(string msg, float time, bool customPos, Vector3 position, byte _spriteIndex = 0)
    {

        if (!isToastAutoDisappear)
            time = 9999;
        //Localisation
        string localizedMsg = string.Empty;
        if (LocalizationManager.IsReady)
        {
            localizedMsg = TextLocalization.GetLocaliseTextByKey(msg);
        }
        if(localizedMsg.IsNullOrEmpty()) localizedMsg = msg;

        var spriteIndex = _spriteIndex.Equals(0) ? successOrFailuerThumbnail.sprite = successFailureSprites[0] :
        successOrFailuerThumbnail.sprite = successFailureSprites[1];

        if (msgText) msgText.text = localizedMsg;
        if (msgTextPro) msgTextPro.text = localizedMsg;
        //Debug.Log("Text Overflow  " + localizedMsg.Length);
        Transform tElements = elements.Duplicate();

        tElements.localScale = Vector3.one;
        tElements.gameObject.SetActive(true);

        ToastElementAnimation toastElement = tElements.GetComponent<ToastElementAnimation>();

        if (toastElement != null)
        {
            Delayed.Function((g) =>
            {
                g.PlayOpenAnimation(time);
            }, toastElement, .01f);
            return;
        }
        //new Interpolate.Position(tElements, position, position + new Vector3(0, 50 * elements.lossyScale.x, 0), .6f);
        print("Interpolate 1");
        new Interpolate.Position(tElements, tElements.position + new Vector3(300 * elements.lossyScale.x, 0, 0), tElements.position, .6f);

        Delayed.Function<Transform>((g) =>
        {
            print("Interpolate 2");
            new Interpolate.Position(tElements, tElements.position, new Vector3(300 * elements.lossyScale.x, 0, 0) + tElements.position, .2f);
            //new Interpolate.Scale(g, Vector3.one, Vector3.zero, .2f);
        }, tElements, time);

        Delayed.Function<GameObject>((g) =>
        {
            print("Destroy 1");
            Destroy(g);
        }, tElements.gameObject, time + .3f);

    }



}
