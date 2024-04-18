using UnityEngine;

public class FrameHandler : MonoBehaviour
{
    public GameObject[] frames;

    // Start is called before the first frame update
    public void Start()  // also call by unityEvent that's why it is public method
    {
        foreach (GameObject frame in frames) { frame.SetActive(false); }
    }

    public void UpdateFrame(int frameInd)
    {
        frames[frameInd].SetActive(true);
    }

}
