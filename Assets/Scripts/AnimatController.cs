using UnityEngine;

public class AnimatController : MonoBehaviour
{
    public GameObject controller;
    Animator animator;
    PlayerController playerController;

    float forward;
    float strafe;

    private void Start()
    {
        playerController = controller.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (animator)
        {
            forward = playerController.forward;
            strafe = playerController.strafe;
            animator.SetFloat("Forward", playerController.forward);
            animator.SetFloat("Strafe", playerController.strafe);
            //if (forward > 0.01 || forward < -0.01)
            //    animator.SetFloat("Forward", forward > 0 ? 1 : -1);
            //else
            //    animator.SetFloat("Forward", forward);
            //if (strafe > 0.01 || strafe < -0.01)
            //    animator.SetFloat("Strafe", strafe > 0 ? 1 : -1);
            //else
            //    animator.SetFloat("Strafe", strafe);
        }
    }
}
