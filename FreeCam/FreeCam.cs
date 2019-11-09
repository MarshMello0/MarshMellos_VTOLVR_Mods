using System;
using System.Collections.Generic;
using UnityEngine;
public class FreeCam : VTOLMOD
{
    public bool isAttached;
    private FlyCamera script;
    private static FreeCam _instance;

    private bool hudActive = true;
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        Log("Free Cam Loaded");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            hudActive = !hudActive;
        }
    }
    private void OnGUI()
    {
        if (!hudActive)
            return;
        GUI.Label(new Rect(Screen.width - 300, 0, 150, 20), "Press P to hide buttons");
        if (!isAttached && GUI.Button(new Rect(Screen.width - 150, 0, 150, 20), "Enable Free Cam"))
        {
            Cursor.lockState = CursorLockMode.Confined;
            if (!script)
            {
                GameObject go = new GameObject("Free Cam", typeof(Camera), typeof(FlyCamera));
                script = go.GetComponent<FlyCamera>();
                go.GetComponent<Camera>().farClipPlane = 999999;
                go.tag = "MainCamera";
            }
            else
            {
                script.gameObject.SetActive(true);
                script.enabled = true;
            }
            isAttached = true;

        }
        if (isAttached && GUI.Button(new Rect(Screen.width - 150, 0, 150, 20), "Disable Free Cam"))
        {
            script.gameObject.SetActive(false);
            isAttached = false;
            Cursor.lockState = CursorLockMode.None;
        }
        if (isAttached && script.enabled && GUI.Button(new Rect(Screen.width - 150, 20, 150, 20), "Disable Mouse Lock") || isAttached && Input.GetKeyDown(KeyCode.Escape))
        {
            script.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (isAttached && !script.enabled && GUI.Button(new Rect(Screen.width - 150, 20, 150, 20), "Enable Mouse Lock"))
        {
            script.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (isAttached)
        {
            GUI.Label(new Rect(Screen.width - 150, 40, 150, 20), "Mouse Sensitivity: " + script.sensitivity.x);
            float mouseSensitivity = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 60, 150, 20), script.sensitivity.x, 1, 10), 2);
            script.sensitivity = new Vector2(mouseSensitivity, mouseSensitivity);
            GUI.Label(new Rect(Screen.width - 150, 80, 150, 20), "Mouse Smoothing: " + script.smoothing.x);
            float mouseSmoothing = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 100, 150, 20), script.smoothing.x, 1, 10), 2);
            script.smoothing = new Vector2(mouseSmoothing, mouseSmoothing);
            GUI.Label(new Rect(Screen.width - 150, 120, 150, 20), "Movement Speed: " + script.speed);
            script.speed = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 140, 150, 20), script.speed, 0.1f, 10), 2);
            GUI.Label(new Rect(Screen.width - 150, 160, 150, 20), "Sprint Speed: " + script.fastSpeed);
            script.fastSpeed = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 180, 150, 20), script.fastSpeed, 0.2f, 10), 2);
        }
    }
}

public static class FreeCamConsole
{
    public static void Log(object message)
    {
        Debug.Log("FreeCam: " + message);
    }
}

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


    private void OnEnable()
    {
        cam = GetComponent<Camera>();
        targetDirection = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        Zoom();
        SmoothMouseLook();
        Movement();
        DebugObject();
    }

    private void DebugObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                FreeCamConsole.Log("Hit: " + objectHit.name);
            }
            else
            {
                FreeCamConsole.Log("We Missed");
            }
        }
    }

    private void Movement()
    {
        float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : speed;

        Vector3 newPos = transform.position +=
            Input.GetAxis("Horizontal") * moveSpeed * transform.right +
            Input.GetAxis("Vertical") * moveSpeed * transform.forward;

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
        cam.fieldOfView -= Input.mouseScrollDelta.y;

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