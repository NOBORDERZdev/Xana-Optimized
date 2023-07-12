using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VideoRecordingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image button, countdown, pauseIcon;
    public UnityEvent onTouchDown, onTouchUp;
    public bool pressed = false;
    private const float MaxRecordingTime = 15f; // seconds

	private void Start()
	{
		Reset();
	}

	private void Reset()
	{
		// Reset fill amounts
		if (button != null)
			button.fillAmount = 1.0f;
		if (countdown != null)
			countdown.fillAmount = 0.0f;
        if (pauseIcon != null)
        {
			pauseIcon.gameObject.SetActive(false);
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
	{
        /*if (ARFaceModuleManager.Instance.r_IsCaptureTypeImage)
        {
			return;
        }
		// Start counting
		StartCoroutine(Countdown());*/
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
	{
		// Reset pressed
		/*pressed = false;*/
	}


	public void OnClickStartRecording()
    {
		if (ARFaceModuleManager.Instance.r_IsCaptureTypeImage)
		{
			return;
		}
		if (!pressed)
		{
			// Start counting
			RecordVideoBehaviour.instance.StartRecording();
			StartCoroutine(Countdown());
        }
        else
        {
			pressed = false;
			RecordVideoBehaviour.instance.StopRecording();
		}
	}

	public void OnClickStopRecording()
	{
		/*if (ARFaceModuleManager.Instance.r_IsCaptureTypeImage)
		{
			return;
		}
		RecordVideoBehaviour.instance.StopRecording();*/
	}

	private IEnumerator Countdown()
	{
		// First wait a short time to make sure it's not a tap
		yield return new WaitForSeconds(0.1f);
		pressed = true;

		if (pauseIcon != null)
		{
			pauseIcon.gameObject.SetActive(true);
		}

		if (!pressed)
			yield break;
		// Start recording
		onTouchDown?.Invoke();
		// Animate the countdown
		float startTime = Time.time, ratio = 0f;
		while (pressed && (ratio = (Time.time - startTime) / MaxRecordingTime) < 1.0f)
		{
			countdown.fillAmount = ratio;
			button.fillAmount = 1f - ratio;
			yield return null;
		}
		// Reset
		Reset();
		// Stop recording
		onTouchUp?.Invoke();

		if (pressed)//Auto Stop Recodring if Second is Completed.......
		{
			pressed = false;
			RecordVideoBehaviour.instance.StopRecording();
		}
	}

	public void CancelingVideoToBackButtonPress()
    {
        if (pressed)
        {
			ARFaceModuleManager.Instance.r_IsCapturingVideoBack = true;//this is check to capture video time to back.......
			pressed = false;
			RecordVideoBehaviour.instance.StopRecording();
			Reset();
		}
    }
}