using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Jint.Parser;

public class CreatePost : MonoBehaviour
{
    public GameObject CommentUI;
    [SerializeField]
    private TMP_InputField inputField;

    void Start()
    {

    }

    public void Send_PostOrComment()
    {
        if (!inputField.text.IsNullOrEmpty())
        {
            GameObject ui = Instantiate(CommentUI);
            ui.transform.SetParent(transform, false);
            ui.GetComponent<CommentUIManager>().SetComment("Xana PMY", inputField.text);
            inputField.text = "";
        }
        else
        {
            Debug.LogError("Please Enter something");
        }
    }

}
