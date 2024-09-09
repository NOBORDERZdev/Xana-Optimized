using UnityEngine;
using TMPro;
using System;

public class PermissionPopusSystem : MonoBehaviour
{
    public static PermissionPopusSystem Instance { get; private set; }
    public PermissionTexts permissionTexts;
    public enum TextType { none, Camera, Mic, Gallery };
    [Space(5)]
    public TextType textType;

    [Serializable]
    public class PopupsScreen
    {
        public GameObject canvasObj;
        public LandScapeData landScapeData;
        public PotraitData potraitData;
    }
    [Serializable]
    public class LandScapeData
    {
        public GameObject permissionPopupLand;
        public TextMeshProUGUI descriptionTxt_land;
    }
    [Serializable]
    public class PotraitData
    {
        public GameObject permissionPopupPort;
        public TextMeshProUGUI descriptionTxt_port;
    }

    public PopupsScreen popupsScreen;

    public Action onCloseAction; // The callback to be invoked on popup close
    public Action<int> onCloseActionWithParam; // The callback to be invoked on popup close

    private int actionParameter;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this; // Set the current instance
            DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void OpenPermissionScreen(int ActionParam = 0)
    {
        actionParameter = ActionParam;
        ShowPermissionScreen();
    }


    public void OnClickContinue()
    {
        popupsScreen.canvasObj.SetActive(false);
        popupsScreen.potraitData.permissionPopupPort.SetActive(false);
        popupsScreen.landScapeData.permissionPopupLand.SetActive(false);
        if (actionParameter != null && actionParameter != 0)
        {
            onCloseActionWithParam?.Invoke(actionParameter);
        }
        else
        {
            onCloseAction?.Invoke();
        }
        textType = TextType.none;
    }

    public void OnClickClose()
    {
        popupsScreen.canvasObj.SetActive(false);
        popupsScreen.potraitData.permissionPopupPort.SetActive(false);
        popupsScreen.landScapeData.permissionPopupLand.SetActive(false);
        textType = TextType.none;
        onCloseAction = null;
        onCloseActionWithParam = null;
    }

    private void ShowPermissionScreen()
    {
        popupsScreen.landScapeData.descriptionTxt_land.text = "";
        popupsScreen.potraitData.descriptionTxt_port.text = "";
        popupsScreen.canvasObj.SetActive(true);

        if (ScreenOrientationManager._instance != null && !ScreenOrientationManager._instance.isPotrait)
        {
            if (textType.Equals(TextType.Mic))
            {
                popupsScreen.landScapeData.descriptionTxt_land.text = permissionTexts.micPermission;
            }
            else if (textType.Equals(TextType.Camera))
            {
                popupsScreen.landScapeData.descriptionTxt_land.text = permissionTexts.cameraPermission;
            }
            else if (textType.Equals(TextType.Gallery))
            {
                popupsScreen.landScapeData.descriptionTxt_land.text = permissionTexts.galleryPermission;
            }
            popupsScreen.landScapeData.permissionPopupLand.SetActive(true);
        }
        else
        {
            if (textType.Equals(TextType.Mic))
            {
                popupsScreen.potraitData.descriptionTxt_port.text = permissionTexts.micPermission;
            }
            else if (textType.Equals(TextType.Camera))
            {
                popupsScreen.potraitData.descriptionTxt_port.text = permissionTexts.cameraPermission;
            }
            else if (textType.Equals(TextType.Gallery))
            {
                popupsScreen.potraitData.descriptionTxt_port.text = permissionTexts.galleryPermission;
            }
            popupsScreen.potraitData.permissionPopupPort.SetActive(true);
        }
    }

}
