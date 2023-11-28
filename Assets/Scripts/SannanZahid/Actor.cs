using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    ActorBehaviour.Behaviour _PlayerBehaviour;
    Animator _PlayerAnimator;
    [SerializeField]
    Queue<MoveBehaviour> _playerMoves= new Queue<MoveBehaviour>();
    int StateMoveBehaviour = 0;
    bool _moveFlag = true;
    float MoveSpeed = default;
    Transform MoveTarget;
    void SetMoveActions(MoveBehaviour move)
    {
        _playerMoves.Enqueue(move);
    }
    void ClearMoves()
    {
        _playerMoves.Clear();
    }
    public void Init(ActorBehaviour playerBehaviour)
    {
        _PlayerAnimator = GetComponent<Animator>();
        _PlayerBehaviour = playerBehaviour.behaviour;
        foreach(MoveBehaviour move in playerBehaviour.ActorMoveBehaviours)
            SetMoveActions(move);
        MoveTarget = GameManager.Instance.avatarPathSystemManager.GetAvatarSpawnPoint();
        Debug.LogError("Spawn Point === " + MoveTarget.name);
        transform.position = MoveTarget.position;
        StartCoroutine(StartActorBehaviour());
    }
    IEnumerator StartActorBehaviour()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        switch(StateMoveBehaviour)
        {
            case 0:
                if(_moveFlag)
                {
                    MoveBehaviour move = _playerMoves.Dequeue();
                    StateMoveBehaviour = 1;
                    Debug.LogError("Move Behaviour === " + move.behaviour.ToString());
                    MoveTarget = GameManager.Instance.avatarPathSystemManager.GetNextPoint(move.behaviour, MoveTarget);
                    Debug.LogError("Next Move === " + MoveTarget.name);
                    MoveSpeed = move.Speed;
                    transform.position = Vector3.MoveTowards(transform.position, MoveTarget.position, MoveSpeed * Time.deltaTime);
                    _playerMoves.Enqueue(move);
                    _PlayerAnimator.SetFloat("Blend", 0.25f);
                    _PlayerAnimator.SetBool("isMoving", true);
                }
                break;
            case 1:

                transform.position = Vector3.MoveTowards(transform.position, MoveTarget.position, MoveSpeed * Time.deltaTime);
                transform.LookAt(MoveTarget);
                if (Vector3.Distance(transform.position, MoveTarget.position) < 0.001f)
                {
                    MoveBehaviour move = _playerMoves.Dequeue();
                    if(move.behaviour == MoveBehaviour.Behaviour.Action)
                    {
                        StateMoveBehaviour = 2;
                    }
                    else
                    {
                        MoveTarget = GameManager.Instance.avatarPathSystemManager.GetNextPoint(move.behaviour, MoveTarget);
                        _playerMoves.Enqueue(move);
                        Debug.LogError("Next Move === " + MoveTarget.name);
                        MoveSpeed = move.Speed;
                    }
                }
                break;
            case 2:
                break;
        }
        StartCoroutine(StartActorBehaviour());
    }
}

