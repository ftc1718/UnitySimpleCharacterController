using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    Camera cam;
    FixedJoystick leftJoystick;

    public float rotateSpeed = 0.5f;
    public float moveSpeed = 4f;

    CharacterController characterController;

    Vector3 characterMovement = new Vector3(0, 0, 0);
    [HideInInspector]
    public float forward;
    [HideInInspector]
    public float strafe;
    float fallDown;

    private void Start()
    {
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
            if (characterController == null)
            {
                player.AddComponent<CharacterController>();
                characterController = player.GetComponent<CharacterController>();
                //characterController.height = characterControllerHeight;
                //characterController.center = characterControllerCenter;
            }
            if (player.GetComponent<AnimatController>() == null)
            {
                player.AddComponent<AnimatController>();
                player.GetComponent<AnimatController>().controller = this.gameObject;
            }
            cam = GetComponent<CameraController>().cam;
            leftJoystick = GetComponent<CameraController>().leftJoystick;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        forward = 0;
        strafe = 0;

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

            if (strafe >= 0.8 || strafe <= -0.8 || forward >= 0.8 || forward <= -0.8)
            {
                characterMovement = desiredMoveDirection.normalized;

                characterController.Move(characterMovement * moveSpeed * Time.deltaTime);
            }
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
