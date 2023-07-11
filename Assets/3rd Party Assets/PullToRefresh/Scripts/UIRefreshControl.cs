using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/*
    MIT License

    Copyright (c) 2018 kiepng

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
namespace PullToRefresh
{
    public class UIRefreshControl : MonoBehaviour
    {
        [Serializable] public class RefreshControlEvent : UnityEvent {}

        //[SerializeField] private ScrollRect m_ScrollRect;
        [SerializeField] private ScrollRectFasterEx m_ScrollRect;
        [SerializeField] private float m_PullDistanceRequiredRefresh = 150f;
        [SerializeField] private Animator m_LoadingAnimator;
        //[SerializeField] RefreshControlEvent m_OnRefresh = new RefreshControlEvent();
        public GameObject loaderObj;
      
        private float m_InitialPosition;
        private float m_Progress;
        public bool m_IsPulled;
        public bool m_IsRefreshing;
        private Vector2 m_PositionStop;
        //private IScrollable m_ScrollView;

        /// <summary>
        /// Progress until refreshing begins. (0-1)
        /// </summary>
        public float Progress
        {
            get { return m_Progress; }
        }

        /// <summary>
        /// Refreshing?
        /// </summary>
        public bool IsRefreshing
        {
            get { return m_IsRefreshing; }
        }

        /// <summary>
        /// Callback executed when refresh started.
        /// </summary>
        /*public RefreshControlEvent OnRefresh
        {
            get { return m_OnRefresh; }
            set { m_OnRefresh = value; }
        }*/

         /// <summary>
        /// Call When Refresh is End.
        /// </summary>
        public void EndRefreshing()
        {
            m_IsPulled = false;
            m_IsRefreshing = false;
            loaderObj.GetComponent<RectTransform>().anchoredPosition = loaderStartPos;
            loaderObj.SetActive(false);
            //m_LoadingAnimator.SetBool(_activityIndicatorStartLoadingName, false);
        }

        const string _activityIndicatorStartLoadingName = "Loading";

        public Vector3 loaderStartPos;


        private void Start()
        {

            m_InitialPosition = GetContentAnchoredPosition();
            m_PositionStop = new Vector2(m_ScrollRect.content.anchoredPosition.x, m_InitialPosition - m_PullDistanceRequiredRefresh);
            //m_ScrollView = m_ScrollRect.GetComponent<IScrollable>();
            //m_ScrollRect.onValueChanged.AddListener(OnScroll);
            loaderStartPos = loaderObj.GetComponent<RectTransform>().anchoredPosition;

            //Debug.LogError("InitialPos:" + m_InitialPosition + ":Name:"+this.transform.parent.parent.name);

        }


        private void LateUpdate()
        {
            if (m_ScrollRect.isClaimedDragging)
            {
                var distance = m_InitialPosition - GetContentAnchoredPosition();
                //Debug.LogError("Distance:" + distance + ":m_InitialPosition:"+ m_InitialPosition +":anchor:"+ GetContentAnchoredPosition());
                if (distance >= -1f)
                {
                    var tempDistance = m_ScrollRect.m_InitialPosition - m_ScrollRect.currentDragPos;
                    float tempPos = tempDistance / m_PullDistanceRequiredRefresh;
                    //Debug.LogError("tempPos:" + tempPos + ":InitalPos:"+ m_ScrollRect.m_InitialPosition + ":CurrentPos:"+ m_ScrollRect.currentDragPos);
                    
                    OnScroll1(tempPos, tempDistance);
                }
            }
            else
            {              
                if (m_IsPulled)
                {
                    CheckForRefreshing();
                    m_IsPulled = false;
                }
                else
                {
                    if (loaderObj.activeSelf && !m_IsRefreshing)
                    {
                        loaderObj.GetComponent<RectTransform>().anchoredPosition = loaderStartPos;
                        loaderObj.SetActive(false);
                    }
                }
            }
            return;

            /*if (!m_IsPulled)
            {
                return;
            }

            if (!m_IsRefreshing)
            {
                return;
            }

            m_ScrollRect.content.anchoredPosition = m_PositionStop;*/
        }

        private void OnScroll1(float distance , float Ypos)
        {
            //Debug.LogError("Distance:" + distance + ":Ypos:" + Ypos);
            /*if (loaderObj.activeSelf)
            {
                FeedUIController.Instance.fingerTouch.SetActive(false);
            }
            else
            {
                FeedUIController.Instance.fingerTouch.SetActive(true);
            }*/

            if (distance > 0.05f)
            {
                if (loaderObj.gameObject.GetComponent<RectTransform>().anchoredPosition.y >= -150)
                {
                    loaderObj.SetActive(true);
                    //loaderObj.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -Ypos, 0);
                    float yp = distance * 35;
                    if (yp > 150)
                    {
                        yp = 150;
                    }
                    loaderObj.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -yp, 0);
                }
                else
                {
                    loaderObj.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -151f, 0);
                }
            }
            else
            {
                loaderObj.GetComponent<RectTransform>().anchoredPosition = loaderStartPos;
                loaderObj.SetActive(false);
            }

            if (distance < 0f)
            {
                return;
            }
            OnPull1(distance);
        }

        private void OnPull1(float distance)
        {
            if (m_IsRefreshing && Math.Abs(distance) < 3.5f)
            {
                m_IsRefreshing = false;
            }

            m_Progress = distance / m_PullDistanceRequiredRefresh;

            if (distance < 3.5f)
            {
                m_IsPulled = false;
                return;
            }

            // Start animation when you reach the required distance while dragging.
            m_IsPulled = true;

            //CheckForRefreshing();

            m_Progress = 0f;
        }

        void CheckForRefreshing()
        {
            if (m_IsPulled && !m_ScrollRect.isClaimedDragging)
            {
                m_IsRefreshing = true;
                //m_OnRefresh.Invoke();
                Debug.LogError("gggg name:" + this.gameObject.name);
                LoaderController.Instance.m_UIRefreshControl = this;
                LoaderController.Instance.RefreshItems();
            }
        }

        private float GetContentAnchoredPosition()
        {
            return m_ScrollRect.content.anchoredPosition.y;
        }
    }
}