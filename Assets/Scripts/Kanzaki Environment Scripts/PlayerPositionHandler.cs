using System.Collections;
using UnityEngine;

public class PlayerPositionHandler : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _daisenPortals;
    [SerializeField] private Transform _dunePortals;
    [SerializeField] private GameObject _portalTriggerHandler;
    private void OnEnable()
    {
        BuilderEventManager.AfterPlayerInstantiated += SetPlayerPosition;
        _portalTriggerHandler.SetActive(false);
    }
    private void OnDisable()
    {
        BuilderEventManager.AfterPlayerInstantiated -= SetPlayerPosition;
    }

    private void SetPlayerPosition()
    {
        if (ConstantsHolder.xanaConstants.comingFrom == ConstantsHolder.ComingFrom.Daisen)
        {
            GameplayEntityLoader.instance.mainController.transform.SetPositionAndRotation(_daisenPortals.position, _daisenPortals.rotation);
            //SetCameraAngles(0f, 0.5f);
        }
        else if (ConstantsHolder.xanaConstants.comingFrom == ConstantsHolder.ComingFrom.Dune)
        {
            GameplayEntityLoader.instance.mainController.transform.SetPositionAndRotation(_dunePortals.position, _dunePortals.rotation);
            //SetCameraAngles(0f, 0.5f);
        }
        else
        {
            GameplayEntityLoader.instance.mainController.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
        }
        StartCoroutine(ResetCollider());
        StartCoroutine(SetCameraAngles(0f, 0.5f));

    }

    private IEnumerator ResetCollider()
    {
        yield return new WaitForSeconds(1f);
        _portalTriggerHandler.SetActive(true);
    }

    //private void SetCameraAngles(float x, float y)
    //{

    //    StartCoroutine(GameplayEntityLoader.instance.setPlayerCamAngle(x,y));
    //}
    public IEnumerator SetCameraAngles(float xValue, float yValue)
    {
        GameplayEntityLoader.instance.PlayerCamera.enabled = false;
        yield return new WaitForSeconds(0.01f);
        GameplayEntityLoader.instance.PlayerCamera.enabled = true;
        yield return new WaitForSeconds(0.1f);
        GameplayEntityLoader.instance.PlayerCamera.m_XAxis.Value = xValue;
        GameplayEntityLoader.instance.PlayerCamera.m_YAxis.Value = yValue;
    }
}
