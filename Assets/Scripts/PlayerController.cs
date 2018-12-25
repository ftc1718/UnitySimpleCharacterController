using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public Camera cam;
    public FixedJoystick leftJoystick;

    public float rotateSpeed = 0.5f;
    public float moveSpeed = 2.5f;

    CharacterController characterController;

    Vector3 characterMovement = new Vector3(0, 0, 0);
    [HideInInspector]
    public float forward;
    [HideInInspector]
    public float strafe;
    float fallDown;

    private void Start()
    {
        if (player == null)
            this.enabled = false;
        else
            characterController = player.GetComponent<CharacterController>();
    }

    private void Update()
    {
        forward = Input.GetAxisRaw("Vertical");
        strafe = Input.GetAxisRaw("Horizontal");

        if(leftJoystick != null && leftJoystick.inputVector != Vector2.zero)
        {
            forward += leftJoystick.inputVector.y;
            strafe += leftJoystick.inputVector.x;
        }

        MoveControl(forward, strafe);
        ApplyGravity();
    }

    void MoveControl(float forward, float strafe)
    {
        characterMovement = Vector3.zero;

        if (strafe != 0 || forward != 0)
        {
            Vector3 camForward = cam.transform.forward;
            Vector3 camRight = cam.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();
            Vector3 desiredMoveDirection = camForward * forward + camRight * strafe;

            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotateSpeed);
            characterMovement = desiredMoveDirection.normalized;

            characterController.Move(characterMovement * moveSpeed * Time.deltaTime);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded())
        {
            fallDown += Time.deltaTime * 9.8f;
        }
        Vector3 gravityVec = new Vector3();
        gravityVec.y -= fallDown;
        characterController.Move(gravityVec * Time.deltaTime);
    }

    bool isGrounded()
    {
        RaycastHit hit;
        Vector3 start = player.transform.position + player.transform.up;
        Vector3 dir = Vector3.down;
        float radius = characterController.radius;
        if (Physics.SphereCast(start, radius, dir, out hit, characterController.height / 2))
        {
            return true;
        }

        return false;
    }
}
