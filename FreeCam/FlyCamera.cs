using UnityEngine;
using System;
public class FlyCamera : MonoBehaviour
{
    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    private float Horizontal, Vertical;

    public Transform target;
    public Vector3 offset;

    public new bool enabled;


    private void Start()
    {
        targetDirection = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        Position();
        if (enabled)
        {
            MouseLook();
            Movement();
        }
    }

    private void Position()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
        else
            transform.position = offset;
    }

    private void Movement()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? UIManager.sSpeed : UIManager.speed;

        if (Input.GetKey(KeyCode.A))
            Horizontal = -5f;
        else if (Input.GetKey(KeyCode.D))
            Horizontal = 5f;
        else
            Horizontal = 0;
        if (Input.GetKey(KeyCode.W))
            Vertical = 5f;
        else if (Input.GetKey(KeyCode.S))
            Vertical = -5f;
        else
            Vertical = 0;


        Vector3 newPos = offset + ((Horizontal * moveSpeed * transform.right) + (Vertical * moveSpeed * transform.forward));
        
        if (Input.GetKey(KeyCode.Space))
            newPos.y = newPos.y + moveSpeed * Vector3.up.y * 2;
        if (Input.GetKey(KeyCode.LeftControl))
            newPos.y = newPos.y - moveSpeed * Vector3.up.y * 2;

        offset = new Vector3(
                Mathf.Lerp(offset.x, newPos.x,Time.fixedDeltaTime),
                Mathf.Lerp(offset.y, newPos.y,Time.fixedDeltaTime),
                Mathf.Lerp(offset.z, newPos.z,Time.fixedDeltaTime));

    }

    private void MouseLook()
    {
        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        Vector3 mouseDelta = UIUtils.RewiredMouseInput() * 0.2f;
        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(UIManager.sensitivity, UIManager.sensitivity));

        // Find the absolute mouse movement value from point zero.
        _mouseAbsolute += new Vector2(mouseDelta.x, mouseDelta.y);

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