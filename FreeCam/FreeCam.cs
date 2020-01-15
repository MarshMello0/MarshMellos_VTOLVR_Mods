using System;
using System.Collections.Generic;
using UnityEngine;
public class FreeCam : VTOLMOD
{
    public bool isAttached;
    private FlyCamera script;
    private static FreeCam _instance;

    private bool hudActive = true;
    private bool followPlayer = false;
    private float mouseSensitivity, mouseSmoothing;
    private void Awake()
    {
        _instance = this;
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
                script.enabled = false; //We want to start with mouse look disabled.
            }
            else
            {
                script.gameObject.SetActive(true);
                script.enabled = false;
            }
            isAttached = true;

        }

        if (!isAttached)
            return;

        if (GUI.Button(new Rect(Screen.width - 150, 0, 150, 20), "Disable Free Cam"))
        {
            script.gameObject.SetActive(false);
            isAttached = false;
            Cursor.lockState = CursorLockMode.None;
        }
        if (script.enabled && GUI.Button(new Rect(Screen.width - 150, 20, 150, 20), "Disable Mouse Lock") || isAttached && Input.GetKeyDown(KeyCode.Escape))
        {
            script.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!script.enabled && GUI.Button(new Rect(Screen.width - 150, 20, 150, 20), "Enable Mouse Lock"))
        {
            script.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        GUI.Label(new Rect(Screen.width - 150, 40, 150, 20), "Mouse Sensitivity: " + script.sensitivity.x);
        mouseSensitivity = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 60, 150, 20), script.sensitivity.x, 0, 10), 2);
        script.sensitivity = new Vector2(mouseSensitivity, mouseSensitivity);
        GUI.Label(new Rect(Screen.width - 150, 80, 150, 20), "Mouse Smoothing: " + script.smoothing.x);
        mouseSmoothing = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 100, 150, 20), script.smoothing.x, 1, 10), 2);
        script.smoothing = new Vector2(mouseSmoothing, mouseSmoothing);
        GUI.Label(new Rect(Screen.width - 150, 120, 150, 20), "Movement Speed: " + script.speed);
        script.speed = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 140, 150, 20), script.speed, 0.1f, 10), 2);
        GUI.Label(new Rect(Screen.width - 150, 160, 150, 20), "Sprint Speed: " + script.fastSpeed);
        script.fastSpeed = (float)Math.Round(GUI.HorizontalSlider(new Rect(Screen.width - 150, 180, 150, 20), script.fastSpeed, 0.2f, 10), 2);

        if (!followPlayer && GUI.Button(new Rect(Screen.width - 150, 200, 150, 20), "Follow Player"))
        {
            VRSDKSwitcher switcher = FindObjectOfType<VRSDKSwitcher>();
            if (switcher == null)
            {
                LogError("Couldn't find player's SDK Switcher");
                return;
            }
            script.transform.SetParent(switcher.transform);
            followPlayer = true;
        }
        else if (followPlayer && GUI.Button(new Rect(Screen.width - 150, 200, 150, 20), "Detach from Player"))
        {
            followPlayer = false;
            script.transform.parent = null;
        }

    }
}