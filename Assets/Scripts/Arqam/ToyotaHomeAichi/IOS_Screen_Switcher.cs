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
        if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_filter.mesh = iosScreen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
