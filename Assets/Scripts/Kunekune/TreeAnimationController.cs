using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class TreeAnimationController : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        
        if (anim != null)
        {
            anim.enabled = false;
        }
    }

    void OnBecameVisible()
    {
        if (anim != null)
        {
            anim.enabled = true; 
        }
    }

    void OnBecameInvisible()
    {
        if (anim != null)
        {
            anim.enabled = false;
        }
    }
}