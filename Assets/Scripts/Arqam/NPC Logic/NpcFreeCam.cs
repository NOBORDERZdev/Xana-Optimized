using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC {
    public class NpcFreeCam : MonoBehaviour
    {
        [SerializeField] GameObject Controller;
        [SerializeField] NpcBehaviourSelector npcBehaviour;
        [SerializeField] Animator anim;
        private int minTime = 5;
        private int maxTime = 20;


        void Start()
        {

        }

        public void PerformFreeCam()
        {
            anim.SetBool("freecam", true);
            Controller.SetActive(true);
            Invoke(nameof(DisableFreeCam), Random.Range(minTime, maxTime));
        }

        void DisableFreeCam()
        {
            anim.SetBool("freecam", false);
            Controller.SetActive(false);
            npcBehaviour.isPerformingAction = false;

            if (npcBehaviour.ActionCoroutine != null)
                StopCoroutine(npcBehaviour.ActionCoroutine);

            npcBehaviour.ActionCoroutine = StartCoroutine(npcBehaviour.PerformAction());
        }


    }
}
