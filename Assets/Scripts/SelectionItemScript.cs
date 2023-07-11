using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionItemScript : MonoBehaviour
{
    public bool isUseImage = false;

    public bool isChangeText = false;
    public bool isSetBold = false;
    public List<TextMeshProUGUI> AllText = new List<TextMeshProUGUI>();
    public int getTextFromChild = 0;

    public List<Image> AllImage = new List<Image>();
    public Color sellectedColor = new Color();
    public Color unSellectedColor = new Color();
    public int getImageFromChild = 0;
    public int CurrentSelection = 0;

    public SelectionItemScript sameSelectionScript;
    public Sprite selectionIcon;
    public Sprite unSelectionIcon;

    public bool isDefultEnable = false;

    public bool isNotChangeUnselectedLineColor = false;

    public bool isDontAddButtonClick = false;

    public static int ownedNftIndex;

    private void Awake()
    {
        AllImage.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            switch (getImageFromChild)
            {
                case 0:
                    AllImage.Add(this.transform.GetChild(i).GetComponent<Image>());
                    break;
                case 1:
                    AllImage.Add(this.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>());
                    break;
                case 2:
                    AllImage.Add(this.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>());
                    break;
                default:
                    break;
            }

            if (isChangeText)
            {
                switch (getTextFromChild)
                {
                    case 0:
                        AllText.Add(this.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
                        break;
                    case 1:
                        AllText.Add(this.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                        break;
                    case 2:
                        AllText.Add(this.transform.GetChild(i).transform.GetChild(1).GetComponent<TextMeshProUGUI>());
                        break;
                    default:
                        break;
                }
            }

            if (!isDontAddButtonClick)
            {
                int index = i;
                this.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => OnSelectedClick(index));
            }
        }

      //  ownedNftIndex = CurrentSelection;

       
        OnSelectedClick(CurrentSelection);
    }

    private void Start()
    {       
    }


    int opencount = 0;
   

    private void OnEnable()
    {
        if (isDefultEnable)
        {
            //Debug.LogError("here");
            CurrentSelection = 0;
            
        }
        if (opencount > 0)
        {
            OnSelectedClick(CurrentSelection);
        }
        opencount = 1;
    }

    public void OnSelectedClick(int index)
    {
        
        if (sameSelectionScript != null)
        {
            if (sameSelectionScript.AllImage.Count == 0)
            {
                AddValueOnList();
            }
            //Debug.LogError("index" + index);
            for (int i = 0; i < sameSelectionScript.AllImage.Count; i++)
            {
                if (i == index)
                {
                    if (sameSelectionScript.isUseImage)
                    {
                        sameSelectionScript.AllImage[i].sprite = sameSelectionScript.selectionIcon;
                    }
                    else
                    {
                        sameSelectionScript.AllImage[i].color = sameSelectionScript.sellectedColor;
                    }

                    if (sameSelectionScript.isChangeText)
                    {
                        sameSelectionScript.AllText[i].color = sameSelectionScript.sellectedColor;
                        if (sameSelectionScript.isSetBold)
                        {
                            sameSelectionScript.AllText[i].fontStyle = FontStyles.Bold;
                        }
                    }
                    //Debug.LogError("index" + index);
                    sameSelectionScript.CurrentSelection = index;
                    
               
                    //Debug.LogError("CurrentSelection" + sameSelectionScript.CurrentSelection);
                }
                else
                {
                    if (sameSelectionScript.isUseImage)
                    {
                        sameSelectionScript.AllImage[i].sprite = sameSelectionScript.unSelectionIcon;
                    }
                    else
                    {
                        if (!isNotChangeUnselectedLineColor)
                        {
                            sameSelectionScript.AllImage[i].color = sameSelectionScript.unSellectedColor;
                        }
                        else
                        {
                            sameSelectionScript.AllImage[i].color = Color.white;
                        }
                    }

                    if (sameSelectionScript.isChangeText)
                    {
                        sameSelectionScript.AllText[i].color = sameSelectionScript.unSellectedColor;
                        if (sameSelectionScript.isSetBold)
                        {
                            sameSelectionScript.AllText[i].fontStyle = FontStyles.Normal;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < AllImage.Count; i++)
        {            
            if (i == index)
            {
                if (isUseImage)
                {
                    AllImage[i].sprite = selectionIcon;
                }
                else
                {
                    AllImage[i].color = sellectedColor;
                }

                if (isChangeText)
                {
                    AllText[i].color = sellectedColor;
                    if (isSetBold)
                    {
                        AllText[i].fontStyle = FontStyles.Bold;
                    }
                }
                CurrentSelection = index;
            }
            else
            {
                if (isUseImage)
                {
                    AllImage[i].sprite = unSelectionIcon;
                }
                else
                {
                    if (!isNotChangeUnselectedLineColor)
                    {
                        AllImage[i].color = unSellectedColor;
                    }
                    else
                    {
                        AllImage[i].color = Color.white;
                    }
                }

                if (isChangeText)
                {
                    AllText[i].color = unSellectedColor;
                    if (isSetBold)
                    {
                        AllText[i].fontStyle = FontStyles.Normal;
                    }
                }
            }
        }
    }

    void AddValueOnList()
    {
        sameSelectionScript.AllImage.Clear();
        for (int i = 0; i < sameSelectionScript.transform.childCount; i++)
        {
            switch (sameSelectionScript.getImageFromChild)
            {
                case 0:
                    sameSelectionScript.AllImage.Add(sameSelectionScript.transform.GetChild(i).GetComponent<Image>());
                    break;
                case 1:
                    sameSelectionScript.AllImage.Add(sameSelectionScript.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>());
                    break;
                case 2:
                    sameSelectionScript.AllImage.Add(sameSelectionScript.transform.GetChild(i).transform.GetChild(1).GetComponent<Image>());
                    break;
                default:
                    break;
            }

            if (sameSelectionScript.isChangeText)
            {
                switch (sameSelectionScript.getTextFromChild)
                {
                    case 0:
                        sameSelectionScript.AllText.Add(sameSelectionScript.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
                        break;
                    case 1:
                        sameSelectionScript.AllText.Add(sameSelectionScript.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                        break;
                    case 2:
                        sameSelectionScript.AllText.Add(sameSelectionScript.transform.GetChild(i).transform.GetChild(1).GetComponent<TextMeshProUGUI>());
                        break;
                    default:
                        break;
                }
            }
            if (!isDontAddButtonClick)
            {
                int index = i;
                sameSelectionScript.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => OnSelectedClick(index));
            }
        }
    }
}