using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumRaycaster : MonoBehaviour
{
    public static MuseumRaycaster instance;

    public RaycastHit hit;
    public Camera playerCamera;

    public float rayDistance;

    private int layerMask;

    [HideInInspector]
    public static bool canOpenPicture = true;

    void Awaka()
    {
        instance = this;

    }

    private void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("PictureInteractable");
        canOpenPicture = true;
        if (playerCamera == null)
        {
            playerCamera = ReferencesForGamePlay.instance.randerCamera;
        }
    }

    public void SelfieClose()
    {
        StartCoroutine(WaitForCooldown());

    }

    public void ChangePictureState(bool state)
    {
        canOpenPicture = state;
    }

    public void Update()
    {
        //if (EmoteAnimationHandler.Instance.isEmoteActive) return;

//        if (Input.GetMouseButtonUp(0) && canOpenPicture)
//        {
//#if !UNITY_EDITOR
//            if (Input.touchCount == 1)
//            {
//#endif

//            Debug.Log("-0-------------->>>. " + playerCamera.gameObject.name);
//            bool headBlock = false;
//            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

//            if (Physics.Raycast(ray, out hit))
//            {
//                Debug.Log("-1-------------->>>. " + hit.collider.gameObject.name);

//                if (hit.collider.gameObject.name == "Head Button")
//                {
//                    headBlock = true;
//                }
//            }


//            if (!headBlock && GalleryImageManager.Instance.CheckIfPicturePanelIsOpen())
//            {
//                //RaycastHit hits = Physics.RaycastAll(ray, rayDistance);

//                if (Physics.Raycast(ray, out hit, rayDistance))
//                {
//                    if (hit.collider.gameObject.name == "PictureInteractable")
//                    {
//                        Debug.Log("-2-------------->>>. " + hit.collider.gameObject.name);
//                        canOpenPicture = false;
//                        hit.collider.GetComponent<GalleryImageDetails>().OnPictureClicked();
//                    }
//                }

//                //for (int i = 0; i < hits.Length; i++)
//                //{
//                //    RaycastHit hit = hits[i];

//                //    Debug.Log(hit.collider.name);

//                //    if (hit.collider.gameObject.name == "PictureInteractable")
//                //    {
//                //        canOpenPicture = false;
//                //        hit.collider.GetComponent<GalleryImageDetails>().OnPictureClicked();
//                //    }
//                //}
//#if !UNITY_EDITOR

//                }
//#endif
//            }

//        }
    }

    public IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(0.5f);

        canOpenPicture = true;
    }
}
