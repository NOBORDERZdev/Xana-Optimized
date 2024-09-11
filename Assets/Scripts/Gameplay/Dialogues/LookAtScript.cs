using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtScript : MonoBehaviour
{
    public GameObject cameraObject;

    public float x;
    public float y;
    public float z;
    PlayerController player; // Player script
    private void Awake()
    {
        player = AvatarSpawnerOnDisconnect.Instance.spawnPoint.GetComponent<PlayerController>();
        //cameraObject = ReferencesForGamePlay.instance.randerCamera.gameObject;
        //SetCam();
    }

    private void FixedUpdate()
    {
        this.gameObject.transform.LookAt(new Vector3(player.ActiveCamera.transform.position.x, player.ActiveCamera.transform.position.y, player.ActiveCamera.transform.position.z));
    }

 
}
