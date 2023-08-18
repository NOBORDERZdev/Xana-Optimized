using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePausePanel : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Destroy(this.gameObject);
    }

    public void ResumeGame()
    {
        OnlineExit scriptInstance = FindObjectOfType<OnlineExit>();
        if (scriptInstance != null)
        {
            scriptInstance.CheckMode();
        }
        Destroy(this.gameObject);
    }
}
