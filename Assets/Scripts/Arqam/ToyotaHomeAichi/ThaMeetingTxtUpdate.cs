using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Networking;

public class ThaMeetingTxtUpdate : MonoBehaviour
{
    public TextMeshProUGUI MeetingRoomText;

    private MeshRenderer _portalMesh;
    private BoxCollider _boxCollider;
    private int _testnetRoomId = 4;
    private int _mainnetRoomId = 2;
    void Awake()
    {
        MeetingRoomText.text = "";
        NFT_Holder_Manager.instance.meetingTxtUpdate = this;
        _portalMesh = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        WrapObjectOnOff();
    }

    public void UpdateMeetingTxt(string data)
    {
        MeetingRoomText.text = "";
        MeetingRoomText.text = data;
        // tmp.color = txtColor;
        MeetingRoomText.alpha = 1f;
    }

    public async void WrapObjectOnOff()
    {
        StringBuilder ApiURL = new StringBuilder();
        if (APIBasepointManager.instance.IsXanaLive)
        {
            ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.wrapobjectApi + _mainnetRoomId);
        }
        else
        {
            ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.wrapobjectApi + _testnetRoomId);
        }
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
                WrapObjectClass wrapObjectClass = JsonConvert.DeserializeObject<WrapObjectClass>(request.downloadHandler.text);

                Debug.Log("Wrap Object Status is :: " + wrapObjectClass.data);
                _portalMesh.enabled = wrapObjectClass.data;
                _boxCollider.enabled = wrapObjectClass.data;
            }
        }
    }

    public class WrapObjectClass
    {
        public bool success;
        public bool data;
        public string msg;
    }
}
