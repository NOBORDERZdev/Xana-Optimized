using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class NpcJump : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] NpcBehaviourSelector npcBehaviour;

        private void Start()
        {
            
        }

        public void AiJump()
        {
            anim.SetBool("standJump", true);
            StartCoroutine(StopJump());
        }

        IEnumerator StopJump()
        {
            yield return new WaitForSeconds(0.5f);
            anim.SetBool("OnlyJump", false);
            anim.SetBool("standJump", false);
            yield return new WaitForSeconds(0.5f);
            StartNewAction();
        }

        private void StartNewAction()
        {
            npcBehaviour.isPerformingAction = false;
            if (npcBehaviour.ActionCoroutine != null)
                StopCoroutine(npcBehaviour.ActionCoroutine);
            npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
        }

    }
}
