using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class THA_AI_Conversation : MonoBehaviour
{
    //https://avatarchat-ai.xana.net/tha_chat?input_string=Who%20are%20you%3F%20What%20is%20your%20oppupation&usr_id=1&owner_id=2121
    public string msg = "Hello Airin. What are you doing?";
    [Space(5)]
    public AirinFeedback airinFeedback;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            StartCoroutine(SetApiData());
    }

    IEnumerator SetApiData()
    {
        string id = ConstantsHolder.userId;
        string worldId = ConstantsHolder.xanaConstants.MuseumID;
        string ip = "https://avatarchat-ai.xana.net/tha_chat?input_string=";

        //if (!APIBasepointManager.instance.IsXanaLive)
        //    ip = "http://182.70.242.10:8034/";
        //else if (APIBasepointManager.instance.IsXanaLive)
        //    ip = "http://15.152.55.82:8054/";

        string url = ip + msg + "&usr_id=" + id + "&owner_id =" + worldId;
        Debug.LogError("<color=red> Communication URL(Airin): " + url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            airinFeedback = JsonUtility.FromJson<AirinFeedback>(request.downloadHandler.text);
            Debug.LogError("Message: " + airinFeedback.data);
            yield return null;
        }
        else
            Debug.LogError(request.error);
    }

    [System.Serializable]
    public class AirinFeedback
    {
        public string data = "";
    }

}
