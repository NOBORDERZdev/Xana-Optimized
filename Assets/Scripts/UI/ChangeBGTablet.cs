using UnityEngine;

public class ChangeBGTablet : MonoBehaviour
{
    public GameObject mobileBG;
    public GameObject tabBG;
    // Start is called before the first frame update
    void Start()
    {
        if (XanaConstants.xanaConstants.screenType == XanaConstants.ScreenType.TabScreen)
        {
            tabBG.SetActive(true);
            mobileBG.SetActive(false);
        }
        else
        {
            mobileBG.SetActive(true);
            tabBG.SetActive(false);
        }
    }
}
