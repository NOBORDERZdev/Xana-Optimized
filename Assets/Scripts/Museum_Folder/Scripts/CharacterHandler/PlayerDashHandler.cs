using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDashHandler : MonoBehaviour
{
    PlayerController _playerController;

    [SerializeField]
    private float _dashSpeed = 10f;
    [SerializeField]
    private float _dashTime = 0.5f;
    [SerializeField]
    private float _fovIncreasingSpeed = 1.65f;
    [SerializeField]
    private float _fovDecreasingSpeed = 0.7f;
    [SerializeField]
    private Image _dashBtnImageLandscape;
    [SerializeField]
    private Image _dashBtnImagePortrait;
    [SerializeField]
    private GameObject _dashEffect;
    [SerializeField]
    private bool _canDash = true;
    [SerializeField]
    private Color _buttonDisableColor;
    [SerializeField]
    private AudioSource _audioSource;

    private float _sprintTime = 4f;
    private float _actualSpringSpeed;
    public bool IsPlayerInOtherState
    {
        get
        {
            if (!_playerController.animator.IsInTransition(0) && (_playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("NormalStatus") || _playerController.animator.GetCurrentAnimatorStateInfo(0).IsName("Dwarf Idle")))
            {
                return false;
            }
            else
                return true;
        }
    }
    private void OnEnable()
    {
        _audioSource = _dashEffect.GetComponent<AudioSource>();
        _audioSource.volume = SoundSettings.soundManagerSettings.totalVolumeSlider.value;
        BuilderEventManager.BGMVolume += BGMVolume;
    }

    private void OnDisable()
    {
        BuilderEventManager.BGMVolume -= BGMVolume;
    }

    void BGMVolume(float volume)
    {
        _audioSource.volume = volume;
    }
    private void Start()
    {
        if (gameObject.TryGetComponent(out PlayerController playerController))
        {
            _playerController = playerController;
            _actualSpringSpeed = _playerController.sprintSpeed;
        }
        else
        {
            Debug.LogError("PlayerController not found");
        }
        _canDash = true;
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Tab))
    //    {
    //        StartCoroutine(DashRoutine());
    //    }
    //}

    public void DashButton()
    {
        if (_playerController == null || !_canDash || IsPlayerInOtherState || _playerController.animator.GetFloat("Blend") == 0)
            return;

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        EnableDashButton(false);

        float startTime = Time.time;
        float endTime = startTime + _dashTime;

        if (_dashEffect)
            _dashEffect.SetActive(true);  // Enable speed lines effect on Camera

        if (_playerController.animator)
           _playerController.animator.SetBool("IsDashing", true);

        while (Time.time < endTime)
        {
            _playerController.cinemachineFreeLook.m_Lens.FieldOfView += _fovIncreasingSpeed;
            _playerController.characterController.Move(_dashSpeed * Time.deltaTime * transform.forward);
            yield return null;
        }

        if (_playerController.animator)
            _playerController.animator.SetBool("IsDashing", false);

        if (_playerController.desiredMoveDirection != Vector3.zero || _playerController.desiredMoveDirectionFPP != Vector3.zero)
        {
            StartCoroutine(DashEndRoutine());
        }
        else
        {
            StartCoroutine(IdleDashEndRoutine());
        }
    }

    private IEnumerator DashEndRoutine()
    {

        _playerController.sprintSpeed = 16f;
        while (_playerController.cinemachineFreeLook.m_Lens.FieldOfView >= 60f)
        {
            _playerController.cinemachineFreeLook.m_Lens.FieldOfView -= _fovDecreasingSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(0.32f);
        if (_dashEffect)
            _dashEffect.SetActive(false);
        yield return new WaitForSeconds(3f);
        _playerController.sprintSpeed = _actualSpringSpeed;
        EnableDashButton(true);

    }
    private IEnumerator IdleDashEndRoutine()
    {
        while (_playerController.cinemachineFreeLook.m_Lens.FieldOfView >= 60f)
        {
            _playerController.cinemachineFreeLook.m_Lens.FieldOfView -= _fovDecreasingSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(0.32f);
        if (_dashEffect)
            _dashEffect.SetActive(false);

        EnableDashButton(true);
    }

    private void EnableDashButton(bool isEnable)
    {
        if (isEnable)
        {
            _canDash = true;
            _dashBtnImageLandscape.color = new Color(1, 1, 1, 1);
            _dashBtnImagePortrait.color = new Color(1, 1, 1, 1);
        }
        else
        {
            _canDash = false;
            _dashBtnImageLandscape.color = _buttonDisableColor;
            _dashBtnImagePortrait.color = _buttonDisableColor;
        }
    }

    //not in use
    #region Sprint

    //public void SprintButton()
    //{
    //    if (_playerController == null || _playerController.characterController.velocity.y != 0f || _playerController.desiredMoveDirection == Vector3.zero || !_canDash)
    //        return;
    //    StartCoroutine(SprintRoutine());
    //}

    //private IEnumerator SprintRoutine()
    //{
    //    EnableDashButton(false);
    //    if (_dashEffect)
    //        _dashEffect.SetActive(true);  // Enable speed lines effect on Camera

    //    _playerController.sprintSpeed = 16f;
    //    StartCoroutine(SprintEndRoutine());

    //    while (_playerController.cinemachineFreeLook.m_Lens.FieldOfView < 70)
    //    {
    //        _playerController.cinemachineFreeLook.m_Lens.FieldOfView += _fovIncreasingSpeed;
    //        yield return null;
    //    }
    //}

    //private IEnumerator SprintEndRoutine()
    //{
    //    yield return new WaitForSeconds(_sprintTime);
    //    if (_dashEffect)
    //        _dashEffect.SetActive(false);
    //    _playerController.sprintSpeed = _actualSpringSpeed;
    //    while (_playerController.cinemachineFreeLook.m_Lens.FieldOfView >= 60f)
    //    {
    //        _playerController.cinemachineFreeLook.m_Lens.FieldOfView -= _fovDecreasingSpeed;
    //        yield return null;
    //    }
    //    EnableDashButton(true);

    //}
    #endregion 

}
