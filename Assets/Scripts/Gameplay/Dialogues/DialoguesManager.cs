using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguesManager : MonoBehaviour
{
    #region Singleton
    public static DialoguesManager Instance;

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    [Header("Dialogues")]
    public DialoguesAttributes dialogueAttributes;
    public List<Dialogues> dialogues;
    public GameObject messageBox;

    public GameObject gozHeadButton;

    public float dialogueTypeTime;

    private int currentDialogueIndex;
    private int currentDialogues;

    private string currentDialogueText;

    private bool isDialogueStarted;

    private Coroutine currentCoroutine;

    private MuseumRaycaster raycaster;

    private void Start()
    {
        if (dialogues == null)
            dialogues = new List<Dialogues>();



        raycaster = FindObjectOfType<MuseumRaycaster>();
    }

    public void StartDialogue(Dialogues _currentDialogue)
    {
        currentDialogueIndex = 0;                            // AH Working

        dialogues.Add(_currentDialogue);

        OnNextDialogue();
    }

    /// <summary>
    /// This function is executed when we need to show the next Dialogue.
    /// This can be an OnClick listener on the button or can be directly called
    /// through the script
    /// </summary>
    public void OnNextDialogue()
    {
        if (raycaster)
            raycaster.ChangePictureState(false);

        if (currentDialogueIndex == 0)
        {
            messageBox.SetActive(true);
            dialogueAttributes.nextDialogueButton.onClick.RemoveAllListeners();
            dialogueAttributes.nextDialogueButton.onClick.AddListener(() => OnNextDialogue());
        }

        if (isDialogueStarted)
        {
            if (currentDialogueIndex == dialogues[currentDialogues].dialogues.Length)
            {
                messageBox.SetActive(false);
                isDialogueStarted = false;
                return;
            }

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                if (GameManager.currentLanguage == "ja")
                    SetDialogue(dialogues[currentDialogues].japaneseDialogues[currentDialogueIndex]);
                else
                    SetDialogue(dialogues[currentDialogues].dialogues[currentDialogueIndex]);
                currentDialogueIndex += 1;

                if (currentDialogueIndex >= dialogues[currentDialogues].dialogues.Length)
                {
                    messageBox.SetActive(false);
                    if (gozHeadButton)
                        gozHeadButton.SetActive(true);
                    currentDialogueIndex = 0;
                    if (raycaster)
                        raycaster.ChangePictureState(true);



                }

                isDialogueStarted = false;
            }
        }
        else
        {
            currentCoroutine = StartCoroutine(StartDialogue());
        }
    }

    void SetDialogue(string message)
    {
        if (dialogueAttributes.dialogueText)
            dialogueAttributes.dialogueText.text = message;

        if (dialogueAttributes.dialogueTextPro)
            dialogueAttributes.dialogueTextPro.text = message;


    }

    IEnumerator StartDialogue()
    {
        isDialogueStarted = true;

        int currentPointer = 0;
        string mainDialogue = "";
        if (GameManager.currentLanguage == "ja")
            mainDialogue = dialogues[currentDialogues].japaneseDialogues[currentDialogueIndex];
        else
            mainDialogue = dialogues[currentDialogues].dialogues[currentDialogueIndex];

        currentDialogueText = "";

        while (currentDialogueText.Length < mainDialogue.Length)
        {
            currentDialogueText = mainDialogue.Substring(0, currentPointer);
            SetDialogue(currentDialogueText);
            currentPointer += 1;
            yield return new WaitForSeconds(dialogueTypeTime);
        }

        currentPointer = 0;
        currentDialogueIndex += 1;

        if (currentDialogueIndex >= dialogues[currentDialogues].dialogues.Length)
        {

            gozHeadButton.SetActive(true);
            yield return new WaitForSeconds(3.0f);

            messageBox.SetActive(false);
            isDialogueStarted = false;
            if (raycaster)
                raycaster.ChangePictureState(true);
            currentDialogueIndex = 0;


        }

        isDialogueStarted = false;
    }

    private void OnDisable()
    {
        Instance = null;
        if (messageBox)
            messageBox.SetActive(false);
        isDialogueStarted = false;
        StopCoroutine(StartDialogue());
    }
}

[System.Serializable]
public class DialoguesAttributes
{
    public Text dialogueText;
    public TextMeshProUGUI dialogueTextPro;

    [Space(10f)]
    public Button nextDialogueButton;
}
