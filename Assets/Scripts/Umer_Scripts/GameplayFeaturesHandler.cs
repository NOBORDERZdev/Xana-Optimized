using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayFeaturesHandler : MonoBehaviour
{
    #region Variables

    //UI references
    public GameObject selfieBtn;
    public GameObject EmojiesBtn;
    public GameObject ReactionBtn;
    public GameObject favouriteBtn;
    public GameObject textChatBtn;
    public GameObject textChatBtn2;
    public GameObject voiceChatOnBtn;
    public Image voiceChatOffBtn1;
    public Image voiceChatOffBtn2;
    public GameObject voiceChatSettingBtn;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        if (EventDetails.eventDetails.DataIsInitialized)
        {
            selfieBtn.SetActive(EventDetails.eventDetails.selfie);
            EmojiesBtn.SetActive(EventDetails.eventDetails.emotes);
            ReactionBtn.SetActive(EventDetails.eventDetails.emotes);
            favouriteBtn.SetActive(EventDetails.eventDetails.emotes);
            textChatBtn.SetActive(EventDetails.eventDetails.messages);
            textChatBtn2.SetActive(EventDetails.eventDetails.messages);
            voiceChatOnBtn.SetActive(EventDetails.eventDetails.voiceChat);
            voiceChatOffBtn1.enabled = EventDetails.eventDetails.voiceChat;
            voiceChatOffBtn2.enabled = EventDetails.eventDetails.voiceChat;
            voiceChatSettingBtn.SetActive(EventDetails.eventDetails.voiceChat);
            if (!EventDetails.eventDetails.voiceChat)
            {
                ConstantsHolder.xanaConstants.mic = 1;
                ConstantsHolder.xanaConstants.StopMic();
            }
        }
    }

    #endregion
}
