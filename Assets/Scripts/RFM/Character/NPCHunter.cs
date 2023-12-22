using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static StoreManager;

namespace RFM.Character
{
    public class NPCHunter : Hunter
    {
        // TODO : Assign a new target if the current target is caught by another hunter

        //[SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;


        private NavMeshAgent _navMeshAgent;
        // private float _maxSpeed;
        private List<GameObject> _allRunners;
        private Transform _target;
        private Vector3 _targetPosition;

        private bool _hasTarget = false;

        // public Transform cameraTarget/* => cameraPosition*/;

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
            EventsManager.onGameStart/*onTakePositionTimeStart*/ += OnGameStarted;
            EventsManager.onDestroyAllNPCHunters += OnDestroyAllNPCHunters;
        }

        private void Start()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                nickName = RFM.Character.StaticData.CharacterNames[
                    Random.Range(0, RFM.Character.StaticData.CharacterNames.Length - 1)];

                // Send an RPC to only this PhotonView on all clients to set the nickname.

                GetComponent<PhotonView>().RPC(nameof(SetNickname), RpcTarget.OthersBuffered, nickName);
            }
        }

        [PunRPC]
        private void SetNickname(string _nickName)
        {
            nickName = _nickName;
        }

        internal override void OnGameStarted()
        {
            base.OnGameStarted();

            if (PhotonNetwork.IsMasterClient)// Only the master client controls the hunter.
                                             // Other clients just sync the movement
            {
                GetAllRunners();
                InvokeRepeating(nameof(SearchForTarget), 1, 1);
            }
        }

        private void OnDisable()
        {
            EventsManager.onGameStart -= OnGameStarted;
            EventsManager.onDestroyAllNPCHunters += OnDestroyAllNPCHunters;
        }

        private void GetAllRunners()
        {
            _allRunners = new List<GameObject>();

            var runners = FindObjectsOfType<Runner>(false); // TODO
            foreach (var runner in runners)
            {
                if (runner.enabled && runner.gameObject.activeInHierarchy)
                {
                    _allRunners.Add(runner.gameObject);
                }
            }
        }

        private void SearchForTarget()
        {
            if (Globals.gameState != Globals.GameState.Gameplay) return;

            // if any of the object in _players list is missing, remove it from the list.
            for (int i = 0; i < _allRunners.Count; i++)
            {
                if (_allRunners[i] == null || !_allRunners[i].gameObject.activeInHierarchy)
                {
                    _allRunners.RemoveAt(i);
                }
            }

            if (_allRunners == null || _allRunners.Count == 0)
            {
                GetAllRunners();
            }

            if (_allRunners.Count > 0) // if any runner is found
            {
                var closestRunner = _allRunners[0];
                foreach (var runner in _allRunners) // get the closest runner
                {
                    if (runner == null) continue;

                    if (Vector3.Distance(transform.position, runner.transform.position) <
                                           Vector3.Distance(transform.position, closestRunner.transform.position))
                    {
                        closestRunner = runner;
                    }
                }

                // if the closestRunner is not the current target, set it as target and set _hasTarget to true.
                // otherwise, set a random target and set _hasTarget to true.
                if (_target != closestRunner.transform)
                {
                    _target = closestRunner.transform;
                    _hasTarget = true;
                }
                else
                {
                    //_target = _allRunners[Random.Range(0, _allRunners.Count)].transform;
                    //_hasTarget = true;
                    //Debug.LogError("Random target locked");
                }
            }
            else // if no runner is found
            {
                _hasTarget = false;
                _navMeshAgent.isStopped = true;
            }

            if (_target == null) // sometimes, the target is null even though _hasTarget is true
                                 // such as when the target is caught by another hunter
            {
                _hasTarget = false;
            }

            //if (_hasTarget) return;

            //if (_allRunners.Count > 0)
            //{
            //    // if any of the runners in _allRunners list is closer than 5 units, set it as target and set _hasTarget
            //    // to true. otherwise, set a random target and set _hasTarget to true.
            //    foreach (var runner in _allRunners)
            //    {
            //        if (runner == null) continue;

            //        if (Vector3.Distance(transform.position, runner.transform.position) < 50)
            //        {
            //            _target = runner.transform;
            //            _hasTarget = true;
            //            Debug.LogError("Closest target locked");
            //            break;
            //        }
            //    }
            //    if (!_hasTarget)
            //    {
            //        _target = _allRunners[Random.Range(0, _allRunners.Count)].transform;
            //        _hasTarget = true;
            //        Debug.LogError("Random target locked");
            //    } 
            //}
            //else
            //{
            //    _hasTarget = false;
            //    _navMeshAgent.isStopped = true;
            //}
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
            if (Globals.gameState != Globals.GameState.Gameplay)
                //||
                //!_hasTarget ||
                //_target == null)
            {
                _navMeshAgent.isStopped = true;
                return;
            }
            else
            {
                _navMeshAgent.isStopped = false;
            }

            _navMeshAgent.SetDestination(_targetPosition);
        }

        private void Update()
        {
            // Draw a line from the hunter to the target
            if (_hasTarget && _target != null)
            {
                Debug.DrawLine(transform.position, _target.position, Color.red);
            }

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

            npcAnim.SetFloat("speed", _navMeshAgent.velocity.magnitude);


            //if (RFM.Globals.DevMode) return;
            //if (!PhotonNetwork.IsMasterClient) return;


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
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _allRunners.Remove(_inRangePlayer);
                        _hasTarget = false;
                    }
                    killVFX.SetActive(true);


                    // _inRangePlayer.GetComponent<PlayerRunner>()?.PlayerRunnerCaught(/*this*//*CameraTarget*/);

                    var runnerViewId = _inRangePlayer.GetComponent<PhotonView>().ViewID;
                    var myViewId = GetComponent<PhotonView>().ViewID;

                    object[] prameters = new object[] { runnerViewId, myViewId };

                    PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                        prameters,
                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                        SendOptions.SendReliable);

                    rewardMultiplier++;
                    _inRangePlayer.gameObject.SetActive(false);

                }
            }
        }

        private GameObject CheckPlayerInRange()
        {
            foreach (var col in Physics.OverlapSphere(transform.position, catchRadius))
            {
                //if (col.CompareTag(Globals.PLAYER_TAG))
                var playerRunner = col.GetComponent<PlayerRunner>();
                if (playerRunner != null && playerRunner.enabled)
                {
                    if (col.GetComponent<PhotonView>().IsMine)
                    {
                        if (_inRangePlayer == col.gameObject) // if this runner is already in range, return it
                            return _inRangePlayer;

                        //if (col.GetComponent<PhotonView>().IsMine) // ?
                        {
                            return col.gameObject;
                        }
                    }
                }
            }

            return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (RFM.Globals.DevMode) return;
            //if (!PhotonNetwork.IsMasterClient) return;
            if (RFM.Globals.gameState != RFM.Globals.GameState.Gameplay) // Only catch players in gameplay state
            {
                return;
            }

            //if (other.CompareTag(Globals.RUNNER_NPC_TAG))
            if (other.GetComponentInParent<NPCRunner>())
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    _allRunners.Remove(other.gameObject);
                    _hasTarget = false;
                    killVFX.SetActive(true);
                    other.transform.parent.GetComponent<NPCRunner>().AIRunnerCaught();

                    rewardMultiplier++;

                    return;
                }
                
            }

            //else if (other.CompareTag(Globals.PLAYER_TAG))
            var playerRunner = other.GetComponent<PlayerRunner>();
            if (playerRunner != null && playerRunner.enabled)
            {
                // if the playerRunner is the local player, call the PlayerRunnerCaught() method on the PlayerRunner script
                if (other.GetComponent<PhotonView>().IsMine)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _allRunners.Remove(other.gameObject);
                        _hasTarget = false;
                    }
                    killVFX.SetActive(true);
                    //other.GetComponent<PlayerRunner>().PlayerRunnerCaught(/*this.cameraTarget*/);

                    //// Raise a PhotonNetwork.RaiseEvent() event here to notify other clients that the player has been caught
                    //// The other clients will then call the PlayerRunnerCaught() method on their respective PlayerRunner script
                    //// Send photonview ID of other in parameters.
                    var runnerViewId = other.GetComponent<PhotonView>().ViewID;
                    var myViewId = GetComponent<PhotonView>().ViewID;

                    object[] prameters = new object[] { runnerViewId, myViewId };

                    PhotonNetwork.RaiseEvent(PhotonEventCodes.PlayerRunnerCaught,
                        prameters,
                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                        SendOptions.SendReliable);

                    other.GetComponent<Collider>().enabled = false;
                    rewardMultiplier++;
                }
            }
        }

        private void OnDestroyAllNPCHunters()
        {
            PhotonNetwork.Destroy(gameObject);
        }


        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Master client sends data
                stream.SendNext(_navMeshAgent.destination);
                stream.SendNext(rewardMultiplier);
            }
            else
            {
                // Other clients receive data
                _targetPosition = (Vector3)stream.ReceiveNext();
                rewardMultiplier = (int)stream.ReceiveNext();

                // Check for discrepancies and lag compensation
                if (Vector3.Distance(_navMeshAgent.destination, _targetPosition) > 1.0f)
                {
                    _navMeshAgent.SetDestination(_targetPosition);
                }

                //// teleport to the new position if the distance is too far
                //if (Vector3.Distance(transform.position, _targetPosition) > 5.0f)
                //{
                //    transform.position = _targetPosition;
                //}

                // Additional security and validation checks can be implemented here
            }
        }
    }
}