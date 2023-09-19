using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UserRegisterationManager;

public class CheckCharacterType : MonoBehaviour
{
    private string presetGetApi = "/hot/items/get-admin/";
    public string emailData;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetEmailData());
    }

    public IEnumerator GetEmailData()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }

        MyClassOfLoginJson loginData = new MyClassOfLoginJson();
        loginData = loginData.CreateFromJSON(PlayerPrefs.GetString("UserNameAndPassword"));
        string api = ConstantsGod.API_BASEURL + presetGetApi + loginData.email;
        //print("api ~~~~~~" + api);
        print("Tokken :: " + ConstantsGod.AUTH_TOKEN);
        using (UnityWebRequest request = UnityWebRequest.Get(api))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            //print(" web request " + request.downloadHandler.text);

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    SpecialPresetData apiData = new SpecialPresetData();
                    SpecialPresetServerData apiData1 = new SpecialPresetServerData();

                    apiData = JsonUtility.FromJson<SpecialPresetData>(request.downloadHandler.text);
                    //print("API DATA IS " + apiData.data.presets);
                    if (apiData == null)
                        Debug.LogError(" NO data get from API");
                    else
                        emailData = apiData.data.email;
                }
            }
        }
    }

}
