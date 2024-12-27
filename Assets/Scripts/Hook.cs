using System.Runtime.CompilerServices;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private float      tolerance = 5.0f;
    [SerializeField] private BallColor  _hookColor;
    [SerializeField] private AudioClip  rightSound;
    [SerializeField] private AudioClip  wrongSound;

    Ball hookedBall;
    float cooldown;

    public BallColor hookColor => _hookColor;
    public bool isCorrect => (hookedBall != null) && (hookedBall.ballColor == _hookColor);

    void Start()
    {
        
    }

    void Update()
    {
        if (cooldown > 0.0f)
        {
            cooldown -= Time.deltaTime;
        }
        else if (hookedBall == null)
        {
            // Find a ball
            var balls = FindObjectsByType<Ball>(FindObjectsSortMode.None);
            foreach (var ball in balls)
            {
                if (ball.isHooked) continue;

                float distance = Vector3.Distance(ball.transform.position.xy0(), transform.position.xy0());
                if (distance < tolerance)
                {
                    // Hook this ball
                    ball.Hook(this);
                    hookedBall = ball;

                    if (hookedBall.ballColor == hookColor)
                    {
                        if (rightSound) SoundManager.PlaySound(SoundType.PrimaryFX, rightSound, 1.0f, Random.Range(0.9f, 1.1f));
                    }
                    else
                    {
                        if (wrongSound) SoundManager.PlaySound(SoundType.PrimaryFX, wrongSound, 1.0f, Random.Range(0.9f, 1.1f));
                    }
                }
            }
        }
    }

    public void Release()
    {
        if (hookedBall != null)
        {
            hookedBall.Hook(null);
            hookedBall = null;
            cooldown = 2.0f;
        }
    }
}
