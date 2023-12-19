using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


    public class ButtonAnimationScript : MonoBehaviour
    {
    public RectTransform ButtonReferencetobeAnimated;
    public Vector3 CurrentPos;
    public Vector3 MovingPosition;
    public GameObject ButtontoMove;


        public void moveButtonUp()
        {
        ButtonReferencetobeAnimated.DOLocalMoveY(MovingPosition.y, 0.2f);
        }

        public void moveButtonDown()
        {
        ButtonReferencetobeAnimated.DOLocalMoveY(CurrentPos.y, 0.2f);
        }

    }

