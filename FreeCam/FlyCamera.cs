using UnityEngine;
using System;
public class FlyCamera : MonoBehaviour
{
    private Vector3 input;
    private float Horizontal, Vertical,x,y,z;

    public Transform target, fixedTransform;
    public Vector3 offset;

    public new bool enabled;
    public enum FollowType { FreeFollow, Fixed };
    public FollowType followType = FollowType.FreeFollow;

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
            if (followType == FollowType.Fixed)
                transform.localPosition = offset;
            else
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
        if (followType == FollowType.Fixed && fixedTransform != null)
        {
            fixedTransform.localRotation = Quaternion.Euler(-x, y, z);
            return;
        }
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

    public void CreateCross(Transform parent)
    {
        GameObject go = new GameObject("Cross");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(0, 0, 0);
        go.transform.localRotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i < 2; i++)
        {
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cube);
            a.GetComponent<BoxCollider>().enabled = false;
            if (i == 0)
                a.transform.localScale = new Vector3(1, 1, 50);
            else if (i == 1)
                a.transform.localScale = new Vector3(50, 1, 1);
            a.transform.SetParent(go.transform, false);
            a.transform.localPosition = Vector3.zero;
            a.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}