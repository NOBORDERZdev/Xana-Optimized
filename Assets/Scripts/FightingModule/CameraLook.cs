
using UnityEngine;
using Cinemachine;
using System.Collections;

namespace FightingModule
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CameraLook : MonoBehaviour
    {

        #region Vars
        [SerializeField] CinemachineFreeLook cinemachine;
        private Movement _playerMovement;
        [SerializeField] private float lookSpeed = 1f;
        bool canMove = true;

        CinemachineBasicMultiChannelPerlin m_ChannelPerlin;
        #endregion

        #region Unity Funcs
        /// <summary>
        /// m_ChannelPerlin is used for getting Camera middle rig CinemachineBasicMultiChannelPerlin where we sets it's 6D shake property
        /// amplitude and frequency for our camera shake effect
        /// </summary>

        private void Awake()
        {
            _playerMovement = new Movement();
            m_ChannelPerlin = cinemachine.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        private void OnEnable()
        {
            _playerMovement.Enable();
        }
        private void OnDisable()
        {
            _playerMovement.Disable();
        }


        // Update is called once per frame
        void Update()
        {
            if (canMove)
            {
                Vector2 delta = _playerMovement.PlayerMain.LookAround.ReadValue<Vector2>();
                if (cinemachine.m_YAxis.Value < 0.38f || cinemachine.m_YAxis.Value > 0.8f)
                {
                    cinemachine.m_YAxis.Value = Mathf.Clamp(cinemachine.m_YAxis.Value, 0.38f, 0.8f);
                }
                cinemachine.m_XAxis.Value += delta.x * 200 * lookSpeed * Time.deltaTime;
                cinemachine.m_YAxis.Value += delta.y * lookSpeed * Time.deltaTime;
            }
        }
        #endregion

        #region UserFuncs
        /// <summary>
        /// Set if player can rotate and set camera or not
        /// </summary>

        public void lockCameraFreeLock()
        {
            if (canMove)
            {
                canMove = false;
            }
            else
            {
                canMove = true;
            }
        }
        /// <summary>
        /// Camera Shake Effect
        /// </summary>

        public void ShakeCamera()
        {
            m_ChannelPerlin.m_AmplitudeGain = 2;
            m_ChannelPerlin.m_FrequencyGain = 1f;
            StartCoroutine(SetCollisionEffectToNormal());
        }
        IEnumerator SetCollisionEffectToNormal()
        {
            yield return new WaitForSeconds(0.1f);
            m_ChannelPerlin.m_AmplitudeGain = 0;
            m_ChannelPerlin.m_FrequencyGain = 0;
        }
        #endregion
    }
}