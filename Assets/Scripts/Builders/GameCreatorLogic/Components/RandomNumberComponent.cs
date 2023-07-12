using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Models;
using Photon.Pun;

//[RequireComponent(typeof(Rigidbody))]
public class RandomNumberComponent : ItemComponent
{
    float _minNumber = 0, _maxNumber = 0, GeneratedNumber = 0;

    [SerializeField]
    private RandomNumberComponentData randomNumberComponentData;

    private bool isActivated = false;


    // Start is called before the first frame update
    void Start()
    {
        _minNumber = randomNumberComponentData.minNumber;
        _maxNumber = randomNumberComponentData.maxNumber;
        GenerateNumber();
    }

    void GenerateNumber()
    {
        GeneratedNumber = (int)UnityEngine.Random.Range(_minNumber, _maxNumber);
    }

    public void Init(RandomNumberComponentData randomNumberComponentData)
    {
        Debug.Log(JsonUtility.ToJson(randomNumberComponentData));
        this.randomNumberComponentData = randomNumberComponentData;

        isActivated = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Random Number Collision Enter: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player") || (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine))
        {

            //TimeStats.canRun = false;
            BuilderEventManager.OnRandomCollisionEnter?.Invoke(GeneratedNumber);
            GenerateNumber();
        }
    }

    //onCollsion Exit to ontrigger exit
    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Random Number Collision Exit: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player") || (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            BuilderEventManager.OnRandomCollisionExit?.Invoke();
        }
    }
}