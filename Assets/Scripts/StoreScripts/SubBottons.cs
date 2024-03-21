using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SubBottons : MonoBehaviour
{
    public GameObject[] TotalBtns;
    public Color NormalColor;
    public Color HighlightedColor;
    public bool ClothBool;
    public bool AvatarBool;

    private CharacterCustomizationUIManager customizationUIManager;

    // Start is called before the first frame update
    void Start()
    {
        customizationUIManager = FindObjectOfType<CharacterCustomizationUIManager>();
    }

    private int currentSelectedCategoryIndex;

    public void ClickBtnFtn(int m_Index)
    {

        for (int i = 0; i < TotalBtns.Length; i++)
        {
            TotalBtns[i].GetComponentInChildren<ButtonScript>().BtnTxt.color = NormalColor;
            TotalBtns[i].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Normal;
        }
        TotalBtns[m_Index].GetComponentInChildren<ButtonScript>().BtnTxt.color = HighlightedColor;
        TotalBtns[m_Index].transform.GetChild(0).GetComponent<Text>().fontStyle = FontStyle.Bold;


        if (ClothBool)
        {
            InventoryManager.instance.OpenClothContainerPanel(m_Index);
            if (m_Index == 2)
            {
                CharacterCustomizationUIManager.Instance.LoadMyFaceCustomizationPanel();
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(1);
            }
            else
            {
                CharacterCustomizationUIManager.Instance.LoadMyClothCustomizationPanel();
                GameManager.Instance.mainCharacter.GetComponent<FaceIK>().SetLookPos(2);
            }
        }
        else if (AvatarBool)
        {
            InventoryManager.instance.OpenAvatarContainerPanel(m_Index);
            currentSelectedCategoryIndex = m_Index;

            if (m_Index == 10 || m_Index == 6)
            {
                CharacterCustomizationUIManager.Instance.LoadMyClothCustomizationPanel();
            }
            else
            {
                CharacterCustomizationUIManager.Instance.LoadMyFaceCustomizationPanel();
            }
        }
        // print(m_Index);
    }


}
