using AdvancedInputFieldPlugin;
using AdvancedInputFieldSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNSChatInputController : MonoBehaviour
{
	[SerializeField]
	private SNSChatView view;

	private void Start()
	{
		//view.UpdateChatHistorySize();

		EmojiKeyboard emojiKeyboard = view.MessageInput.GetComponentInChildren<EmojiKeyboard>();

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

	public void OnMessageInputBeginEdit(BeginEditReason reason)
	{
		Debug.Log("OnMessageInputBeginEdit");
		view.UpdateOriginalMessageInputPosition();

#if (UNITY_ANDROID || UNITY_IOS)
		if (!Application.isEditor || Settings.SimulateMobileBehaviourInEditor)
		{
			OnMessageInputSizeChanged(view.MessageInput.Size); //Move to top of keyboard on mobile on begin edit
		}
#endif
	}

	public void OnMessageInputEndEdit(string result, EndEditReason reason)
	{
		Debug.Log("OnMessageInputEndEdit");
		view.RestoreOriginalMessageInputPosition();
	}

	public void OnMessageInputSizeChanged(Vector2 size)
	{
		Debug.Log("OnMessageInputSizeChanged: " + size);
		view.UpdateMessageInputPosition();
		view.UpdateChatHistorySize();
	}

	//public void OnMessageSendClick()
	//{
	//	Debug.Log("OnMessageSendClick");
	//	string message = view.MessageInput.RichText;
	//	if (!string.IsNullOrEmpty(message))
	//	{
	//		view.MessageInput.Clear();
	//		SNS_MessageController.Instance.OnChatVoiceOrSendButtonEnable();
	//	}
	//	StartCoroutine(WaitToResetScroll());
	//}

	IEnumerator WaitToResetScroll()
    {
		yield return new WaitForSeconds(0.05f);
		view.UpdateChatHistorySize();
	}

	public void OnKeyboardHeightChanged(int keyboardHeight)
	{
		Debug.Log("OnKeyboardHeightChanged: " + keyboardHeight);
		view.UpdateKeyboardHeight(keyboardHeight);
		view.UpdateChatHistorySize();
	}

	float LastInputfieldHeight;
	public void OnInputFieldValueChange()
    {
		Debug.Log("OnInputFieldValueChange: " + view.MessageInput.Size.y + "	:lastheight:" + LastInputfieldHeight);
		if (LastInputfieldHeight != view.MessageInput.Size.y)
		{
			view.UpdateChatHistorySize();
			LastInputfieldHeight = view.MessageInput.Size.y;
		}
    }
}