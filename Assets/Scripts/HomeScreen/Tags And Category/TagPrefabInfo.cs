using TMPro;
using UnityEngine;
using System.Collections;

public class TagPrefabInfo : MonoBehaviour
{

    public TextMeshProUGUI tagName;
    public TextMeshProUGUI tagNameHighlighter;
    public GameObject hightLighter;
    [HideInInspector]
    public GameObject descriptionPanel;

    public void ClickOnTag()
    {
        WorldSearchManager.OpenSearchPanel?.Invoke(tagName.text);
        WorldSearchManager.SearchWorld?.Invoke(tagName.text);
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public bool isSelected = false;
    public void Select_UnselectTags()
    {
        isSelected = !isSelected;   
        if (isSelected)
        {
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.15f, 0.15f, 0.15f, 1);// Color.black;
            tagName.color = Color.white;

            if(!MyProfileDataManager.Instance.userSelectedTags.Contains(tagName.text))
                MyProfileDataManager.Instance.userSelectedTags.Add(tagName.text);
        }
        else
        {
            GetComponent<UnityEngine.UI.Image>().color = Color.white;
            tagName.color = Color.black; 

            if (MyProfileDataManager.Instance.userSelectedTags.Contains(tagName.text))
                MyProfileDataManager.Instance.userSelectedTags.Remove(tagName.text);
        }   
    }
    private void OnEnable()
    {
        isSelected = false;
    }
}
