using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class CharacterType : MonoBehaviourPunCallbacks
{
    public static CharacterType instance;
    private ContestantsAnimationHandler ref_ContestantsAnimationHandler;

    public enum PlayerType : byte
    {
        None, Judge, Contestents, CameraMan
    };
    public PlayerType playerType;

    public bool isStartShow = false;
    public Transform pos;
    public PlayerControllerNew pc;
    public string email;

    private bool alreadyExist;

    void Start()
    {
        pc = GetComponentInParent<PlayerControllerNew>();
        ref_ContestantsAnimationHandler = GetComponent<ContestantsAnimationHandler>();

        ReferrencesForDynamicMuseum.instance.judgeBtn1.onClick.AddListener(delegate { JudgesButtonClicked(0); });
        ReferrencesForDynamicMuseum.instance.judgeBtn2.onClick.AddListener(delegate { JudgesButtonClicked(1); });
        ReferrencesForDynamicMuseum.instance.judgeBtn3.onClick.AddListener(delegate { JudgesButtonClicked(2); });
        ReferrencesForDynamicMuseum.instance.judgeBtn4.onClick.AddListener(delegate { JudgesButtonClicked(3); });
        ReferrencesForDynamicMuseum.instance.judgeBtn5.onClick.AddListener(delegate { JudgesButtonClicked(4); });
        ReferrencesForDynamicMuseum.instance.judgeBtn6.onClick.AddListener(delegate { JudgesButtonClicked(5); });
        ReferrencesForDynamicMuseum.instance.startShowBtn.onClick.AddListener(StartShowButton);
    }

    public void SetPlayerType(string emailData)
    {
        GetComponent<PhotonView>().RPC(nameof(DifferentiatePlayersBasedOnEmail), RpcTarget.AllBuffered, emailData);
        UpdateCharacterStats();
    }

    [PunRPC]
    void DifferentiatePlayersBasedOnEmail(string data)
    {
        email = data;
        for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
        {
            if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().email == email)
            {
                if (alreadyExist && GetComponent<PhotonView>().IsMine)
                {
                    LoadFromFile.instance._uiReferences.LoadMain(false);
                    return;
                }
                alreadyExist = true;
            }
        }
        for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
        {
            for (int j = 0; j < XanaConstants.xanaConstants.JudgesEmailAddresses.Length; j++)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().email == XanaConstants.xanaConstants.JudgesEmailAddresses[j])
                {
                    if (GetComponent<PhotonView>().IsMine)
                        pos = GameObject.Find("Pos" + (j + 1)).transform;
                    Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType = PlayerType.Judge;
                    EnableStartShowBtnIfContestantExist();
                }
            }
            if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType != PlayerType.Judge)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().email == XanaConstants.xanaConstants.cameraManEmailAddress)
                {
                    Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType = PlayerType.CameraMan;
                }
                else
                {
                    Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType = PlayerType.Contestents;
                    EnableStartShowBtnForJudges();
                    if (GetComponent<PhotonView>().IsMine)
                    {
                        foreach (GameObject obj in CanvasButtonsHandler.inst.objectToDisableForContestant)
                        {
                            obj.SetActive(false);
                        }
                    }
                }
            }
        }
    }

        private void EnableStartShowBtnIfContestantExist()
        {
            bool isContestantExist = false;
            for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType == CharacterType.PlayerType.Contestents)
                    isContestantExist = true;
            }
            if (isContestantExist)
            {
                if (ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharacterType>().playerType == PlayerType.Judge)
                    ReferrencesForDynamicMuseum.instance.startShowBtn.gameObject.SetActive(true);
            }
        }
        private void EnableStartShowBtnForJudges()
        {
            for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType == CharacterType.PlayerType.Judge)
                {
                    if (ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharacterType>().playerType == PlayerType.Judge)
                        ReferrencesForDynamicMuseum.instance.startShowBtn.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateCharacterStats()
        {
            if (playerType.Equals(PlayerType.Judge))
            {
                XanaVoiceChat.instance.TurnOnMic();
            }
            else if (playerType.Equals(PlayerType.Contestents))
            {
                pos = GameObject.Find("ContestantPos").transform;
                //pc.enabled = false;
                ReferrencesForDynamicMuseum.instance.JoyStick.SetActive(false);
                XanaVoiceChat.instance.TurnOnMic();
            }
            else if (playerType.Equals(PlayerType.CameraMan))
            {

                pos = GameObject.Find("CameramanPos").transform;
                pc.FreeFloatToggleButton(true);
                ReferrencesForDynamicMuseum.instance.hiddenButtonDisable();
                ReferrencesForDynamicMuseum.instance.JoyStick.SetActive(true);
                XanaVoiceChat.instance.TurnOffMic();
            }
            SetTransforms();
        }
        private void SetTransforms()
        {
            if (pos)
            {
                ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.position = pos.position;
                ReferrencesForDynamicMuseum.instance.MainPlayerParent.transform.rotation = pos.rotation;
            }
        }


        private void StartShowButton()
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                if (!isStartShow && playerType.Equals(PlayerType.Judge))
                {
                    GetComponent<PhotonView>().RPC(nameof(StartShow), RpcTarget.All);
                }
            }
        }

        [PunRPC]
        void StartShow()
        {
            for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
            {
                isStartShow = true;
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType == CharacterType.PlayerType.Judge)
                {
                    Launcher.instance.playerobjects[i].GetComponent<CharacterType>().SetTransforms();
                    if (GetComponent<PhotonView>().IsMine)
                    {
                        //pc.enabled = false;
                        GetComponent<PhotonTimer>().InitializeTimer();
                    }
                    if (ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharacterType>().playerType == PlayerType.Judge)
                    {
                        XanaVoiceChat.instance.TurnOffMic();
                        JudgesUIUpdate(false);
                    }
                }
                else if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType == CharacterType.PlayerType.Contestents)
                {
                    if (ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharacterType>().playerType == PlayerType.Contestents)
                        XanaVoiceChat.instance.TurnOnMic();
                    Launcher.instance.playerobjects[i].GetComponent<CharacterType>().ref_ContestantsAnimationHandler.PlaySingingAnimation(true);
                }
            }
        }
        void JudgesUIUpdate(bool isEnable)
        {
            ReferrencesForDynamicMuseum.instance.startShowBtn.gameObject.SetActive(isEnable);
            ReferrencesForDynamicMuseum.instance.JoyStick.SetActive(isEnable);
            ReferrencesForDynamicMuseum.instance.judgeBtnScreen.SetActive(!isEnable);
        }


        public void CallEventEnd()
        {
            GetComponent<PhotonView>().RPC(nameof(EndShow), RpcTarget.All);
        }

        [PunRPC]
        void EndShow()
        {
            for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType == CharacterType.PlayerType.Contestents)
                {
                    Launcher.instance.playerobjects[i].GetComponent<CharacterType>().ref_ContestantsAnimationHandler.PlaySingingAnimation(false);
                }
            }

            if (ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharacterType>().playerType == PlayerType.Judge)
            {
                if (GetComponent<PhotonView>().IsMine)
                    pc.enabled = true;
                JudgesUIUpdate(true);
                XanaVoiceChat.instance.TurnOnMic();
            }
            ReferrencesForDynamicMuseum.instance.m_34player.GetComponent<CharacterType>().isStartShow = false;
        }


        public void JudgesButtonClicked(int index)
        {
            if (GetComponent<PhotonView>().IsMine)
                GetComponent<PhotonView>().RPC(nameof(JudgesAnimationSync), RpcTarget.All, GetComponent<PhotonView>().ViewID, index);
        }

        [PunRPC]
        void JudgesAnimationSync(int viewId, int index)
        {
            for (int i = 0; i < Launcher.instance.playerobjects.Count; i++)
            {
                if (Launcher.instance.playerobjects[i].GetComponent<CharacterType>().playerType == CharacterType.PlayerType.Judge
                    && Launcher.instance.playerobjects[i].GetComponent<PhotonView>().ViewID == viewId)
                {
                    Launcher.instance.playerobjects[i].GetComponent<Animator>().SetTrigger("JudgesAnimPlay");
                    Launcher.instance.playerobjects[i].GetComponent<Animator>().SetInteger("JudgesAnimCounter", index);
                }
            }
        }

        public void ResetAnimCondition()
        {
            GetComponent<Animator>().SetInteger("JudgesAnimCounter", -1);
        }
    }
