using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SummitUserDetailPopup : MonoBehaviour
{
    public GameObject userProfilePopup;

    public TMPro.TextMeshProUGUI userName;
    public TMPro.TextMeshProUGUI company;
    public TMPro.TextMeshProUGUI role;
    public TMPro.TextMeshProUGUI Interest;
    public TMPro.TextMeshProUGUI wantToConnectWith;
    public TMPro.TextMeshProUGUI freeSelfIntroduction;
    private bool playerInfoAvailable;
    public string res;
    object[] playerData = new object[7];

    private bool isHolding;
    private float holdStartTime;
    private float holdEndTime = 0.5f;
    private int tapCounter;
    private void OnEnable()
    {
        if (GetComponent<PhotonView>().IsMine)
            GetUserInfo();
    }

    private void OnMouseDown()
    {
        isHolding = true;
        if (tapCounter == 0)
            holdStartTime = Time.time;
        tapCounter++;
    }

    private void OnMouseUp()
    {
        if (!playerInfoAvailable)
            return;

        isHolding = false;
        float holdTime = Time.time - holdStartTime;

        if (holdTime < holdEndTime && tapCounter == 2) // Adjust hold time threshold as needed
        {
            SetPopUpActive();
            tapCounter = 0;
        }
        if (tapCounter >= 2)
            tapCounter = 0;
    }

    async void GetUserInfo()
    {
        string s = await GetUserData(ConstantsGod.API_BASEURL + ConstantsGod.GETUSERDETAIL);
        if (string.IsNullOrEmpty(s))
            return;
        PlayerInfoParent playerInfo = JsonUtility.FromJson<PlayerInfoParent>(s);
        SendPlayerDataRPC(playerInfo);
        //SetPlayerData(playerInfo);
    }


    void SendPlayerDataRPC(PlayerInfoParent playerInfo)
    {
        playerData[0] = GetComponent<PhotonView>().ViewID;
        playerData[1] = playerInfo.data.name;
        playerData[2] = playerInfo.data.company;
        playerData[3] = playerInfo.data.role;
        playerData[4] = playerInfo.data.interest;
        playerData[5] = playerInfo.data.wantToConnectWith;
        playerData[6] = playerInfo.data.selfIntroduction;

        playerInfoAvailable = true;
        this.GetComponent<PhotonView>().RPC(nameof(SetPlayerDataOnRemoteSide), RpcTarget.AllBuffered, playerData as object);
    }

    [PunRPC]
    void SetPlayerDataOnRemoteSide(object[] playerDataReceived)
    {

        if (int.Parse(playerDataReceived[0].ToString()) == GetComponent<PhotonView>().ViewID)
        {
            userName.text = playerDataReceived[1].ToString();
            company.text = playerDataReceived[2].ToString();
            role.text = playerDataReceived[3].ToString();
            Interest.text = playerDataReceived[4].ToString();
            wantToConnectWith.text = playerDataReceived[5].ToString();
            freeSelfIntroduction.text = playerDataReceived[6].ToString();
            playerInfoAvailable = true;
        }
    }

    void SetPlayerData(PlayerInfoParent playerInfo)
    {
        userName.text = playerInfo.data.name;
        company.text = playerInfo.data.company;
        role.text = playerInfo.data.role;
        Interest.text = playerInfo.data.interest;
        wantToConnectWith.text = playerInfo.data.wantToConnectWith;
        freeSelfIntroduction.text = playerInfo.data.selfIntroduction;
    }

    async Task<string> GetUserData(string url)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
        string emailId = PlayerPrefs.GetString("LoggedInMail");
        Debug.LogError("get data for" + PlayerPrefs.GetString("LoggedInMail"));
        if (string.IsNullOrEmpty(emailId))
            return null;
        unityWebRequest.SetRequestHeader("Authorization", ConstantsHolder.xanaToken);
        unityWebRequest.SetRequestHeader("email", emailId);
        await unityWebRequest.SendWebRequest();
        Debug.LogError(unityWebRequest.downloadHandler.text);
        if (!string.IsNullOrEmpty(unityWebRequest.error))
            return null;
        return unityWebRequest.downloadHandler.text;
    }

    void SetPopUpActive()
    {
        userProfilePopup.SetActive(true);
    }

    [System.Serializable]
    public class PlayerInfoParent
    {
        public bool success;
        public PlayerInfo data;
        public string msg;
    }

    [System.Serializable]
    public class PlayerInfo
    {

        public string id;
        public string name;
        public string company;
        public string role;
        public string interest;
        public string wantToConnectWith;
        public string selfIntroduction;

    }
}
