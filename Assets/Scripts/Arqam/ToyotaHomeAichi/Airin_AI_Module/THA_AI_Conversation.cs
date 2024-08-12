using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class THA_AI_Conversation : MonoBehaviour
{
    //https://avatarchat-ai.xana.net/tha_chat?input_string=Who%20are%20you%3F%20What%20is%20your%20oppupation&usr_id=1&owner_id=2121
    public class AirinFeedback
    {
        public string data = "";
    }

    [SerializeField]
    private string _msg = "Hello Airin. What's going on?";
    private AirinFeedback _airinFeedback;
    private string _playerName = "";
    private Animator _animator;
    private bool _isAirinTyping = false;
    private string _ip;
    private string _url;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        ScreenOrientationManager.switchOrientation += CheckOrientation;
    }
    private void OnDisable()
    {
        ScreenOrientationManager.switchOrientation -= CheckOrientation;
    }

    public void AirinDeActivated()
    {
        NFT_Holder_Manager.instance.Extended_XCS.AirinQuestion -= ReplyUserMsg;
        //XanaChatSystem.instance.InputFieldChat.onSubmit.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        XanaChatSystem.instance.InputFieldChat.onSubmit.AddListener(XanaChatSystem.instance.OnEnterSend);
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.RemoveAllListeners();
        ConstantsHolder.xanaConstants.IsChatUseByOther = false;
    }

    public void StartConversation(string name)
    {
        ConstantsHolder.xanaConstants.IsChatUseByOther = true;
        //XanaChatSystem.instance.InputFieldChat.onSubmit.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        XanaChatSystem.instance.InputFieldChat.onSubmit.RemoveAllListeners();
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.AddListener(NFT_Holder_Manager.instance.Extended_XCS.SendMessage);
        NFT_Holder_Manager.instance.Extended_XCS.AirinQuestion += ReplyUserMsg;

        _playerName = name;
        StartCoroutine(SetApiData());
        StartCoroutine(EnableChatWindow());
    }

    private IEnumerator EnableChatWindow()
    {
        yield return new WaitForSeconds(2f);
        if (!NFT_Holder_Manager.instance.Extended_XCS.IsShowChatWindow())
            XanaChatSystem.instance.OpenCloseChatDialog();
    }

    private void ReplyUserMsg(string msg)
    {
        this._msg = msg;
        _isAirinTyping = true;
        NFT_Holder_Manager.instance.Extended_XCS.ShowMsgLocally("Airin", "typing...");
        _animator.SetBool("isChating", true);
        StartCoroutine(SetApiData());
    }
    private IEnumerator SetApiData()
    {
        yield return new WaitForSeconds(0.1f);
        string id = ConstantsHolder.userId;
        string worldId = ConstantsHolder.xanaConstants.MuseumID;
        if (ConstantsHolder.xanaConstants.MuseumID == "2871")
        {
            _ip = "http://182.70.242.10:8042/npc-chat?input_string=";
            _url = _ip + _msg + "&npc_id=" + worldId + "&personality_id=" + worldId + "&usr_id=" + id + "&personality_name=karen";
           // Debug.Log("jjtest " + ConstantsHolder.xanaConstants.MuseumID);
        }
        else
        {
            _ip = "https://avatarchat-ai.xana.net/tha_chat?input_string=";
            _url = _ip + _msg + "&usr_id=" + id + "&owner_id =" + worldId;
        }
        //if (!APIBasepointManager.instance.IsXanaLive)
        //    ip = "http://182.70.242.10:8034/";
        //else if (APIBasepointManager.instance.IsXanaLive)
        //    ip = "http://15.152.55.82:8054/";

        //Debug.LogError("<color=red> Communication URL(Airin): " + _url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(_url);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
        while (!request.isDone)
        {
            yield return null;
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            _airinFeedback = JsonUtility.FromJson<AirinFeedback>(request.downloadHandler.text);
            //Debug.LogError("Message: " + _airinFeedback.data);
            NFT_Holder_Manager.instance.Extended_XCS.ShowAirinMsg("Airin", _airinFeedback.data, _isAirinTyping);
            _isAirinTyping = false;
            _animator.SetBool("isChating", false);
        }
        else
        {
            NFT_Holder_Manager.instance.Extended_XCS.ClearAirinTypingMsg();
            _animator.SetBool("isChating", false);
            Debug.LogError(request.error);
        }
        request.Dispose();
    }

    private void CheckOrientation(bool IsPortrait)
    {
        StartCoroutine(RemoveListnerFromChat());
    }

    private IEnumerator RemoveListnerFromChat()
    {
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.RemoveAllListeners();
        yield return new WaitForSeconds(1);
        NFT_Holder_Manager.instance.SetChatRefrence();
        yield return new WaitForEndOfFrame();
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.RemoveAllListeners();
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.AddListener(NFT_Holder_Manager.instance.Extended_XCS.SendMessage);
    }


}
