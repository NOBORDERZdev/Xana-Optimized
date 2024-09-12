using Models;
using Photon.Pun;
using UnityEngine;

public class SMBCQuizComponent : MonoBehaviour
{
    public SMBCCollectibleType RequireCollectible;
    private QuizComponentData _quizComponentData;


    private void Start()
    {
        Debug.Log("Quiz function calling");
        SMBCManager.Instance?.InitQuizComponent(this);
    }

    public void Init(QuizComponentData quizComponentData)
    {
        this._quizComponentData = quizComponentData;

        if (this._quizComponentData.correctAnswerRate == 0)
            this._quizComponentData.correctAnswerRate = 100;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PhotonLocalPlayer" && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (SMBCManager.Instance.CheckForObjectCollectible(RequireCollectible))
                BuilderEventManager.OnSMBCQuizComponentCollisionEnter?.Invoke(this, _quizComponentData);
            else
            {
                switch (RequireCollectible)
                {
                    case SMBCCollectibleType.DoorKey:
                        BuilderEventManager.OnDoorKeyCollisionEnter?.Invoke("Please collect all require keys first!!");
                        break;
                    case SMBCCollectibleType.Axe:
                        BuilderEventManager.OnDoorKeyCollisionEnter?.Invoke("Please collect axe first!!");
                        break;
                }
            }
        }
    }
}
