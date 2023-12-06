using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace RFM.Character
{
    public class NPCRunner : Runner
    {
        public string nickName = "Player";
        public int money;
        public float timeSurvived;
        
        [SerializeField] private Animator animator;
        [SerializeField] private string velocityNameX, velocityNameY;

        private NavMeshAgent _navMeshAgent;
        private float _maxSpeed;
        private Transform _closestHunterTransform = null;
        private float _minDistance = 10f;

        public float minDistanceToStartRunning = 10f;
        public List<Transform> huntersTransforms = new();

        private Vector3 _targetPosition;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        private void OnEnable()
        {
            EventsManager.onTakePositionTimeStart += TakePositionTimeStarted;
            EventsManager.onGameStart += OnGameStarted;
            EventsManager.onGameTimeup += GameOver;

            PhotonNetwork.NetworkingClient.EventReceived += ReceivePhotonEvents;
        }
        
        private void OnDisable()
        {
            EventsManager.onTakePositionTimeStart -= TakePositionTimeStarted;
            EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onGameTimeup -= GameOver;

            PhotonNetwork.NetworkingClient.EventReceived -= ReceivePhotonEvents;
        }

        private void Start()
        {
            nickName = $"Player{GetComponent<PhotonView>().ViewID}";
            _maxSpeed = _navMeshAgent.speed;
        }


        private void TakePositionTimeStarted()
        {
            if (PhotonNetwork.IsMasterClient) // Only master controls the movement 
            {
                InvokeRepeating(nameof(EscapeFromHunters), 1, 0.2f);
            }
        }


        private void OnGameStarted()
        {
            StartCoroutine(AddMoney());
            StartCoroutine(TimeSurvived());
        }

        IEnumerator TimeSurvived()
        {
            timeSurvived = 0;
            while (true)
            {
                timeSurvived += 1;
                yield return new WaitForSecondsRealtime(1);
            }

        }

        private IEnumerator AddMoney()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(
                    RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval);
                money += RFM.Managers.RFMManager.CurrentGameConfiguration.MoneyPerInterval;
            }
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

            else // When no hunters are close, move randomly
            {
                Vector3 newPost = transform.position + transform.forward +
                                  new Vector3(Random.Range(-1.0f, 1.0f), 0, 0);


                NavMeshPath path = new NavMeshPath();

                if (_navMeshAgent.CalculatePath(newPost, path))
                {
                    _navMeshAgent.SetDestination(newPost);
                }
                else
                {
                    newPost = transform.position + Random.insideUnitSphere * 1;
                    _navMeshAgent.SetDestination(newPost);
                }
            }
        }
        
        public void AIRunnerCaught()
        {
            StopCoroutine(AddMoney());
            CancelInvoke(nameof(EscapeFromHunters));
            StopCoroutine(TimeSurvived());
            
            RFM.Managers.RFMUIManager.Instance.RunnerCaught(nickName, money, timeSurvived);

            PhotonNetwork.Destroy(this.gameObject);
        }
        
        private void GameOver()
        {
            // Clear up all remaining NPCRunners are GameOver.
            if (PhotonNetwork.IsMasterClient)
            {
                AIRunnerCaught();
            }
        }

        private void OnDestroy()
        {
            // Called on all non-master clients when the runner is caught.
            if (!PhotonNetwork.IsMasterClient)
            {
                StopCoroutine(AddMoney());
                CancelInvoke(nameof(EscapeFromHunters));
                StopCoroutine(TimeSurvived());

                RFM.Managers.RFMUIManager.Instance.RunnerCaught(nickName, money, timeSurvived);
            }
        }

        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_navMeshAgent.destination);
                stream.SendNext(money);
                stream.SendNext(timeSurvived);
            }
            else
            {
                _targetPosition = (Vector3)stream.ReceiveNext();
                money = (int)stream.ReceiveNext();
                timeSurvived = (float)stream.ReceiveNext();

                if (Vector3.Distance(_navMeshAgent.destination, _targetPosition) > 1.0f)
                {
                    _navMeshAgent.SetDestination(_targetPosition);
                }
            }
        }

        private void ReceivePhotonEvents(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case PhotonEventCodes.PlayerRunnerCaught: // Event is only sent to the master client
                    {
                        int viewId = (int)((object[])photonEvent.CustomData)[0];

                        if (viewId == GetComponent<PhotonView>().ViewID)
                        {
                            AIRunnerCaught(); // When caught by PlayerHunter
                        }
                        break;
                    }
            }
        }
    }
}
