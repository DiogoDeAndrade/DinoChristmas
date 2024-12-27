using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    enum GameState { Play, Complete, GameOver };

    [SerializeField] PlayerInput    playerInput;
    [SerializeField, InputPlayer("playerInput"), InputButton] 
    InputControl   continueControl;
    [SerializeField] CanvasGroup        levelCompleteGroup;
    [SerializeField] CanvasGroup        gameOverGroup;
    [SerializeField] TextMeshProUGUI    levelTimer;

    [SerializeField] Level[]        levelPrefabs;

    XMasTree[]  trees;
    GameState   state;
    Level       level;
    float       currentTime = 0.0f;

    void Start()
    {
        state = GameState.Play;
        continueControl.playerInput = playerInput;

        FullscreenFader.FadeIn(1.0f);

        level = FindAnyObjectByType<Level>(FindObjectsInactive.Include);
        if (level == null)
        {
            // Instantiate level
            level = Instantiate(levelPrefabs[GameManager.currentLevel - 1]);
        }

        var cg = levelTimer.GetComponent<CanvasGroup>();
        if (level.timeInSeconds > 0)
        {
            cg.FadeIn(0.25f);
            currentTime = level.timeInSeconds;
        }
        else
        {
            cg.FadeOut(0.25f);
            currentTime = 0;
        }

        trees = FindObjectsByType<XMasTree>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Play)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                FullscreenFader.FadeOut(1.0f, Color.black, () =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                });
            }

            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    state = GameState.GameOver;
                    gameOverGroup.FadeIn(0.5f);
                }

                int mins = Mathf.Max(0, Mathf.FloorToInt(currentTime / 60.0f));
                int secs = Mathf.Max(0, Mathf.FloorToInt(currentTime % 60.0f));

                levelTimer.text = $"{mins.ToString("D2")}:{secs.ToString("D2")}";
            }

            if ((trees != null) && (trees.Length > 0))
            {
                bool isComplete = true;
                foreach (var tree in trees)
                {
                    if (tree != null)
                    {
                        if (!tree.isComplete)
                        {
                            isComplete = false;
                            break;
                        }
                    }
                }
                if (isComplete)
                {
                    state = GameState.Complete;
                    levelCompleteGroup.FadeIn(0.5f);
                }
            }
        }
        else if (state == GameState.Complete)  
        {
            if (continueControl.IsDown())
            {
                FullscreenFader.FadeOut(1.0f, Color.black, () =>
                {
                    if (GameManager.NextLevel())
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                    else
                    {
                        SceneManager.LoadScene("EndGame");
                    }
                });
            }
        }
        else if (state == GameState.GameOver)
        {
            if (continueControl.IsDown())
            {
                FullscreenFader.FadeOut(1.0f, Color.black, () =>
                {
                    SceneManager.LoadScene(0);
                });
            }
        }
    }
}
