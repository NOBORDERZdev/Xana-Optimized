using UnityEngine;

public class RFMGameOverPanel : MonoBehaviour
{
    [SerializeField] private GameObject runnersSelectionImage;
    [SerializeField] private GameObject huntersSelectionImage;

    [SerializeField] private GameObject runnerLeaderBoard;
    [SerializeField] private GameObject hunterLeaderBoard;

    private void OnEnable()
    {
        RunnersButtonClicked();
    }

    public void RunnersButtonClicked()
    {
        runnersSelectionImage.SetActive(true);
        huntersSelectionImage.SetActive(false);
        runnerLeaderBoard.SetActive(true);
        hunterLeaderBoard.SetActive(false);
    }

    public void HuntersButtonClicked()
    {
        huntersSelectionImage.SetActive(true);
        runnersSelectionImage.SetActive(false);
        hunterLeaderBoard.SetActive(true);
        runnerLeaderBoard.SetActive(false);
    }
}
