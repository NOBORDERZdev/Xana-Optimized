using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XanaAi { 
  public class AiFreeCam : MonoBehaviour{
 
    [SerializeField] GameObject Controller;
    [SerializeField] AiController aiController;
    [SerializeField] Animator anim;
    [SerializeField] float minTime;
    [SerializeField] float maxTime;

    void Start(){
        
    }

    public void PerformFreeCam(){
        anim.SetBool("freecam",true);
        Controller.SetActive(true);
        Invoke(nameof(DisableFreeCam),Random.Range(minTime,maxTime));
    }

    void DisableFreeCam(){
        anim.SetBool("freecam",false);
        Controller.SetActive(false);
        aiController.isPerformingAction = false;
        if (aiController.ActionCoroutine !=null)
        {
            aiController.StopCoroutine(aiController.ActionCoroutine);
        }
        aiController.ActionCoroutine =  aiController.StartCoroutine( aiController.PerformAction());
    }
}
  
}
