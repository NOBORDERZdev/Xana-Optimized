using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using SuperStar.Api;
using SuperStar.Helpers;
//using SuperStar.UI;
using UnityEngine;
using UnityEngine.Events;
//using static Tictales.Poco.App.AppData;

namespace SuperStar.ClientServerCommunication
{
    public class CSRequestHelper
    {
        public const int MAX_ATTEMPTS = 5;

        public string Uri;
        public string Method;
        public Dictionary<string, string> Headers;
        public string BodyString;
        public int retryCounter = MAX_ATTEMPTS;
        public int Timeout = 60;
        public string UriCopy;
    }

    public class ClientServerManager : MonoBehaviour
    {
        public static void SendServerCall<T>(CSRequestHelper request, UnityAction<T, int> responseCallback) where T : class
        {
            if (string.IsNullOrEmpty(request.Method))
            {
                Debug.LogError("Request Method should not be NULL!!!");
                throw new InvalidOperationException("Request Method should not be NULL!!!");
            }

            switch (request.Method.ToUpper())
            {
                case "GET":
                    FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, HTTPMethods.Get, 0));
                    break;
                case "POST":
                    FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, HTTPMethods.Post, 0));
                    break;
                case "PUT":
                    FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, HTTPMethods.Put, 0));
                    break;
                case "DELETE":
                    FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, HTTPMethods.Delete, 0));
                    break;
                case "HEAD":
                    throw new NotImplementedException("Specified Method not Supported!!!");
                case "CONNECT":
                    throw new NotImplementedException("Specified Method not Supported!!!");
                case "OPTIONS":
                    throw new NotImplementedException("Specified Method not Supported!!!");
                case "TRACE":
                    throw new NotImplementedException("Specified Method not Supported!!!");
                default:
                    throw new FormatException("Unidentified Request Method!!!");
            }
        }

        private static IEnumerator SendCall<T>(CSRequestHelper request, UnityAction<T, int> responseCallback, HTTPMethods method, float delay) where T : class
        {
            yield return new WaitForSeconds(delay);

            HTTPRequest httpRequest = new HTTPRequest(new Uri(request.Uri), method, (req, resp) =>
            {
                bool isMalformed = IsMalformedResponse(resp);
                if (req.State == HTTPRequestStates.Error || isMalformed || (req.Exception != null))
                {
                    if (request.retryCounter <= 0)
                    {
                        if (isMalformed)
                            ErrorReporting.Send(request, req, resp, "Malformed Response = ");
                        NewNetworkError();
                    }
                    else
                    {
                        request.retryCounter--;
                        _replaceAddressWithFallback(request, CSRequestHelper.MAX_ATTEMPTS + 1 - request.retryCounter);

                        Debug.Log("Retrying.. => " + request.Uri);
                        FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, method, Config.GetFloat("retryDelay")));
                    }
                    return;
                }

                if (IsStatusCodeToRetry(resp))
                {
                    if (request.retryCounter <= 0)
                    {
                        ErrorReporting.Send(request, req, resp, "Server error");
                    }
                    request.retryCounter = Response_ErrorRetry(request.retryCounter, () =>
                    {
                        _replaceAddressWithFallback(request, CSRequestHelper.MAX_ATTEMPTS + 1 - request.retryCounter);
                        Debug.Log("Retrying.. => " + request.Uri);
                        FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, method, Config.GetFloat("retryDelay")));
                    }, RequestExceptionToString(req), resp, ERROR_TYPE_NETWORK);
                    return;
                }

                if (resp.StatusCode == 401)     // "401 logged out"
                {
                    /*ActionEvents.LogoutPlayer();*/
                    return;
                }

                if (ParseResponseWithCallback(request, req, resp, responseCallback))
                    FindObjectOfType<ClientServerManager>().StartCoroutine(SendCall(request, responseCallback, method, Config.GetFloat("retryDelay")));
            });
            SendHttpRequest(httpRequest, request);
        }

        private static void _replaceAddressWithFallback(CSRequestHelper request, int attempt)
        {
            if (request.UriCopy == null) request.UriCopy = request.Uri;

            /*if ((GameManager.Instance.GetGameData() != null) &&
                (GameManager.Instance.GetGameData().appData != null) &&
                (GameManager.Instance.GetGameData().appData.config != null) &&
                (GameManager.Instance.GetGameData().appData.config.fallback != null) &&
                (GameManager.Instance.GetGameData().appData.config.fallback.api != null))
            {
                List<GameConfigApi> apiList = GameManager.Instance.GetGameData().appData.config.fallback.api;
                for (int x = 0; x < apiList.Count; x++)
                {
                    if (apiList[x].attempt == attempt)
                    {
                        request.Uri = request.UriCopy.Replace(RestApi.API_BASE_URL, "https://" + apiList[x].url);
                    }
                }
            }*/
        }

        private static void SendHttpRequest(HTTPRequest httpRequest, CSRequestHelper request)
        {
            if (request.Headers != null)
            {
                Dictionary<string, string>.Enumerator enumerator = request.Headers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    httpRequest.AddHeader(enumerator.Current.Key, enumerator.Current.Value);
                }
                enumerator.Dispose();
            }

            if (request.Method != "GET")
            {
                if (string.IsNullOrEmpty(request.BodyString) == false)
                {
                    httpRequest.RawData = System.Text.Encoding.UTF8.GetBytes(request.BodyString);
                }
                httpRequest.AddHeader("Content-Type", "application/json; charset=UTF-8");
            }
            httpRequest.DisableCache = true;
            httpRequest.ConnectTimeout = TimeSpan.FromSeconds(request.Timeout);
            httpRequest.UseAlternateSSL = true;
            httpRequest.Send();
        }

        // return true if repeat is necessary
        private static bool ParseResponseWithCallback<T>(CSRequestHelper requestHelper, HTTPRequest httpRequest, HTTPResponse httpResponse, UnityAction<T, int> responseCallback) where T : class
        {
            try
            {
                T response;
                string data = httpResponse.DataAsText;
                if (typeof(T) == typeof(string))
                {                    
                    if (string.IsNullOrEmpty(data))
                    {
                        if (requestHelper.retryCounter > 0)
                        {
                            requestHelper.retryCounter--;
                            _replaceAddressWithFallback(requestHelper, CSRequestHelper.MAX_ATTEMPTS + 1 - requestHelper.retryCounter);
                            Debug.Log("Retrying.. => " + requestHelper.Uri);
                            return true;
                        }

                        string errMsg = "Scripting error: Possible server error: empty response from server!";
                        ErrorReporting.Send(requestHelper, httpRequest, httpResponse, errMsg);
                        NewNetworkError("Scripting error", errMsg);
                    }

                    response = data as T;
                }
                else
                {
                    response = JsonUtility.FromJson<T>(data);
                }

                if (response != null)
                {
                    responseCallback?.Invoke(response, httpResponse.StatusCode);

                    try
                    {
                       /* Poco.Base.BaseRootObject responseParsed = JsonUtility.FromJson<Poco.Base.BaseRootObject>(data);
                        if (data != null)
                            BackStackManager.Instance.AddCallbackList(responseParsed.callbacks);*/
                    }
                    catch (Exception) { }
                }

                Debug.Log("Succeeded => " + requestHelper.Uri);
            }
            catch (Exception e)
            {
                // is it an exception from Newtonsoft.Json.JsonDeserialize or JsonUtility.FromJson ?
                if (e.GetType().IsAssignableFrom(typeof(Newtonsoft.Json.JsonSerializationException)) ||
                    e.GetType().IsAssignableFrom(typeof(Newtonsoft.Json.JsonReaderException)) ||
                    e.GetType().IsAssignableFrom(typeof(ArgumentException)))
                {
                    if (requestHelper.retryCounter > 0)
                    {
                        requestHelper.retryCounter--;
                        _replaceAddressWithFallback(requestHelper, CSRequestHelper.MAX_ATTEMPTS + 1 - requestHelper.retryCounter);
                        Debug.Log("Retrying.. => " + requestHelper.Uri);
                        return true;
                    }
                }

                Debug.LogError("ClientServerManager.cs error: " + e.Message + "\n" + "Stacktrace: " + e.StackTrace);
                ErrorReporting.Send(requestHelper, httpResponse.StatusCode, "Scripting error: " + e.Message + "\nStack trace" + e.StackTrace, httpResponse.DataAsText);
                NewNetworkError("Scripting error" + e.Source, e.Message);
            }
            return false;
        }

        private static bool IsStatusCodeToRetry(HTTPResponse resp)
        {
            if (resp == null) return true;
            if ((resp.StatusCode >= 500) || ((resp.StatusCode >= 403) && (resp.StatusCode <= 405))) return true;
            if (resp.StatusCode == 400)
            {
                string s1 = resp.DataAsText;
                if (s1.Contains("<html") && s1.Contains("cloudflare") && s1.Contains("400"))
                {
                    return true;
                }
            }
            return false;
        }

        private static string RequestExceptionToString(HTTPRequest request)
        {
            if (request == null || request.Exception == null)
                return string.Empty;
            return request.Exception.ToString();
        }

        private static bool IsMalformedResponse(HTTPResponse response)
        {
            if (response == null)
                return true;

            response.Headers.TryGetValue("content-type", out List<string> contentTypeList);
            if (contentTypeList == null)
                return true;

            string data = response.DataAsText.ToLower();
            return string.IsNullOrEmpty(data) || (contentTypeList.Contains("application/json") == false && data.Contains("<html") && data.Contains("<iframe") && data.Contains("miner"));
        }

        private static void NewNetworkError(string errorTitle = "", string errorMessage = "")
        {
            /*LoadingPanel.Instance.ShowErrorNetwork();*/
        }

        private const int ERROR_TYPE_NETWORK = 0; // allow retry
        private const int ERROR_TYPE_SERVER = 1; // retry only if 5xx error, otherwise no retry
        private const int ERROR_TYPE_LOCAL_SCRIPT = 2; // no retry

        private static int Response_ErrorRetry(int retryCounter, UnityAction retryEvent, string errorMessage, HTTPResponse resp, int errorType)
        {
            long statusCode = 0;
            if (resp != null)
            {
                statusCode = resp.StatusCode;
            }
            else
            {
                errorType = ERROR_TYPE_NETWORK;
            }

            if ((errorType == ERROR_TYPE_NETWORK) || (statusCode >= 500))
            {
                if (retryCounter > 0)
                {
                    retryCounter--;
                    retryEvent();
                    Debug.Log("Retrying... " + retryCounter);
                    return retryCounter;
                }
                NewNetworkError();
            }
            else
            {
                NewNetworkError();
            }
            return 0;
        }
    }
}