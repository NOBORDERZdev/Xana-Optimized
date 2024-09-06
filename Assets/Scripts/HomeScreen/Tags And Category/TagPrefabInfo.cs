using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TagPrefabInfo : MonoBehaviour
{
    [SerializeField]
    private Color _selectedColor;
    public TextMeshProUGUI tagName;
    public TextMeshProUGUI tagNameHighlighter;
    public GameObject hightLighter;
    [HideInInspector]
    public GameObject descriptionPanel;
    public void ClickOnTag()
    {
        SearchWorldUIController.OpenSearchPanel?.Invoke(tagName.text);
        SearchWorldUIController.SearchWorld?.Invoke(tagName.text);
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public bool isSelected = false;
    public void Select_UnselectTags()
    {
        isSelected = !isSelected;   
        if (isSelected)
        {
            GetComponent<Image>().color = _selectedColor;
            tagName.color = Color.white;

            if(!MyProfileDataManager.Instance.userSelectedTags.Contains(tagName.text))
                MyProfileDataManager.Instance.userSelectedTags.Add(tagName.text);
        }
        else
        {
            GetComponent<Image>().color = Color.white;
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
