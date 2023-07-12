using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDown : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool pressed;
    public float timer = 0;
    public bool tapAndHolded;
    public GameObject tapAndHoldPanel;

    void Update()
    {
        if (Input.touchCount > 0 && pressed)
        {
            Touch first = Input.GetTouch(0);

            if (first.phase == TouchPhase.Began)
            {
                timer = 0;
            }
            if (first.phase == TouchPhase.Stationary)
            {
                //Debug.LogError("Entered " + first.phase);
                timer += Time.deltaTime;
            }
            if (timer > 0.6f)
            {
                tapAndHolded = true;
                //Debug.LogError("Entered " + timer);
                this.GetComponent<LoginPageManager>().CheckWorld();
                if (this.GetComponent<FeedEventPrefab>() != null)
                    this.GetComponent<FeedEventPrefab>().m_WorldNameTH.text = this.GetComponent<FeedEventPrefab>().m_EnvironmentName;
                this.GetComponent<FeedEventPrefab>().OnClickPrefab();
                timer = 0;
            }
            if (first.phase == TouchPhase.Ended)
            {
                timer = 0;
            }
        }
        if (pressed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                timer = 0;
            }
            if (Input.GetMouseButton(0))
            {
                timer += Time.deltaTime;
                if (timer > 0.6)
                {
                    tapAndHolded = true;
                    this.GetComponent<LoginPageManager>().CheckWorld();
                    if (this.GetComponent<FeedEventPrefab>() != null)
                    {
                        this.GetComponent<FeedEventPrefab>().m_WorldNameTH.text = this.GetComponent<FeedEventPrefab>().m_EnvironmentName;
                        this.GetComponent<FeedEventPrefab>().OnClickPrefab();
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (timer > 0.6f)
                {
                    timer = 0;
                }

            }
        }
    }
        

    public void ResetTimer()
    {
        timer = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }
}
