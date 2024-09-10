using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class EmailEntryController : MonoBehaviour
{
    public string WorldIdTestnet;
    public string WorldIdMainnet;
    public string WorldId;
    public bool IsEmailVerificationReq = false;
    public SingleDomeAuthEmails AuthEmailData;
    private bool alreadyTriggered;

    private void OnEnable()
    {
        if (APIBasepointManager.instance.IsXanaLive)
            WorldId = WorldIdMainnet;
        else
            WorldId = WorldIdTestnet;
    }

    // Start is called before the first frame update
    async void Start()
    {
        AuthEmailData = await GetSingleDomeEmails(ConstantsHolder.domeId.ToString());
        if (AuthEmailData.data != null && AuthEmailData.data.Count > 0)
        {
            foreach (string email in AuthEmailData.data)
            {
                Debug.Log("==============Auth Email==============: " + email);
            }
        }
        else
        {
            Debug.Log("No Auth Email Data Found Against Dome ID: " + ConstantsHolder.domeId);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && !alreadyTriggered)
            {
                alreadyTriggered = true;
                if (IsEmailVerificationReq)
                {

                    
                }
                else
                {
                    TriggerSceneLoading(WorldId);
                    DisableCollider();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (PhotonNetwork.InRoom)
        {
            if (other.tag == "PhotonLocalPlayer" && other.GetComponent<PhotonView>().IsMine && alreadyTriggered)
            {
                alreadyTriggered = false;
            }
        }
    }

    void TriggerSceneLoading(string WorldId)
    {
        if (ConstantsHolder.MultiSectionPhoton)
        {
            ConstantsHolder.DiasableMultiPartPhoton = true;
        }
        BuilderEventManager.LoadSceneByName?.Invoke(WorldId, transform.GetChild(1).transform.position);
    }

    async void DisableCollider()
    {
        await Task.Delay(2000);
        alreadyTriggered = false;
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
