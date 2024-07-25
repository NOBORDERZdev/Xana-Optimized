/*--------------------------------------
   Email  : hamza95herbou@gmail.com
   Github : https://github.com/herbou
----------------------------------------*/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class ButtonLongPressListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private GameObject copiedTextPanel;
    public PersonalData Data;
    public enum PersonalData
    {
        Email,
        phoneNumber,
        WalletAddress
    }
    [Tooltip("Hold duration in seconds")]
    [Range(0.3f, 5f)] public float holdDuration = 0.5f;
    public UnityEvent onLongPress;

    private bool isPointerDown = false;
    private bool isLongPressed = false;
    private DateTime pressTime;
    private Button button;
    private WaitForSeconds delay;



    private void Awake()
    {
        button = GetComponent<Button>();
        delay = new WaitForSeconds(0.1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pressTime = DateTime.Now;
        StartCoroutine(Timer());
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        isLongPressed = false;
    }
    public void DisplayMsg()
    {
        //string msg = "";
        TextEditor textEditor = new TextEditor();
        switch (Data)
        {
            case PersonalData.Email:
                //msg = "Your Xana email [" + FeedUIController.Instance.SNSSettingController.personalInfoEmailText.text + "] has been copied";
                textEditor.text = FeedUIController.Instance.SNSSettingController.personalInfoEmailText.text;
                break;
            case PersonalData.phoneNumber:
                //msg = "Your Xana Phone number [" + FeedUIController.Instance.SNSSettingController.personalInfoPhoneNumberText.text + "] has been copied";
                textEditor.text = FeedUIController.Instance.SNSSettingController.personalInfoPhoneNumberText.text;
                break;
            case PersonalData.WalletAddress:
                //msg = "Your Xana wallet address [" + FeedUIController.Instance.SNSSettingController.personalInfoPublicaddressText.text + "] has been copied";
                textEditor.text = FeedUIController.Instance.SNSSettingController.personalInfoPublicaddressText.text;
                break;
        }
        textEditor.SelectAll();
        textEditor.Copy();

#if UNITY_IOS || UNITY_EDITOR
        copiedTextPanel.SetActive(true);
        Invoke("DisableToast",1f);
#endif
        //SNSNotificationHandler.Instance.ShowNotificationMsg(msg);
    }


    private IEnumerator Timer()
    {
        while (isPointerDown && !isLongPressed)
        {
            double elapsedSeconds = (DateTime.Now - pressTime).TotalSeconds;

            if (elapsedSeconds >= holdDuration)
            {
                isLongPressed = true;
                if (button.interactable)
                    onLongPress?.Invoke();

                yield break;
            }

            yield return delay;
        }
    }
    private void DisableToast()
    {
        copiedTextPanel.SetActive(false);
    }
}
