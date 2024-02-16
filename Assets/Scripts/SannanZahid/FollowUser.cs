using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUser : MonoBehaviour
{
   // [HideInInspector]
    public Transform targ;
    public Transform boxerplayerTransform;
    public Transform newplayerTransform;
    public Vector3 Offset;
    public Transform MainCamera;
    public float divisable = 2f, divisableDistance = 1f, MultipleDistance = 1, FixedYOffset = 1.05f;
    public int Multiple = 1;
    int tempDistance, PreviousDistance;
    float tempper;

    /// <summary>
   public  float FixedYOffsetLabel = 1.34f;
   /* public float FixedYOffset2 = 0.89f;
    public float FixedYOffset3 = 0.89f;
   public  float FixedYOffset4 = 0.94f;
   public  float FixedYOffset5 = 1f;
   public  float FixedYOffset6 = 1.06f;
   public  float FixedYOffset7 = 1.2f;
   public  float FixedYOffset8 = 1.34f;
    public float FixedYOffset9 = 1.35f;*/
    /// </summary>
    private void OnEnable()
    {
        PreviousDistance = (int) Vector3.Distance(MainCamera.position, targ.position);
        BoxerNFTEventManager.OnNFTequip += (nft) => targ = boxerplayerTransform.transform;
        BoxerNFTEventManager.OnNFTUnequip += () => targ = newplayerTransform.transform;
    }

    private void OnDisable()
    {
        BoxerNFTEventManager.OnNFTequip -= (nft) => targ = boxerplayerTransform.transform;
        BoxerNFTEventManager.OnNFTUnequip -= () => targ = newplayerTransform.transform;

    }
    void Update()
    {
        tempDistance = (int) Vector3.Distance(MainCamera.position, targ.position);

        if(PreviousDistance > tempDistance)
            MultipleDistance = 1f;
        else
            MultipleDistance = -1f;

        tempper = Vector3.Distance(MainCamera.position, targ.position) / divisableDistance;
        FixedYOffset = FixedYOffsetLabel;
       /* if (tempper > 0.055f)
        {
            FixedYOffset = FixedYOffset1;//1
        }
        else if (tempper < 0.055f && tempper > 0.05f)
        {
            FixedYOffset = FixedYOffset2;//2
        }
        else if (tempper > 0.045f)
        {
            FixedYOffset = FixedYOffset3;//3
        }
        else if (tempper < 0.045f && tempper > 0.04f)
        {
            FixedYOffset = FixedYOffset4;//4
        }
        else if (tempper > 0.035f )
        {
            FixedYOffset = FixedYOffset5;//5
        }
        else if (tempper < 0.035f && tempper > 0.03f)
        {
            FixedYOffset = FixedYOffset6;//6
        }
        else if (tempper > 0.027f)
        {
            FixedYOffset = FixedYOffset7;//7
        }
        else if (tempper > 0.024f)
        {
            FixedYOffset = FixedYOffset8;//8
        }
        else if (tempper < 0.025f )
        {
            FixedYOffset = FixedYOffset9;//9
        }*/
        Offset = new Vector3(
                            Multiple * (MainCamera.position.x - targ.position.x) / divisable,
                            FixedYOffset + (MultipleDistance * (Vector3.Distance(MainCamera.position, targ.position) / divisableDistance)), 
                            Offset.z
                            );
        transform.position = targ.position + Offset;//Vector3.MoveTowards(transform.position, targ.position + Offset, Time.deltaTime);
        PreviousDistance = tempDistance;
    }
}
