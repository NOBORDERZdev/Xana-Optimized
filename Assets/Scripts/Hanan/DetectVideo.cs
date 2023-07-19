using UnityEngine;


public class DetectVideo : MonoBehaviour
{

    public DynamicGalleryData data;
    public bool Playing;
    RaycastHit hit;
    Camera MainCamera;

    private void Start()
    {
        MainCamera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {


        if (Physics.Raycast(transform.position, MainCamera.transform.TransformDirection(Vector3.forward), out hit, 5))
        {
            Debug.DrawRay(transform.position, MainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");

            if (hit.collider.GetComponent<DynamicGalleryData>())
            {
                data = hit.collider.GetComponent<DynamicGalleryData>();
                if (data.detail.description.Length > 0)
                {
                    if (!data.VideoPlaying && data.videoPlayerWithStats != null)
                    {
                        //hasData = true;
                        data.videoPlayerWithStats.Play();
                        data.VideoPlaying = true;
                        Debug.Log("Video Playing");
                    }
                }
                else
                {
                    if (!data.VideoPlaying && data.videoPlayer != null)
                    {
                        //hasData = true;
                        data.videoPlayer.Play();
                        data.VideoPlaying = true;
                        Debug.Log("Video Playing");
                    }
                }

            }
            else
            {
                if (data.detail.description.Length > 0)
                {
                    if (data != null && data.videoPlayerWithStats != null)
                    {
                        data.videoPlayer.Pause();
                        data.VideoPlaying = false;
                    }
                }
                else
                {
                    if (data != null && data.videoPlayer != null)
                    {
                        data.videoPlayer.Pause();
                        data.VideoPlaying = false;
                    }
                }
                data = null;

            }

        }


    }
}
