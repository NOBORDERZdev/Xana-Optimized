using UnityEngine;

public class CommingSoonMessageDisplayer : MonoBehaviour
{
   public void CommingSoonMessage()
    {
        SNSNotificationHandler.Instance.ShowNotificationMsg("Coming soon");
    }
}
