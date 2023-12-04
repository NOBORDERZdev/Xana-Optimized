using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    [NonReorderable]
    [SerializeField]
    public List<ActorBehaviour> actorBehaviour;
    [SerializeField] GameObject _worldObj;
    [SerializeField] GameObject _storeCam;
    [SerializeField] GameObject _worldCam;
    [SerializeField] GameObject _cinemaCam;
    [SerializeField] Transform _menuViewPoint;
    Vector3 _previousPos, _previousRot;
    private void Awake()
    {
        _storeCam.SetActive(false);
        _worldCam.SetActive(true);
        _worldObj.SetActive(true);
    }
    void Start()
    {
        GameManager.Instance.mainCharacter.GetComponent<Actor>().Init(actorBehaviour[0]);
    }
    public void SetMood(string mood)
    {
        ActorBehaviour actor = actorBehaviour.Find(x => x.Name == mood);
        Debug.LogError("Actor Behaviour === " + actor.Name);
        GameManager.Instance.mainCharacter.GetComponent<Actor>().Init(actor);
    }
    public void IdlePlayerAvatorForMenu(bool flag)
    {
        GameManager.Instance.mainCharacter.GetComponent<Actor>().IdlePlayerAvatorForMenu(flag);
        Debug.LogError(" IdlePlayerAvatorForMenu -----> " + flag);
        if(flag)
        {
            _cinemaCam.SetActive(true);
            _storeCam.SetActive(true);
            _worldCam.SetActive(false);
            _worldObj.SetActive(false);
            _previousPos = GameManager.Instance.mainCharacter.transform.position;
            _previousRot = GameManager.Instance.mainCharacter.transform.eulerAngles;
            GameManager.Instance.mainCharacter.transform.position = _menuViewPoint.position;
            GameManager.Instance.mainCharacter.transform.eulerAngles = _menuViewPoint.eulerAngles;
            Debug.LogError(" Space Player on Menu Point -----> " + _menuViewPoint.position);
        }
        else
        {
            _cinemaCam.SetActive(false);
            _storeCam.SetActive(false);
            _worldCam.SetActive(true);
            _worldObj.SetActive(true);
            GameManager.Instance.mainCharacter.transform.position = _previousPos;
            GameManager.Instance.mainCharacter.transform.eulerAngles = _previousRot;
        }
    }
    public int  GetNumberofIdleAnimations(string name)
    {
        ActorBehaviour actor = actorBehaviour.Find(x => x.Name == name);
        return actor.NumberOfIdleAnimations;
    }
}
[Serializable]
public class ActorBehaviour
{
    public string Name;
    public enum Category
    { 
        Fun,
        Down, 
        Flat, 
        Up,
        Work,
        Hobby
    };
    public enum Behaviour
    { 
        Jumping,
        Stretching, 
        Bouncing, 
        Praying, 
        Sad,
        Crying,
        Angry,
        Frustration,
        Falling,
        Knocking,
        Laying,
        Happy,
        SillyDance,
        Excited,
        Joy,
        Celebrate,
        OfficeWorking,
        SendingFax,
        Typing,
        Exercise,
        Meditate,
        Dancing,
        Getting,
        Landing,
        Scared,
        Spin,
        Study,
        Meeting,
        Singing,
        Gaming
    };
    public Category CategoryOfMode;
    public Behaviour BehaviourOfMood;
    public int NumberOfIdleAnimations = 1;
    public bool IdleAnimationFlag = false;
    [NonReorderable]
    public List<MoveBehaviour> ActorMoveBehaviours;
   // public List<string> Idle, Move = new List<string>();
}
