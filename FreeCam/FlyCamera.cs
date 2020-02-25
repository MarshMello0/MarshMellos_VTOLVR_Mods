using UnityEngine;
using System;
public class FlyCamera : MonoBehaviour
{
    private Vector3 input;
    private float Horizontal, Vertical,x,y,z;

    public Transform target;
    public Vector3 offset;

    public new bool enabled;

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
        input = UIUtils.RewiredMouseInput() * 0.2f;
        x += input.y * UIManager.sensitivity * Time.deltaTime * 50f;
        y += input.x * UIManager.sensitivity * Time.deltaTime * 50f;
        x = Mathf.Clamp(x, -90, 90);

        if (Input.GetKey(KeyCode.Q))
            z -= 1 * Time.deltaTime;
        else if (Input.GetKey(KeyCode.E))
            z += 1 * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(-x, y, z);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }
}