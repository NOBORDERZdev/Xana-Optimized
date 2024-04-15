using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    [NonReorderable]
    [SerializeField]
    public List<ActorBehaviour> actorBehaviour;
    [SerializeField] GameObject _worldObj;
    [SerializeField] GameObject _storeCam;
    [SerializeField] GameObject _worldCam;
    [SerializeField] public GameObject _cinemaCam;
    [SerializeField] public Transform _menuViewPoint, _postViewPoint;
    Vector3 _previousPos, _previousRot;
    private void Awake()
    {
        _storeCam.SetActive(false);
        _worldCam.SetActive(true);
        _worldObj.SetActive(true);
    }
    void Start()
    {
        //Transform defaultPoint = GameManager.Instance.avatarPathSystemManager.GetGridCenterPoint();
        //GameManager.Instance.mainCharacter.GetComponent<AvatarControllerHome>().worldCam.GetComponent<HomeCameraController>().ViewPlayer();
        //_previousPos = defaultPoint.position;
       // _previousRot = defaultPoint.eulerAngles;
        
        //GameManager.Instance.mainCharacter.GetComponent<Actor>().Init(actorBehaviour[GetPostRandomDefaultAnim()],defaultPoint);
    }
    public void Init()
    {
        Transform defaultPoint = GameManager.Instance.avatarPathSystemManager.GetGridCenterPoint(); 
        _previousPos = defaultPoint.position;
        _previousRot = defaultPoint.eulerAngles;
        GameManager.Instance.mainCharacter.GetComponent<Actor>().Init(actorBehaviour[GetPostRandomDefaultAnim()], defaultPoint);
    }
    public void SetMood(string mood)
    {
        ActorBehaviour actor = actorBehaviour.Find(x => x.Name == mood);
        GameManager.Instance.mainCharacter.GetComponent<Actor>().Init(actor);
    }
    public void IdlePlayerAvatorForMenu(bool flag)
    {
        GameManager.Instance.mainCharacter.GetComponent<Actor>().IdlePlayerAvatorForMenu(flag,false);
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(!flag);
        if (flag)
        {
            _cinemaCam.SetActive(true);
            _storeCam.SetActive(true);
            _worldCam.SetActive(false);
            _worldObj.SetActive(false);
            _previousPos = GameManager.Instance.mainCharacter.transform.position;
            _previousRot = GameManager.Instance.mainCharacter.transform.eulerAngles;
            GameManager.Instance.mainCharacter.transform.position = _menuViewPoint.position;
            GameManager.Instance.mainCharacter.transform.eulerAngles = _menuViewPoint.eulerAngles;
        }
        else
        {
            _cinemaCam.SetActive(false);
            _storeCam.SetActive(false);
            _worldCam.SetActive(true);
            _worldObj.SetActive(true);
            GameManager.Instance.mainCharacter.transform.position = GameManager.Instance.mainCharacter.GetComponent<Actor>().LastMoveToPosition();//new Vector3(3f, _previousPos.y, _previousPos.z);
            GameManager.Instance.mainCharacter.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }
    public void IdlePlayerAvatorForPostMenu(bool flag)
    {
       
        GameManager.Instance.mainCharacter.GetComponent<Actor>().IdlePlayerAvatorForMenu(flag,true);
        GameManager.Instance.FriendsHomeManager.GetComponent<FriendHomeManager>().EnableFriendsView(!flag);
        if (flag)
        {
            _cinemaCam.SetActive(true);
            _storeCam.SetActive(true);
            _worldCam.SetActive(false);
            _worldObj.SetActive(false);
            _previousPos = GameManager.Instance.mainCharacter.transform.position;
            _previousRot = GameManager.Instance.mainCharacter.transform.eulerAngles;
            GameManager.Instance.mainCharacter.transform.position = _postViewPoint.position;
            GameManager.Instance.mainCharacter.transform.eulerAngles = _postViewPoint.eulerAngles;
        }
        else
        {
            _cinemaCam.SetActive(false);
            _storeCam.SetActive(false);
            _worldCam.SetActive(true);
            _worldObj.SetActive(true);
            GameManager.Instance.mainCharacter.transform.position = GameManager.Instance.mainCharacter.GetComponent<Actor>().LastMoveToPosition(); ;
            GameManager.Instance.mainCharacter.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }
    public int  GetNumberofIdleAnimations(string name)
    {
        ActorBehaviour actor = actorBehaviour.Find(x => x.Name == name);
        return actor.NumberOfIdleAnimations;
    }

    int GetPostRandomDefaultAnim(){ 
        float _rand = UnityEngine.Random.Range(0.1f, 3.0f);
        int value; 
        if (_rand <= 1.0f)
        {
            value= 0;
        }
        else if (_rand >= 1.0f && _rand <= 2.0f)
        {
            value= 1;
        }
        else
        {
            value= 2;
        }
        return value;
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
}
