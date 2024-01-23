using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace RFM.Character
{
    public class NPCRunner : Runner
    {
        public float timeSurvived;

        [SerializeField] private Animator animator;
        [SerializeField] private string velocityNameX, velocityNameY;

        private NavMeshAgent _navMeshAgent;
        private float _maxSpeed;
        private Transform _closestHunterTransform = null;
        private float _minDistance = 10f;
        public TextMeshProUGUI nickNameText;

        public float minDistanceToStartRunning = 10f;
        public List<Transform> huntersTransforms = new();

        private Vector3 _targetPosition;

        private int viewIDOfHunterThatCaughtThisRunner = -999;

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
            if (PhotonNetwork.IsMasterClient)
            {
                nickName = RFM.Character.StaticData.CharacterNames[
                Random.Range(0, RFM.Character.StaticData.CharacterNames.Length - 1)];

                nickNameText.text = nickName.ToString();
                // Send an RPC to only this PhotonView on all clients to set the nickname.
                GetComponent<PhotonView>().RPC(nameof(SetNickname), RpcTarget.OthersBuffered, nickName);
            }

            _maxSpeed = _navMeshAgent.speed;
        }

        [PunRPC]
        private void SetNickname(string _nickName)
        {
            nickName = _nickName;
            nickNameText.text = nickName.ToString();
        }


        private void TakePositionTimeStarted()
        {
            //if (PhotonNetwork.IsMasterClient) // Only master controls the movement 
            {
                InvokeRepeating(nameof(EscapeFromHunters), 1, 0.2f);
            }
        }


        internal override void OnGameStarted()
        {
            base.OnGameStarted();

            InvokeRepeating(nameof(AddMoney), RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval,
                RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval);
        }

        private void AddMoney()
        {
            money += RFM.Managers.RFMManager.CurrentGameConfiguration.MoneyPerInterval;
            timeSurvived += RFM.Managers.RFMManager.CurrentGameConfiguration.GainingMoneyTimeInterval;
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
            if (GetComponent<PhotonView>().Owner.IsMasterClient)
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
        }

        private void EscapeFromHunters()
        {
            if (PhotonNetwork.IsMasterClient)
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
        }

        public void AIRunnerCaught()
        {
            CancelInvoke(nameof(AddMoney));
            CancelInvoke(nameof(EscapeFromHunters));

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
                CancelInvoke(nameof(AddMoney));
                CancelInvoke(nameof(EscapeFromHunters));

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

                return;
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
                        int hunterViewID = (int)((object[])photonEvent.CustomData)[1];

                        if (viewId == GetComponent<PhotonView>().ViewID)
                        {
                            if (viewIDOfHunterThatCaughtThisRunner == -999) // if this runner has not been caught yet
                            {
                                viewIDOfHunterThatCaughtThisRunner = hunterViewID;

                                var hunterPV = PhotonView.Find(hunterViewID);
                                if (hunterPV != null)
                                {
                                    if (hunterPV.TryGetComponent(out PlayerHunter _))
                                    {
                                        var oldValue = 0;
                                        if (hunterPV.Owner.CustomProperties.ContainsKey("rewardMultiplier"))
                                        {
                                            oldValue = (int)hunterPV.Owner.CustomProperties["rewardMultiplier"];
                                        }
                                        else
                                        {
                                            hunterPV.Owner.SetCustomProperties(
                                                new ExitGames.Client.Photon.Hashtable { { "rewardMultiplier", 0 } });
                                        }

                                        hunterPV.Owner.SetCustomProperties(
                                            new ExitGames.Client.Photon.Hashtable { { "rewardMultiplier", oldValue + 1 } }, // to be set
                                            new ExitGames.Client.Photon.Hashtable { { "rewardMultiplier", oldValue } } // expected value
                                            );
                                    }
                                }

                                AIRunnerCaught(); // When caught by PlayerHunter
                            }
                        }
                        break;
                    }
            }
        }
    }
}
