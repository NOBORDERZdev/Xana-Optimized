using Models;
using Photon.Pun;
using UnityEngine;

public class QuizComponent : MonoBehaviour
{
    private QuizComponentData quizComponentData;

    public void Init(QuizComponentData quizComponentData)
    {
        this.quizComponentData = quizComponentData;

        if (this.quizComponentData.correctAnswerRate == 0)
            this.quizComponentData.correctAnswerRate = 100;
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            BuilderEventManager.OnQuizComponentCollisionEnter?.Invoke(this, quizComponentData);
        }
    }
}