using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCommingSoonNotification : MonoBehaviour
{
   public void CommingSoonMessage()
    {
        SNSNotificationManager.Instance.ShowNotificationMsg("Coming soon");
    }
}
