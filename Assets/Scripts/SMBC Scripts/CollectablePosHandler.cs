using UnityEngine;

public class CollectablePosHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _collectableObject;
    [SerializeField]
    private Transform[] _collectablePositions;

    private void Start()
    {
        _collectableObject.transform.position = _collectablePositions[Random.Range(0, _collectablePositions.Length)].position;
    }
}
