using UnityEngine;

public class CircleFadeScript : MonoBehaviour
{

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartCircleFadeOut()
    {
        animator.SetTrigger("StartFadeOut");
    }

}
