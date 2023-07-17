using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using XanaAi;
namespace XanaAi{ 
    public class AiSelfie : MonoBehaviour
    {
        [SerializeField] GameObject SelfieObject;
        [SerializeField] Animator animator;
        [SerializeField] AiController ai;
        void Start(){ 
            //animator = GetComponent<Animator>();
            //ai = GetComponent<AiController>();
        }

        public void SelfieAction()
        {
            if (SelfieObject != null)
            {
                 print("Open Selfie Cam");
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

        void DisableSelfie(){ 
            SelfieObject.SetActive(false);
            animator.SetBool("isSelfie", false);
            print("Close Selfie Cam");
            ai.isPerformingAction = false;
            if (ai.ActionCoroutine !=null)
            {
                ai.StopCoroutine(ai.ActionCoroutine);
            }
            ai.ActionCoroutine =  ai.StartCoroutine( ai.PerformAction());
        }

        public void ForceFullyDisableSelfie(){ 
            SelfieObject.SetActive(false);
            animator.SetBool("isSelfie", false);
        }
}
}