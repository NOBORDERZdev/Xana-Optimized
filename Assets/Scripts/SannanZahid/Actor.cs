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
    public Transform NameTagHolderObj;
    bool _startCoroutineFLag = false;
    public AnimatorOverrideController overrideController;
    bool _lastAction = false;

    private void OnEnable()
    {
        if(_startCoroutineFLag)
        {
           // Debug.LogError("OnEnable  --- "+ _lastAction);
            _PlayerAnimator.SetBool("Action", _lastAction);
            StartCoroutine(StartActorBehaviour());
        }
    }
    private void OnDisable()
    {
        if (_startCoroutineFLag)
        {
          //  Debug.LogError("OnDisable  --- " + _lastAction);

            StopCoroutine(StartActorBehaviour());
        }
    }
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
        _startCoroutineFLag = true;
        _PlayerAnimator = this.transform.GetComponent<Animator>();
         overrideController = new AnimatorOverrideController(_PlayerAnimator.runtimeAnimatorController);
        _PlayerAnimator.runtimeAnimatorController = overrideController;
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

        if(playerBehaviour.IdleAnimationFlag)
        {
            _PlayerAnimator.SetBool("Action", true);
            _lastAction = true;

            StateMoveBehaviour = 2;

        }
        else
        {
            _PlayerAnimator.SetBool("Action", false);
            _lastAction = false;

            StateMoveBehaviour = 1;

        }
        _PlayerAnimator.SetBool("Menu Action", false);
        //StopAllCoroutines();
        //_moveFlag = true;
       ///----- StateMoveBehaviour = 1;
        //StartCoroutine(StartActorBehaviour());

    }
    IEnumerator StartActorBehaviour()
    {
        CheckedInLoop:
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
                    _lastAction = false;
                    _PlayerAnimator.SetBool("IdleMenu", false);
                }
                break;
            case 1:
                    transform.position = Vector3.MoveTowards(transform.position, MoveTarget.position, MoveSpeed * Time.deltaTime);
                    transform.LookAt(MoveTarget);
                    if (Vector3.Distance(transform.position, MoveTarget.position) < 0.001f)
                    {
                        MoveBehaviour move = _playerMoves.Dequeue();
                        if (move.behaviour == MoveBehaviour.Behaviour.Action)
                        {
                            StateMoveBehaviour = 2;
                            _PlayerAnimator.SetBool("Action", true);
                            _lastAction = true;
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
                yield return new WaitForSeconds(ActionClipTime*2f);
                if(!_moveFlag)
                {
                    StateMoveBehaviour = 0;
                }
                else
                {
                    if (_playerMoves.Peek().behaviour != MoveBehaviour.Behaviour.Action)
                    {
                        _PlayerAnimator.SetBool("Action", false);
                        _lastAction = false;
                    }
                    StateMoveBehaviour = 1;
                }
                break;
        }
        goto CheckedInLoop;
    }
    public void IdlePlayerAvatorForMenu(bool flag,bool MAction)
    {
       // Debug.LogError("IdlePlayerAvatorForMenu  --- " + flag);
        if(flag)
        {
            _PlayerAnimator.SetBool("IdleMenu", flag) ;
            _PlayerAnimator.SetBool("Menu Action", MAction);
            _moveFlag = false;
            StateMoveBehaviour = 0;
        }
        else
        {
            _PlayerAnimator.SetBool("IdleMenu", flag);
            _PlayerAnimator.SetBool("Menu Action", false);
            StateMoveBehaviour = 1;
            _moveFlag = true;
        }
        NameTagHolderObj.gameObject.SetActive(!flag);
        this.GetComponent<FaceIK>().enabled = !flag;
        this.GetComponent<FootStaticIK>().enabled = !flag;
    }
}