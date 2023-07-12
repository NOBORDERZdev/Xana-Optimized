using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseDownScript : MonoBehaviour
{
    public Dialogues dialogues;
    public GameObject actionButton;

    private void Start()
    {
        DialoguesManager.Instance.gozHeadButton = actionButton;
    }
    private void OnMouseUp()
    {
        DialoguesManager.Instance.StartDialogue(dialogues);
        actionButton.SetActive(false);
    }
}
