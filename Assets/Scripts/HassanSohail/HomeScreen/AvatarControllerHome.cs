using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEngine.AI;

public class AvatarControllerHome : MonoBehaviour
{
    #region Editor_Wandering
    [Header("Wandering")]
    [SerializeField] float minRadius;
    [SerializeField] float maxRadius;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] float walkRotateSpeed;
    #endregion

    #region PrivateVar_Wandering
    bool isMoving = false;
    float wanderTimer;
    Transform target;
    float timer;
    #endregion

    bool isPerformingAction = false;
    private void OnEnable()
    {
        timer = wanderTimer;
        agent.enabled = false;
        //animator.runtimeAnimatorController = EmoteAnimationPlay.Instance.controller;
        animator.SetBool("IsGrounded", true);
    }

    private void Start()
    {
        // PerformAction();
        Wander();
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
                        isMoving = false;
                        agent.enabled = false;
                        animator.SetFloat("Blend", 0.0f);
                        animator.SetFloat("BlendY", 0.0f);

                        isPerformingAction = false;

                        //PerformAction();
                        perfomAnim();
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
                    animator.SetFloat("BlendY", agent.transform.eulerAngles.y);
                    agent.updateRotation = false;
                    FaceTarget(agent.destination);
                }
            }
            else
            {
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

    /// <summary>
    /// To perform different action like wandering, animations etc.
    /// </summary>
    void PerformAction() {
        int rand = 0;//Random.Range(0,TotalActions);
        switch (rand)
        {
            case 0:
                Wander();
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// To wander in world
    /// </summary>
    void Wander() {
        animator.SetBool("Action", false);
        agent.enabled = true;
        Vector3 newPos = RandomNavSphere(transform.position, Random.Range(minRadius, maxRadius), -1);
        agent.SetDestination(newPos);
        animator.SetBool("IsGrounded", true);
        animator.SetFloat("Blend", 0.01f);
        animator.SetFloat("Blend", 0f);
        int rand = 0;//Random.Range(0, 2);
        switch (rand)
        {
            case 0: // walk
                animator.SetFloat("Blend", 0.25f);
                agent.speed = 0.9f;
                break;
           /* case 1: // run
                animator.SetFloat("Blend", 1.0f);
                agent.speed = 2.2f;
                break;
            default: // sprint
                animator.SetFloat("Blend", 1.3f);
                agent.speed = 3;
                break;*/
        }
        isMoving = true;

    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, walkRotateSpeed);
    }

    void perfomAnim() {
        animator.SetBool("Action",true);
        animator.SetFloat("Blend", 0f);
        Invoke(nameof(Wander), 8f);
    }

}
