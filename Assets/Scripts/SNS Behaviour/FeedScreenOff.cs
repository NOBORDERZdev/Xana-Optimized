using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedScreenOff : MonoBehaviour
{
    private void OnEnable()
    {
        
    }
    public void OffFeedScreen()
    {
        Invoke(nameof(check),0.2f);
    }

    void check()
    {
        if (FeedUIController.Instance.AddFriendPanel.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}
