using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Title : UIGroup
{
    [SerializeField] UIButton startButton;
    [SerializeField] UIButton creditsButton;
    [SerializeField] UIButton quitButton;
    [SerializeField, Scene] string gameScene;
    [SerializeField] CanvasGroup buttonCanvas;
    [SerializeField] CanvasGroup creditsPanel;
    [SerializeField] AudioClip titleMusic;

    BigTextScroll creditsScroll;

    protected override void Start()
    {
        base.Start();

        Cursor.visible = false;

        if (creditsPanel)
        {
            creditsScroll = creditsPanel.GetComponentInChildren<BigTextScroll>();
        }

        if (startButton) startButton.onInteract += StartGame;
        if (creditsButton) creditsButton.onInteract += ShowCredits;
        if (quitButton) quitButton.onInteract += QuitGame;

        if (titleMusic)
        {
            SoundManager.PlayMusic(titleMusic);
        }
    }

    private void ShowCredits(BaseUIControl control)
    {
        _uiEnable = false;

        buttonCanvas.FadeOut(0.5f);

        creditsPanel.FadeIn(0.5f);

        creditsScroll.Reset();

        creditsScroll.onEndScroll += BackToOptions;
    }

    private void BackToOptions()
    {
        buttonCanvas.FadeIn(0.5f);

        creditsPanel.FadeOut(0.5f);

        _uiEnable = true;
        selectedControl = startButton;

        creditsScroll.onEndScroll -= BackToOptions;
    }

    void StartGame(BaseUIControl control)
    {
        _uiEnable = false;
        GameManager.StartGame();

        FullscreenFader.FadeOut(0.5f, Color.black, () =>
        {
            SceneManager.LoadScene(gameScene);
        });

    }

    private void QuitGame(BaseUIControl control)
    {
        _uiEnable = false;
        FullscreenFader.FadeOut(0.5f, Color.black, () =>
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#else
            Application.Quit();
#endif
        });
    }

}
