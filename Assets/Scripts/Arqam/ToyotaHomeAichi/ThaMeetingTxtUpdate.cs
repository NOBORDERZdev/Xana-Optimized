using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using Photon.Realtime;
using static ThaMeetingStatusUpdate;
using System.Text;
using UnityEngine.Networking;
using static ThaMeetingTxtUpdate;

public class ThaMeetingTxtUpdate : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    private MeshRenderer portalMesh;
    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Awake()
    {
        //tmp.text = "Join Meeting Now!";
        NFT_Holder_Manager.instance.meetingTxtUpdate = this;
        portalMesh = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        WrapObjectOnOff();
    }

    public void UpdateMeetingTxt(string data)
    {
        tmp.text = "";
        tmp.text = data;
        // tmp.color = txtColor;
        tmp.alpha = 1f;
    }

    public async void WrapObjectOnOff()
    {
        StringBuilder ApiURL = new StringBuilder();
        ApiURL.Append(ConstantsGod.API_BASEURL + ConstantsGod.wrapobjectApi + 4);
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
                WrapObjectClass wrapObjectClass = JsonConvert.DeserializeObject<WrapObjectClass>(data.ToString());

                Debug.Log("Wrap Object Status is :: " + wrapObjectClass.success);
                portalMesh.enabled = wrapObjectClass.success;
                boxCollider.enabled = wrapObjectClass.success;
            }
        }
    }

    public class WrapObjectClass
    {
        public bool success { get; set; }
        public bool data { get; set; }
        public string msg { get; set; }
    }
}
