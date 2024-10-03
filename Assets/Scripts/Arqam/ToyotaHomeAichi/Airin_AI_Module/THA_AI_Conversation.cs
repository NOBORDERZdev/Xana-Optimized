using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class THA_AI_Conversation : MonoBehaviour
{
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
    private Coroutine _apiCoroutine;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        ScreenOrientationManager.switchOrientation += CheckOrientation;
    }

    private void OnDisable()
    {
        // Remove all event listeners to prevent memory leaks
        ScreenOrientationManager.switchOrientation -= CheckOrientation;
    }

    private void OnDestroy()
    {
        // Clean up and stop running coroutines
        if (_apiCoroutine != null)
        {
            StopCoroutine(_apiCoroutine);
        }

        // Remove event listeners to avoid memory leaks
        AirinDeActivated();
    }

    public void AirinDeActivated()
    {
        NFT_Holder_Manager.instance.Extended_XCS.AirinQuestion -= ReplyUserMsg;
        XanaChatSystem.instance.InputFieldChat.onSubmit.AddListener(XanaChatSystem.instance.OnEnterSend);
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.RemoveAllListeners();
        ConstantsHolder.xanaConstants.IsChatUseByOther = false;
    }

    public void StartConversation(string name)
    {
        ConstantsHolder.xanaConstants.IsChatUseByOther = true;
        XanaChatSystem.instance.InputFieldChat.onSubmit.RemoveAllListeners();
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.AddListener(NFT_Holder_Manager.instance.Extended_XCS.SendMessage);
        NFT_Holder_Manager.instance.Extended_XCS.AirinQuestion += ReplyUserMsg;

        _playerName = name;

        // Start coroutine to set API data
        if (_apiCoroutine != null)
        {
            StopCoroutine(_apiCoroutine);
        }
        _apiCoroutine = StartCoroutine(SetApiData());
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
        _msg = msg;
        _isAirinTyping = true;
        _animator.SetBool("isChating", true);

        if (_apiCoroutine != null)
        {
            StopCoroutine(_apiCoroutine);
        }
        _apiCoroutine = StartCoroutine(SetApiData());
    }

    private IEnumerator SetApiData()
    {
        yield return new WaitForSeconds(0.1f);
        string id = ConstantsHolder.userId;

        if (ConstantsHolder.xanaConstants.MuseumID == "2871")
        {
            string worldId = ConstantsHolder.xanaConstants.MuseumID;
            _ip = "http://182.70.242.10:8042/npc-chat?input_string=";
            _url = _ip + _msg + "&npc_id=" + worldId + "&personality_id=" + worldId + "&usr_id=" + id + "&personality_name=karen";
        }
        else
        {
            string worldId = "41424_srz5bkcbnk";
            _ip = "https://avatarchat-ai.xana.net/tha_chat?input_string=";
            _url = _ip + _msg + "&usr_id=" + id + "&owner_id=" + worldId;
        }

        //Debug.LogError("<color=red> Communication URL(Airin): " + _url + "</color>");

        UnityWebRequest request = UnityWebRequest.Get(_url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            _airinFeedback = JsonUtility.FromJson<AirinFeedback>(request.downloadHandler.text);
            Debug.LogError("Message: " + _airinFeedback.data);
            NFT_Holder_Manager.instance.Extended_XCS.ShowAirinMsg("Airin", _airinFeedback.data, _isAirinTyping);
            _isAirinTyping = false;
            _animator.SetBool("isChating", false);
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
            _animator.SetBool("isChating", false);
        }

        request.Dispose();
    }

    private void CheckOrientation(bool IsPortrait)
    {
        StartCoroutine(RemoveListenerFromChat());
    }

    private IEnumerator RemoveListenerFromChat()
    {
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.RemoveAllListeners();
        yield return new WaitForSeconds(1);
        NFT_Holder_Manager.instance.SetChatRefrence();
        yield return new WaitForEndOfFrame();
        NFT_Holder_Manager.instance.Extended_XCS.InputFieldChat.onSubmit.AddListener(NFT_Holder_Manager.instance.Extended_XCS.SendMessage);
    }
}