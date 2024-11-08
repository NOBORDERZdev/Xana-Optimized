using System.Collections;
using System.Collections.Generic;
using UFE3D;
using UnityEngine;


public class SwimmingController : MonoBehaviour
{
   public PlayerController _playerController;
   private Coroutine swimCoroutine;
   [SerializeField]
   private GameObject[] objectsToEnableDisable;
   public GameObject swimJumpButtom;
   private bool isInPool = false;
    private void Start()
    {
        if (gameObject.TryGetComponent(out PlayerController playerController))
        {
            _playerController = playerController;
           
        }
        else
        {
            Debug.LogError("PlayerController not found");
        }
       
    }
    public void StartSwimming() 
    {
        UpdateUIelement(false);
        isInPool = true;
        swimJumpButtom.SetActive(true);
        _playerController.animator.SetBool("IsSwimIdle", true);
        if (swimCoroutine == null)
        {
            swimCoroutine = StartCoroutine(SwimmingState()); 
        } 
    }
    public void StopSwimming()
    {
        isInPool = false;
        UpdateUIelement(true);
        swimJumpButtom.SetActive(false);
        _playerController.animator.SetBool("IsSwimIdle", false);
        _playerController.animator.SetBool("IsSwimming", false);
        if (swimCoroutine != null)
        {
            StopCoroutine(swimCoroutine);
            swimCoroutine = null;
        }
    }
    private IEnumerator SwimmingState()
    {
        bool isMoving;
        while (true)
        {
            if (_playerController.desiredMoveDirection != Vector3.zero || _playerController.desiredMoveDirectionFPP != Vector3.zero)
            { isMoving = true; }
            else { 
            isMoving = false;
            }
            _playerController.animator.SetBool("IsSwimming", isMoving);
            yield return null;
        }
    }
    public void UpdateUIelement(bool enable)
    {
        objectsToEnableDisable?.SetActive(enable);
    }
    public void Jump() {
        if (_playerController.animator.GetBool("IsSwimming") || _playerController.animator.GetBool("IsSwimIdle"))
        {
            _playerController.animator.SetBool("IsSwimIdle", false);
            _playerController.animator.SetBool("IsSwimming", false);
            _playerController.Jump();
           
            StartCoroutine(CheckJumpAnimationComplete());
        }
    }
    private IEnumerator CheckJumpAnimationComplete() 
    {
        AnimatorStateInfo stateInfo = _playerController.animator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.IsName("Jump") && stateInfo.normalizedTime < 1.0f)
        { 
            yield return null;
            stateInfo = _playerController.animator.GetCurrentAnimatorStateInfo(0);
        }
        Invoke("OnJumpAnimationComplete",2.0f); 
    }
    private void OnJumpAnimationComplete()
    { // Perform actions after the jump animation is complete
      
        //Debug.Log("Jump animation completed!");
        if (isInPool) 
        {
            _playerController.animator.SetBool("IsSwimIdle", true);
        }
    }

}