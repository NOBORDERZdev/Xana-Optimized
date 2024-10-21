using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using DG.Tweening;

public class CharacSelectScroll : MonoBehaviour
{
    public Transform contentParent;
    public GameObject backBtnstore;
    private GameObject SelectedOBJ;
    HorizontalScrollSnap hssRef;
    public List<GameObject> items;
    public static CharacSelectScroll instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        hssRef = this.GetComponent<HorizontalScrollSnap>();
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
                contentParent.GetChild(i).GetChild(0).DOScale(new Vector3(1.06f, 0.985f, 0.1f), 0.1f);

                contentParent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                SelectedOBJ = contentParent.GetChild(i).GetChild(0).gameObject;

                for (int a = 0; a < contentParent.childCount; a++)
                {
                    if (a != i)
                    {
                        //contentParent.GetChild(a).GetChild(0).localScale = Vector2.Lerp(contentParent.GetChild(a).GetChild(0).localScale, new Vector2(0.91f, 0.91f), 0.1f);
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
        if (SelectedOBJ != null)
        {
            GameManager.Instance.HomeCameraInputHandler(true);
            if (!ConstantsHolder.xanaConstants.SwitchXanaToXSummit)
            {
                UserLoginSignupManager.instance.SelectedPresetImage.sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
                UserLoginSignupManager.instance.SelectPresetImageforEditProfil.sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;

            }
            //UserRegisterationManager.instance.LogoImage.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
                //UserRegisterationManager.instance.LogoImage2.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
                //UserRegisterationManager.instance.LogoImage3.GetComponent<Image>().sprite = SelectedOBJ.transform.GetChild(0).GetComponent<Image>().sprite;
                //Debug.LogError("selected obj name :- "+SelectedOBJ.name);
                SelectedOBJ.GetComponent<PresetData_Jsons>().ChangecharacterFromPresetPanel();
            GameManager.Instance.HomeCamera.GetComponent<HomeCameraController>().CenterAlignCam();
            if (ConstantsHolder.xanaConstants.LoggedInAsGuest)
            {
                UserLoginSignupManager.instance.UserNameFieldObj.SetActive(false);
            } else {
                UserLoginSignupManager.instance.UserNameFieldObj.SetActive(true);
            }
        }
    }

}
