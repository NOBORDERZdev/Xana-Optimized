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
        runnersSelectionImage.color = new Color(1f, 0.212f, 0.827f, 1f);
        huntersSelectionImage.color = Color.black;
        runnerLeaderBoard.SetActive(true);
        hunterLeaderBoard.SetActive(false);
    }

    public void HuntersButtonClicked()
    {
        huntersSelectionImage.color = new Color(1f, 0.212f, 0.827f, 1f);
        runnersSelectionImage.color = Color.black;
        hunterLeaderBoard.SetActive(true);
        runnerLeaderBoard.SetActive(false);
    }
}
