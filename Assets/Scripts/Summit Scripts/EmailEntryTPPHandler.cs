using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class EmailEntryTPPHandler : MonoBehaviour
{
    public List<GameObject> TeleportPoints = new List<GameObject>();
    public SingleDomeAuthEmails AuthEmailData;
    public bool IsEmailVerificationReq = false;
    public bool IsFeatureRequired = false;

    private void OnEnable()
    {
        if (ConstantsHolder.isFromXANASummit && IsFeatureRequired)
        {
            foreach (GameObject obj in TeleportPoints)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject obj in TeleportPoints)
            {
                obj.SetActive(false);
            }
        }
    }

    async void Start()
    {
        if (IsEmailVerificationReq && IsFeatureRequired)
        {
            AuthEmailData = await GetSingleDomeEmails(ConstantsHolder.domeId.ToString());
        }
    }

    async Task<SingleDomeAuthEmails> GetSingleDomeEmails(string _domeID)
    {
        string url;
        url = ConstantsGod.API_BASEURL + ConstantsGod.GETSINGLEDOMEAUTHEMAILS + _domeID;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            await www.SendWebRequest();
            if ((www.result == UnityWebRequest.Result.ConnectionError) || (www.result == UnityWebRequest.Result.ProtocolError))
            {
                www.Dispose();
                return null;
            }
            else
            {
                SingleDomeAuthEmails authEmailData = new SingleDomeAuthEmails();
                authEmailData = JsonUtility.FromJson<SingleDomeAuthEmails>(www.downloadHandler.text);
                www.Dispose();
                return authEmailData;
            }
        }
    }

}

[System.Serializable]
public class SingleDomeAuthEmails
{
    public bool success;
    public List<string> data;
    public string msg;
}
