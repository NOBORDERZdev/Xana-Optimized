using Metaverse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    public GameObject cameraObject;

    public float x;
    public float y;
    public float z;
    PlayerControllerNew player; // Player script
    private void Awake()
    {
        player = AvatarManager.Instance.spawnPoint.GetComponent<PlayerControllerNew>();
        //cameraObject = ReferrencesForDynamicMuseum.instance.randerCamera.gameObject;
        //SetCam();
    }

    private void FixedUpdate()
    {
        this.gameObject.transform.LookAt(new Vector3(player.ActiveCamera.transform.position.x, player.ActiveCamera.transform.position.y, player.ActiveCamera.transform.position.z));
    }

 
}
