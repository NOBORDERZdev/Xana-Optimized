using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFE3D;

public class OnlineExit : MonoBehaviour
{
    public GameObject onlinePausePanel_Prefab;

    GameObject pausePanelInstance;
    Button exit;
    Image exitImage;

    private void Awake()
    {
        exit = GetComponent<Button>();
        exitImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        exit.onClick.AddListener(OnExitClick);
    }

    private void Start()
    {
        exitImage.enabled = false;
    }

    private void OnDisable()
    {
        exit.onClick.RemoveAllListeners();
    }

    public void CheckMode()
    {
        if (UFE.gameMode == GameMode.NetworkGame)
        {
            exitImage.enabled = true;
        }
        else
        {
            exitImage.enabled = false;
        }
    }

    void OnExitClick()
    {
        if (pausePanelInstance == null)
        {
            if (UFE.canvas != null)
            {
                exitImage.enabled = false;
                pausePanelInstance = Instantiate(onlinePausePanel_Prefab);
                pausePanelInstance.transform.SetParent(UFE.canvas.transform, false);
                UFE.touchControllerBridge.ShowBattleControls(false,true);
            }
        }
    }
}
