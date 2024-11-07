using System.Collections;
using System.Collections.Generic;
using UFE3D;
using UnityEngine;


public class SwimmingController : MonoBehaviour
{
   PlayerController _playerController;
   private Coroutine swimCoroutine;


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
        _playerController.animator.SetBool("IsSwimIdle", true);
        if (swimCoroutine == null)
        {
            swimCoroutine = StartCoroutine(SwimmingState()); 
        } 
    }
    public void StopSwimming()
    {
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
}