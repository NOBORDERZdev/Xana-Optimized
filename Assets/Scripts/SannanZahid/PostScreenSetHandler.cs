using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostScreenSetHandler : MonoBehaviour
{
    public TMPro.TMP_InputField _postInputField;
    private void OnEnable()
    {
        _postInputField.text = "";
    }
}
