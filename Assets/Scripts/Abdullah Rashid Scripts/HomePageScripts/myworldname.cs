using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class myworldname : MonoBehaviour
{
    public TextMeshProUGUI WorldTextTMP;

   
   public void Start()
    {
        WorldTextTMP.text = WorldItem.m_EnvName.ToUpper();
    }
}
