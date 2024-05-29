using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeTeleportController : MonoBehaviour
{
    public int DomeId;
    public string TeleportWorldName;
    public GameObject PlayerRef;

    private void OnTriggerEnter(Collider other)
    {
        PlayerRef = other.gameObject;
        if (PlayerRef.CompareTag("PhotonLocalPlayer") && PlayerRef.GetComponent<PhotonView>().IsMine)
        {
            //Debug.Log("============Xana Own Player Detected===============");
            if (!string.IsNullOrEmpty(TeleportWorldName))
            {
                //Debug.Log("Getting here 1");
                if (checkWorldComingSoon(TeleportWorldName)/* || isBuilderWorld*/)
                {
                    //Debug.Log("Getting here 2");
                    StartCoroutine(swtichScene(TeleportWorldName));
                }
            }
        }
    }

    private IEnumerator swtichScene(string worldName)
    {
        if (worldName.Contains(" : "))
        {
            string name = worldName.Replace(" : ", string.Empty);
            worldName = name;
        }

        if (ConstantsHolder.xanaConstants.EnviornmentName.Contains("XANA Summit"))
        {
            ConstantsHolder.xanaConstants.isFromXanaSummit = true;
            ConstantsHolder.xanaConstants.PlrOldSpawnPos = PlayerRef.transform.position;
            ConstantsHolder.xanaConstants.PlrOldSpawnPos = new Vector3(ConstantsHolder.xanaConstants.PlrOldSpawnPos.x, 
                ConstantsHolder.xanaConstants.PlrOldSpawnPos.y, (ConstantsHolder.xanaConstants.PlrOldSpawnPos.z - 2f));
            ConstantsHolder.xanaConstants.PlrOldSpawnRot = PlayerRef.transform.eulerAngles;
            ConstantsHolder.xanaConstants.PlrOldSpawnScale = PlayerRef.transform.localScale;
        }

        LoadingHandler.Instance.StartCoroutine(LoadingHandler.Instance.TeleportFader(FadeAction.In));
        if (!ConstantsHolder.xanaConstants.JjWorldSceneChange && !ConstantsHolder.xanaConstants.orientationchanged)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        //if (isMusuem)
        //{
        //    ConstantsHolder.xanaConstants.IsMuseum = true;
        //    //if (APIBasepointManager.instance.IsXanaLive)
        //    //{
        //    //    ConstantsHolder.xanaConstants.MuseumID = MainNet.ToString();
        //    //}
        //    //else
        //    //{
        //    //    ConstantsHolder.xanaConstants.MuseumID = testNet.ToString();
        //    //}
        //}
        //else if (isBuilderWorld)
        //{
        //    ConstantsHolder.xanaConstants.isBuilderScene = true;
        //    if (APIBasepointManager.instance.IsXanaLive)
        //    {
        //        ConstantsHolder.xanaConstants.builderMapID = MainNet;
        //    }
        //    else
        //    {
        //        ConstantsHolder.xanaConstants.builderMapID = testNet;
        //    }
        //}

        yield return new WaitForSeconds(1f);
        ConstantsHolder.xanaConstants.JjWorldSceneChange = true;
        ConstantsHolder.xanaConstants.JjWorldTeleportSceneName = worldName;
        GameplayEntityLoader.instance._uiReferences.LoadMain(false);
    }

    private bool checkWorldComingSoon(string worldName)
    {
        if (!UserPassManager.Instance.CheckSpecificItem(worldName, true))
        {

            return false;
        }
        else
        {
            return true;
        }
    }

}
