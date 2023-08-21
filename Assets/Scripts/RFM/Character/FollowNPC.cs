using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFM
{
    public class FollowNPC : MonoBehaviour
    {
        private Transform target;
        private float followSpeed;


        public void Init(Transform target)
        {
            this.target = target;
            followSpeed = Globals.npcCameraFollowSpeed;
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

        private void LateUpdate()
        {
            if (!target) return;

            transform.position += (target.position - transform.position) * followSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, followSpeed * Time.deltaTime);
        }
    }
}
