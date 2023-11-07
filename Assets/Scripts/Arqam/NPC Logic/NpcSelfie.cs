using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace NPC {
    public class NpcSelfie : MonoBehaviour
    {
        [SerializeField] GameObject SelfieObject;
        [SerializeField] Animator animator;
        [SerializeField] NpcBehaviourSelector npcBehaviour;

        void Start()
        {

        }

        public void SelfieAction()
        {
            if (SelfieObject != null)
            {
                SelfieObject.SetActive(true);
                animator.SetBool("isSelfie", true);
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {  // apply random motion
                    Vector3 tempVec = gameObject.transform.eulerAngles;
                    gameObject.transform.DORotate((new Vector3(tempVec.x, Random.Range(0, 360), tempVec.z)), 10f, RotateMode.FastBeyond360);
                }
                Invoke(nameof(DisableSelfie), Random.Range(5, 10));
            }
        }

        void DisableSelfie()
        {
            SelfieObject.SetActive(false);
            animator.SetBool("isSelfie", false);

            npcBehaviour.isPerformingAction = false;
            if (npcBehaviour.ActionCoroutine != null)
                StopCoroutine(npcBehaviour.ActionCoroutine);
            npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
        }

        public void ForceFullyDisableSelfie()
        {
            SelfieObject.SetActive(false);
            animator.SetBool("isSelfie", false);
        }

    }
}
