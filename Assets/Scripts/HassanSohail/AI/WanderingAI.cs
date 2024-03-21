using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using XanaAi;

namespace XanaAi{ 
    public class WanderingAI : MonoBehaviour
    {

        [SerializeField] float minRadius;
        [SerializeField] float maxRadius;
        //[SerializeField] float minWanderTimer;
        //[SerializeField] float maxwanderTimer;
        public bool isMoving=false;
        //float wanderRadius;
        float wanderTimer;
        private Transform target;
        [SerializeField] private UnityEngine.AI.NavMeshAgent agent;
        private float timer;
        [SerializeField] Animator animator;
        [SerializeField] AiController aiController;
        [SerializeField] float walkRotateSpeed;

        void OnEnable()
        {
           // agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            timer = wanderTimer;
            //animator = gameObject.GetComponent<Animator>();
            //aiController = GetComponent<AiController>();
            agent.enabled=false;
            animator.runtimeAnimatorController = EmoteAnimationHandler.Instance.controller;
            animator.SetBool("IsGrounded", true);

        }

        void Update()
        {
            if (isMoving)
            {
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.25f)
                        {
                             agent.updateRotation = true;
                            // Done
                            isMoving =false;
                            animator.SetFloat("Blend", 0.0f);
                            animator.SetFloat("BlendY", 0.0f);
                            print("Reach Destination of wander");
                            aiController.isPerformingAction = false;
                            if (aiController.ActionCoroutine !=null)
                            {
                                aiController.StopCoroutine(aiController.ActionCoroutine);
                            }
                            aiController.ActionCoroutine =  aiController.StartCoroutine( aiController.PerformAction());
                        }
                        else
                        {
                           agent.updateRotation = false;
                            FaceTarget(agent.destination);
                        }
                    }
                    else
                    {
                        animator.SetBool("IsGrounded", true);
                        //if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
                        //{
                        //    transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
                        //}
                        //animator.SetFloat("Blend", agent.speed);
                        animator.SetFloat("BlendY", agent.transform.eulerAngles.y);
                        agent.updateRotation = false;
                            FaceTarget(agent.destination);
                    }
                }
                else
                {
                    //animator.SetFloat("Blend", agent.speed);
                    animator.SetBool("IsGrounded", true);
                    animator.SetFloat("BlendY", agent.transform.eulerAngles.y);
                    agent.updateRotation = false;
                            FaceTarget(agent.destination);
                }
            }
            else
            {
                animator.SetFloat("Blend", 0.0f);
                animator.SetFloat("BlendY", 0.0f);
            }
        }

        public void Wander(){ 
            agent.enabled=true;
            Vector3 newPos = RandomNavSphere(transform.position, Random.Range(minRadius,maxRadius), -1);
            agent.SetDestination(newPos);
            animator.SetBool("IsGrounded", true);
            animator.SetFloat("Blend", 0.01f);
            animator.SetFloat("Blend", 0f);

            int rand = Random.Range(0, 2);  
            print("Start wander with rand : "+ rand);
            switch (rand)
            {
                case 0: // walk
                    animator.SetFloat("Blend", 0.55f);
                    agent.speed =1.2f;
                    break;
                case 1: // run
                    animator.SetFloat("Blend", 1.0f);
                    agent.speed = 2.2f;
                    break;
                default: // sprint
                    animator.SetFloat("Blend", 1.3f);
                    agent.speed = 3;
                    break;
            }
            isMoving=true;
           
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            UnityEngine.AI.NavMeshHit navHit;

            UnityEngine.AI.NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;
        }

        private void FaceTarget(Vector3 destination)
        {
            Vector3 lookPos = destination - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation,walkRotateSpeed);  
        }
    }

}