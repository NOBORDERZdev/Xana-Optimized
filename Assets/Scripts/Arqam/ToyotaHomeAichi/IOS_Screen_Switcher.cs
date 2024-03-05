using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOS_Screen_Switcher : MonoBehaviour
{
    public MeshFilter m_filter;
    public Mesh iosScreen;

    // Start is called before the first frame update
    void Start()
    {
        if(Application.isEditor)
        {
            m_filter.transform.localRotation = Quaternion.Euler(0f, 0.426f, 180f);
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_filter.mesh = iosScreen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
