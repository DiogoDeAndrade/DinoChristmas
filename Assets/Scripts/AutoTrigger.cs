using NaughtyAttributes;
using UnityEngine;

public class AutoTrigger : MonoBehaviour
{
    [SerializeField, MinMaxSlider(0.1f, 10.0f)]
    private Vector2 duration = new Vector2(5.0f, 5.0f);
    [SerializeField, Range(0.0f, 1.0f)]
    private float   probability = 1.0f;
    [SerializeField]
    private ProbList<string> triggers;

    Animator animator;
    float    timer;

    void Start()
    {
        animator = GetComponent<Animator>();
        timer = duration.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                timer = duration.Random();

                if (Random.Range(0.0f, 1.0f) <= probability)
                {
                    var triggerName = triggers.Get();
                    if (!string.IsNullOrEmpty(triggerName))
                    {
                        animator.SetTrigger(triggerName);
                    }
                }
            }
        }
    }
}
