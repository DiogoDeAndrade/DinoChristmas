using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float settleTime = 2.0f;
    [SerializeField] private float dislodgeVelocity = 50.0f;

    Hook hook;
    Rigidbody2D rb;
    float       hookTime;
    Vector3     startScale;

    public bool isHooked => (hook != null);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startScale = transform.localScale;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hook == null) return;
        if (Time.time - hookTime > settleTime)
        {
            float hitSpeed = collision.relativeVelocity.magnitude;

            var grabber = collision.gameObject.GetComponent<Grabber>();
            if (grabber)
            {
                hitSpeed = grabber.clawSpeed;
            }

            if (hitSpeed > dislodgeVelocity)
            {
                // Unhook
                hook.Release();

                if (grabber)
                {
                    // Just kick it up somewhere, more fun!
                    // Get contact and push
                    var vec = collision.contacts[0].normal;
                    vec.y = 1.0f;
                    vec.Normalize();
                    collision.otherRigidbody.linearVelocity = vec * 500.0f;
                }
            }
        }
    }

    public void Hook(Hook hook)
    {
        if (hook)
        {
            transform.position = hook.transform.position;
            this.hook = hook;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0;
            hookTime = Time.time;
            transform.localScale = startScale * 1.5f;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            this.hook = null;
            transform.localScale = startScale;
        }
    }
}
