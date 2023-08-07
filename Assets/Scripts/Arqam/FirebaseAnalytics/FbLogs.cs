using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class FbLogs : MonoBehaviour
{

    IEnumerator Start()
    {
        string originalString = "Hy I am @ sign";
        originalString = Regex.Replace(originalString, @"\s", "");
        string trimmedString = originalString.Substring(0, Mathf.Min(originalString.Length, 10));

        yield return new WaitForSeconds(4f);
        Debug.Log("<color=red>" + trimmedString + "</color>");
        //RemoteLogs("ThisIsMyFirstLog");
    }

    public void RemoteLogs(string data)
    {
        if (Application.isEditor)
            Debug.Log("<color=red>LogData: " + data + "</color>");

        Firebase.Analytics.FirebaseAnalytics.LogEvent(data);
    }

    private void Update()
    {
        throwExceptionEvery60Updates();
    }

    int updatesBeforeException = 0;
    void throwExceptionEvery60Updates()
    {
        if (updatesBeforeException > 0)
        {
            updatesBeforeException--;
        }
        else
        {
            // Set the counter to 60 updates
            updatesBeforeException = 60;

            // Throw an exception to test your Crashlytics implementation
            RemoteLogs("this_is_my_first_log");
        }
    }


}
