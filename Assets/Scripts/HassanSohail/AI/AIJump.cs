using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XanaAi
{
    public class AIJump : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] AiController aiController;
        public void AiJump()
        {
            anim.SetBool("standJump", true);
            Invoke(nameof(StopJump),0.75f);
        }

        void StopJump(){ 
            anim.SetBool("OnlyJump", false);
            anim.SetBool("standJump", false);
            aiController.isPerformingAction = false;
            if (aiController.ActionCoroutine !=null)
            {
                aiController.StopCoroutine(aiController.ActionCoroutine);
            }
            aiController.ActionCoroutine =  aiController.StartCoroutine( aiController.PerformAction());
        }

    }
}

