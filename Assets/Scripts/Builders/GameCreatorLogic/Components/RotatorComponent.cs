using System.Collections;
using UnityEngine;
using Models;
public class RotatorComponent : ItemComponent
{
    private RotatorComponentData rotatorComponentData;
    private IEnumerator routine;

    public void Init(RotatorComponentData rotatorComponentData)
    {
        InitRotate(rotatorComponentData);
    }

    public void InitRotate(RotatorComponentData rotatorComponentData)
    {
        this.rotatorComponentData = rotatorComponentData;
        PlayBehaviour();
    }

    Vector3 currentRotation;
    IEnumerator RotateModule()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentRotation = gameObject.transform.rotation.eulerAngles;
            currentRotation += new Vector3(0f, rotatorComponentData.speed * Time.deltaTime, 0f);
            gameObject.transform.rotation = Quaternion.Euler(currentRotation);
            yield return null;
        }
    }

    #region BehaviourControl
    private void StartComponent()
    {
        StopComponent();

        routine = RotateModule();
        StartCoroutine(routine);
    }
    private void StopComponent()
    {
        if (routine != null) StopCoroutine(routine);
    }

    public override void StopBehaviour()
    {
        if(isPlaying)
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
        _componentType = Constants.ItemComponentType.RotatorComponent;
    }

    public override void CollisionExitBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    public override void CollisionEnterBehaviour()
    {
        //throw new System.NotImplementedException();
    }

    #endregion
}