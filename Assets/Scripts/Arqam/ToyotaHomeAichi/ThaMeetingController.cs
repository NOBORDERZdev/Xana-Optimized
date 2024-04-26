using UnityEngine;

public class ThaMeetingController : MonoBehaviour
{

    void Start()
    {
        //only user can back to toyota world when press on exit btn
        if (ConstantsHolder.xanaConstants.toyotaMeetingStatus == ConstantsHolder.MeetingStatus.Inprogress)
        {
            ConstantsHolder.xanaConstants.isBackToParentScane = true;
            ConstantsHolder.xanaConstants.parentSceneName = "D_Infinity_Labo";
        }
    }

}

