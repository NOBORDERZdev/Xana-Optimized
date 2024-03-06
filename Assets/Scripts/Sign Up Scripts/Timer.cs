using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Timer : MonoBehaviour
{
    //public Text sendAgainText;
    public string sendAgainText;
    private float timeRemaining = 100;
    public float TotalTimer;
    public bool timerIsRunning = false;
    [Space(5)]
    public Button sendAgain;

    //private Text timerText;
    private void Start()
    {
        timeRemaining = TotalTimer;
        // Starts the timer automatically
        timerIsRunning = true;
        // timerText
        sendAgainText = TextLocalization.GetLocaliseTextByKey(sendAgainText);
    }

    private void OnEnable()
    {
        timeRemaining = TotalTimer;
        // Starts the timer automatically
        timerIsRunning = true;
        // timerText
        now = System.DateTime.Now;
    }
    public void ResetTimer()
    {
        if (sendAgain)
        {
            Debug.Log("Interacteble False");
            sendAgain.interactable = false;
        }
        timeRemaining = TotalTimer;       
    }

    void AdjustTimer(float offset)
    {
        timeRemaining -= offset;
    }
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                float minutes = Mathf.FloorToInt(timeRemaining / 60);
                float seconds = Mathf.FloorToInt(timeRemaining % 60);
                this.GetComponent<Text>().text = minutes.ToString() + ":" + seconds.ToString();// + "     <color=#1D9CFE>" + sendAgainText + "</color>";
                this.GetComponent<Text>().text = string.Format("{00:00}:{01:00}", minutes, seconds);//  + "     <color=#1D9CFE>" + sendAgainText + "</color>";


            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                this.GetComponent<Text>().text ="00:00";// timeRemaining.ToString();// + "     <color=#1D9CFE>" + sendAgainText + "</color>";
                timerIsRunning = false;
                if (sendAgain)
                {
                    Debug.Log("Interacteble True");
                    sendAgain.interactable = true;
                }
            }
        }
    }
    System.DateTime now;
    System.TimeSpan buffr;
    void OnApplicationFocus(bool hasFocus)
    {

        if (hasFocus)
        {
            buffr = (System.DateTime.Now - now);
            AdjustTimer(buffr.Seconds);  // minus the difference 
            //not in background
        }
        else
        {
            now = System.DateTime.Now;   // assign time when in background
            //in the background
        }
    }
}