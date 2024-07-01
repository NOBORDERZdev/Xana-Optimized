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

    object[] playerData = new object[6];
    private void OnEnable()
    {
        GetUserInfo();
    }

    private void OnMouseDown()
    {
        if(playerInfoAvailable)
         SetPopUpActive();
    }

    async void GetUserInfo()
    {
        string s=await GetUserData(ConstantsGod.GETUSERDETAIL);
        if (string.IsNullOrEmpty(s))
            return;        
        PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(s);
        SendPlayerDataRPC(playerInfo);
        //SetPlayerData(playerInfo);
    }


    void SendPlayerDataRPC(PlayerInfo playerInfo)
    {
        playerData[0] = playerInfo.Name;
        playerData[1] = playerInfo.Company;
        playerData[2] = playerInfo.Role;
        playerData[3] = playerInfo.Interest;
        playerData[4] = playerInfo.WantToConnectWith;
        playerData[5] = playerInfo.SelfIntroduction;

        playerInfoAvailable = true;
        this.GetComponent<PhotonView>().RPC(nameof(SetPlayerDataOnRemoteSide), RpcTarget.All, playerData as object);
    }

    [PunRPC]
    void SetPlayerDataOnRemoteSide(object[] playerDataReceived)
    {
        userName.text = playerDataReceived[0].ToString();
        company.text = playerDataReceived[1].ToString();
        role.text = playerDataReceived[2].ToString();
        Interest.text = playerDataReceived[3].ToString();
        wantToConnectWith.text = playerDataReceived[4].ToString();
        freeSelfIntroduction.text = playerDataReceived[5].ToString();
        playerInfoAvailable = true;
    }

    void SetPlayerData(PlayerInfo playerInfo)
    {
        userName.text = playerInfo.Name;
        company.text = playerInfo.Company;
        role.text = playerInfo.Role;
        Interest.text = playerInfo.Interest;
        wantToConnectWith.text = playerInfo.WantToConnectWith;
        freeSelfIntroduction.text = playerInfo.SelfIntroduction;
    }

    async Task<string> GetUserData(string url)
    {
        return res;

        UnityWebRequest unityWebRequest =UnityWebRequest.Get(url);
        string emailId = PlayerPrefs.GetString("LoggedInMail");
        if (string.IsNullOrEmpty(emailId))
            return null;
        unityWebRequest.SetRequestHeader("email", emailId);
        await unityWebRequest.SendWebRequest();
        if (!string.IsNullOrEmpty(unityWebRequest.error))
            return null;
        return unityWebRequest.downloadHandler.text;
    }

    void SetPopUpActive()
    {
        userProfilePopup.SetActive(true);
    }


    [System.Serializable]
    public class PlayerInfo
    {
        public string Name;
        public string Company;
        public string Role;
        public string Interest;
        public string WantToConnectWith;
        public string SelfIntroduction;

    }
}
