using UnityEngine;

public class CommingSoonMessageDisplayer : MonoBehaviour
{
   public void CommingSoonMessage()
    {
        SNSNotificationManager.Instance.ShowNotificationMsg("Coming soon");
    }
}
