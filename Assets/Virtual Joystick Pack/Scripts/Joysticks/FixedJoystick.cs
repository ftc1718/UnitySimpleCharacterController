using UnityEngine;
using UnityEngine.EventSystems;

public class FixedJoystick : Joystick
{
    Vector2 joystickPosition = Vector2.zero;
    private Camera cam = new Camera();

    bool isPointerUp;

    void Start()
    {
        //joystickPosition = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = (eventData.position - RectTransformUtility.WorldToScreenPoint(cam, background.position)) * 4;
        inputVector.x = (direction.x > background.sizeDelta.x / 2f) ? Mathf.Clamp(direction.x, -1, 1) : Mathf.Clamp(direction.x / (background.sizeDelta.x / 2f), -1, 1);
        inputVector.y = (direction.y > background.sizeDelta.y / 2f) ? Mathf.Clamp(direction.y, -1, 1) : Mathf.Clamp(direction.y / (background.sizeDelta.y / 2f), -1, 1);

        //inputVector = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * background.sizeDelta.x / 2f) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
        isPointerUp = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isPointerUp = true;
        handle.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (isPointerUp)
        {
            inputVector = Vector2.Lerp(inputVector, Vector2.zero, 0.5f);
        }
    }
}