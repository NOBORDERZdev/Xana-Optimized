using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XLTutorials : MonoBehaviour
{
    [SerializeField]
    GameObject tutorialPanel;
    [SerializeField]
    Button dontShowBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("ShowLobbyTutorial") == 0 && !ConstantsHolder.xanaConstants.isLobbyTutorialLoaded)
        {
            tutorialPanel.SetActive(true);
            ConstantsHolder.xanaConstants.isLobbyTutorialLoaded = true;
        }
        dontShowBtn.onClick.AddListener(DontShowTutorial);
    }

    void DontShowTutorial()
    {
        PlayerPrefs.SetInt("ShowLobbyTutorial",1);
        tutorialPanel.SetActive(false);
        this.transform.parent.gameObject.SetActive(false);
    }
}
