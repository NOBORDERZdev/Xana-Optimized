using UnityEngine;

public class XanaDuneControllerHandler : MonoBehaviour
{
    private Animator animator;

    //Collider Properties
    private PhysicMaterial _maxFrictionPhysics;
    private CapsuleCollider _capsuleCollider;
    private Vector3 _colliderCenter;
    private float _colliderRadius;
    private float _colliderHeight;

    private void Start()
    {
        Enable_DisableObjects.Instance.DisableScreenRotaionButton();
    }

    public void AddComponentOn34()
    {
        ReferencesForGamePlay.instance.m_34player.AddComponent<SkatingConenctionErrorHandler>();
    }

    public void EnableSkating()
    {
        animator = ReferencesForGamePlay.instance.m_34player.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        animator.SetBool("IsEmote", false);
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
        EnableDisableUI(false);
    }
    public void DisableSkating()
    {
        if (_capsuleCollider)
        {
            _capsuleCollider.center = _colliderCenter;
            _capsuleCollider.radius = _colliderRadius;
            _capsuleCollider.height = _colliderHeight;
            _capsuleCollider.material = null;
        }

        EnableDisableUI(true);
    }

    private void EnableDisableUI(bool setActive)
    {
        Enable_DisableObjects.Instance.EnableDisableUIObjects(setActive);
    }


    //public void SwitchToSkatingController()
    //{
    //    if (_isSkatingControllerOn)
    //    {
    //        player.GetComponent<PlayerController>().enabled = false;
    //        player.GetComponent<CharacterController>().enabled = false;
    //        ReferencesForGamePlay.instance.m_34player.GetComponent<CharacterController>().enabled = false;
    //        foreach (CapsuleCollider child in ReferencesForGamePlay.instance.m_34player.GetComponents<CapsuleCollider>())
    //        {
    //            child.enabled = false;
    //        }
    //        _player34InitialPos = ReferencesForGamePlay.instance.m_34player.transform.localPosition;
    //        ReferencesForGamePlay.instance.m_34player.transform.localPosition = new Vector3(_player34InitialPos.x, 0.014f, _player34InitialPos.z);
    //        Rigidbody playerRb = player.AddComponent<Rigidbody>();
    //        playerRb.mass = 0.1f;
    //        playerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    //        board.GetComponent<FixedJoint>().connectedBody = playerRb;
    //        //player.AddComponent<XanaDuneControllerHandler>();
    //        player.GetComponent<XanaDuneControllerHandler>().EnableSkating();
    //        StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(-88f, 1f));
    //        PlayerCameraController.instance.lockRotation = true;
    //        PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Radius = 2.33f;
    //        PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Height = 2.57f;

    //    }
    //    else
    //    {
    //        player.GetComponent<XanaDuneControllerHandler>().DisableSkating();
    //        Destroy(player.GetComponent<Rigidbody>());
    //        foreach (CapsuleCollider child in ReferencesForGamePlay.instance.m_34player.GetComponents<CapsuleCollider>())
    //        {
    //            child.enabled = true;
    //        }
    //        ReferencesForGamePlay.instance.m_34player.transform.localPosition = _player34InitialPos;
    //        player.GetComponent<CharacterController>().enabled = true;
    //        ReferencesForGamePlay.instance.m_34player.GetComponent<CharacterController>().enabled = true;
    //        player.GetComponent<PlayerController>().enabled = true;
    //        PlayerCameraController.instance.lockRotation = false;
    //        PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Radius = 1.75f;
    //        PlayerCameraController.instance.gameObject.GetComponent<CinemachineFreeLook>().m_Orbits[0].m_Height = 2.47f;

    //    }
    //}
    public void TouchingCrab()
    {
        ReferencesForGamePlay.instance.spawnedSkateBoard.GetComponent<InputManager>().OnTouchingCrab();
    }

    public void TouchingFinish()
    {
        ReferencesForGamePlay.instance.spawnedSkateBoard.GetComponent<InputManager>().force = 500f;
        SandGameManager.Instance.GameOver();
    }

}
