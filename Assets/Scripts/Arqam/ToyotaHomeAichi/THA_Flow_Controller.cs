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

    void Start()
    {
        if (ConstantsHolder.xanaConstants.isBackFromWorld)
        {
            if (PlayerPrefs.GetInt("IsLoggedIn") == 1 || PlayerPrefs.GetInt("WalletLogin") == 1)
            {
                DeleteAcc_Screen.SetActive(true);
            }
        }
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

    public static async Task<string> GetFeaturesList()
    {
        UnityWebRequest www = new UnityWebRequest(ConstantsGod.API_BASEURL + "admin/get-features-list");
                                                                                       
        www.downloadHandler = new DownloadHandlerBuffer();
        await www.SendWebRequest();

        if (www.error != null)
        {
            Debug.Log("GetFeaturesList>> " + www.error);
            return null;
        }
        else
        {
            // Show results as text
            string resultdata = www.downloadHandler.text;
            Debug.LogError("GetFeaturesList Success data : " + resultdata);

            return resultdata;
        }
    }

}
