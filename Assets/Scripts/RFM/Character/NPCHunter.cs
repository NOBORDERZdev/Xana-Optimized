using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RFM.Character
{
    public class NPCHunter : MonoBehaviour, IPunObservable
    {
        // TODO : Assign a new target if the current target is caught by another hunter

        [SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;


        private NavMeshAgent _navMeshAgent;
        private float _maxSpeed;
        private List<GameObject> _players;
        private Transform _target;
        private Vector3 _targetPosition;

        private bool _hasTarget = false;

        public Transform CameraTarget => cameraPosition;

        // Catch player in range
        [SerializeField] private float timeToCatchRunner = 5;
        [SerializeField] private float catchRadius = 2;
        private float _catchTimer;
        private GameObject _inRangePlayer;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            EventsManager.onGameTimeup += GameOver;
            EventsManager.onTakePositionTimeStart += OnTakePositionTimeStart;
        }

        private void OnTakePositionTimeStart()
        {
            GetAllRunners();
        }

        private void OnDisable()
        {
            EventsManager.onGameTimeup -= GameOver;
            EventsManager.onTakePositionTimeStart -= OnTakePositionTimeStart;
        }

        private void Start()
        {
            _maxSpeed = _navMeshAgent.speed;

            InvokeRepeating(nameof(SearchForTarget), 1, 1);
        }

        private void GetAllRunners()
        {
            _players = new List<GameObject>(
                               GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG));
            _players.AddRange(new List<GameObject>(
                               GameObject.FindGameObjectsWithTag(Globals.RUNNER_NPC_TAG)));
        }

        private void SearchForTarget()
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;
            if (_hasTarget) return;

            if (_players.Count > 0)
            {
                _target = _players[Random.Range(0, _players.Count)].transform;
                _hasTarget = true;
            }
            else
            {
                _hasTarget = false;
                _navMeshAgent.isStopped = true;
            }

            if (_players.Count == 0)
            {
                GetAllRunners();
            }
        }

        private void ControlBotMovement()
        {
            if (Globals.gameState != Globals.GameState.Gameplay ||
                !_hasTarget ||
                _target == null)
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            // Bot movement logic goes here. For example:
            _targetPosition = _target.position;
            _navMeshAgent.SetDestination(_targetPosition);
            _navMeshAgent.isStopped = false;
        }

        private void SyncMovement()
        {
            if (Globals.gameState != Globals.GameState.Gameplay ||
                !_hasTarget ||
                _target == null)
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            _navMeshAgent.SetDestination(_targetPosition);
        }

        private void Update()
        {
            // Only the master client controls the bot
            if (PhotonNetwork.IsMasterClient)
            {
                ControlBotMovement();
            }
            else
            {
                // Synchronize movement for non-master clients
                SyncMovement();
            }

            Vector3 velocity = _navMeshAgent.velocity;
            Vector2 velocityDir = new Vector2(velocity.x, velocity.z);
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
            float angle = Vector2.SignedAngle(forward, velocityDir);
            float xVal = Mathf.Cos((angle - 90 / 180) * Mathf.Deg2Rad);
            float yVal = Mathf.Cos(angle * Mathf.Deg2Rad);
            float speed = velocity.magnitude;

            var animVector = new Vector2(xVal, yVal) * speed / _maxSpeed;

            npcAnim.SetFloat(velocityNameX, animVector.x);
            npcAnim.SetFloat(velocityNameY, animVector.y);


            if (RFM.Globals.DevMode) return;


            // Catch player if in range of a sphere of radius = catchRadius
            _inRangePlayer = CheckPlayerInRange();

            if (_inRangePlayer == null)
            {
                _catchTimer = 0;
            }
            else
            {
                _catchTimer += Time.deltaTime;
                if (_catchTimer >= timeToCatchRunner)
                {
                    _catchTimer = 0;
                    _players.Remove(_inRangePlayer);
                    _hasTarget = false;
                    killVFX.SetActive(true);
                    _inRangePlayer.GetComponent<PlayerRunner>()?.PlayerRunnerCaught(this);
                }
            }
        }

        private GameObject CheckPlayerInRange()
        {
            foreach (var col in Physics.OverlapSphere(transform.position, catchRadius))
            {
                if (col.CompareTag(Globals.PLAYER_TAG))
                {
                    if (_inRangePlayer == col.gameObject)
                        return _inRangePlayer;

                    if (col.GetComponent<PhotonView>().IsMine)
                    {
                        return col.gameObject;
                    }
                }
            }

            return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (RFM.Globals.DevMode) return;
            if (!PhotonNetwork.IsMasterClient) return;

            if (other.CompareTag(Globals.RUNNER_NPC_TAG))
            {
                _players.Remove(other.gameObject);
                _hasTarget = false;
                killVFX.SetActive(true);
                other.transform.parent.GetComponent<NPCRunner>().AIRunnerCaught();
            }

            else if (other.CompareTag(Globals.PLAYER_TAG))
            {
                _players.Remove(other.gameObject);
                _hasTarget = false;
                killVFX.SetActive(true);

                other.GetComponent<PlayerRunner>()?.PlayerRunnerCaught(this);
            }
        }

        private void GameOver()
        {
            PhotonNetwork.Destroy(gameObject);
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Master client sends data
                stream.SendNext(_navMeshAgent.destination);
            }
            else
            {
                // Other clients receive data
                _targetPosition = (Vector3)stream.ReceiveNext();

                // Check for discrepancies and lag compensation
                if (Vector3.Distance(_navMeshAgent.destination, _targetPosition) > 1.0f)
                {
                    _navMeshAgent.SetDestination(_targetPosition);
                }

                // Additional security and validation checks can be implemented here
            }
        }
    }
}