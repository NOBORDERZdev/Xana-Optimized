using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;

public class NpcChatSystem : MonoBehaviour
{
 
    // api default link: http://182.70.242.10:8032/docs#/update_npc_prompt_userid_en_api_v1_update_user_prompt_en_post

    //[SerializeField]
    private string apiURL = "http://182.70.242.10:8032/docs#/default/update_npc_prompt_userid_en_api_v1_update_user_prompt_en_post";
    private string defaultApi = "http://182.70.242.10:8032/docs#/";
    private string actualApi = "http://182.70.242.10:8032/api/v1/update_user_prompt_en";

    void Start()
    {
        // Call the API request function
       // StartCoroutine(CheckEndpointValidity(actualApi));
        StartCoroutine(SetApiData());
        //StartCoroutine(GetApiData());
    }

    IEnumerator CheckEndpointValidity(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // The API endpoint is valid, and you can check the response content if needed
            Debug.Log("API Endpoint is valid.");
            Debug.Log("API Endpoint Response: " + request.downloadHandler.text);
        }
        else
        {
            // The API endpoint is not valid, or there was an error
            Debug.LogError("API Endpoint is not valid or there was an error: " + request.error);
        }
    }

    IEnumerator SetApiData()
    {
        //MyData postData = new MyData
        //{
        //    intValue = 42,          // Example integer value
        //    stringValue = "Hello"   // Example string value
        //};

        WWWForm form = new WWWForm();
        form.AddField("id", 5);
        form.AddField("prompt", "I am unity dev");
        UnityWebRequest request;
        request = UnityWebRequest.Post(actualApi, form);
        request.SetRequestHeader("Authorization", ConstantsGod.AUTH_TOKEN);
        yield return request.SendWebRequest();

        string str = request.downloadHandler.text;
        Debug.Log("Communication: " + str);
        APIFeedResponse response = new APIFeedResponse();

        if (!request.isHttpError && !request.isNetworkError)
        {
            response = JsonUtility.FromJson<APIFeedResponse>(str);
            Debug.Log("<color=red> Communication Id: " + response.id + "::::" + response.msg + "</color>");
        }
        else
            Debug.Log("<color=red> Communication_Issue: " + request.error + "</color>");

        request.Dispose();

        //UnityWebRequest request = new UnityWebRequest(apiURL);
        //string jsonData = JsonUtility.ToJson(postData);
        //byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        //request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        //request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        //request.SetRequestHeader("Content-Type", "application/json");
        //yield return request.SendWebRequest();

        //if (request.result == UnityWebRequest.Result.Success)
        //{
        //    // Handle the successful response here
        //    string responseText = request.downloadHandler.text;
        //    Debug.Log("API Response: " + responseText);
        //}
        //else
        //{
        //    // Handle the error here
        //    Debug.LogError("<color=red> API Request Error: " + request.error + "</color>");
        //}
    }

    IEnumerator GetApiData()
    {
        #region Method1
        // Create a WWW object to make the GET request
        WWW www = new WWW(apiURL);

        // Wait for the request to complete
        yield return www;

        // Check for errors
        if (string.IsNullOrEmpty(www.error))
        {
            // Data retrieval successful
            string responseData = www.text;

            int integerValue;
            if (int.TryParse(responseData, out integerValue))
            {
                // Successfully converted to an integer
                Debug.Log("API Integer Response: " + integerValue);
            }
            else
            {
                // Conversion to integer failed
                Debug.LogError("API Response is not a valid integer: " + responseData);
            }
            // You can parse and process the data here
        }
        else
        {
            // Error occurred during the request
            Debug.LogError("API Request Error: " + www.error);
        }
        #endregion

        #region Method2
        //using (WWW www = new WWW(apiURL))
        //{
        //    while (!www.isDone)
        //    {
        //        yield return null;
        //    }
        //    yield return www;
        //    if (www.error != null)
        //    {
        //        throw new Exception("WWW download had an error:" + www.error);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            if (string.IsNullOrEmpty(www.error))
        //            {
        //                // Data retrieval successful
        //                string responseData = www.text;
        //                Debug.Log("API Response: " + responseData);
        //                // parse and process the data here
        //            }
        //        }
        //        catch
        //        {
        //            throw new Exception("oops! Some thing wrong.");
        //        }
        //    }
        //}
        #endregion
    }

}

[System.Serializable]
public class MyData
{
    public int intValue;
    public string stringValue;
}

[Serializable]
public class APIFeedResponse
{
    public int id;
    public string msg;
}
