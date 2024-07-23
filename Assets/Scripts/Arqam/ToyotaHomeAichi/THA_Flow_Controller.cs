using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class THA_Flow_Controller : MonoBehaviour
{
    public GameObject DeleteAcc_Screen;
    public GameObject DeleteAcc_Popup;
    public GameObject LoginWelcome_Screen;

    void Start()
    {
        if (ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            if (PlayerPrefs.GetInt("IsLoggedIn") == 1 || PlayerPrefs.GetInt("WalletLogin") == 1)
            {
                DeleteAcc_Screen.SetActive(true);
            }
        }

        GetEmailData();
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
        string _apiURL = ConstantsGod.API_BASEURL + "admin/get-features-list";
        Debug.Log("API URL is : " + _apiURL);
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
                for (int i=0; i<features.data.Count; i++)
                {
                    Debug.LogError(features.data[i].feature_name + ":" + features.data[i].feature_status);
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
