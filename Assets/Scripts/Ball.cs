using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private BallColor  _ballColor;
    [SerializeField] private float      settleTime = 2.0f;
    [SerializeField] private float      dislodgeVelocity = 50.0f;

    Hook            hook;
    Rigidbody2D     rb;
    SpriteRenderer  spriteRenderer;
    float           hookTime;
    Vector3         startScale;
    Color           startColor;
    Vector3         startPos;

    public bool isHooked => (hook != null);
    public BallColor ballColor => _ballColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();

        startScale = transform.localScale;
        startColor = spriteRenderer.color;
        startPos = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) > 2000.0f)
        {
            // Reset ball
            transform.position = startPos;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0;
            transform.localScale = startScale;
            spriteRenderer.color = startColor;
        }
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
                    collision.otherRigidbody.linearVelocity = vec * 300.0f;
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
            if (hook.hookColor == ballColor)
            {
                transform.localScale = startScale * 1.5f;
                spriteRenderer.color = startColor;
            }
            else
            {
                transform.localScale = startScale * 1.0f;
                spriteRenderer.color = startColor.ChangeAlpha(0.5f);
            }
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            this.hook = null;
            transform.localScale = startScale;
            spriteRenderer.color = startColor;
        }
    }

    public void SetPhysics(bool enable)
    {
        if (enable)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0.0f;
        }
    }
}
