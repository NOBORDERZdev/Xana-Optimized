using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class mycreatorname : MonoBehaviour
{
    public Text creatorTextTMP;


    public void Start()
    {
        creatorTextTMP.text = WorldItem.m_CreaName.ToUpper();
    }
}
