using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public Animator Reseptionist, player;
    public GameObject headSetInReseptionistHand, headSetInPlayerHand,lobby, headsetinworld;

    public AnimatorStateInfo test;
    public float speed;
    public int n;
    public string name;

    void HeadSet1()
    {
        Gamemanager._InstanceGM.animationController.StatBow(false);
        Gamemanager._InstanceGM.animationController.HeadSet(true);
        Gamemanager._InstanceGM.animationController.headSetInReseptionistHand.SetActive(true);
        print("Head set in Hands are true");
        //   Invoke("check", 3.5f);
    }
    public void StatBow(bool check)
    {
        Reseptionist.SetBool("Bow", check);

    }
    public void HeadSet(bool check)
    {
        Reseptionist.SetBool("HeadSet", check);
      //  Reseptionist.HasState(0, 1);
      //  headSetInReseptionistHand.SetActive(true);
    }

    public void DC()
    {
        //  headSetInReseptionistHand.transform.parent = lobby.transform;
      //  headSetInReseptionistHand.transform.parent = lobby.transform;
        headsetinworld.SetActive(true);
        headSetInReseptionistHand.SetActive(false);
        Invoke("HeadSetOff", speed);
    }
    public void ActiveHeadSet()
    {
        headSetInReseptionistHand.SetActive(true);
    }

    public void PlayerAnimation(bool check)
    {
        player.SetBool("headset", check);
    }
    public void StartPlay()
    {
       PlayerAnimation(true);
    }
    void HeadSetOff()
    {
        headsetinworld.SetActive(false);
    }
}
