using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DynamicScrollRect
{
    [Serializable]    
    public class DynamicScrollRestrictionSettings
    {
        public float ContentOverflowRange =  125f;
        public float ContentDecelerationInOverflow = 0.5f;
    }
    public class DynamicScrollRect : ScrollRect
    {
       /* [SerializeField] private DynamicScrollRestrictionSettings _restrictionSettings = null;
        private bool _isDragging = false;
        private bool _runningBack = false;
        private bool _needRunBack = false;
        public  Vector2 _contentStartPos = Vector2.zero;
        private Vector2 _dragStartingPosition = Vector2.zero;
        private Vector2 _dragCurPosition = Vector2.zero;
        private Vector2 _lastDragDelta = Vector2.zero;
        private IEnumerator _runBackRoutine;
        private ScrollContent _content;
        public Transform Burka;
        private ScrollContent _Content
        {
            get
            {
                if (_content == null)
                {
                    _content = GetComponentInChildren<ScrollContent>();
                }

                return _content;
            }
        }
        protected override void Awake()
        {
            movementType = MovementType.Unrestricted;
            onValueChanged.AddListener(OnScrollRectValueChanged);
            base.Awake();
        }
        protected override void OnDestroy()
        {
            onValueChanged.RemoveListener(OnScrollRectValueChanged);
            base.OnDestroy();
        }
        #region Event Callbacks
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!ParentSliderFlag)
            {
                TopScroller.OnBeginDrag(eventData);
                return;
            }
            base.OnBeginDrag(eventData);
            StopRunBackRoutine();
            _isDragging = true;
            _contentStartPos = content.anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewport,
                eventData.position,
                eventData.pressEventCamera,
                out _dragStartingPosition);
            _dragCurPosition = _dragStartingPosition;
        }
        public bool StateBlock = false;
        protected override void LateUpdate()
        {
            
            base.LateUpdate();
            if (!StateBlock)
            {
                if (TopScroller.verticalNormalizedPosition > ParentSliderLimitCheck)
                {
                    if (content.anchoredPosition.y < 0f)
                        content.anchoredPosition = Vector2.zero;
                }
   
            }
        }
        public bool RestrictFlag = false;
        private void Update()
        {
            if(RestrictFlag)
            {
                //TopScroller.vertical = false;   // main scroll view is off due to some reason
                ParentSliderFlag = false;
                return;
            }
            if(!StateBlock)
            {
                if (TopScroller.verticalNormalizedPosition > ParentSliderLimitCheck)
                {
                    ParentSliderFlag = false;
                    if (content.anchoredPosition.y < 0f)
                        content.anchoredPosition = Vector2.zero;
                    return;
                }
                else
                {
                    if (verticalNormalizedPosition > 0.999f)
                    {
                        TopScroller.verticalNormalizedPosition = ParentSliderLimitCheck + 0.001f;
                        content.anchoredPosition = Vector2.zero;
                        ParentSliderFlag = false;
                        TopScroller.vertical = true;

                    }
                    else
                    {
                        //TopScroller.vertical = false;    // main scroll view is off due to some reason
                        ParentSliderFlag = true;
                    }
                }
            }
            else
            {
                ParentSliderFlag = true;
            }
        }
        float ParentSliderLimitCheck = 0.01517484f;
        public bool ParentSliderFlag = false;
        int SliderStateMachine = 0;
        public override void OnDrag(PointerEventData eventData)
        {
            if (!ParentSliderFlag)
            {
                TopScroller.OnDrag(eventData);
                return;
            }
            else
            {
                base.OnDrag(eventData);
            }
            if (!_isDragging)
            {
                return;
            }
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    viewRect,
                    eventData.position,
                    eventData.pressEventCamera, out Vector2 localCursor))
            {
                return;
            }
            StopRunBackRoutine();
            if (!IsDragValid(localCursor - _dragCurPosition))
            {
                Vector2 restrictedPos = GetRestrictedContentPositionOnDrag(eventData);
                _needRunBack = true;
                SetContentAnchoredPosition(restrictedPos);
                return;
            }
            UpdateBounds();
            _needRunBack = false;
            _lastDragDelta = localCursor - _dragCurPosition;
            _dragCurPosition = localCursor;
            SetContentAnchoredPosition(CalculateContentPos(localCursor));
            UpdateItems(_lastDragDelta);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!ParentSliderFlag)
            { 
                TopScroller.OnEndDrag(eventData);
                return; 
            }
            base.OnEndDrag(eventData);
            _isDragging = false;
            if (_needRunBack)
            {
                StopMovement();
                StartRunBackRoutine();
            }
        }
        private void OnScrollRectValueChanged(Vector2 val)
        {
            if (!ParentSliderFlag)
            {
                return;
            }
            if (_runningBack || _isDragging)
            {
                return;
            }
            Vector2 delta = velocity.normalized;
            if (!IsDragValid(delta))
            {
                Vector2 contentPos = GetRestrictedContentPositionOnScroll(delta);
                SetContentAnchoredPosition(contentPos);
                if ((velocity * Time.deltaTime).magnitude < 5)
                {
                    StopMovement();
                    StartRunBackRoutine();
                }
                return;
            }
            UpdateItems(delta);
        }
        #endregion
  
        private void UpdateItems(Vector2 delta)
        {
            bool positiveDelta = delta.y > 0;
           //Debug.LogError("positiveDelta: " + positiveDelta+":---------:"+ (-_Content.GetLastItemPos().y - content.anchoredPosition.y <= viewport.rect.height + _Content.Spacing.y));
            if (-_Content.GetLastItemPos().y - content.anchoredPosition.y <= viewport.rect.height + _Content.Spacing.y)
            {
                _Content.AddIntoTail();
            }

            if (positiveDelta &&
                content.anchoredPosition.y - -_Content.GetFirstItemPos().y >= (2 * _Content.ItemHeight) + _Content.Spacing.y)
            {
                _Content.DeleteFromHead();
            }

            if (!positiveDelta &&
                content.anchoredPosition.y + _Content.GetFirstItemPos().y <= _Content.ItemHeight + _Content.Spacing.y)
            {
                _Content.AddIntoHead();
            }

            if (!positiveDelta &&
                -_Content.GetLastItemPos().y - content.anchoredPosition.y >= viewport.rect.height + _Content.ItemHeight + _Content.Spacing.y)
            {
                _Content.DeleteFromTail();
            }
        }
        private bool IsDragValid(Vector2 delta)
        {
            return CheckDragValidVertical(delta);
        }
        private bool CheckDragValidVertical(Vector2 delta)
        {
            bool positiveDelta = delta.y > 0;
            if (positiveDelta)
            {
                Vector2 lastItemPos = _Content.GetLastItemPos();
                if (!_Content.CanAddNewItemIntoTail() && 
                    content.anchoredPosition.y + viewport.rect.height + lastItemPos.y - _Content.ItemHeight > 0)
                {
                    return false;
                }
            }
            else
            {
                if (!_Content.CanAddNewItemIntoHead() &&
                    content.anchoredPosition.y <= 0)
                {
                    return false;
                }
            }
            return true;
        }
        private Vector2 GetRestrictedContentPositionOnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                viewRect,
                eventData.position,
                eventData.pressEventCamera, out Vector2 localCursor);

            Vector2 delta = localCursor - _dragCurPosition;
            Vector2 position = CalculateContentPos(localCursor);
            float restriction = GetVerticalRestrictionWeight(delta);
            Vector2 result = CalculateRestrictedPosition(content.anchoredPosition, position, restriction);
            result.x = content.anchoredPosition.x;
            return result;
        }
        private Vector2 GetRestrictedContentPositionOnScroll(Vector2 delta)
        {
            float restriction = GetVerticalRestrictionWeight(delta);
            Vector2 deltaPos = velocity * Time.deltaTime;
            deltaPos.x = 0;
            Vector2 curPos = content.anchoredPosition;
            Vector2 nextPos = curPos + deltaPos;
            Vector2 res = CalculateRestrictedPosition(curPos, nextPos, restriction);
            res.x = 0;
            velocity *= _restrictionSettings.ContentDecelerationInOverflow;
            return res;
        }
        private float GetVerticalRestrictionWeight(Vector2 delta)
        {
            bool positiveDelta = delta.y > 0;
            float maxLimit = _restrictionSettings.ContentOverflowRange;
            if (positiveDelta)
            {
                Vector2 lastItemPos = _Content.GetLastItemPos();
                if (Mathf.Abs(lastItemPos.y) <= viewport.rect.height - _Content.ItemHeight)
                {
                    float max = lastItemPos.y + maxLimit;
                    float cur = content.anchoredPosition.y + lastItemPos.y;
                    float diff = max - cur;
                    return 1f - Mathf.Clamp(diff / maxLimit, 0, 1);
                }
                else
                {
                    float max = -(viewport.rect.height - maxLimit - _Content.ItemHeight);
                    float cur = content.anchoredPosition.y + lastItemPos.y;
                    float diff = max - cur;
                    return 1f - Mathf.Clamp(diff / maxLimit, 0, 1);
                }
            }

            float restrictionVal = Mathf.Clamp(Mathf.Abs(content.anchoredPosition.y) / maxLimit, 0, 1);
            return restrictionVal;
        }
        public HomeScreenScrollHandler TopScroller;
        private Vector2 CalculateSnapPosition()
        {
            if (!ParentSliderFlag)
            {
                return default;
            }
            if (content.anchoredPosition.y < 0)
            {
                return Vector2.zero;
            }
            else
            {
                Vector2 lastItemPos = _Content.GetLastItemPos();
                if (Mathf.Abs(lastItemPos.y) <= viewport.rect.height - _Content.ItemHeight)
                {                    
                    return Vector2.zero;
                }
                float target = -(viewport.rect.height - _Content.ItemHeight);
                float cur = content.anchoredPosition.y + lastItemPos.y;
                float diff = target - cur;
                return content.anchoredPosition + new Vector2(0, diff);
            }
        }
        private Vector2 CalculateContentPos(Vector2 localCursor)
        {
            Vector2 dragDelta = localCursor - _dragStartingPosition;
            Vector2 position = _contentStartPos + dragDelta;
            return position;
        }
        private Vector2 CalculateRestrictedPosition(Vector2 curPos, Vector2 nextPos, float restrictionWeight)
        {
            Vector2 weightedPrev = curPos * restrictionWeight;
            Vector2 weightedNext = nextPos * (1 - restrictionWeight);
            Vector2 result = weightedPrev + weightedNext;
            return result;
        }

        #region Run Back Routine
        private void StartRunBackRoutine()
        {
            StopRunBackRoutine();
            _runBackRoutine = RunBackProgress();
            StartCoroutine(_runBackRoutine);
        }
        private void StopRunBackRoutine()
        {
            if (_runBackRoutine != null)
            {
                StopCoroutine(_runBackRoutine);
                _runningBack = false;
            }
        }
        private IEnumerator RunBackProgress()
        {
            _runningBack = true;
            float timePassed = 0;
            float duration = 0.25f;
            Vector2 startPos = content.anchoredPosition;
            Vector2 endPos = CalculateSnapPosition();
            while (timePassed < duration)
            {
                if (!ParentSliderFlag)
                {
                    StopRunBackRoutine();
                    break;
                }
                timePassed += Time.deltaTime;
                Vector2 pos = Vector2.Lerp(startPos, endPos, timePassed / duration);
                SetContentAnchoredPosition(pos);
                yield return null;
            }
            _runningBack = false;
        }
        #endregion*/
    }
}