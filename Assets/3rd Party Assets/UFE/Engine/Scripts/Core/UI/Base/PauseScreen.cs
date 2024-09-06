namespace UFE3D
{
    public class PauseScreen : UFEScreen
    {

        public int backToMenuFrameDelay = 6;

        public virtual void GoToMainMenu()
        {
            UFE.DelayLocalAction(GoToMainMenuDelayed, backToMenuFrameDelay);
        }

        private void GoToMainMenuDelayed()
        {
            UFE.EndGame();
            UFE.PauseGame(false);
            UFE.StartMainMenuScreen();
            Destroy(this.gameObject, 1);
        }

        public virtual void ResumeGame()
        {
            UFE.PauseGame(false);
        }
    }
}