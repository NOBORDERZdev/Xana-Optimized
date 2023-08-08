using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHandler : MonoBehaviour
{
    Animator anim;
    string currentState;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    #region animations
    //const string BASIC_COMBO_01 = "atk01";//Damage 30 
    //const string BASIC_COMBO_02 = "atk02";//Damage 15-20
    //const string BASIC_COMBO_03 = "atk07";//Damage 15
    //const string SPECIAL_COMBO_01 = "atk08";//Damage 45
    //const string BASIC_COMBO_04 = "atk09";//Damage 5>5
    //const string BASIC_COMBO_05 = "combo_01";//Damage 5>5>10>15
    //const string SPECIAL_COMBO_02 = "combo_01_03";//Damage 5>5>10>15
    //const string SPECIAL_COMBO_03 = "combo_01_04";//Damage 40
    //const string SPECIAL_COMBO_04 = "combo_01_05"; //Damage	50
    //const string SPECIAL_COMBO_05 = "combo_02_01"; //Damage	25>30
    //const string BASIC_COMBO_06 = "combo_02_inplace"; //Damage	10>10>5>10
    //const string SPECIAL_COMBO_06 = "Combo_03";//Damage 15>10>10>20
    //const string BASIC_COMBO_07 = "Kick_02"; //Damage 10>25
    //const string BASIC_COMBO_08 = "Kick_03";//Damage 5>10>20
    //const string SPECIAL_COMBO_07 = "Kick_04"; //DAMAGE 10>15>25
    //const string SPECIAL_COMBO_08 = "Kick_04";//Damage	10>15>25
    //const string SPECIAL_COMBO_09 = "Kick_05";//Damage 10>10>20
    //const string SPECIAL_COMBO_10 = "Kick_06";//Damage 15>15>20
    //const string SPECIAL_COMBO_11 = "Kick_07";//Damage 15>15>20
    //const string THROW_01 = "Paired_FaceBuster_att";  //Damage 60
    //const string SPECIAL_GRAB_THROW_01 = "Paired_JawBreaker_att";//Damage	100	
    //const string SPECIAL_GRAB_THROW_02 = "Paired_PowerBombSlam_att";//Damage	100	
    //const string SPECIAL_GRAB_THROW_03 = "Paired_PressUpDrop_att";//Damage	105
    //const string THROW_02 = "Paired_QuickDDT_att";//Damage 55
    //const string THROW_03 = "Paired_ScoopSlam_att";//Damage 60 
    //const string BASIC_COMBO_09 = "Wrestler_Backhandslap";//Damage 	30 
    //const string THROW_ESCAPSE = "skill_chop_1p_inplace";//Damage -	  
    //const string SPECIAL_GRAB_THROW_04 = "wrestling_finisher_01_1p";//Damage 105
    //const string THROW_04 = "wrestling_finisher_02_1p";//Throw	60	
    //const string SPECIAL_GRAB_THROW_05 = "wrestling_finisher_03_1p";	//Damage 100	
    //const string SPECIAL_GRAB_THROW_06 = "wrestling_finisher_07_1p";	//Damage 120
    //const string THROW_05 = "wrestling_finisher_09_1p";//Damage 65
    //const string THROW_06 = "wrestling_finisher_12_1p";//Damage 65
    #endregion
    //void AnimationState(string _newState) {

    //    if (currentState == _newState) {
    //        return;
    //    }
    //    anim.Play(_newState);

    //    currentState = _newState;
    //}
    public void PlayAnim(int _value)
    {
        _value = Random.Range(0, 11);
        anim.SetTrigger("basicCom");
        anim.SetFloat("basicCombo", _value);
    }
    public void SpecialCombo(int _value)
    {
        _value = Random.Range(0, 11);
        anim.SetTrigger("specialCombo");
        anim.SetFloat("specialCombo", _value);
    }
}
