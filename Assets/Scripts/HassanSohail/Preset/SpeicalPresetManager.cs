using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static UserRegisterationManager;

public class SpeicalPresetManager : MonoBehaviour
{
    [SerializeField] List<GameObject> presetButtons;
    string presetGetApi = "/hot/items/get-admin/";
    void Start()
    {
        turnAllPresetOff();
        //print("is logged "+ PlayerPrefs.GetString("IsLoggedIn") );
       
    }

    /// <summary>
    /// To disable all specials presets.
    /// </summary>
    public void turnAllPresetOff() {
        foreach (var preset in presetButtons)
        {
            preset.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// To update speical preset button status
    /// </summary>
    public IEnumerator SetSpecialPresetButtons() {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return new WaitForEndOfFrame();
            print("Internet Not Reachable");
        }

        MyClassOfLoginJson loginData = new MyClassOfLoginJson();
        loginData = loginData.CreateFromJSON(PlayerPrefs.GetString("UserNameAndPassword"));
        string api = ConstantsGod.API_BASEURL + presetGetApi + loginData.email;
        print("api ~~~~~~"+api);
        print("Tokken :: "+ ConstantsGod.AUTH_TOKEN);
        using (UnityWebRequest request = UnityWebRequest.Get(api))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            yield return request.SendWebRequest();
            print(" web request " + request.downloadHandler.text);

            if (!request.isHttpError && !request.isNetworkError)
            {
                if (request.error == null)
                {
                    SpecialPresetData apiData = new SpecialPresetData();
                    SpecialPresetServerData apiData1 = new SpecialPresetServerData();

                    apiData = JsonUtility.FromJson<SpecialPresetData>(request.downloadHandler.text);
                   // apiData1 = JsonUtility.FromJson<SpecialPresetServerData>(request.downloadHandler.text);
                    print("API DATA IS " + apiData.data.presets);
                    if (apiData == null)
                    {
                        Debug.LogError(" NO data get from API");
                    }
                    else
                    {
                        //string[] allowedPresets = new string[apiData.data.presets.Length];
                        if (apiData.data.presets[0]!=null) {
                            print("~~~ DATA IS " + apiData.data.presets);
                            // allowedPresets.CopyTo(apiData.data.presets);
                            ChangePresetStatus(apiData.data.presets);

                        }
                       
                    }
                }
            }
        }
    }

    /// <summary>
    /// to Show and hide preset according to account
    /// </summary>
    /// <param name="data"></param>
    void ChangePresetStatus(string[] dataArary) {
        for (int i = 0; i < presetButtons.Count; i++)
        {
            if (dataArary.Contains(presetButtons[i].gameObject.name))
            {
                presetButtons[i].gameObject.SetActive(true);
            }
            else
            {
                presetButtons[i].gameObject.SetActive(false);
            }
        }
    }
}


[Serializable]
public class SpecialPresetData
{
    public bool success;
    public SpecialPresetServerData data;
    public string msg;
}

[Serializable]
public class SpecialPresetServerData {
    public int id;
    public string name;
    public string email;
    public string[] presets = new string[4];
    public Time createdAt;
    public Time updatedAt;

    public SpecialPresetServerData() { 
    
    }

    public SpecialPresetServerData(int _id, string _name, string _email) {
        id = _id;
        name = _name;
        email = _email;
       // presets = _presets;
    }

}