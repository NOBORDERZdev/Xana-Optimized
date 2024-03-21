using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEngine.AI;

public class AvatarControllerHome : MonoBehaviour
{
    #region Editor_Wandering
    [SerializeField] GameObject worldObj;
    [SerializeField] GameObject storeCam;
    [SerializeField] public GameObject worldCam;
    [SerializeField] GameObject cineCam;
    [SerializeField] RuntimeAnimatorController ArAnimator;
    [Header("Wandering")]
    [SerializeField] float minRadius;
    [SerializeField] float maxRadius;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] float walkRotateSpeed;
    #endregion

    #region PrivateVar_Wandering
    bool isMoving = false;
    //float wanderTimer;
    Transform target;
   // float timer;
    #endregion

    bool isPerformingAction = false;

    bool isInHome = true;
    Vector3 startpos;
    Vector3 startRot;
    Vector3 lastpos;
    public Transform PostBubble;
    private void OnEnable()
    {
       // timer = wanderTimer;
        agent.enabled = false;
        //animator.runtimeAnimatorController = EmoteAnimationHandler.Instance.controller;
        animator.SetBool("IsGrounded", true);
        GetComponent<FaceIK>().ikActive = false;
        GetComponent<FootStaticIK>().ikActive = false;
        startpos = transform.position;
        startRot = transform.eulerAngles;
        if(storeCam!=null)
        storeCam.SetActive(false);
        if(worldCam!= null)
        worldCam.SetActive(true);
        if(worldObj!=null)
        worldObj.SetActive(true);
    }
    private void Start()
    {
        Wander();
    }
    void Update()
    {
        if (isMoving && isInHome)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude <= 0.3f)
                    {
                        agent.updateRotation = true;
                        isMoving = false;
                        agent.enabled = false;
                        animator.SetFloat("Blend", 0.0f);
                        animator.SetFloat("BlendY", 0.0f);
                        isPerformingAction = false;
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
    void Wander() {

        if (!isInHome)
        {
            return;
        }
        animator.SetBool("Action", false);
        animator.SetBool("isMoving", true);
       // Invoke(nameof(ForceActionFalse), 0.05f);
        agent.enabled = true;
        Vector3 newPos = RandomNavSphere(transform.position, Random.Range(minRadius, maxRadius), -1);
        agent.SetDestination(newPos);
        animator.SetBool("IsGrounded", true);
        animator.SetFloat("Blend", 0.25f);
        agent.speed = 0.9f;
        isMoving = true;
    }
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        if (isInHome)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;
            randDirection += origin;

            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
            return navHit.position;
        }
        else
        {
            return gameObject.transform.position;
        }
    }

    private void FaceTarget(Vector3 destination)
    {
        if (!isInHome)
        {
            return;
        }
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, walkRotateSpeed);
    }

    void perfomAnim() {
        animator.SetBool("isMoving", false);
        animator.SetBool("Action", true);
        Invoke(nameof(Wander), 8f);
    }
    /// <summary>
    /// To change player from Home to store and viceversa
    /// </summary>
   /* public void UpdateState(bool setToStore) {

        animator.SetFloat("Blend", 0.0f);
        if (setToStore) // set player avatar for store
        {
            animator.SetBool("Action", false);
            animator.SetBool("isMoving", false);
            isMoving = false;
            isInHome = false;
            GetComponent<NavMeshAgent>().enabled = false;
            print("STORE HOME CALL");
            animator.SetBool("idel", true);
            GetComponent<FaceIK>().ikActive = true;
            GetComponent<FootStaticIK>().ikActive = true;
            if (GameManager.Instance.defaultSelection != 0)
            {
                transform.position = startpos;
                transform.eulerAngles = startRot;
            }
            if(cineCam!=null)
            cineCam.SetActive(true);
            if(storeCam!=null)
            storeCam.SetActive(true);
            if(worldCam!=null)
            worldCam.SetActive(false);
            if(worldObj!=null)
            worldObj.SetActive(false);
            PostBubble.gameObject.SetActive(false);
        }
        else
        {
            if(cineCam!=null)
            cineCam.SetActive(false);
            if(storeCam!=null)
            storeCam.SetActive(false);
            if(worldCam!=null)
            worldCam.SetActive(true);
            if(worldObj!=null)
            worldObj.SetActive(true);
            isInHome = true;
            animator.SetBool("idel", false);
            transform.position = startpos;
            GetComponent<FaceIK>().ikActive = false;
            GetComponent<FootStaticIK>().ikActive = false;
            GetComponent<NavMeshAgent>().enabled = true;
            Wander();
            PostBubble.gameObject.SetActive(true);
        }
    }*/



    public void SetAvatarforAR()
    {
        animator.SetFloat("Blend", 0.0f);
        animator.SetBool("Action", false);
        animator.SetBool("isMoving", false);
        animator.SetBool("idel", true);
        gameObject.GetComponent<CharacterOnScreenNameHandler>().enabled=false;
        gameObject.GetComponent<Animator>().runtimeAnimatorController = ArAnimator;
    }
}
