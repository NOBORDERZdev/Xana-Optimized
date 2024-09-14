using UnityEngine;

public class SMBCRedMoonTrigger : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(SMBCManager.Instance.CheckAllRocketPartIsCollected());
    }
}
