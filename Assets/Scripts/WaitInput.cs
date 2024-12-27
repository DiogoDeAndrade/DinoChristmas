using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class WaitInput : MonoBehaviour
{
    [SerializeField, Scene] private string nextScene;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            FullscreenFader.FadeOut(0.5f, Color.black, () =>
            {
                SceneManager.LoadScene(nextScene);
            });
        }
    }
}
