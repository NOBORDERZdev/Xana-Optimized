using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string hoverText;
    [SerializeField] private Text descriptionText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.gameObject.SetActive(false);
    }

    public void ShowText(string text)
    {
        descriptionText.text = text;       
    }
}
