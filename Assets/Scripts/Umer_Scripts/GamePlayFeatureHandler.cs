using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayFeatureHandler : MonoBehaviour
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
    public GameObject miniMapSettingBtn;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        if (XanaEventDetails.eventDetails.DataIsInitialized)
        {
            selfieBtn.SetActive(XanaEventDetails.eventDetails.selfie);
            EmojiesBtn.SetActive(XanaEventDetails.eventDetails.emotes);
            ReactionBtn.SetActive(XanaEventDetails.eventDetails.emotes);
            favouriteBtn.SetActive(XanaEventDetails.eventDetails.emotes);
            textChatBtn.SetActive(XanaEventDetails.eventDetails.messages);
            textChatBtn2.SetActive(XanaEventDetails.eventDetails.messages);
            voiceChatOnBtn.SetActive(XanaEventDetails.eventDetails.voiceChat);
            voiceChatOffBtn1.enabled = XanaEventDetails.eventDetails.voiceChat;
            voiceChatOffBtn2.enabled = XanaEventDetails.eventDetails.voiceChat;
            voiceChatSettingBtn.SetActive(XanaEventDetails.eventDetails.voiceChat);
            if (!XanaEventDetails.eventDetails.voiceChat)
            {
                ConstantsHolder.xanaConstants.mic = 1;
                ConstantsHolder.xanaConstants.StopMic();
            }
        }
        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("DUNE"))
        {
            voiceChatSettingBtn.SetActive(false);
        }

        //disable minimap button
        if (ConstantsHolder.xanaConstants.IsMetabuzzEnvironment)
        {
            miniMapSettingBtn.SetActive(false);
        }
    }

    #endregion
}
