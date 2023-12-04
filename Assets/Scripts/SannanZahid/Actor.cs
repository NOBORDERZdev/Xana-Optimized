using Photon.Voice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    ActorBehaviour.Behaviour _PlayerBehaviour;
    ActorBehaviour.Category _PlayerCategory;
    Animator _PlayerAnimator;
    [SerializeField]
    Queue<MoveBehaviour> _playerMoves = new Queue<MoveBehaviour>();
    int StateMoveBehaviour = 0;
    bool _moveFlag = true;
    float MoveSpeed = default;
    Transform MoveTarget;
    public float ActionClipTime = 0f;
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
        _PlayerBehaviour = playerBehaviour.BehaviourOfMood;
        _PlayerCategory = playerBehaviour.CategoryOfMode;
        foreach (MoveBehaviour move in playerBehaviour.ActorMoveBehaviours)
            SetMoveActions(move);
        MoveTarget = GameManager.Instance.avatarPathSystemManager.GetAvatarSpawnPoint();
        transform.position = MoveTarget.position;
        StartCoroutine(StartActorBehaviour());
    }
    public void SetNewBehaviour(ActorBehaviour playerBehaviour)
    {
        ClearMoves();
        _PlayerBehaviour = playerBehaviour.BehaviourOfMood;
        _PlayerCategory = playerBehaviour.CategoryOfMode;
        foreach (MoveBehaviour move in playerBehaviour.ActorMoveBehaviours)
            SetMoveActions(move);
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
                    MoveTarget = GameManager.Instance.avatarPathSystemManager.GetNextPoint(move.behaviour, MoveTarget);
                    MoveSpeed = move.Speed;
                    transform.position = Vector3.MoveTowards(transform.position, MoveTarget.position, MoveSpeed * Time.deltaTime);
                    _playerMoves.Enqueue(move);
                    _PlayerAnimator.SetBool("Action", false);
                    _PlayerAnimator.SetBool("IdleMenu", false);
                }
                break;
            case 1:
                    transform.position = Vector3.MoveTowards(transform.position, MoveTarget.position, MoveSpeed * Time.deltaTime);
                    transform.LookAt(MoveTarget);
                    if (Vector3.Distance(transform.position, MoveTarget.position) < 0.001f)
                    {
                        MoveBehaviour move = _playerMoves.Dequeue();
                    Debug.LogError("Behaviour ---> " + move.behaviour.ToString());
                        if (move.behaviour == MoveBehaviour.Behaviour.Action)
                        {
                            StateMoveBehaviour = 2;
                            _PlayerAnimator.SetBool("Action", true);
                            _PlayerAnimator.SetBool("IdleMenu", false);
                            _playerMoves.Enqueue(move);
                    }
                        else
                        {
                            MoveTarget = GameManager.Instance.avatarPathSystemManager.GetNextPoint(move.behaviour, MoveTarget);
                            _playerMoves.Enqueue(move);
                            MoveSpeed = move.Speed;
                        }
                    }
                break;
            case 2:
                Debug.LogError("Action ---> "+ ActionClipTime);
                yield return new WaitForSeconds(ActionClipTime*2f);
                _PlayerAnimator.SetBool("Action", false);
                StateMoveBehaviour = 1;
                break;
        }
        StartCoroutine(StartActorBehaviour());
    }
    public void IdlePlayerAvatorForMenu(bool flag)
    {
        Debug.LogError(" IdlePlayerAvatorForMenu -----> " + flag);
        if(flag)
        {
            _PlayerAnimator.SetBool("IdleMenu", flag) ;
            _moveFlag = false;
            StateMoveBehaviour = 0;
        }
        else
        {
            _PlayerAnimator.SetBool("IdleMenu", flag);
            _moveFlag = true;
        }
        this.GetComponent<FaceIK>().enabled = !flag;
        this.GetComponent<FootStaticIK>().enabled = !flag;
     //   GetComponent<FaceIK>().ikActive = flag;
     //   GetComponent<FootStaticIK>().ikActive = flag;
    }
    public void ViewMoodMenu()
    {

    }
}