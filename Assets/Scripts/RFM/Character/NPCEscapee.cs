using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

namespace RFM.Character
{
    public class NPCEscapee : MonoBehaviour
    {
        [SerializeField] private GameObject killVFX;
        [SerializeField] private Animator npcAnim;
        [SerializeField] private string velocityNameX, velocityNameY;

        private NavMeshAgent navMeshAgent;
        private float maxSpeed;
        private Transform closestHunterTransform = null;
        private float minDistance = 10f;


        public float minDistanceToStartRunning = 10f;
        public List<Transform> huntersTrasnforms = new List<Transform>();

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            maxSpeed = navMeshAgent.speed;
            InvokeRepeating(nameof(EscapeFromHunters), 1, 0.2f);
        }

        void UpdateHuntersTransformList() 
        {
            List<GameObject> hunterObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag(Globals.HUNTER_NPC_TAG));
            huntersTrasnforms.Clear();
            foreach (GameObject hunterObj in hunterObjects) 
            {
                huntersTrasnforms.Add(hunterObj.transform);
            }
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

        private void EscapeFromHunters()
        {
            UpdateHuntersTransformList();

            for (int k = 0; k < huntersTrasnforms.Count; k++) 
            {
                float distance = Vector3.Distance(this.transform.position, huntersTrasnforms[k].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestHunterTransform = huntersTrasnforms[k];
                }
            }

            if (huntersTrasnforms.Count > 0 && minDistance < minDistanceToStartRunning && closestHunterTransform!=null)
            {
                Vector3 dirToSelf = transform.position - closestHunterTransform.position;
                Vector3 newPost = transform.position + dirToSelf;
                navMeshAgent.SetDestination(newPost);
            }
        }
        
        public void AIEscapeeCaught()
        {
            PhotonNetwork.Destroy(this.gameObject);
        }

    }
}
