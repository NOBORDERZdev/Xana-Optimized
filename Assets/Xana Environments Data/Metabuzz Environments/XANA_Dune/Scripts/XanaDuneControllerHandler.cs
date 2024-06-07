using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XanaDuneControllerHandler : MonoBehaviour
{
    public Transform Player34;

    [SerializeField]
    private GameObject _skateBoardPrefab;

    [SerializeField]
    private GameObject _markPrefab;

    [HideInInspector]
    public GameObject _spawnedSkateBoard;

    [HideInInspector]
    public GameObject _spawnedMarkObject;

    private Animator animator;

    //Collider Properties
    private PhysicMaterial _maxFrictionPhysics;
    private CapsuleCollider _capsuleCollider;
    private Vector3 _colliderCenter;
    private float _colliderRadius;
    private float _colliderHeight;

    private void Start()
    {
        Player34 = ReferencesForGamePlay.instance.m_34player.transform;
        _skateBoardPrefab = Resources.Load("Game_Board") as GameObject;
        _markPrefab = Resources.Load("Mark") as GameObject;
        _spawnedSkateBoard = Instantiate(_skateBoardPrefab, ReferencesForGamePlay.instance.MainPlayerParent.transform, false);
        _spawnedMarkObject = Instantiate(_markPrefab, Player34, false);
    }

    public void EnableSkating()
    {
        animator = ReferencesForGamePlay.instance.m_34player.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        // prevents the collider from slipping on ramps
        _maxFrictionPhysics = new PhysicMaterial();
        _maxFrictionPhysics.name = "_maxFrictionPhysics";
        _maxFrictionPhysics.staticFriction = 1f;
        _maxFrictionPhysics.dynamicFriction = 1f;
        _maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;


        // capsule collider info
        _capsuleCollider = GetComponent<CapsuleCollider>();

        // save your collider preferences 
        _colliderCenter = _capsuleCollider.center;
        _colliderRadius = _capsuleCollider.radius;
        _colliderHeight = _capsuleCollider.height;
        _capsuleCollider.center = new Vector3(0, 0.75f, 0);
        _capsuleCollider.radius = 0.29f;
        _capsuleCollider.height = 1.5f;

        _capsuleCollider.material = _maxFrictionPhysics;

    }
    public void DisableSkating()
    {
        _capsuleCollider.center = _colliderCenter;
        _capsuleCollider.radius = _colliderRadius;
        _capsuleCollider.height = _colliderHeight;
        _capsuleCollider.material = null;

    }
    public void TouchingCrab()
    {
        _spawnedSkateBoard.GetComponent<InputManager>().OnTouchingCrab();
    }

    public void TouchingFinish()
    {
        _spawnedSkateBoard.GetComponent<InputManager>().force = 500f;
        SandGameManager.Instance.GameOver();
    }

}
