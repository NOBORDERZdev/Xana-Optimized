using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Models;
using Photon.Pun;

public class TranslateComponent : ItemComponent
{
    #region Translate Module
    public Vector3 LookAtVector;
    TranslateComponentData _translateComponentData;
    float _nextRadius = .5f;
    List<Vector3> _translatePositions;
    int _counter;
    bool _moveForward, _moveBackward;
    bool _activateTranslateComponent = false;
    string _runtimeItemID = "";
    bool _isAgainTouchable;

    public void InitTranslate(TranslateComponentData translateComponentData)
    {
        this._translateComponentData = translateComponentData;
        _runtimeItemID = GetComponent<XanaItem>().itemData.RuntimeItemID;
        _translatePositions = new List<Vector3>();
        _translatePositions = translateComponentData.translatePoints;
        _moveForward = true;
        _moveBackward = false;
        _activateTranslateComponent = true;
        _counter = 0;
        NetworkSyncManager.Instance.TranslateComponentpos.TryAdd(_runtimeItemID, transform.position);
        if (!this._translateComponentData.avatarTriggerToggle)
        {
            PlayBehaviour();
        }
    }

    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.tag == "PhotonLocalPlayer" && _other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            if (_translateComponentData.avatarTriggerToggle && !_isAgainTouchable)
            {
                if (GamificationComponentData.instance.withMultiplayer)
                    GamificationComponentData.instance.photonView.RPC("GetObject", RpcTarget.All, _runtimeItemID, _componentType);
                else
                    GamificationComponentData.instance.GetObjectwithoutRPC(_runtimeItemID, _componentType);
            }
        }
    }

    private bool CheckDistance()
    {
        if (_translatePositions.Count > 0 && (Vector3.Distance(this.transform.position, _translatePositions[_counter])) < _nextRadius)
        {
            //counter = (counter == 0) ? 1 : 0;
            if (_moveForward == true && _counter < _translatePositions.Count - 1)
            {
                _counter++;
            }
            else
            {
                if (_translateComponentData.isLoop)
                {
                    _counter = 0;
                }
                else
                {
                    _moveForward = false;
                    _moveBackward = true;
                }

            }
            if (_moveBackward == true && _counter > 0)
            {
                _counter--;
            }
            else
            {
                _moveBackward = false;
                _moveForward = true;
            }

            return false;
        }
        else return true;
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_activateTranslateComponent)
            {
                NetworkSyncManager.Instance.TranslateComponentpos[_runtimeItemID] = transform.position;
            }
        }
        else
        {
            object obj;
            if (NetworkSyncManager.Instance.TranslateComponentpos.TryGetValue(_runtimeItemID, out obj))
            {
                transform.position = (Vector3)obj;
            }
        }


    }

    IEnumerator translateModule()
    {
        while (_activateTranslateComponent)
        {
            yield return new WaitForSeconds(0f);
            if (CheckDistance())
            {
                this.transform.position = Vector3.MoveTowards(
                   this.transform.position, _translatePositions[_counter],
                   _translateComponentData.translateSpeed * Time.deltaTime
                   );
                if (this._translateComponentData.IsFacing)
                {
                    this.transform.LookAt(_translatePositions[_counter]);
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
        _activateTranslateComponent = true;
        _isAgainTouchable = true;
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(translateModule());
    }
    private void StopComponent()
    {
        _activateTranslateComponent = false;
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