using System.Runtime.CompilerServices;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private float tolerance = 5.0f;

    Ball hookedBall;
    float cooldown;

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
