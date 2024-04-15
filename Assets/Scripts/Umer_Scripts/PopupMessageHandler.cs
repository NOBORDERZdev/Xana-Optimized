using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PopupMessageHandler : MonoBehaviour
{
    #region Varibales
    //Instance
    public static PopupMessageHandler Instance;

    //Gameobject References
    public GameObject dialoguePanel;

    //UI References
    public Text headingText, descriptionText, timerText;

    #endregion

    #region BuiltIn Methods

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    #endregion

    #region Custom Methods

    public void SetText(Text _textfield, string _text)
    {
        _textfield.text = _text;
    }

    public void SetPanelState(bool _state)
    {
        if (_state)
        {
            dialoguePanel.SetActive(true);
        }
    }

    #endregion
}
