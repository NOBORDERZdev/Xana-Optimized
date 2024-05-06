using ExitGames.Client.Photon.StructWrapping;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class RegisterAsCompanyEmail : MonoBehaviour
{
    public static RegisterAsCompanyEmail ins;
    [SerializeField] int thaCompanyId;
    [SerializeField] int thaPageNumber;
    [SerializeField] int thaPageSize;
    public List<string> emailList = new List<string>();
    void Start()
    {
        ins = this;
        GetEmailData();
    }
    public async void GetEmailData()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaEmailApi + thaCompanyId + "/" + thaPageNumber + "/" + thaPageSize);
        Debug.Log("API URL is : " + ApiURL.ToString());
        using (UnityWebRequest request = UnityWebRequest.Get(ApiURL.ToString()))
        {
            request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Error is" + request.error);
            }
            else
            {
               StringBuilder data = new StringBuilder();
                data.Append(request.downloadHandler.text);
                THAEmailDataResponse json = JsonConvert.DeserializeObject<THAEmailDataResponse>(data.ToString());
                Debug.Log("Data is : " + json.data.rows.Count);
                for (int i = 0; i < json.data.rows.Count; i++)
                {
                    Debug.Log("Email is : " + json.data.rows[i].email);
                    emailList.Add(json.data.rows[i].email);
                }
            }
        }
    }
}
public class THAJson
{
    public int count { get; set; }
    public List<THAItemsData> rows { get; set; }
}

public class THAEmailDataResponse
{
    public bool success { get; set; }
    public THAJson data { get; set; }
    public string msg { get; set; }
}

public class THAItemsData
{
    public int id { get; set; }
    public int worldId { get; set; }
    public string email { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}
