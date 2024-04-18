using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ApiResponseHolder", fileName = "ScriptableObjects/ApiResponseHolder")]
public class ResponseHolder : ScriptableObject
{

    public List<Response> apiResponses = new List<Response>();

    public void AddReponse(string key, string response)
    {
        Response res = new Response();
        res.apiKey = key;
        res.response = response;
        apiResponses.Add(res);
    }

    public bool CheckResponse(string apiKey)
    {
        foreach (var response in apiResponses)
        {
            if (response.apiKey == apiKey)
            {
                return true;
            }
        }

        return false;
    }

    public string GetResponse(string apiKey)
    {
        foreach (var response in apiResponses)
        {
            if (response.apiKey == apiKey)
            {
                return response.response;
            }
        }

        return null;
    }

    public void ChangeReponse(string key, string response)
    {
        Response res = apiResponses.Find(x => x.apiKey == key);
        if (res != null)
            res.response = response;
    }

    private void OnDisable()
    {
        apiResponses.Clear();
    }

    [Serializable]
    public class Response
    {
        public string apiKey;
        public string response;
    }
}
