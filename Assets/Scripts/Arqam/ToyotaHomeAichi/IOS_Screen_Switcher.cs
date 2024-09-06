using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOS_Screen_Switcher : MonoBehaviour
{
    public MeshFilter m_filter;
    public Mesh iosScreen;
    public Mesh androidScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isEditor && WorldItemView.m_EnvName.Contains("D_Infinity_Labo"))
        {
            m_filter.transform.localRotation = Quaternion.Euler(0f, 0.426f, 180f);
        }
        //else if (Application.platform == RuntimePlatform.IPhonePlayer)
        //{
        //    m_filter.mesh = iosScreen;
        //}
        //else if (Application.platform == RuntimePlatform.Android)
        //{
        //    m_filter.mesh = androidScreen;
        //}
#if UNITY_ANDROID
        m_filter.mesh = androidScreen;
#elif UNITY_IOS
           m_filter.mesh = iosScreen;
#endif
    }
}
