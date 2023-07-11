using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabTextGroup : MonoBehaviour
{
    public List<TextMeshProUGUI> allButton = new List<TextMeshProUGUI>();
    public Color sellectedColor = new Color();
    public Color unSellectedColor = new Color();
    public int getImageFromChild = 0;
    public int defaultSelection = 0;

    private void Start()
    {
        allButton.Clear();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            switch (getImageFromChild)
            {
                case 0:
                    allButton.Add(this.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
                    break;
                case 1:
                    allButton.Add(this.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                    break;
                default:
                    break;
            }

            int index = i;
            this.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(() => OnSelectedClick(index));
        }
        OnSelectedClick(defaultSelection);
    }

    int opencount = 0;
    private void OnEnable()
    {
        if (opencount > 0)
        {
            OnSelectedClick(defaultSelection);
        }
        opencount = 1;
    }

    public void OnSelectedClick(int index)
    {
        for (int i = 0; i < allButton.Count; i++)
        {
            if (i == index)
            {
                allButton[i].color = sellectedColor;
            }
            else
            {
                allButton[i].color = unSellectedColor;
            }
        }
    }
}