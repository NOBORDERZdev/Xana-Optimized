using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedEventTextWorldName : MonoBehaviour
{

    public int PreferredLength;
   

    private Text _myText;
    private TextMeshProUGUI _myTextMesh;
    // Start is called before the first frame update
    private void OnEnable()
    {
        InvokeRepeating("WorldNameClipper",0.1f,0.01f);
    }
    private void OnDisable()
    {
        CancelInvoke("WorldNameClipper");
      
    }


    void Start()
    {
        if (GameManager.currentLanguage == "ja" || LocalizationManager.forceJapanese)
        {
            if (ConstantsHolder.xanaConstants.screenType == ConstantsHolder.ScreenType.TabScreen)
                PreferredLength = 40;
            else
                PreferredLength = 12;
        }
        else if(ConstantsHolder.xanaConstants.screenType == ConstantsHolder.ScreenType.TabScreen)
            PreferredLength = 40;
        _myText = GetComponent<Text>();
        //Length = _myText.text.Length;
        if (!_myText)
        {
            _myTextMesh = GetComponent<TextMeshProUGUI>();
            //Length = _myText.text.Length;
        }
    }
    // Update is called once per frame
    void WorldNameClipper()
    {
        if (GameManager.currentLanguage == "ja")
        {

            if (_myText)
            {
                if (_myText.text.Length > PreferredLength)
                {

                    _myText.text = _myText.text.Remove(PreferredLength) + "...";
                }
              
            }

            if (_myTextMesh)
            {
                if (_myTextMesh.text.Length > PreferredLength)
                {

                    _myTextMesh.text = _myText.text.Remove(PreferredLength) + "...";
                }
               
            }
        }
        else
        {
            if (_myText)
            {
                if (_myText.text.Length > PreferredLength)
                {

                    _myText.text = _myText.text.Remove(PreferredLength) + "...";
                }
               
            }

            if (_myTextMesh)
            {
                if (_myTextMesh.text.Length > PreferredLength)
                {

                    _myTextMesh.text = _myText.text.Remove(PreferredLength) + "...";
                }
                
            }
        }
     

    }
}
