using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AdvancedInputFieldPlugin;

public class InputFieldHightResetScript : MonoBehaviour
{
    public LayoutElement mainParentLayoutElement;
    public GameObject mainInputFieldObj;
    public ContentSizeFitter inputFieldContentSizeFitter;
    public GameObject inputFieldTextObj;
    public TMP_InputField inputField;
    public TextMeshProUGUI characterLimitCountText;
    public bool isTmpInputField = false;

    public void OnValueChangeAfterResetHeight()
    {
        //inputFieldContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        if (characterLimitCountText != null)
        {
            CharacterLimitCountTextSetup();
        }

        if (co != null)
        {
            StopCoroutine(co);
        }
        co = StartCoroutine(WaitToResetContentSizeFitter());
    }

    Coroutine co;
    IEnumerator WaitToResetContentSizeFitter()
    {
        yield return new WaitForSeconds(0.01f);
        //inputFieldContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (mainParentLayoutElement != null)
        {
            mainParentLayoutElement.preferredHeight = mainInputFieldObj.GetComponent<RectTransform>().rect.height + 68;
        }
        inputFieldTextObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        if (isTmpInputField)
        {
            if (mainInputFieldObj.transform.GetChild(0).transform.GetChild(0).name == "Caret")
            {
                mainInputFieldObj.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().offsetMax = Vector2.zero;
                mainInputFieldObj.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().offsetMin = Vector2.zero;
            }
        }
        // MyProfileDataManager.Instance.editProfilemainInfoPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        yield return new WaitForSeconds(0.1f);
        //MyProfileDataManager.Instance.editProfilemainInfoPart.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void CharacterLimitCountTextSetup()
    {
        characterLimitCountText.text = (inputField.characterLimit - inputField.text.Length).ToString();
    }

    public void NormalCharacterLimitCountTextSetup()
    {
        if (isAdvanceInputField)
        {
            characterLimitCountText.text = (advanceInputField.CharacterLimit - emojiCharacterLimitFilter.GetCharacterCount(advanceInputField.Text)).ToString();
            //characterLimitCountText.text = (advanceInputField.CharacterLimit - advanceInputField.Text.Length).ToString();
        }
        else
        {
            characterLimitCountText.text = (normalInputField.characterLimit - normalInputField.text.Length).ToString();
        }
    }

    [Header("NormalInputField")]
    public InputField normalInputField;
    public RectTransform normalInputFieldMainLayoutElement;
    public Text normalTempText;

    public AdvancedInputField advanceInputField;
    public RectTransform advanceInputFieldMainLayoutElement;
    public bool isAdvanceInputField = false;
    public EmojiCharacterLimitFilter emojiCharacterLimitFilter;

    //Normal InputField Reset.......
    public void OnValueChangeAndResetNormalInputField()
    {
        if (isAdvanceInputField)
        {
            normalTempText.text = advanceInputField.Text;
        }
        else
        {
            normalTempText.text = normalInputField.text;
        }
        if (characterLimitCountText != null)
        {
            NormalCharacterLimitCountTextSetup();
        }

        if (normalCo != null)
        {
            StopCoroutine(normalCo);
        }
        normalCo = StartCoroutine(WaitToResetNormalInputField());
    }

    Coroutine normalCo;
    IEnumerator WaitToResetNormalInputField()
    {
        yield return new WaitForSeconds(0f);
        if (isAdvanceInputField)
        {
            if (advanceInputFieldMainLayoutElement != null && advanceInputFieldMainLayoutElement.sizeDelta.y < 800)
            {
                //Debug.LogError("WaitToResetNormalInputField:" + normalTempText.gameObject.GetComponent<RectTransform>().rect.height + 55);
                advanceInputFieldMainLayoutElement.sizeDelta = new Vector2(advanceInputFieldMainLayoutElement.sizeDelta.x, normalTempText.gameObject.GetComponent<RectTransform>().rect.height + 70);
            }
            else
            {
                advanceInputFieldMainLayoutElement.sizeDelta = new Vector2(advanceInputFieldMainLayoutElement.sizeDelta.x, 800);
            }
        }
        else
        {
            if (normalInputFieldMainLayoutElement != null && normalInputFieldMainLayoutElement.sizeDelta.y < 800)
            {
                //Debug.LogError("WaitToResetNormalInputField:" + normalTempText.gameObject.GetComponent<RectTransform>().rect.height + 55);
                normalInputFieldMainLayoutElement.sizeDelta = new Vector2(normalInputFieldMainLayoutElement.sizeDelta.x, normalTempText.gameObject.GetComponent<RectTransform>().rect.height + 65);
            }
        }
    }
}