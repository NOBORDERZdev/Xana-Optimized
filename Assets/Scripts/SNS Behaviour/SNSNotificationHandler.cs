using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SNSNotificationHandler : MonoBehaviour
{
    public static SNSNotificationHandler Instance;
    [Header("Top Notification Reference")]
    public RectTransform notificationPanelOnb;
    public TextMeshProUGUI messageText;

    [Header("Delete Loader Reference")]
    public GameObject deleteLoader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ShowNotificationMsg(string msg)
    {
        notificationPanelOnb.gameObject.SetActive(true);
        messageText.text = TextLocalization.GetLocaliseTextByKey(msg);
        notificationPanelOnb.DOAnchorPosY(-40, 0.3f).SetEase(Ease.Linear);
        if (waitToCloseNotificationCo != null)
        {
            StopCoroutine(waitToCloseNotificationCo);
        }
        waitToCloseNotificationCo = StartCoroutine(NotificationPanelClose());
    }

    Coroutine waitToCloseNotificationCo;
    IEnumerator NotificationPanelClose()
    {
        yield return new WaitForSeconds(2.5f);
        notificationPanelOnb.DOAnchorPosY(250, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        notificationPanelOnb.gameObject.SetActive(false)
        ); 
    }

    public void DeleteLoaderShow(bool isActive)
    {
        deleteLoader.SetActive(isActive);
    }

    public void ResetAndInstantHideNotificationBar()
    {
        if (notificationPanelOnb.gameObject.activeInHierarchy)
        {
            notificationPanelOnb.anchoredPosition = new Vector2(0, 250);
            notificationPanelOnb.gameObject.SetActive(false);
        }
    }
}