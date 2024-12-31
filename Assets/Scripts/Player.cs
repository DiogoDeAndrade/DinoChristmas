using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] 
    PlayerInput    playerInput;
    [SerializeField, InputPlayer(nameof(playerInput))] 
    InputControl   clawControl;
    [SerializeField, InputPlayer(nameof(playerInput))] 
    InputControl   extendControl;
    [SerializeField, InputPlayer(nameof(playerInput))]
    InputControl    aimControl;
    [SerializeField]
    Transform       grabCenter;
    [SerializeField]
    LayerMask       grabMask;

    Grabber     grabber;
    Rigidbody2D grabberRB;
    Ball        prevHookedBall;

    void Start()
    {
        grabber = GetComponentInChildren<Grabber>();
        grabberRB = grabber.GetComponent<Rigidbody2D>();

        clawControl.playerInput = playerInput;
        extendControl.playerInput = playerInput;
        aimControl.playerInput = playerInput;
    }

    private void FixedUpdate()
    {
        if (prevHookedBall)
        {
            prevHookedBall.transform.position = grabCenter.position;
        }

        // Check if we have a ball hooked
        Ball hookedBall = GetHookedBall();
        if (prevHookedBall != hookedBall)
        {
            prevHookedBall?.SetPhysics(true);
        }
        prevHookedBall = hookedBall;

        var aimTowards = aimControl.GetAxis2();
        var upVector = new Vector2(-aimTowards.y, aimTowards.x);
        if (upVector.magnitude > 0.1f)
        {
            grabberRB.MoveRotation(Quaternion.LookRotation(Vector3.forward, upVector));
        }

        if (hookedBall)
        {
            hookedBall.SetPhysics(false);
            hookedBall.transform.position = grabCenter.position;
        }
    }

    void Update()
    {
        grabber.SetParameters(extendControl.GetAxis(), clawControl.GetAxis());        
    }

    Ball GetHookedBall()
    {
        if (clawControl.GetAxis() < 1.0f) return null;

        var objects = Physics2D.OverlapCircleAll(grabCenter.position, 10.0f, grabMask);
        foreach (var obj in objects)
        {
            var ball = obj.GetComponent<Ball>();
            if (ball)
            {
                if (!ball.isHooked)
                {
                    return ball;
                }
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (grabCenter)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(grabCenter.position, 10.0f);
        }
    }
}
