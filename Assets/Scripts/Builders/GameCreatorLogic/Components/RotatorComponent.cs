using System.Collections;
using UnityEngine;
using Models;
using Photon.Pun;

public class RotatorComponent : ItemComponent
{
    private RotatorComponentData rotatorComponentData;
    private bool startComponent;
    private Vector3 currentRotation;
    private string itemID;
    public void Init(RotatorComponentData rotatorComponentData,string itemid)
    {
        InitRotate(rotatorComponentData);
        itemID = itemid;
        NetworkSyncManager.instance.rotatorComponent.Add(itemid,Vector3.zero);
    }

    public void InitRotate(RotatorComponentData rotatorComponentData)
    {
        this.rotatorComponentData = rotatorComponentData;
     
        PlayBehaviour();
    }

   
    private void FixedUpdate() //Provide better performance than infinite corutine
    {
        if (startComponent)
        {
            if (PhotonNetwork.IsMasterClient) {
                currentRotation = gameObject.transform.rotation.eulerAngles;
                currentRotation += new Vector3(0f, rotatorComponentData.speed * Time.deltaTime, 0f);
                gameObject.transform.rotation = Quaternion.Euler(currentRotation);
                NetworkSyncManager.instance.rotatorComponent[itemID] = currentRotation;
            }
            else
            {
                object obj;
                if (NetworkSyncManager.instance.rotatorComponent.TryGetValue(itemID,out obj))
                {
                    currentRotation = (Vector3)obj;
                    gameObject.transform.rotation = Quaternion.Euler (currentRotation);
                }
            }
        }
        
    }

    #region BehaviourControl
    private void StartComponent()
    {
        StopComponent();

        startComponent = true;
    }
    private void StopComponent()
    {
       startComponent = false;
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