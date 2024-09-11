using Models;
using Photon.Pun;
using UnityEngine;

public class QuizComponent : ItemComponent
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
            BuilderEventManager.onComponentActivated?.Invoke(_componentType);
            PlayBehaviour();
        }
    }

    private void OnDisable()
    {
        BuilderEventManager.OnQuizComponentColse?.Invoke();
    }

    #region BehaviourControl
    private void StartComponent()
    {
        BuilderEventManager.OnQuizComponentCollisionEnter?.Invoke(this, quizComponentData);
    }
    private void StopComponent()
    {

    }

    public override void StopBehaviour()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopComponent();
        }
    }

    public override void PlayBehaviour()
    {
        isPlaying = true;
        StartComponent();
    }

    public override void ToggleBehaviour()
    {
        isPlaying = !isPlaying;

        if (isPlaying)
            PlayBehaviour();
        else
            StopBehaviour();
    }
    public override void ResumeBehaviour()
    {
        PlayBehaviour();
    }

    public override void AssignItemComponentType()
    {
        _componentType = Constants.ItemComponentType.QuizComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //CollisionEnter();
    }

    #endregion
}