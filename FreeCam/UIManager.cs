using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager _instance { get; private set; }
    public RectTransform openPos, closePos;
    public RectTransform rectTransform;
    public AnimationCurve movementCurve;
    public float time;
    private bool uiOpen, freecamEnabled, mouseLookEnabled, uiVisable, waitingForTarget;

    public MouseOver toggleUI, freecamOver,mouselookOver, refreshActorsOver;
    public TextMeshProUGUI freecamText, mouselookText, speedText, sSpeedText, sensitivityText,fovText, rotationText;
    public FlyCamera camera;
    public Camera cam;
    public Slider speedSlider, sSpeedSlider, sensitivitySlider, fovSlider, rotationSlider;
    public TMP_Dropdown actorDropdown, followTypeDropdown;

    public static float speed, sSpeed, sensitivity, fov, rotSpeed;

    private void Awake()
    {
        _instance = this;
        SceneManager.sceneLoaded += SceneLoaded;
        uiVisable = true;
    }

    private void Update()
    {
        //If the script is enabled, this toggle should always be turning it to false.
        if (Input.GetKeyDown(KeyCode.Escape) && camera != null && camera.enabled)
            ToggleMouseLook();
        if (Input.GetKeyDown(KeyCode.P))
        {
            uiVisable = !uiVisable;
            rectTransform.gameObject.SetActive(uiVisable);
        }
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Every time the scene changes you have to reset these listeners
        toggleUI.OnClick.RemoveAllListeners();
        toggleUI.OnClick.AddListener(ToggleUI);
        freecamOver.OnClick.RemoveAllListeners();
        freecamOver.OnClick.AddListener(ToggleFreeCam);
        mouselookOver.OnClick.RemoveAllListeners();
        mouselookOver.OnClick.AddListener(ToggleMouseLook);
        speedSlider.onValueChanged.RemoveAllListeners();
        speedSlider.onValueChanged.AddListener(SpeedChanged);
        sSpeedSlider.onValueChanged.RemoveAllListeners();
        sSpeedSlider.onValueChanged.AddListener(SprintSpeedChanged);
        sensitivitySlider.onValueChanged.RemoveAllListeners();
        sensitivitySlider.onValueChanged.AddListener(SensitivityChanged);
        refreshActorsOver.OnClick.RemoveAllListeners();
        refreshActorsOver.OnClick.AddListener(UpdateActors);
        actorDropdown.onValueChanged.RemoveAllListeners();
        actorDropdown.onValueChanged.AddListener(SelectedActor);
        fovSlider.onValueChanged.RemoveAllListeners();
        fovSlider.onValueChanged.AddListener(FOVChanged);
        rotationSlider.onValueChanged.RemoveAllListeners();
        rotationSlider.onValueChanged.AddListener(RotationSpeedChanged);
        followTypeDropdown.onValueChanged.RemoveAllListeners();
        followTypeDropdown.onValueChanged.AddListener(FollowTypeChanged);
    }

    public void ToggleUI()
    {
        Log("Toggling UI");
        StartCoroutine(Toggle());
    }

    private IEnumerator Toggle()
    {
        uiOpen = !uiOpen;
        float t = 0f;
        if (uiOpen)
        {
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                rectTransform.position = new Vector3(Mathf.Lerp(closePos.position.x,
                                                                openPos.position.x,
                                                                movementCurve.Evaluate(t)),
                                                    rectTransform.position.y,
                                                    rectTransform.position.z);
                yield return new WaitForEndOfFrame();
            }

        }
        else
        {
            while (t < 1f)
            {
                t += Time.deltaTime / time;
                rectTransform.position = new Vector3(Mathf.Lerp(openPos.position.x,
                                                                closePos.position.x,
                                                                movementCurve.Evaluate(t)),
                                                    rectTransform.position.y,
                                                    rectTransform.position.z);
                yield return new WaitForEndOfFrame();
            }

        }
        Log("Finished Toggling UI");
    }

    public void ToggleFreeCam()
    {
        freecamEnabled = !freecamEnabled;
        if (freecamEnabled)
        {
            if (camera == null)
            {
                CreateCamera();
            }
            else
            {
                camera.gameObject.SetActive(true);
            }
            freecamText.text = "Disable\nFree Cam";
        }
        else
        {
            if (camera != null)
            {
                camera.gameObject.SetActive(false);
            }
            freecamText.text = "Enable\nFree Cam";
        }
    }

    private void CreateCamera(bool enable = true)
    {
        GameObject go = Instantiate(FreeCam.cameraPrefab);
        go.AddComponent<FloatingOriginTransform>();
        cam = go.GetComponent<Camera>();
        cam.farClipPlane = 999999; //Float.maxvalue does some weaird stuff.
        cam.fieldOfView = fovSlider.value;
        camera = go.AddComponent<FlyCamera>();
        camera.enabled = false;
        if (VRHead.instance != null)
            go.transform.position = VRHead.instance.transform.position;
        go.SetActive(enable);
    }

    public void ToggleMouseLook()
    {
        mouseLookEnabled = !mouseLookEnabled;
        if (camera == null)
            CreateCamera(mouseLookEnabled);

        if (camera != null)
        {
            if (!camera.gameObject.activeInHierarchy && mouseLookEnabled)
                ToggleFreeCam();
            camera.enabled = mouseLookEnabled;
            mouselookText.text = (mouseLookEnabled ? "Disable" : "Enable") + "\nMouse Look";
            Cursor.lockState = (mouseLookEnabled ? CursorLockMode.Locked : CursorLockMode.None);
            Cursor.visible = !mouseLookEnabled;
        }
    }

    public void SpeedChanged(float value)
    {
        speed = value;
        speedText.text = "Speed = " + value;
    }

    public void SprintSpeedChanged(float value)
    {
        sSpeed = value;
        sSpeedText.text = "Sprint Speed = " + value;
    }

    public void SensitivityChanged(float value)
    {
        sensitivity = value;
        sensitivityText.text = "Sensitivity = " + value;
    }

    public void UpdateActors()
    {
        Log("Updating Actors");
        actorDropdown.options.Clear();
        actorDropdown.options.Add(new TMP_Dropdown.OptionData("No Actor"));
        for (int i = 0; i < TargetManager.instance.allActors.Count; i++)
        {
            actorDropdown.options.Add(new TMP_Dropdown.OptionData(TargetManager.instance.allActors[i].actorName));
        }
    }

    public void SelectedActor(int value)
    {
        Log("Selected Actor " + value);
        if (value == 0)
        {
            camera.target = null;
            return;
        }
        value--;
        if (camera == null)
            CreateCamera(false);

        if (camera != null)
        {
            camera.target = TargetManager.instance.allActors[value].transform;
            camera.offset = new Vector3(0, 0, 0);
            camera.CreateCross(camera.target);
            if (camera.followType == FlyCamera.FollowType.Fixed)
                camera.transform.SetParent(camera.target);

        }
        else
        {
            LogError("Camera was null, when selecting an actor");
            actorDropdown.value = 0;
        }
    }

    public void FOVChanged(float value)
    {
        fov = value;
        fovText.text = "Fov = " + value;
        if (cam != null)
        {
            cam.fieldOfView = value;
        }
    }
    public void RotationSpeedChanged(float value)
    {
        rotSpeed = value;
        rotationText.text = "Rotation = " + value;
    }
    public void FollowTypeChanged(int index)
    {
        camera.followType = index == 0 ? FlyCamera.FollowType.FreeFollow : FlyCamera.FollowType.Fixed;
        Log("Changed follow type to " + camera.followType);
        if (camera.followType == FlyCamera.FollowType.Fixed && camera.target != null)
            camera.transform.SetParent(camera.target);
        else if (camera.followType == FlyCamera.FollowType.Fixed)
            waitingForTarget = true;

        if (camera.followType == FlyCamera.FollowType.FreeFollow)
            camera.transform.parent = null;

    }
    public void FollowTypeChanged(int index, bool forceUI)
    {
        if (forceUI)
        {
            followTypeDropdown.value = index;
            followTypeDropdown.RefreshShownValue();
        }
        FollowTypeChanged(index);
    }

    public void Log(object message)
    {
        FreeCam._instance.Log(message);
    }
    public void LogWarning(object message)
    {
        FreeCam._instance.LogWarning(message);
    }
    public void LogError(object message)
    {
        FreeCam._instance.LogError(message);
    }
}
