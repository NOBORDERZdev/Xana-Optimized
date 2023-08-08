/// <summary>
/// Animators referneces are taken in this group and we set them in player animator by getting animators from here 
/// if user choses 1st combo then we set 1st combo action and reaction animator for the player getting it from here
/// </summary>


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimManager : MonoBehaviour
{

    [Header("BASIC COMBO 1 ASSETS")]
    public RuntimeAnimatorController[] _combo1ActionAnims;
    public RuntimeAnimatorController[] _combo1ReactionAnims;

    [Space(20)]
    [Header("BASIC COMBO 2 ASSETS")]
    public RuntimeAnimatorController[] _combo2ActionAnims;
    public RuntimeAnimatorController[] _combo2ReactionAnims;
}
