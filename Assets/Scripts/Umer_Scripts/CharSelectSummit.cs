using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using DG.Tweening;

public class CharSelectSummit : MonoBehaviour
{
    public Transform contentParent;
    public GameObject backBtnstore;
    private GameObject selectedObj;
    public List<GameObject> items;
    public static CharSelectSummit instance;
    private int currentCharacterIndex = 0;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        
        foreach (GameObject item in items)
        {
            GameObject itemGameObject = Instantiate(item, contentParent);
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance.UiManager.isAvatarSelectionBtnClicked)
        {
            backBtnstore.SetActive(true);
        }
        else
        {
            backBtnstore.SetActive(false);
        }
        CharacterSelectionChange();
    }

    public void CharacterSelectionChange()
    {
        for (int i = 0; i < contentParent.childCount; i++)
        {
            if (i == currentCharacterIndex)
            {
                contentParent.GetChild(i).GetChild(0).DOScale(new Vector3(1.06f, 0.985f, 0.1f), 0.1f);
                contentParent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                selectedObj = contentParent.GetChild(i).GetChild(0).gameObject;

                for (int a = 0; a < contentParent.childCount; a++)
                {
                    if (a != i)
                    {
                        contentParent.GetChild(a).GetChild(0).DOScale(new Vector3(0.91f, 0.91f, 0.1f), 0.1f);
                    }
                }
            }
            else
            {
                contentParent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    public void OnClickNext()
    {
        //if (selectedObj != null)
        //{
        //    GameManager.Instance.HomeCameraInputHandler(true);
        //    UserLoginSignupManager.instance.SelectedPresetImage.sprite = selectedObj.transform.GetChild(0).GetComponent<Image>().sprite;
        //    UserLoginSignupManager.instance.SelectPresetImageforEditProfil.sprite = selectedObj.transform.GetChild(0).GetComponent<Image>().sprite;
        //    selectedObj.GetComponent<PresetData_Jsons>().ChangecharacterFromPresetPanel();
        //    GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
        //    if (ConstantsHolder.xanaConstants.LoggedInAsGuest)
        //    {
        //        UserLoginSignupManager.instance.UserNameFieldObj.SetActive(false);
        //    }
        //    else
        //    {
        //        UserLoginSignupManager.instance.UserNameFieldObj.SetActive(true);
        //    }
        //}
    }

    public void OnClickLeft()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + contentParent.childCount) % contentParent.childCount;
        CharacterSelectionChange();
    }

    public void OnClickRight()
    {
        currentCharacterIndex = (currentCharacterIndex + 1) % contentParent.childCount;
        CharacterSelectionChange();
    }
}