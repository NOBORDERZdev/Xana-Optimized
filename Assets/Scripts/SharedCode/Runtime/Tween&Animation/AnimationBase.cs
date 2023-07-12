using UnityEngine; 
using UnityEngine.Events; 

public abstract class AnimationBase : MonoBehaviour
{
    public bool startOnEnable;
    public bool loop;

    public UnityEvent onStart;
    public UnityEvent onEnd;  

    public virtual float Duration
    {
        get { return 0; }
        set { }
    }

    public abstract void StartAnim();   
    public abstract void StopAnim();
    public abstract void ResetAnim();

    public virtual void AddCallbackOnStart(UnityAction act)
    {
        onStart.AddListener(act);
    }
    public virtual void RemoveCallbackFromStart(UnityAction act)
    {
        onStart.RemoveListener(act);
    }
    public virtual void AddCallbackOnEnd(UnityAction act)
    {
        onEnd.AddListener(act);
    }
    public virtual void RemoveCallbackFromEnd(UnityAction act)
    {
        onEnd.RemoveListener(act);
    }
}