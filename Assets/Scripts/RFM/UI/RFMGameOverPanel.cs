using UnityEngine;

public class RFMGameOverPanel : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image runnersSelectionImage;
    [SerializeField] private UnityEngine.UI.Image huntersSelectionImage;

    [SerializeField] private GameObject runnerLeaderBoard;
    [SerializeField] private GameObject hunterLeaderBoard;

    private void OnEnable()
    {
        RunnersButtonClicked();
    }

    public void RunnersButtonClicked()
    {
        runnersSelectionImage.color = Color.white;
        huntersSelectionImage.color = Color.black;
        runnerLeaderBoard.SetActive(true);
        hunterLeaderBoard.SetActive(false);
    }

    public void HuntersButtonClicked()
    {
        huntersSelectionImage.color = Color.white;
        runnersSelectionImage.color = Color.black;
        hunterLeaderBoard.SetActive(true);
        runnerLeaderBoard.SetActive(false);
    }
}
