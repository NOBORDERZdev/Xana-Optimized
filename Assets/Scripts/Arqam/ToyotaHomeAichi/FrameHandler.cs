using UnityEngine;

public class FrameHandler : MonoBehaviour
{
    public GameObject[] frames;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject frame in frames) { frame.SetActive(false); }
    }

    public void UpdateFrame(int frameInd)
    {
        frames[frameInd].SetActive(true);
    }

}
