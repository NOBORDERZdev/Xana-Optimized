using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace RFM.Character
{
    public class NPCEscapee : MonoBehaviour
    {
        private string _nickName = "Player";
        private int _money;
        
        [SerializeField] private Animator animator;
        [SerializeField] private string velocityNameX, velocityNameY;

        private NavMeshAgent _navMeshAgent;
        private float _maxSpeed;
        private Transform _closestHunterTransform = null;
        private float _minDistance = 10f;


        public float minDistanceToStartRunning = 10f;
        public List<Transform> huntersTransforms = new();

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        private void OnEnable()
        {
            EventsManager.onGameTimeup += GameOver;
        }
        
        private void OnDisable()
        {
            EventsManager.onGameTimeup -= GameOver;
        }

        private void Start()
        {
            _nickName = $"Player{GetComponent<PhotonView>().ViewID}";
            InvokeRepeating(nameof(AddMoney),
                RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.GainingMoneyTimeInterval,
                RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.GainingMoneyTimeInterval);
            
            _maxSpeed = _navMeshAgent.speed;
            InvokeRepeating(nameof(EscapeFromHunters), 1, 0.2f);
        }
        
        private void AddMoney()
        {
            _money += RFM.Managers.RFMManager/*.Instance*/.CurrentGameConfiguration.MoneyPerInterval;
        }

        private void UpdateHuntersTransformList() 
        {
            var hunterObjects = new List<GameObject>(
                GameObject.FindGameObjectsWithTag(Globals.HUNTER_NPC_TAG));
            huntersTransforms.Clear();
            foreach (GameObject hunterObj in hunterObjects) 
            {
                huntersTransforms.Add(hunterObj.transform);
            }
        }

        private void Update()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector2 velocityDir = new Vector2(velocity.x, velocity.z);
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
            float angle = Vector2.SignedAngle(forward, velocityDir);
            float xVal = Mathf.Cos((angle - 90 / 180) * Mathf.Deg2Rad);
            float yVal = Mathf.Cos(angle * Mathf.Deg2Rad);
            float speed = velocity.magnitude;

            var animVector = new Vector2(xVal, yVal) * speed / _maxSpeed;

            animator.SetFloat(velocityNameX, animVector.x);
            animator.SetFloat(velocityNameY, animVector.y);
        }

        private void EscapeFromHunters()
        {
            UpdateHuntersTransformList();

            foreach (var t in huntersTransforms)
            {
                float distance = Vector3.Distance(this.transform.position, t.position);
                if (distance < _minDistance)
                {
                    _minDistance = distance;
                    _closestHunterTransform = t;
                }
            }

            if (huntersTransforms.Count > 0 && _minDistance < minDistanceToStartRunning &&
                _closestHunterTransform != null)
            {
                Vector3 dirToSelf = transform.position - _closestHunterTransform.position;
                Vector3 newPost = transform.position + dirToSelf;
                _navMeshAgent.SetDestination(newPost);
            }
        }
        
        public void AIEscapeeCaught()
        {
            CancelInvoke(nameof(AddMoney));
            RFM.Managers.RFMUIManager.Instance.EscapeeCaught(_nickName, _money);
            PhotonNetwork.Destroy(this.gameObject);
        }
        
        private void GameOver()
        {
            AIEscapeeCaught();
        }

    }
}
