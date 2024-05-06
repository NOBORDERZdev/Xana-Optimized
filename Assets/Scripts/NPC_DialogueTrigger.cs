using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class NPC_DialogueTrigger : MonoBehaviour
{
    [Header("Picture Index")]
    public int m_PictureIndex;

    [Header("Info Text")]
    public TextMeshPro m_InfoText;

    public float m_Speed = 1.5f;

    [Header("Non Player Character")]
    public bool m_IsNonPlayerCharacter;
    public int m_IndexNonPlayerCharacter;

    [Header("NPC Dialogue Text")]
    public Dialogue npcDialogue;

    [Space(20)]

    public bool _isShowCanusBuddle, judgeAvatarbool;

    public GameObject headButton;

    Vector3 projectCameraForward;

    public TextMeshPro info_textMesh;
    public TextMeshProUGUI textMesh;
    

    public GameObject focusCam;

    public bool ReceptionAnimbool, isVideoRoom;



    private void Start()
    {

        if(!m_IsNonPlayerCharacter)
        m_InfoText.text = DialogueManager.Instance.m_Dialogues[(m_PictureIndex - 1)].m_Name;
    }





    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag == "Player")
        {
            if (gameObject.tag == "Mayor" || gameObject.tag == "StaffMember")
            {
                DialogueManager.Instance.OpenNPCDialogueButton(m_IndexNonPlayerCharacter, this);
            }
            else if (gameObject.tag == "Reseption")
            {
                DialogueManager.Instance.PlayReceptionGirlDialogue(m_IndexNonPlayerCharacter);
            }
            else
            {
                DialogueManager.Instance.PlayGalleryPictureAudio(m_PictureIndex - 1); // when we collide picture in indoor-room then play its audio
                TriggerDialogue();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {

        SoundController.Instance.EffectsSource.Stop();
        SoundController.Instance.StopNPCAudio();


        if (other.CompareTag("Player") && this.gameObject.tag == "Reseption")
        {
            DialogueManager.Instance.m_MessageBox.SetActive(false);
            DialogueManager.Instance.StopReceptionGirlDialogue();
        }
        else if (other.CompareTag("Player") && this.gameObject.tag == "Mayor")
        {
            DialogueManager.Instance.m_MessageBox.SetActive(false);
            DialogueManager.Instance.StopNonPlayerCharacterDialogue(m_IndexNonPlayerCharacter);
        }
        else if (other.CompareTag("Player") && this.gameObject.tag == "StaffMember")
        {
            DialogueManager.Instance.m_MessageBox.SetActive(false);
            DialogueManager.Instance.StopNonPlayerCharacterDialogue(m_IndexNonPlayerCharacter);
        }
        else if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.RestorInfoTextField(m_InfoText, (m_PictureIndex - 1));
        }

    }



    private void Update()
    {
        //if (!CanvusHandler.canvusHandlerInstance.is_Trigger && !headButton && headButton != null)
        //{
        //    Debug.Log("Checking here What is going on");
        //    headButton.SetActive(true);
        //}
        //if (headButton != null)
        //{
        //    headButton.transform.LookAt(DialogueManager.Instance.mainCamera.transform);
        //}

        //if (isVideoRoom && !DialogueManager.Instance.MusicMute)
        //{
        //    DialogueManager.Instance.MusicMute = false;
        //}

    }



    public void TriggerDialogue()
    {
        if (_isShowCanusBuddle)
        {
            DialogueManager.Instance.StartDialogue((m_PictureIndex - 1), m_InfoText, m_Speed);
        }
        else
        {
            DialogueManager.Instance.StartDialogue((m_PictureIndex - 1), m_InfoText, m_Speed);
        }
    }

    public void OnHeadButtonClicked()
    {


        if (focusCam != null)
        {

            focusCam.SetActive(true);
        }

        CanvusHandler.canvusHandlerInstance._VcamClear.gameObject.SetActive(true);
        headButton.SetActive(false);

        Gamemanager._InstanceGM._isNPCMoving = false;

        DialogueManager.Instance.PlayNonPlayerCharacterDialogue(m_IndexNonPlayerCharacter, this);
    }




    public void ButtonPushed()
    {
        if (ReceptionAnimbool)
        {
            Gamemanager._InstanceGM.animationController.StatBow(true);
        }

        if (focusCam != null)
        {
            
            focusCam.SetActive(true);
        }

        CanvusHandler.canvusHandlerInstance._VcamClear.gameObject.SetActive(true);
        headButton.SetActive(false);
        TriggerDialogue();

        Gamemanager._InstanceGM._isNPCMoving = false;

        
    }

    

    void LobbySetting(GameObject player)
    {
        player.transform.position = Gamemanager._InstanceGM.teleportPoints[1].transform.position;
        Invoke("Test", 0.5f);
    }

    void Test()
    {
        Gamemanager._InstanceGM.mainPlayer.GetComponent<PlayerController>().enabled = true;
    }
}

