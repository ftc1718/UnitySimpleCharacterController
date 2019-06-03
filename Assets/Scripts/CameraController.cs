using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Camera cam;

    public Transform pivot;
    public float zoomDistance = 5.0f;
    public float xRotateSpeed = 120.0f;
    public float yRotateSpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public float cameraSensitivity = 250f;
    //public float climbSpeed = 4f;
    public float cameraMoveSpeed = 10f;

    float x = 0.0f;
    float y = 0.0f;

    Quaternion rotation = Quaternion.Euler(0, 0, 0);

    public bool freeCameraEnable = false;

    public FixedTouchField touchField;
    public FixedJoystick leftJoystick;

    Vector3 freeCameraLookPosition;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
        if (touchField == null)
            touchField = GetComponentInChildren<FixedTouchField>();
        if (leftJoystick == null)
            leftJoystick = GetComponentInChildren<FixedJoystick>();
        Vector3 angles = cam.transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        freeCameraLookPosition = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 2f));
    }

    private void OnApplicationQuit()
    {
       
    }

    void LateUpdate()
    {
        if (pivot)
        {
            if (freeCameraEnable)
            {
                FreeCamera();
                GetComponent<PlayerController>().enabled = false;
            }
            else
            {
                RotateCamera();
                GetComponent<PlayerController>().enabled = true;
            }
        }
        else
        {
            FreeCamera();
        }
    }

    void RotateCamera()
    {
        Input.simulateMouseWithTouches = false;

        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xRotateSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * yRotateSpeed * 0.02f;
        }

        if (touchField != null && touchField.TouchDist != Vector2.zero)
        {
            x += touchField.TouchDist.x;
            y -= touchField.TouchDist.y;
        }

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        rotation = Quaternion.Euler(y, x, 0);

        zoomDistance -= Input.GetAxis("Mouse ScrollWheel") * 5;

        if (Input.touchCount == 2 && leftJoystick != null && leftJoystick.inputVector == Vector2.zero)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            zoomDistance -= deltaMagnitudeDiff / 20f;
        }

        zoomDistance = Mathf.Clamp(zoomDistance, distanceMin, distanceMax);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -zoomDistance);
        Vector3 position = rotation * negDistance + pivot.position;

        cam.transform.rotation = rotation;
        cam.transform.position = position;
    }

    void FreeCamera()
    {
        Input.simulateMouseWithTouches = false;

        if (Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        }

        if (touchField != null && touchField.TouchDist != Vector2.zero)
        {
            x += touchField.TouchDist.x;
            y -= touchField.TouchDist.y;
        }

        y = Mathf.Clamp(y, -90, 90);
        rotation = Quaternion.Euler(y, x, 0);
        cam.transform.rotation = rotation;

        float vertical = 0f;
        float horizontal = 0f;

        y = Mathf.Clamp(y, -90, 90);
        rotation = Quaternion.Euler(y, x, 0);
        cam.transform.localRotation = rotation;

        if ((Input.GetMouseButton(1)))
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
        }

        if (leftJoystick.inputVector != Vector2.zero)
        {
            vertical += leftJoystick.inputVector.y;
            horizontal += leftJoystick.inputVector.x;
        }

        cam.transform.position += cam.transform.forward * cameraMoveSpeed * vertical * Time.deltaTime;
        cam.transform.position += cam.transform.right * cameraMoveSpeed * horizontal * Time.deltaTime;

        //if (Input.GetKey(KeyCode.Q)) { cam.transform.position += cam.transform.up * climbSpeed * Time.deltaTime; }
        //if (Input.GetKey(KeyCode.E)) { cam.transform.position -= cam.transform.up * climbSpeed * Time.deltaTime; }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
