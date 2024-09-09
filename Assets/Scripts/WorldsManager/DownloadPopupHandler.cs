
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadPopupHandler : MonoBehaviour
{
    public GameObject DownloadPopup;
    public TextMeshProUGUI DownloadText;
    public Toggle AlwaysAllow;
    public Button AllowButton;
    public Button DenyButton;

    public HomeSceneLoader HomeSceneLoaderInstance;
    public static bool AlwaysAllowDownload;

    private TaskCompletionSource<bool> userResponseTCS;

    public static DownloadPopupHandler DownloadPopupHandlerInstance;
    private void Awake()
    {
        DownloadPopupHandlerInstance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("AlwaysAllow"))
            AlwaysAllowDownload = true;

        AllowButton.onClick.AddListener(OnAcceptButtonClicked);
        DenyButton.onClick.AddListener(OnDeclineButtonClicked);

    }
    //public void OnClickAllow()
    //{
    //    DownloadPopup.SetActive(false);
    //}

    //public void OnClickDeny()
    //{
    //    HomeSceneLoaderInstance.LoadMain(false);
    //}

    public void OnAlwaysAllow()
    {
        if(AlwaysAllow.isOn)
        {
            PlayerPrefs.SetFloat("AlwaysAllow",1);
            AlwaysAllowDownload = true;
        }
        else
        {
            PlayerPrefs.DeleteKey("AlwaysAllow");
            AlwaysAllowDownload = false;
        }
    }

    public async Task<bool> ShowDialogAsync()
    {
        AlwaysAllow.isOn = true;
        DownloadPopup.SetActive(true);

        userResponseTCS = new TaskCompletionSource<bool>();
        DownloadText.text = (XanaWorldDownloader.downloadSize/(1024*1024)).ToString()+" MB";
        // Wait until the user responds
        bool userResponse = await userResponseTCS.Task;

        DownloadPopup.SetActive(false); // Hide the dialog panel after response
        return userResponse;
    }

    // Called when the user clicks the Accept button
    private void OnAcceptButtonClicked()
    {
        userResponseTCS?.TrySetResult(true); // Complete the task with a result of true
        OnAlwaysAllow();
        XanaWorldDownloader.downloadSize = 0;
    }

    // Called when the user clicks the Decline button
    private void OnDeclineButtonClicked()
    {
        userResponseTCS?.TrySetResult(false); // Complete the task with a result of false
        HomeSceneLoaderInstance.LoadMain(false);
        XanaWorldDownloader.downloadSize = 0;
    }
}
