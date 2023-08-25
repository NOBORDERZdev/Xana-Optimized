using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.Hub;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RFM
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;
        private NavMeshAgent navMeshAgent;
        private float maxSpeed;

        private List<GameObject> _players;
        private Transform _target;

        public Transform CameraTarget { get { return cameraPosition; } }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
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
            maxSpeed = navMeshAgent.speed;
            
            InvokeRepeating(nameof(SearchForTarget), 3, 3);
        }

        private void SearchForTarget()
        {
            // if (_target) return;
            
            _players = new List<GameObject>(GameObject.FindGameObjectsWithTag(Globals.LOCAL_PLAYER_TAG));

            _target = _players[Random.Range(0, _players.Count)].transform;
            
            FollowTarget(_target.position);
        }

        private void Update()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector2 velocityDir = new Vector2(velocity.x, velocity.z); 
            Vector2 forward = new Vector2(transform.forward.x, transform.forward.z);
            float angle = Vector2.SignedAngle(forward, velocityDir);
            float xVal = Mathf.Cos((angle - 90 / 180) * Mathf.Deg2Rad);
            float yVal = Mathf.Cos(angle * Mathf.Deg2Rad);
            float speed = velocity.magnitude;

            var animVector = new Vector2(xVal, yVal) * speed / maxSpeed;

            npcAnim.SetFloat(velocityNameX, animVector.x);
            npcAnim.SetFloat(velocityNameY, animVector.y);
        }

        public void FollowTarget(Vector3 targetPosition)
        {
            if(Globals.gameState != Globals.GameState.Gameplay)
            {
                navMeshAgent.isStopped = true;
                return;
            }

            navMeshAgent.SetDestination(targetPosition);
            navMeshAgent.isStopped = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError("RFM OnTriggerEnter() with " + other.gameObject.name + ", " + other.tag);
            
            if (other.CompareTag(/*Globals.PLAYER_TAG*/Globals.LOCAL_PLAYER_TAG))
            {
                if (Globals.player == null) Globals.player = other.GetComponent<PlayerControllerNew>().gameObject;
                _players.Remove(other.gameObject);
                
                int Collidedviewid = other.GetComponent<PhotonView>().ViewID;
                RFMManager.Instance.photonView.RPC("LocalPlayerCaughtByHunter", RpcTarget.All, Collidedviewid);
                
                EventsManager.PlayerCaught(this);
            }
        }

        private void GameOver()
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
