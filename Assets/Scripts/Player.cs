using UnityEngine;
using UnityEngine.InputSystem;

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

    Grabber     grabber;
    Rigidbody2D grabberRB;

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
        var aimTowards = aimControl.GetAxis2();
        var upVector = new Vector2(-aimTowards.y, aimTowards.x);
        if (upVector.magnitude > 0.1f)
        {
            grabberRB.MoveRotation(Quaternion.LookRotation(Vector3.forward, upVector));
        }
    }

    void Update()
    {
        grabber.SetParameters(extendControl.GetAxis(), clawControl.GetAxis());        
    }
}
