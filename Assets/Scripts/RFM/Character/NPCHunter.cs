using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RFM.Character
{
    public class NPCHunter : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;
        [SerializeField] private GameObject killVFX;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;
        private NavMeshAgent _navMeshAgent;
        private float _maxSpeed;

        private List<GameObject> _players;
        private Transform _target;

        public Transform CameraTarget => cameraPosition;

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
            _maxSpeed = _navMeshAgent.speed;
            
            InvokeRepeating(nameof(SearchForTarget), 1, 1);
        }

        private void SearchForTarget()
        {
            if (_target) return;
            
            _players = new List<GameObject>(
                GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG));
            _players.AddRange(new List<GameObject>(
                GameObject.FindGameObjectsWithTag(Globals.ESCAPEE_NPC_TAG)));

            if (_players.Count > 0)
            {
                _target = _players[Random.Range(0, _players.Count)].transform;
                FollowTarget(_target.position);
            }
            else
            {
                _navMeshAgent.isStopped = true;
            }
        }

        private void Update()
        {
            if (_target)
            {
                FollowTarget(_target.position);
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
        }

        private void FollowTarget(Vector3 targetPosition)
        {
            if(Globals.gameState != Globals.GameState.Gameplay)
            {
                _navMeshAgent.isStopped = true;
                return;
            }

            _navMeshAgent.SetDestination(targetPosition);
            _navMeshAgent.isStopped = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Globals.ESCAPEE_NPC_TAG))
            {
                _players.Remove(other.gameObject);
                _target = null;
                killVFX.SetActive(true);
                other.transform.parent.GetComponent<NPCEscapee>().AIEscapeeCaught();
            }
            
            else if (other.CompareTag(Globals.PLAYER_TAG)/*Globals.LOCAL_PLAYER_TAG*/)
            {
                // if (Globals.player == null) Globals.player = other.GetComponent<PlayerControllerNew>().gameObject;
                _players.Remove(other.gameObject);
                _target = null;

                // PhotonView is on the parent of the gameobject that has a collider.
                // int Collidedviewid = other.transform.parent.GetComponent<PhotonView>().ViewID;
                
                // Uncomment for player hunter
                // RFMManager.Instance.photonView.RPC("LocalPlayerCaughtByHunter", RpcTarget.All, Collidedviewid);
                killVFX.SetActive(true);
                EventsManager.PlayerCaught(this);
            }
        }

        private void GameOver()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
