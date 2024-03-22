using ControlFreak2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BDGamePlayUIHandler : MonoBehaviour
{
    public RectTransform joystick, LK, LP, HP, HK, SP, B;
    public Image[] buttonImgs;
    public TouchButtonSpriteAnimator[] buttonImgs2;
    public static BDGamePlayUIHandler inst;

    private void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(this);
        }
        else
        {
            inst = this;
        }
    }
}
