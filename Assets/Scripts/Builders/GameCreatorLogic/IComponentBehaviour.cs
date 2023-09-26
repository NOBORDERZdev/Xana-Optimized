public interface IComponentBehaviour
{
    public Constants.ItemComponentType ComponentType { get; }
    public void AssignItemComponentType();

    public void PlayBehaviour();

    public void StopBehaviour();

    public void ResumeBehaviour();

    public void ToggleBehaviour();
}