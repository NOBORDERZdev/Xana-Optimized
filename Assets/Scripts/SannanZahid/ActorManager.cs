using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    [NonReorderable]
    [SerializeField]
    public List<ActorBehaviour> actorBehaviour;
    void Start()
    {
        GameManager.Instance.mainCharacter.GetComponent<Actor>().Init(actorBehaviour[0]);
    }
}
[Serializable]
public class ActorBehaviour
{
    public enum Behaviour { Basic, Happy, Sad, Jouful, Sick };
    public Behaviour behaviour;
    [NonReorderable]
    public List<MoveBehaviour> ActorMoveBehaviours;
}
