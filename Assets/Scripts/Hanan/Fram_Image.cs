using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fram_Image : MonoBehaviour
{
    #region Veriables

    public GameObject render;
    
    

    [HideInInspector]
    public float widthOffset;
    [HideInInspector]
    public float heightOffset;
    [HideInInspector]
    public float zOffset;

    [Header("Cube Settings")]
    private  List<GameObject> FramePoints = new List<GameObject>();
    private Vector3 topRight;
    private Vector3 topLeft; 
    private Vector3 botLeft;
    private Vector3 botRight;


    SpriteRenderer Newrenderer;
    RawImage rawImage;
    RectTransform RawTransfom;
    bool isSpriteRanderer;

    #endregion



    // this function is to create plane type frame if needed
    public void Create()
    {
        float width;
        float height;

        //what size of frame should be created

        width = (render.transform.lossyScale.x * widthOffset);
        height = (render.transform.localScale.y * heightOffset);

        // create frame with the size of image
        CreateFrameHelper.instance.CreatePlane(width, height, CreateFrameHelper.instance.Materials,true);

        // setting frame pos according to image in scene
        CreateFrameHelper.instance.Frame.transform.SetPositionAndRotation(render.transform.GetChild(0).position, render.transform.GetChild(0).rotation);

        // pushing frame little bit behind image
        CreateFrameHelper.instance.Frame.transform.position = new Vector3(CreateFrameHelper.instance.Frame.transform.position.x, CreateFrameHelper.instance.Frame.transform.position.y, CreateFrameHelper.instance.Frame.transform.position.z- zOffset);


    }

    // this function creates frames with deapth
    public void CreateCubeFrame()
    {
        // this function finds the 4 croners of image/raw image
        GetSpriteCorners(render);
        // this function creats and spwans cubes on 4 corners 
        SpawnCubeOnPoint();
        // this function sets the cube size and creates the frame
        SetSizeForCubes();
       
    }

   

    public void GetSpriteCorners(GameObject renderer)
    {
        FilterImageType(renderer);

        if (isSpriteRanderer)
        {
            // getting 4 corners of sprite rander
            topRight = Newrenderer.transform.TransformPoint(Newrenderer.sprite.bounds.max);
            botRight = Newrenderer.transform.TransformPoint(new Vector3(Newrenderer.sprite.bounds.max.x, Newrenderer.sprite.bounds.min.y, 0.0f));
            botLeft = Newrenderer.transform.TransformPoint(Newrenderer.sprite.bounds.min);
            topLeft = Newrenderer.transform.TransformPoint(new Vector3(Newrenderer.sprite.bounds.min.x, Newrenderer.sprite.bounds.max.y, 0.0f));

        }
        else 
        {
            // scaling down orginal raw image accoring to world size 
            render.GetComponent<RectTransform>().sizeDelta = new Vector2(4f, 4f);

            // getting 4 corners for raw image
            Vector3[] corners = new Vector3[4];
            rawImage.rectTransform.GetWorldCorners(corners);
            botLeft = corners[0];
            topLeft = corners[1];
            topRight = corners[2];
            botRight = corners[3];
        }


    }

   

    // filtering if gameobject has sprite randerer or raw image
    void FilterImageType(GameObject G)
    {
        if (G.TryGetComponent<SpriteRenderer>(out SpriteRenderer SpriteRenderer))
        {
            Newrenderer = SpriteRenderer;

            isSpriteRanderer = true;

        }
        else if (G.TryGetComponent<RawImage>(out RawImage raw))
        {
            rawImage = raw;
            isSpriteRanderer = false;
        }


    }

    // this function creats and spwans cubes on 4 corners 
    public void SpawnCubeOnPoint()
    {
      // create cube with the default size of 1,1,1
        for (int i = 0; i < 4; i++)
        {
            CreateFrameHelper.instance.CreateCube(1, 1, CreateFrameHelper.instance.Materials, true);
           
            FramePoints.Add(CreateFrameHelper.instance.FrameCube);

        }

        // setting cubes on the corner points of image/raw image
        FramePoints[0].transform.SetPositionAndRotation(botLeft, render.transform.rotation); 
        FramePoints[1].transform.SetPositionAndRotation(botLeft, render.transform.rotation); 
        FramePoints[2].transform.SetPositionAndRotation(topLeft, render.transform.rotation);
        FramePoints[3].transform.SetPositionAndRotation(botRight, render.transform.rotation); 

    }


    // this function sets the cube size and creates the frame
    void SetSizeForCubes()
    {
        if (isSpriteRanderer) // if gameobj has sprite randerer
        {
           
            // setting the size of frame according to sprite

            FramePoints[0].transform.localScale = new Vector3(Newrenderer.sprite.bounds.size.x, 0.15f, 0.1f);
            FramePoints[1].transform.localScale = new Vector3(0.15f, Newrenderer.sprite.bounds.size.y, 0.1f);

            FramePoints[2].transform.localScale = new Vector3(Newrenderer.sprite.bounds.size.x + 0.15f, 0.15f, -0.1f);
            FramePoints[3].transform.localScale = new Vector3(0.15f, Newrenderer.sprite.bounds.size.y, -0.1f);

            // making farme child of sprite 
            for (int i = 0; i < 4; i++)
            {

             FramePoints[i].transform.transform.SetParent(render.transform);

            }

            // scaling down whole sprite object according to world size
            render.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        else
        {
            RawTransfom = RawTransfom.GetComponent<RectTransform>();

            // setting the size of frame according to Raw image

            FramePoints[0].transform.localScale = new Vector3(RawTransfom.rect.width, 0.15f, -0.1f);
            FramePoints[1].transform.localScale = new Vector3(0.15f, RawTransfom.rect.height, -0.1f);

            FramePoints[2].transform.localScale = new Vector3(RawTransfom.rect.width + 0.15f, 0.15f, -0.1f);
            FramePoints[3].transform.localScale = new Vector3(0.15f, RawTransfom.rect.height, -0.1f);

            // making frame childe of main raw image
            for (int i = 0; i < 4; i++)
            {

                FramePoints[i].transform.transform.SetParent(render.transform);

            }

        }
        

    }

  


}
