using UnityEngine;
using System;
public class FlyCamera : MonoBehaviour
{
    public float speed = 1.0f;
    public float fastSpeed = 2.0f;
    private Camera cam;

    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public Vector2 sensitivity = new Vector2(.5f, .5f);
    public Vector2 smoothing = new Vector2(10, 10);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    private float Horizontal, Vertical;


    private void Start()
    {
        cam = GetComponent<Camera>();
        targetDirection = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        Zoom();
        SmoothMouseLook();
        Movement();
    }

    private void Movement()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : speed;

        if (Input.GetKey(KeyCode.A))
            Horizontal = -1f;
        else if (Input.GetKey(KeyCode.D))
            Horizontal = 1f;
        else
            Horizontal = 0;
        if (Input.GetKey(KeyCode.W))
            Vertical = 1f;
        else if (Input.GetKey(KeyCode.S))
            Vertical = -1f;
        else
            Vertical = 0;


        Vector3 newPos = transform.position +=
            Horizontal * moveSpeed * transform.right +
            Vertical * moveSpeed * transform.forward;

        if (Input.GetKey(KeyCode.Space))
            newPos.y = newPos.y + moveSpeed * transform.up.y * 2;
        if (Input.GetKey(KeyCode.LeftControl))
            newPos.y = newPos.y - moveSpeed * transform.up.y * 2;

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, newPos.x, 5 * Time.fixedDeltaTime),
            Mathf.Lerp(transform.position.y, newPos.y, 5 * Time.fixedDeltaTime),
            Mathf.Lerp(transform.position.z, newPos.z, 5 * Time.fixedDeltaTime));
    }

    private void Zoom()
    {
        //cam.fieldOfView -= Input.mouseScrollDelta.y;

        if (cam.fieldOfView <= 1)
        {
            cam.fieldOfView = 1;
        }
    }

    private void SmoothMouseLook()
    {

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));


        // Interpolate mouse movement over time to apply smoothing delta.
        _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += _smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
        transform.localRotation *= yRotation;

    }
}