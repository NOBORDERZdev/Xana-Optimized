using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public class RegisterAsCompanyEmails : MonoBehaviour
{
    [SerializeField] private int _thaCompanyId;
    [SerializeField] private int _thaPageNumber;
    [SerializeField] private int _thaPageSize;
    
    private string _toyotaUserEmail;

    private void Start()
    {
        Web3AuthCustom.Instance.onLoginAction += UserLoggedIn;
        if (APIBasepointManager.instance.IsXanaLive)
        {
            _thaCompanyId = 2;
        }
        else
        {
            _thaCompanyId = 4;
        }
    }
    private void OnDisable()
    {
        Web3AuthCustom.Instance.onLoginAction -= UserLoggedIn;
    }

    private void UserLoggedIn(string userEmail)
    {
        _toyotaUserEmail = userEmail;
        GetEmailData();
    }

    public async void GetEmailData()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.toyotaEmailApi + _thaCompanyId + "/" + _thaPageNumber + "/" + _thaPageSize);
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
                for (int i = 0; i < json.data.rows.Count; i++)
                {
                    FB_Notification_Initilizer.Instance.companyEmails.Add(json.data.rows[i].email);
                    FB_Notification_Initilizer.Instance.fbTokens.Add(json.data.rows[i].userToken);
                }
                SetEmailData();
            }
        }
    }

    // Call when user logged In
    private void SetEmailData()
    {
        FB_Notification_Initilizer.Instance.InitPushNotification(_toyotaUserEmail);
    }

    #region OutputClasses
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
        public string userToken { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
    #endregion
}



