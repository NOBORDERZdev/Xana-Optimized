using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using Photon.Realtime;
using static ThaMeetingStatusUpdate;
using System.Text;
using UnityEngine.Networking;

public class ThaMeetingTxtUpdate : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public GameObject portalObject;
    // Start is called before the first frame update
    void Awake()
    {
        //tmp.text = "Join Meeting Now!";
        NFT_Holder_Manager.instance.meetingTxtUpdate = this;
        WrapObjectOnOff();
    }

    public void UpdateMeetingTxt(string data)
    {
        tmp.text = "";
        tmp.text = data;
        // tmp.color = txtColor;
        tmp.alpha = 1f;
    }
    async void WrapObjectOnOff()
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
                if(wrapObjectClass.success)
                {
                    Debug.Log("Portal Object is Active");
                    portalObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Portal Object is DeActive");
                    portalObject.SetActive(false);
                }
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
