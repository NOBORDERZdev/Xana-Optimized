using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class THA_Flow_Controller : MonoBehaviour
{
    public GameObject DeleteAcc_Screen;
    public GameObject DeleteAcc_Popup;
    public GameObject LoginWelcome_Screen;
    [Header("On Boarding Features Button")]
    public GameObject WalletBtn;
    public GameObject EmailBtn;
    public GameObject GoogleBtn;
    public GameObject AppleBtn;

    //https://api-test.xana.net/admin/get-features-list         // Get API
    //https://api-test.xana.net/admin/delete-feature-control    // Delete API
    //https://api-test.xana.net/admin/update-feature-control    // add feature API

    void Awake()
    {
        WalletBtn.SetActive(false);
        GoogleBtn.SetActive(false);
        AppleBtn.SetActive(false);
       
        if (PlayerPrefs.GetInt("IsLoggedIn") == 1 || PlayerPrefs.GetInt("WalletLogin") == 1)
        {
            if (!ConstantsHolder.xanaConstants.isBackFromWorld)
                Web3AuthCustom.Instance.onLoginAction += AfterLogin;
            DeleteAcc_Screen.SetActive(true);
        }
        if (PlayerPrefs.GetInt("WalletLogin") == 0)
        {
            DeleteAcc_Screen.SetActive(false);
            InventoryManager.instance.StartPanel_PresetParentPanel.SetActive(true);
        }
        LoadingHandler.Instance.nftLoadingScreen.SetActive(false);

    }

    private void Start()
    {
        GetEmailData();
    }
    private void OnDisable()
    {
        Web3AuthCustom.Instance.onLoginAction -= AfterLogin;
        // MainSceneEventHandler.OpenLandingScene -= OpenLandingScene;
    }

    //private void OpenLandingScene()
    //{
    //    Debug.LogError("NFT_LoadingTrue");
    //    LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
    //}

    private void AfterLogin(string email)
    {
        Debug.Log("<color=red>Email:" + email + "</color>");
        LoadingHandler.Instance.nftLoadingScreen.SetActive(false);
    }

    public void DeleteAccount()
    {
        UserLoginSignupManager.instance.DeleteAccount(() =>
        {
            DeleteAcc_Screen.SetActive(false);
            DeleteAcc_Popup.SetActive(false);
            LoginWelcome_Screen.SetActive(true);
        });
    }

    public void Load_THAWorld()
    {
        LoadingHandler.Instance.nftLoadingScreen.SetActive(true);
        MainSceneEventHandler.OpenLandingScene?.Invoke();
        StartCoroutine(Load_THA());
    }

    private IEnumerator Load_THA()
    {
        yield return new WaitForSeconds(0.1f);
        DeleteAcc_Screen.SetActive(false);
    }

    public async void GetEmailData()
    {
        string _apiURL = ConstantsGod.API_BASEURL + "/admin/get-features-list";
        //Debug.Log("<color=red>API URL is : " + _apiURL + "</color>");
        using (UnityWebRequest request = UnityWebRequest.Get(_apiURL))
        {
            //request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Error is" + request.error);
            }
            else
            {
                GetFeatureResponse features = JsonConvert.DeserializeObject<GetFeatureResponse>(request.downloadHandler.text);
                for (int i = 0; i < features.data.Count; i++)
                {
                    switch (features.data[i].feature_name)
                    {
                        case "WalletBtn":
                            WalletBtn.SetActive(features.data[i].feature_status);
                            break;
                        case "EmailBtn":
                            EmailBtn.SetActive(features.data[i].feature_status);
                            break;
                        case "GoogleBtn":
                            GoogleBtn.SetActive(features.data[i].feature_status);
                            break;
                        case "AppleBtn":
                            AppleBtn.SetActive(features.data[i].feature_status);
                            break;
                    }
                    //Debug.LogError(features.data[i].feature_name + ":" + features.data[i].feature_status);
                }
            }
        }
    }

    private class GetFeatureResponse
    {
        public bool success;
        public List<Data> data;
        public string msg;
    }

    private class Data
    {
        public int id;
        public string feature_name;
        public bool feature_status;
        public string created_at;
        public string updated_at;
    }

}
