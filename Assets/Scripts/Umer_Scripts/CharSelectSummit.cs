using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class CharSelectSummit : MonoBehaviour
{
    public Transform contentParent;
    public GameObject backBtnstore;
    private GameObject SelectedOBJ;
    HorizontalScrollSnap hssRef;
    public List<GameObject> items;
    public static CharSelectSummit instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        hssRef = this.GetComponent<HorizontalScrollSnap>();
        foreach (GameObject item in items)
        {
            GameObject itemGameObject = Instantiate(item, contentParent);
        }
    }

    private void OnEnable()
    {
        if (!hssRef.enabled)
        {
            hssRef.enabled = true;
        }
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
            if (hssRef.CurrentPage == i)
            {

                //contentParent.GetChild(i).GetChild(0).localScale = Vector2.Lerp(contentParent.GetChild(i).GetChild(0).localScale, new Vector2(1.06f, 0.985f), 0.1f);
                contentParent.GetChild(i).GetChild(0).DOScale(new Vector3(1.75f, 1.45f, 0.1f), 0.1f);
                contentParent.GetChild(i).GetChild(0).localPosition = new Vector3(contentParent.GetChild(i).GetChild(0).localPosition.x, -6, contentParent.GetChild(i).GetChild(0).localPosition.z);
                contentParent.GetChild(i).GetChild(0).GetChild(0).DOScale(new Vector3(0.90f, 1.01f, 0.1f), 0.1f);
                contentParent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                SelectedOBJ = contentParent.GetChild(i).GetChild(0).gameObject;

                for (int a = 0; a < contentParent.childCount; a++)
                {
                    if (a != i)
                    {
                        //contentParent.GetChild(a).GetChild(0).localScale = Vector2.Lerp(contentParent.GetChild(a).GetChild(0).localScale, new Vector2(0.91f, 0.91f), 0.1f);
                        contentParent.GetChild(a).GetChild(0).DOScale(new Vector3(1.2f, 1.2f, 0.1f), 0.1f);
                        contentParent.GetChild(a).GetChild(0).GetChild(0).DOScale(new Vector3(1.01f, 1.01f, 0.1f),0.1f);
                        contentParent.GetChild(a).GetChild(0).localPosition = new Vector3(contentParent.GetChild(a).GetChild(0).localPosition.x, -60, contentParent.GetChild(a).GetChild(0).localPosition.z);
                    }
                    
                }
                if (i != 0 || i != 43)
                {
                    if (i == 0)
                    {
                        contentParent.GetChild(1).GetChild(0).GetComponent<Image>().enabled = false;
                    }
                    if (contentParent.childCount < i)
                    {
                        contentParent.GetChild(i - 1).GetChild(0).DOScale(new Vector3(1.3f, 1.3f, 0.1f), 0.1f);
                    }
                    if (contentParent.childCount < i)
                    {
                        contentParent.GetChild(i + 1).GetChild(0).DOScale(new Vector3(1.3f, 1.3f, 0.1f), 0.1f);
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
        if (SelectedOBJ != null)
        {
            GameManager.Instance.HomeCameraInputHandler(true);
            //if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            //{
            //    UserLoginSignupManager.instance.SelectedPresetImage.sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
            //    UserLoginSignupManager.instance.SelectPresetImageforEditProfil.sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;

            //}
            //UserRegisterationManager.instance.LogoImage.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
            //UserRegisterationManager.instance.LogoImage2.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
            //UserRegisterationManager.instance.LogoImage3.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
            //Debug.LogError("selected obj name :- "+SelectedOBJ.name);
            SelectedOBJ.GetComponent<PresetData_Jsons>().ChangecharacterFromPresetPanel();
            GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
            if (ConstantsHolder.xanaConstants.LoggedInAsGuest||ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                UserLoginSignupManager.instance.UserNameFieldObj.SetActive(false);
            }
            else
            {
                UserLoginSignupManager.instance.UserNameFieldObj.SetActive(true);
            }
        }
    }

}