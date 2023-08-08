/// <summary>
/// In this script we are storing data of shot selection for each player
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotsSelectionData : MonoBehaviour
{
    public static ShotsSelectionData _instance;

    public int _basicCombo1Val = -1, _basicCombo2Val = -1, _specialComboVal = -1, _throwVal = -1;//, _grabVal = -1;
    public int combo1Selected = -1, combo2Selected = -1;
    [HideInInspector] public RuntimeAnimatorController combo1ActionAnims, combo2ActionAnims, combo1ReactionAnims, combo2ReactionAnims;

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null) _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
