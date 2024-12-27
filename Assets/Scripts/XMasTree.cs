using UnityEngine;

public class XMasTree : MonoBehaviour
{
    Hook[] hooks;

    void Start()
    {
        hooks = GetComponentsInChildren<Hook>();
    }

    public bool isComplete
    {
        get
        {
            if ((hooks != null) || (hooks.Length > 0))
            {
                foreach (var hook in hooks)
                {
                    if (!hook.isCorrect) return false;
                }
            }

            return true;
        }
    }
}
   