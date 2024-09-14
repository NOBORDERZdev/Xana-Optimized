using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCollisonAvoider : MonoBehaviour
{

    // Start is called before the first frame update
    public List<SplineFollower> cars = new List<SplineFollower>();

    private void OnEnable()
    {
        MutiplayerController.onRespawnPlayer += () => cars.Clear();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CAR")
        {
            var follower = other.GetComponent<SplineFollower>();
            Debug.Log("cARS IN COLLIDER  " + cars.Count);
            if (cars.Count > 0)
            {
              
                follower.StopCar = true;
                follower.Speed = 0;
            }
            cars.Add(follower);
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "CAR")
        {
            cars.RemoveAt(0);
            if (cars.Count > 0)
            {
                cars[0].StopCar = false;
                cars[0].Speed = 5;
            }
        }
    }
}
