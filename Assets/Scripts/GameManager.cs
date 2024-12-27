using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int maxLevel = 3;

    int _currentLevel = 1;

    public static int currentLevel => Instance._currentLevel;

    private static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void _StartGame()
    {
        _currentLevel = 1;
    }

    bool _NextLevel()
    {
        if (_currentLevel < maxLevel)
        {
            _currentLevel++;
            return true;
        }

        return false;
    }

    public static void StartGame() { Instance._StartGame(); }
    public static bool NextLevel() {  return Instance._NextLevel(); }
}
