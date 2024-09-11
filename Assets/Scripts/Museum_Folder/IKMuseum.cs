using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class IKMuseum : MonoBehaviour
{


    [Header("Animator")]
    public Animator m_Animator;

    [Header("TargetPosition")]
    public GameObject m_TargetPosition;
    public GameObject Lookatorder;

    [Header("Right Hand")]
    public GameObject RightHand;

    [Header("Selfie Stick")]
    public GameObject m_SelfieStick;

    [Header("Selfie Stick for Server player")]
    public GameObject m_SelfieStickOther;
    

    [Header("Selfie Camera")]
    public GameObject selfieCamera;

    [Header("FreeCamConsol")]
    public GameObject ConsoleObj;
    [Header("Consol for Server player")]
    public GameObject m_ConsoleObjOther;

    //[HideInInspector]
    public GameObject gourd;

    [Range(0, 1)]
    public float handIkPos = 1;
    [Range(0, 1)]
    public float handIkRot = 1;

    // ** ------ ** //
    public bool m_IsIkActive;

    private Quaternion IkRot;
    private bool updateIKValueCheck;
    public void Initialize()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex == 0)
        {
            //IsMainScene
        }
        else
        {
            PlayerSelfieController.Instance.InitializeCharacter(m_SelfieStick, this.transform.parent.gameObject, m_TargetPosition, this.gameObject,Lookatorder);
           
        }

    }

    //private void Update()
    //{
    //    if (m_TargetPosition.transform.rotation != IkRot && updateIKValueCheck)
    //    {
    //        IkRot = m_TargetPosition.transform.rotation;
    //        this.GetComponent<PhotonView>().RPC("SynchronizeSelfieIKData", RpcTarget.OthersBuffered, this.GetComponent<PhotonView>().ViewID, IkRot);
    //    }
    //}


    void OnAnimatorIK()
    {
        if (m_IsIkActive)
        {
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handIkPos);
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_TargetPosition.transform.position);
            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handIkRot);
            m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_TargetPosition.transform.rotation);

            m_Animator.SetLookAtWeight(0.7f);
            m_Animator.SetLookAtPosition(Lookatorder.transform.position);
            //if (updateIKValueCheck)
            //{
            //    m_Animator.SetLookAtPosition(m_TargetPosition.transform.position);
            //}
            //else
            //{
            //    m_Animator.SetLookAtPosition(m_TargetPosition.transform.position);
            //}
        }
    }



    public void EnableIK()
    {
        m_IsIkActive = true;
        //m_Animator.Play("Selfie");
        m_Animator.SetBool("isSelfie", true);
        RPCForSelfieEnable();
    }

    public void DisableIK()
    {
        m_IsIkActive = false;
        m_Animator.SetBool("isSelfie", false);
        m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        RPCForSelfieDisable();
    }

    public void RPCForSelfieEnable()
    {
        this.GetComponent<PhotonView>().RPC("EnableSelfieOnRemoteSide", RpcTarget.Others, this.GetComponent<PhotonView>().ViewID, m_TargetPosition.transform.rotation);
        updateIKValueCheck = true;
    }


    public void RPCForSelfieDisable()
    {
        this.GetComponent<PhotonView>().RPC("DisableSelfieOnRemoteSide", RpcTarget.Others, this.GetComponent<PhotonView>().ViewID, m_TargetPosition.transform.rotation);
        updateIKValueCheck = false;
    }


    public void RPCForFreeCamEnable()
    {
        this.GetComponent<PhotonView>().RPC("EnableFreeCamOnRemoteSide", RpcTarget.AllBuffered, this.GetComponent<PhotonView>().ViewID);
        
    }

    public void RPCForFreeCamDisable()
    {
        this.GetComponent<PhotonView>().RPC("DisableFreeCamOnRemoteSide", RpcTarget.AllBuffered, this.GetComponent<PhotonView>().ViewID);

    }
    public void RPCForKanzakiGourdEnable()
    {
        this.GetComponent<PhotonView>().RPC("EnableGourdOnRemoteSide", RpcTarget.AllBuffered, this.GetComponent<PhotonView>().ViewID);

    }
    public void RPCForKanzakiGourdDisable()
    {
        this.GetComponent<PhotonView>().RPC("DisableGourdOnRemoteSide", RpcTarget.AllBuffered, this.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void EnableSelfieOnRemoteSide(int _viewID, Quaternion transformData)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            m_SelfieStickOther.SetActive(true);
            //GetComponent<IKMuseum>().m_IsIkActive = true;
            //m_TargetPosition.transform.localPosition = transformData;
            //m_IsIkActive = true;
           // m_Animator.Play("Selfie");
            m_Animator.SetBool("isSelfie", true);
            m_TargetPosition.transform.rotation = transformData;
           
        }
    }

    [PunRPC]
    public void DisableSelfieOnRemoteSide(int _viewID, Quaternion transformData)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            m_SelfieStickOther.SetActive(false);
            //GetComponent<IKMuseum>().m_IsIkActive = false;
            //m_TargetPosition.transform.localPosition = transformData;
            //m_IsIkActive = false;
            m_Animator.SetBool("isSelfie", false);
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            m_TargetPosition.transform.rotation = transformData;
            
        }
    }

    [PunRPC]
    public void SynchronizeSelfieIKData(int _viewID, Quaternion rot)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            m_TargetPosition.transform.rotation = rot;
        }
    }

    [PunRPC]
    public void EnableFreeCamOnRemoteSide(int _viewID)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            if(!ReferencesForGamePlay.instance.playerControllerNew.isFirstPerson)
                m_ConsoleObjOther.SetActive(true);
            m_Animator.SetBool("freecam", true);
           
            Debug.Log("EnableFreeCamOnRemoteSideeeeee");
        }
        Debug.Log("EnableFreeCamOnRemoteSide");
    }

    [PunRPC]
    public void DisableFreeCamOnRemoteSide(int _viewID)
    {
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            m_ConsoleObjOther.SetActive(false);
            m_Animator.SetBool("freecam", false);
          

        }
    }

    #region XANA_Kanzaki Environment Events

    [HideInInspector]
    public UIController_Shine uIController_Shine;

    [PunRPC]
    public void EnableGourdOnRemoteSide(int _viewID)
    {
        //Debug.Log("EnableGourdOnRemoteSide RPC, ME ID : " + gameObject.GetComponent<PhotonView>().ViewID + " -- other ID : "+ _viewID);
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            if (gourd)
            {
                gourd.SetActive(true);

            }
        }
    }
    [PunRPC]
    public void DisableGourdOnRemoteSide(int _viewID)
    {
        //Debug.Log("DisableGourdOnRemoteSide RPC , ME ID : " + gameObject.GetComponent<PhotonView>().ViewID + " -- other ID : " + _viewID);
        if (gameObject.GetComponent<PhotonView>().ViewID == _viewID)
        {
            gourd.SetActive(false);
            //Destroy(SpawnedGourd);
        }
    }




    //These are the events that are called from amination events
    private void WashFin()
    {
        ReferencesForGamePlay.instance.MainPlayerParent.GetComponent<PlayerController>().m_IsMovementActive = true;
        PlayerController.PlayerIsIdle?.Invoke();
        Enable_DisableObjects.Instance.DisableDashButton(true);
        RPCForKanzakiGourdDisable();

    }

    private void BowTwiceFin()
    {
        if(uIController_Shine)
            uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
    }

    private void ClapFin()
    {
        if(uIController_Shine)
            uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
    }

    private void PrayFin()
    {
        if(uIController_Shine)
            uIController_Shine.GetWorshipGameUI().gameObject.SetActive(true);
    }

    private void BowOnceFin()
    {
        if(uIController_Shine)
            uIController_Shine.GetOmikuziUI().gameObject.SetActive(true);
    }

    #endregion
}
