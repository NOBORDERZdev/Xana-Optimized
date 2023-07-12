using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject actionButtonObject;
    public Dialogues characterDialogue;

    private Button actionButton;

    private void Start()
    {
        if (DialoguesManager.Instance.dialogues == null)
            DialoguesManager.Instance.dialogues = new List<Dialogues>();

        actionButtonObject.GetComponent<OnMouseDownScript>().dialogues = characterDialogue;
        actionButtonObject.GetComponent<OnMouseDownScript>().actionButton = actionButtonObject;
        
        //actionButton = actionButtonObject.GetComponent<Button>();
        //actionButton.onClick.AddListener(() => DialoguesManager.Instance.StartDialogue(characterDialogue));
    }

    public void ChangeInteractableButton(bool state)
    {
        actionButtonObject.SetActive(state);
    }

}
