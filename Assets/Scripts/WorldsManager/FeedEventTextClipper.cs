using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedEventTextClipper : MonoBehaviour
{

    public int PreferredLength;
    private Text _myText;
    private TextMeshProUGUI _myTextMesh;
  
    void Start()
    {
        if (GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese)
            PreferredLength = 12;
        else
            PreferredLength = 16;

        _myText = GetComponent<Text>();
      
        if (!_myText)
        {
            _myTextMesh = GetComponent<TextMeshProUGUI>();
          
        }
        
    }
    public void TextclipperHomepage()
    {
       
        if (GameManager.currentLanguage == "ja")
        {
            if (_myText)
            {
                if (_myText.text.Length > PreferredLength)
                {
                    _myText.text = _myText.text.Remove(PreferredLength - 1) + "...";
                }
            }

            if (_myTextMesh)
            {
                if (_myTextMesh.text.Length > PreferredLength)
                {
                    _myTextMesh.text = _myTextMesh.text.Remove(PreferredLength) + "...";
                }
            }
        }
        else {
            
                if (_myText)
                {
                    if (_myText.text.Length > PreferredLength)
                    {
                        _myText.text = _myText.text.Remove(PreferredLength - 1) + "...";
                    }
                    _myText.text = _myText.text.ToUpper();
                }

                if (_myTextMesh)
                {
                    if (_myTextMesh.text.Length > PreferredLength)
                    {
                        _myTextMesh.text = _myTextMesh.text.Remove(PreferredLength) + "...";
                    }
                    _myTextMesh.text = _myTextMesh.text.ToUpper();
                }
           }

    }
}
