using AdvancedInputFieldPlugin;
using AdvancedInputFieldSamples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SNSChatView : MonoBehaviour
{
	public int Y_SPACING = 10;

	[SerializeField]
	private AdvancedInputField messageInput;
	[SerializeField]
	private Canvas canvas;
	private ScrollRect scrollRect;
	private Vector2 originalMessageInputPosition;
	private float keyboardHeight;

	public ScrollRect ScrollRect { get { return scrollRect; } }
	public AdvancedInputField MessageInput { get { return messageInput; } }

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

	private void Awake()
	{
		scrollRect = GetComponentInChildren<ScrollRect>();
	}

	int cnt = 0;
    void OnEnable()
    {
		if (cnt == 0)
		{
			StartCoroutine(WaitReset());
			cnt += 1;
		}
		Invoke("UpdateChatHistorySize",0.1f);

	}

	IEnumerator WaitReset()//default height set.......
    {
		messageInput.Mode = InputFieldMode.SCROLL_TEXT;
		yield return new WaitForSeconds(0.05f);
		messageInput.Mode = InputFieldMode.VERTICAL_RESIZE_FIT_TEXT;
	}

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

	public void UpdateChatHistorySize()
	{
		if(MessageInput == null)
        {
			return;
        }
		RectTransform messageInputTransform = MessageInput.RectTransform;
		RectTransform chatHistoryTransform = ScrollRect.GetComponent<RectTransform>();
		float messageInputTopY = GetAbsoluteTopY(messageInputTransform);
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA)
		EmojiKeyboard emojiKeyboard = messageInputTransform.GetComponentInChildren<EmojiKeyboard>();
		RectTransform emojiKeyboardTransform = emojiKeyboard.GetComponent<RectTransform>();
		messageInputTopY = GetAbsoluteTopY(emojiKeyboardTransform);
#endif
		float chatHistoryBottomY = GetAbsoluteBottomY(chatHistoryTransform);
		float differenceY = chatHistoryBottomY - messageInputTopY;

		Vector2 sizeDelta = chatHistoryTransform.sizeDelta;
		sizeDelta.y += differenceY;
		sizeDelta.y -= Y_SPACING;//rik
		chatHistoryTransform.sizeDelta = sizeDelta;

		//ResetContainerPosition();
	}

	public float GetAbsoluteTopY(RectTransform rectTransform)
	{
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);

		float topY = corners[1].y;
		float normalizedBottomY = 0;
		if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			normalizedBottomY = topY / Screen.height;
		}
		else
		{
			Camera camera = Canvas.worldCamera;
			normalizedBottomY = (topY + camera.orthographicSize) / (camera.orthographicSize * 2);
		}

		return (normalizedBottomY * Canvas.pixelRect.height) / Canvas.scaleFactor;
	}

	public float GetAbsoluteBottomY(RectTransform rectTransform)
	{
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);

		float bottomY = corners[0].y;
		float normalizedBottomY = 0;
		if (Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			normalizedBottomY = bottomY / Screen.height;
		}
		else
		{
			Camera camera = Canvas.worldCamera;
			normalizedBottomY = (bottomY + camera.orthographicSize) / (camera.orthographicSize * 2);
		}

		return (normalizedBottomY * Canvas.pixelRect.height) / Canvas.scaleFactor;
	}

	public void ResetContainerPosition()
    {
		scrollRect.verticalNormalizedPosition = 0;
	}
}