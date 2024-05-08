using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class RegisterAsCompanyEmails : MonoBehaviour
{
    public enum ActorType { User, CompanyUser};
    public ActorType actorType;
    [Space(5)]
    public UnityEvent companyMemberAction;
    [SerializeField] int thaCompanyId;
    [SerializeField] int thaPageNumber;
    [SerializeField] int thaPageSize;
    public readonly List<string> emailList = new List<string>();

    void Start()
    {
        ConstantsHolder.xanaConstants.THA_CompanyEmail = this;
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
                for (int i = 0; i < json.data.rows.Count; i++)
                {
                    emailList.Add(json.data.rows[i].email);
                }
                SetEmailData(ConstantsHolder.xanaConstants.toyotaEmail);
            }
        }
    }

    // Call when user logged In
    public void SetEmailData(string mail)
    {
        Debug.LogError("Email: " + mail);
        ConstantsHolder.xanaConstants.toyotaEmail = mail;
        actorType = CheckEmailStatus() ? ActorType.CompanyUser : ActorType.User;
    }

    private bool CheckEmailStatus()
    {
        if (emailList.Contains(ConstantsHolder.xanaConstants.toyotaEmail))
        {
            companyMemberAction.Invoke();
            return true;
        }
        else
            return false;
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
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
    #endregion
}



