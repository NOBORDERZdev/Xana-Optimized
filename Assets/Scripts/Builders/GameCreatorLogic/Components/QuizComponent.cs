using Models;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class QuizComponent : MonoBehaviour
{
    private QuizComponentData quizComponentData;

    public void Init(QuizComponentData quizComponentData)
    {
        this.quizComponentData = quizComponentData;
    }

    private void OnCollisionEnter(Collision _other)
    {
        Debug.Log("Display Message Collision Enter " + _other.gameObject.name);
        if (_other.gameObject.CompareTag("Player") || (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine))
        {
            BuilderEventManager.OnQuizComponentCollisionEnter?.Invoke(this, quizComponentData);
        }
    }
}