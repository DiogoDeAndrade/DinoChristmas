using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int _timeInSeconds = 0;

    public int timeInSeconds => _timeInSeconds;
}
