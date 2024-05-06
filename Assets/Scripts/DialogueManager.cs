using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Dialogue
{
    public string m_Name;

    [TextArea(3, 10)]
    public string[] m_Sentences;
    public float[] m_SentencesSpeed;
}

[Serializable]
public class NonPlayerCharacter
{
    public int m_ID;
    public string m_Name;
    public AudioClip m_AudioClip;
    public GameObject m_HeadButton;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

  //  [Header("Audio Clips")]
  //  public AudioClip[] m_AudioClips;

    [Header("Dialogues")]
    public Dialogue[] m_Dialogues;

  //  [Header("Reception Dialogue")]
  //  public Dialogue m_ReceptionDialogue;
    public GameObject m_MessageBox;
    public TextMeshProUGUI m_MessageText;

   // [Header("Non Player Characters")]
  //  public NonPlayerCharacter[] m_NonPlayerCharacters;
  //  [Header("NPC Animators")]
 //   public AnimationController receptionistAnimator;

    //[Header("Mayor And Judge Dialogue Audio Clips")]
    //public AudioClip[] m_MayorJudgeAudioClips;

    //[Header("Staff Members AudioClips")]
    //public AudioClip[] m_StaffMembersAudioClips;


    // ** Sentences And Their Speeds
    Queue<string> m_Sentences;
    List<int> m_SentencesSpeeds;

    // ** Current Runing Sentence Index
    int m_SentenceIndex;

    TextMeshPro m_InfoTextTemp;
    TextMeshProUGUI m_InfoTextTMProGUI;
    float m_Speed;

    [Space(20)]



  //  public Text nameText;
  //  public Text dialogueText;
   // public GameObject canvas, AndroidNonXRcanvas,StandaloneCanvus,AndroidButtonCanvus;
   // public GameObject mainCamera;
   // public TextMeshProUGUI textMesh;
   // public TMP_Text nameTextTmp, dialogueText_TMP;
   // public Text UI_bubbleText;
   // 
  //  public bool MusicMute;
  //  public GameObject headbutton;
 //   public bool test;
    public GameObject mainCam;

    
    



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        //LoadSentences();  //call this function if the sentences data in the inspector is coroupted

