using UnityEngine;

public class OnClickIntroductryPanel : MonoBehaviour
{
    public void OnClickIntroductryPanelMethod()
    {
        SMBCManager.Instance.OnIntroductryPanelClicked?.Invoke();
    }
}
