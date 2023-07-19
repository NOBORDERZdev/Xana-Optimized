using SoftMasking.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGet : MonoBehaviour
{
    public Material Materials;
    RawImage Raw;
    private Vector3 topRight;
    private Vector3 topLeft;
    private Vector3 botLeft;
    private Vector3 botRight;
    // Start is called before the first frame update
    void Start()
    {
        
        Raw = GetComponent<RawImage>();

        GetCornerPoints();

        SpawnCubeOnPoint();

        SetSize();
    }

   void GetCornerPoints()
    {
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(2f, 2f);

        Vector3[] corners = new Vector3[4];
        Raw.rectTransform.GetWorldCorners(corners);

        botLeft = corners[0];
        topLeft = corners[1];
        topRight = corners[2];
        botRight = corners[3];

        for (int i = 0; i < corners.Length; i++)
        {
            Debug.Log(corners[i]);
        }
    }

    private List<GameObject> FramePoints = new List<GameObject>();
    public void SpawnCubeOnPoint()
    {
       

        for (int i = 0; i < 4; i++)
        {
            CreateFrameHelper.instance.CreateCube(1, 1, Materials, true);

            FramePoints.Add(CreateFrameHelper.instance.FrameCube);

        }

        FramePoints[0].transform.SetPositionAndRotation(botLeft, Raw.transform.rotation);
        FramePoints[1].transform.SetPositionAndRotation(botLeft, Raw.transform.rotation);
        FramePoints[2].transform.SetPositionAndRotation(topLeft, Raw.transform.rotation);
        FramePoints[3].transform.SetPositionAndRotation(botRight,Raw.transform.rotation);

    }

    RectTransform RawTransfom;

    void SetSize()
    {
        RawTransfom = Raw.GetComponent<RectTransform>();

        FramePoints[0].transform.localScale = new Vector3(RawTransfom.rect.width, 0.15f, -0.1f);
        FramePoints[1].transform.localScale = new Vector3(0.15f, RawTransfom.rect.height, -0.1f);

        FramePoints[2].transform.localScale = new Vector3(RawTransfom.rect.width + 0.15f, 0.15f, -0.1f);
        FramePoints[3].transform.localScale = new Vector3(0.15f, RawTransfom.rect.height, -0.1f);


        for (int i = 0; i < 4; i++)
        {

            FramePoints[i].transform.transform.SetParent(Raw.transform);

        }

        

    }

}
