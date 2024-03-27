using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class TranslateComponent : ItemComponent
{
    #region Translate Module
    TranslateComponentData translateComponentData;
    float nextRadius = .5f;
    List<Vector3> translatePositions;
    int counter;
    bool moveForward, moveBackward;
    bool activateTranslateComponent = false;
    public Vector3 lookAtVector;
    string RuntimeItemID = "";
    private bool IsAgainTouchable;

    public void InitTranslate(TranslateComponentData translateComponentData)
    {
        this.translateComponentData = translateComponentData;
        RuntimeItemID = GetComponent<BuilderItem>().itemData.RuntimeItemID;
        translatePositions = new List<Vector3>();
        translatePositions = translateComponentData.translatePoints;
        moveForward = true;
        moveBackward = false;
        activateTranslateComponent = true;
        counter = 0;
        if (!this.translateComponentData.avatarTriggerToggle)
        {
            PlayBehaviour();
        }
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (translateComponentData.avatarTriggerToggle && !IsAgainTouchable)
            {
                if (GamificationComponentData.instance.withMultiplayer)
                    GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, RuntimeItemID, _componentType);
                else GamificationComponentData.instance.GetObjectwithoutRPC(RuntimeItemID, _componentType);
            }
        }
    }

    private bool CheckDistance()
    {
        if ((Vector3.Distance(this.transform.position, translatePositions[counter])) < nextRadius)
        {
            //counter = (counter == 0) ? 1 : 0;
            if (moveForward == true && counter < translatePositions.Count - 1)
            {
                counter++;
            }
            else
            {
                if (translateComponentData.isLoop)
                {
                    counter = 0;
                }
                else
                {
                    moveForward = false;
                    moveBackward = true;
                }

            }
            if (moveBackward == true && counter > 0)
            {
                counter--;
            }
            else
            {
                moveBackward = false;
                moveForward = true;
            }

            return false;
        }
        else return true;
    }

    IEnumerator translateModule()
    {
        while (activateTranslateComponent)
        {
            yield return new WaitForSeconds(0f);
            if (CheckDistance())
            {
                this.transform.position = Vector3.MoveTowards(
                   this.transform.position, translatePositions[counter],
                   translateComponentData.translateSpeed * Time.deltaTime
                   );
                if (this.translateComponentData.IsFacing)
                {
                    this.transform.LookAt(translatePositions[counter]);
                    this.transform.Rotate(new Vector3(0, 1, 0), 180f);
                }
            }
        }
        yield return null;
    }
    #endregion

    #region BehaviourControl
    private void StartComponent()
    {
        activateTranslateComponent = true;
        IsAgainTouchable = true;
        StartCoroutine(translateModule());
    }
    private void StopComponent()
    {
        activateTranslateComponent = false;
    }

    public override void StopBehaviour()
    {
                isPlaying = false;
        StopComponent();
        StopComponent();
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
        _componentType = Constants.ItemComponentType.TranslateComponent;
    }

    #endregion
}