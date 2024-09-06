using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenRotation : MonoBehaviour
{
    public bool rotateScreen = true;

    public Vector3 rotateScreenValue;
    // Start is called before the first frame update
    void Start()
    {
        if (!rotateScreen)
            return;
        //GetLivestreamUrl(_livestreamUrl);
#if UNITY_ANDROID
        if (WorldItemView.m_EnvName.Contains("BreakingDown Arena"))
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        else
            transform.localRotation = Quaternion.Euler(rotateScreenValue);//Quaternion.Euler(180, 0, 0);
#endif
    }
}
