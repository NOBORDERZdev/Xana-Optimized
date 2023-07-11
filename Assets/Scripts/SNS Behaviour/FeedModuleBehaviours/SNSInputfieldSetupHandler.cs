using AdvancedInputFieldPlugin;
using AdvancedInputFieldSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SNSInputfieldSetupHandler : MonoBehaviour
{
    public int Y_SPACING = 10;

    public AdvancedInputField messageInput;
    [SerializeField]
    private Canvas canvas;
    private Vector2 originalMessageInputPosition;
    private float keyboardHeight;
    public Button sendButton;

    public Canvas Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = GetComponentInParent<Canvas>();
            }
            return canvas;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EmojiKeyboard emojiKeyboard = messageInput.GetComponentInChildren<EmojiKeyboard>();

#if (UNITY_ANDROID || UNITY_IOS)
        if (!Application.isEditor || Settings.SimulateMobileBehaviourInEditor)
        {
            NativeKeyboardManager.AddKeyboardHeightChangedListener(OnKeyboardHeightChanged);
        }
#endif

#if (!UNITY_EDITOR) && (UNITY_ANDROID || UNITY_IOS)			
		emojiKeyboard.gameObject.SetActive(false);
#else
        emojiKeyboard.gameObject.SetActive(true);
#endif
    }

    private void OnEnable()
    {
        if (sendButton != null)
        {
            if (string.IsNullOrEmpty(messageInput.RichText))
            {
                sendButton.interactable = false;
            }
        }
    }

    public void OnValueChange()
    {
        if (sendButton != null)
        {
            SendButtonSetUp1();
            //SendButtonSetUp2();
        }
    }

    //This method is used to Comment InputField validation check on OnValueChange.......
    public void SendButtonSetUp1()
    {
        bool isFind = false;
        foreach (char c in messageInput.RichText)
        {
            if (!c.Equals(' ') && !c.Equals('\n'))
            {
                isFind = true;
                break;
            }
        }
        //Debug.LogError("IsFind: " + isFind);
        if (isFind)
        {
            sendButton.interactable = true;
        }
        else
        {
            sendButton.interactable = false;
        }
    }

    //This method is used to Comment InputField validation check on OnValueChange(extra solution).......
    public void SendButtonSetUp2()
    {
        char[] tempCharArray = { ' ', '\n'};
        string tempSTR = messageInput.RichText.TrimStart(tempCharArray);
        //Debug.LogError("TempSTR:" + tempSTR);
        if (((messageInput.RichText.StartsWith(" ") || messageInput.RichText.StartsWith("\n")) && string.IsNullOrEmpty(tempSTR)) || string.IsNullOrEmpty(messageInput.RichText))
        {
            sendButton.interactable = false;
        }
        else
        {
            sendButton.interactable = true;
        }
    }
        
    public void OnMessageInputBeginEdit(BeginEditReason reason)
    {
        Debug.Log("OnMessageInputBeginEdit");
        UpdateOriginalMessageInputPosition();

#if (UNITY_ANDROID || UNITY_IOS)
        if (!Application.isEditor || Settings.SimulateMobileBehaviourInEditor)
        {
            OnMessageInputSizeChanged(messageInput.Size); //Move to top of keyboard on mobile on begin edit
        }
#endif
    }

    public void OnMessageInputEndEdit(string result, EndEditReason reason)
    {
        Debug.Log("OnMessageInputEndEdit");
        RestoreOriginalMessageInputPosition();
    }

    public void OnMessageInputSizeChanged(Vector2 size)
    {
        Debug.Log("OnMessageInputSizeChanged: " + size);
        UpdateMessageInputPosition();
    }

    public void OnKeyboardHeightChanged(int keyboardHeight)
    {
        Debug.Log("OnKeyboardHeightChanged: " + keyboardHeight);
        UpdateKeyboardHeight(keyboardHeight);
    }

    //for view reference.......
    public void UpdateOriginalMessageInputPosition()
    {
        originalMessageInputPosition = messageInput.RectTransform.anchoredPosition;
    }

    public void RestoreOriginalMessageInputPosition()
    {
        messageInput.RectTransform.anchoredPosition = originalMessageInputPosition;
    }

    public void UpdateKeyboardHeight(int keyboardHeight)
    {
        this.keyboardHeight = keyboardHeight;
        if (messageInput.Selected)
        {
            UpdateMessageInputPosition();
        }
    }

    public void UpdateMessageInputPosition()
    {
#if (UNITY_ANDROID || UNITY_IOS)
        if (!Application.isEditor || Settings.SimulateMobileBehaviourInEditor)
        {
            Vector2 position = messageInput.RectTransform.anchoredPosition;
            position.y = keyboardHeight / Canvas.scaleFactor;
            position.y += Y_SPACING;//rik.......
            messageInput.RectTransform.anchoredPosition = position;
        }
#endif
    }
}