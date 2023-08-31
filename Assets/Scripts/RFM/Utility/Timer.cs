using System;
using System.Collections;
using UnityEngine;

namespace RFM
{
    /// <summary>
    /// A timer
    /// </summary>
    /// <remarks>Muneeb</remarks>
    public class Timer : MonoBehaviour
    {
    	#region Fields
    	
        private float _totalSeconds = 0;
        private float _elapsedSeconds = 0;
        private float _elapsedSeconds2 = 0;
        
        private bool _running = false;
        private bool _finished = false;

        private Action _onFinishedCallback;
        private Action<float> _onTickCallback;
        private TMPro.TextMeshProUGUI TimeText;

        #endregion
    	
    	#region Properties
        
        // public float ElapsedTime => _elapsedSeconds;
        // public float TotalTime => _totalSeconds;
        
        #endregion
    
        #region Methods


        /// <summary>
        /// Sets the duration of the timer
        /// The duration can only be set if the timer isn't currently running
        /// </summary>
        /// <param name="value">duration in seconds</param>
        /// <param name="onFinishedCallback">called when timer is finished</param>
        /// <param name="timeText">Text that shows remaining time</param>
        /// <param name="onOneSecondCallback">Called after each second</param>
        public static void SetDurationAndRun(float value, Action onFinishedCallback = null, 
	        TMPro.TextMeshProUGUI timeText = null, Action<float> onOneSecondCallback = null)
        {
	        var timerObj = new GameObject("timerObj");
	        var timer = timerObj.AddComponent<RFM.Timer>();

	        timer._totalSeconds = value;
	        
	        timer._onFinishedCallback = null; // clear the callback in case it was used previously
	        timer._onTickCallback = null;
	        if (onFinishedCallback != null) timer._onFinishedCallback = onFinishedCallback;
	        if (onOneSecondCallback != null) timer._onTickCallback = onOneSecondCallback;

	        timer.TimeText = null;
	        if (timeText) timer.TimeText = timeText;

	        timer._finished = false;
	        timer.Run();
        }


        private void Update()
        {
	        if (!_running) return;
	        
	        _elapsedSeconds += Time.deltaTime;
	        _elapsedSeconds2 += Time.deltaTime;

	        if (TimeText) TimeText.text = (_totalSeconds - _elapsedSeconds).ToString("F0");

	        if (_elapsedSeconds2 > 1)
	        {
		        _elapsedSeconds2 = 0;
		        _onTickCallback?.Invoke(_totalSeconds - _elapsedSeconds);
	        }
            
            if (_elapsedSeconds >= _totalSeconds)
            {
	            _finished = true;
	            _running = false;
	            _onFinishedCallback?.Invoke();
	            Destroy(gameObject);
            }
        }
        
        
        private void Run()
        {	
    		// only run with valid duration
    		if (_totalSeconds > 0)
            {
	            _running = true;
                _elapsedSeconds = 0;
    		}
    	}

        public static IEnumerator SetDurationAndRunEnumerator(float value, Action onFinishedCallback = null, 
	        TMPro.TextMeshProUGUI timeText = null, Action<float> onOneSecondCallback = null)
        {
	        // SetDurationAndRun(value, onFinishedCallback, timeText);
	        
	        var timerObj = new GameObject("timerObj");
	        var timer = timerObj.AddComponent<RFM.Timer>();

	        timer._totalSeconds = value;
	        
	        timer._onFinishedCallback = null; // clear the callback in case it was used previously
	        timer._onTickCallback = null;
	        if (onFinishedCallback != null) timer._onFinishedCallback = onFinishedCallback;
	        if (onOneSecondCallback != null) timer._onTickCallback = onOneSecondCallback;

	        timer.TimeText = null;
	        if (timeText) timer.TimeText = timeText;

	        timer._finished = false;
	        timer.Run();
	        
	        yield return new WaitUntil(() => timer._finished);
        }

        #endregion
    }
}