        m_Speed = 1.5f;
        m_Sentences = new Queue<string>();
        m_SentencesSpeeds = new List<int>();
    }



    public void PlayReceptionGirlDialogue(int l_NPCID) // NPC stands for non player character
    {
        //MuseumManager.Instance.m_LobbyDialogueCamera.SetActive(true);
        //SoundController.Instance.PlayNPCAudio(m_NonPlayerCharacters[l_NPCID].m_AudioClip);
        //m_NonPlayerCharacters[l_NPCID].m_HeadButton.SetActive(true);

        //MuseumManager.Instance.m_MainPlayerCamera.SetActive(false);
        //MuseumManager.Instance.m_LobbyVirtualCamera.SetActive(true);

        ////StartDialogue();
        //StartReceptionistDialogue();
    }
    void StartReceptionistDialogue()
    {
        //m_MessageBox.SetActive(true);
        //m_Speed = 15;
        //m_InfoTextTMProGUI = m_MessageText.GetComponent<TextMeshProUGUI>();
        //m_Sentences.Clear();
        //m_SentencesSpeeds.Clear();

        //foreach (string sentence in m_ReceptionDialogue.m_Sentences)
        //{
        //    m_Sentences.Enqueue(sentence);
        //}

        //foreach (int n in m_ReceptionDialogue.m_SentencesSpeed)
        //{
        //    m_SentencesSpeeds.Add(n);
        //}

        //currentNpcSpeedIndex = 0;
        //DisplayNextReceptionistSentence();
    }

    public void StopReceptionGirlDialogue()
    {
        //MuseumManager.Instance.m_LobbyDialogueCamera.SetActive(false);
        //m_MessageBox.SetActive(false);
        //m_NonPlayerCharacters[7].m_HeadButton.SetActive(true);
        //MuseumManager.Instance.m_MainPlayerCamera.SetActive(true);
        //MuseumManager.Instance.m_LobbyVirtualCamera.SetActive(false);
        //EndDialogue();
    }

    public void CloseRecptionDialogueUIPanel()
    {
        MuseumManager.Instance.m_MainPlayerCamera.SetActive(true);
        MuseumManager.Instance.m_LobbyDialogueCamera.SetActive(false);
        SoundController.Instance.StopNPCAudio();
        EndDialogue();
        m_MessageBox.SetActive(false);
    }


    void DisplayNextReceptionistSentence()
    {
        //if (m_Sentences.Count == 0)
        //{
        //    EndImageDialogue();
        //    return;
        //}

        //if (currentNpcSpeedIndex == 0)
        //{
        //    receptionistAnimator.StatBow(true);
        //}

        //if (currentNpcSpeedIndex == 2)
        //{
        //    receptionistAnimator.HeadSet(true);
        //    receptionistAnimator.ActiveHeadSet();
        //}

        //string l_Sentence = m_Sentences.Dequeue();
        //StopAllCoroutines();
        //StartCoroutine(TypeSentence(l_Sentence));
        //Invoke("DisplayNextSentence", m_SentencesSpeeds[currentNpcSpeedIndex]);
        //currentNpcSpeedIndex += 1;
    }


    public void PlayNonPlayerCharacterDialogue(int l_NPCID, NPC_DialogueTrigger _dialogueTrigger)
    {
        ////SoundController.Instance.Play(m_NonPlayerCharacters[l_NPCID].m_AudioClip);
        //SoundController.Instance.PlayNPCAudio(m_NonPlayerCharacters[l_NPCID].m_AudioClip);
        //m_NonPlayerCharacters[l_NPCID].m_HeadButton.SetActive(true);

        //StartDialogue(l_NPCID, m_InfoTextTemp, _dialogueTrigger);
    }

    public void StopNonPlayerCharacterDialogue(int l_NPCID)
    {
      //  m_NonPlayerCharacters[l_NPCID].m_HeadButton.SetActive(false);
    }

    public void OpenNPCDialogueButton(int npcIndex, NPC_DialogueTrigger _dialogueTrigger)
    {
        //m_NonPlayerCharacters[npcIndex].m_HeadButton.SetActive(true);
        //m_NonPlayerCharacters[npcIndex].m_HeadButton.GetComponent<OnClickCube>().anEvent.RemoveAllListeners();
        //m_NonPlayerCharacters[npcIndex].m_HeadButton.GetComponent<OnClickCube>().anEvent.AddListener(() => _dialogueTrigger.OnHeadButtonClicked());
    }



    public void PlayMayorAndJudgeDialogue(int l_AudioClipIndex)
    {
        //SoundController.Instance.Play(m_MayorJudgeAudioClips[l_AudioClipIndex]);
    }

    public void PlayStaffDialogue(int l_AudioClipIndex)
    {
        //SoundController.Instance.Play(m_StaffMembersAudioClips[l_AudioClipIndex]);
    }

    public void PlayGalleryPictureAudio(int l_PictureIndex)
    {
      //  SoundController.Instance.Play(m_AudioClips[l_PictureIndex]);
    }

    public void StopGalleryPictureAudio()
    {
        SoundController.Instance.EffectsSource.Stop();
    }



    public void RestorInfoTextField(TextMeshPro l_InfoText,int l_PictureIndex)
    {
        l_InfoText.text = "";
        l_InfoText.text = m_Dialogues[l_PictureIndex].m_Name;
    }


    public void StartDialogue() // using this for reception text dialogue
    {
        //m_MessageBox.SetActive(true);
        //m_Speed = 15;
        //m_InfoTextTMProGUI = m_MessageText.GetComponent<TextMeshProUGUI>();
        //m_Sentences.Clear();
        //m_SentencesSpeeds.Clear();

        //foreach (string sentence in m_ReceptionDialogue.m_Sentences)
        //{
        //    m_Sentences.Enqueue(sentence);
        //}

        //foreach (int n in m_ReceptionDialogue.m_SentencesSpeed)
        //{
        //    m_SentencesSpeeds.Add(n);
        //}

        //currentNpcSpeedIndex = 0;
        //DisplayNextSentence();
    }

    public void StartDialogue(int l_PictureIndex, TextMeshPro l_InfoText,float l_Speed)
    {
        m_Speed = l_Speed;
        m_InfoTextTemp = l_InfoText;
        m_Sentences.Clear();

        foreach (string sentence in m_Dialogues[l_PictureIndex].m_Sentences)
        {
            m_Sentences.Enqueue(sentence);
        }

        foreach (int n in m_Dialogues[l_PictureIndex].m_SentencesSpeed)
        {
            m_SentencesSpeeds.Add(n);
        }
        
        DisplayNextSentence();
    }

    private int currentNpcSpeedIndex;

    public void StartDialogue(int npcDialogueIndex, TextMeshPro _messageText, NPC_DialogueTrigger _dialogueTrigger)
    {
        //m_Speed = l_Speed;
        m_MessageBox.SetActive(true);
        currentNpcSpeedIndex = 0;
        m_InfoTextTemp = _messageText;
        m_Sentences.Clear();
        m_SentencesSpeeds.Clear();

        foreach (string sentence in _dialogueTrigger.npcDialogue.m_Sentences)
        {
            m_Sentences.Enqueue(sentence);
        }

        foreach (int n in _dialogueTrigger.npcDialogue.m_SentencesSpeed)
        {
            m_SentencesSpeeds.Add(n);
        }

        StopAllCoroutines();
        StartCoroutine(DisplayNextSentences());
    }

    private Coroutine currentTypeCoroutine;

    IEnumerator DisplayNextSentences()
    {
        while (m_Sentences.Count != 0)
        {
            string l_Sentence = m_Sentences.Dequeue();
            if (currentTypeCoroutine != null)
                StopCoroutine(currentTypeCoroutine);

            currentTypeCoroutine = StartCoroutine(TypeSentence(l_Sentence));

            yield return new WaitForSeconds(m_SentencesSpeeds[currentNpcSpeedIndex]);
            currentNpcSpeedIndex += 1;
        }

        EndDialogue();
    }


    void DisplayNextSentence()
    {
        if (m_Sentences.Count == 0)
        {
            EndImageDialogue();
            return;
        }

        string l_Sentence = m_Sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(l_Sentence));

        if (m_SentencesSpeeds.Count != 0)
        {
            Invoke("DisplayNextSentence", m_SentencesSpeeds[currentNpcSpeedIndex]);
            currentNpcSpeedIndex += 1;
        }
        else
        { 
            Invoke("DisplayNextSentence", m_Speed);
        }
    }

    
    IEnumerator TypeSentence(string l_Sentence)
    {
        if (m_InfoTextTemp != null)
            m_InfoTextTemp.text = "";
        if (m_InfoTextTMProGUI != null)
            m_InfoTextTMProGUI.text = "";

        foreach (char letter in l_Sentence.ToCharArray())
        {
            if (m_InfoTextTemp != null)
                m_InfoTextTemp.text += letter;
            if (m_InfoTextTMProGUI != null)
                m_InfoTextTMProGUI.text += letter;

            yield return null;
        }
    }


    void EndDialogue()
    {

        SoundController.Instance.EffectsSource.Stop();


        CancelInvoke();
    }


    void EndImageDialogue()
    {
        CancelInvoke();
    }


    void LoadSentences()
    {
        string[] l_Data = System.IO.File.ReadAllLines(Application.dataPath + "/1_PROJECTMAIN/Sentences.txt");

        for (int i = 0; i < l_Data.Length; i++)
        {
            string[] l_SplitData = l_Data[i].Split(',');
            m_Dialogues[i].m_Sentences = new string[(l_SplitData.Length - 1)];

            m_Dialogues[i].m_Name = l_SplitData[0];
            int l_Counter = 0;
            for (int j = 1; j < l_SplitData.Length; j++)
            {
                m_Dialogues[i].m_Sentences[l_Counter] = l_SplitData[j];
                l_Counter++;
            }
        }
    }








    /*

    public void CloseCanvas()
    {
        canvas.gameObject.SetActive(false);
        AndroidNonXRcanvas.gameObject.SetActive(false);
        StandaloneCanvus.gameObject.SetActive(false);
        AndroidButtonCanvus.gameObject.SetActive(true);
        headbutton.SetActive(true);

        Gamemanager._InstanceGM.mainPlayer.SetActive(true);

        CanvusHandler.canvusHandlerInstance.is_Trigger = false;
        CanvusHandler.canvusHandlerInstance.Display();


        SoundController.Instance.EffectsSource.Stop();

        CanvusHandler.canvusHandlerInstance.info_Panel.SetActive(false);
        Gamemanager._InstanceGM._isNPCMoving = true;
        mainCam.SetActive(true);
        SoundController.Instance.VolumeMethodForVideo();


        CanvusHandler.canvusHandlerInstance.offallVcam();
        m_SentencesSpeeds.Clear();
        m_SentenceIndex = 0;
        CancelInvoke();

    }

    */

    #region Waste Code

    /*
        if (Gamemanager._InstanceGM._IsXRPlatform)
        {
            dialogueText_meshpro.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText_meshpro.text += letter;
                yield return null;
            }
        }
        else if (Gamemanager._InstanceGM._AndroidPlatfrom)
        {
            if (test)
            {
                UI_bubbleText.text = "";
                foreach (char letter in sentence.ToCharArray())
                {
                    UI_bubbleText.text += letter;
                    yield return null;
                }
            }
            else
            {
                dialogueText_meshpro.text = "";
                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText_meshpro.text += letter;
                    yield return null;
                }
            }
        }
        else
        {
            dialogueText_meshpro.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText_meshpro.text += letter;
                yield return null;
            }
        }
        */

    #endregion
}
