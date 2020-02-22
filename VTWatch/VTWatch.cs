using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class VTWatch : VTOLMOD
{
    private static readonly string assetBundlePath = @"\VTOLVR_ModLoader\mods\VTWatch\vtwatch.assets";
    private static AssetBundle assetBundle;
    private static GameObject clockPrefab,f45ClockPrefab;

    //Settings
    public static UnityAction<bool> use24Callback, useBatteryCallback;
    /// <summary>
    /// If they want a 24 hour clock or not.
    /// </summary>
    private static bool use24H;
    /// <summary>
    /// If they are using the vehicles battery to power the clock or not.
    /// </summary>
    private static bool useBattery = true;


    //InScene Items
    private TimeSpan lastTime, lastTimer;
    private TextMeshPro currentText;
    private GameObject currentClock;
    private bool updateText;
    private bool onTime = true;
    private VRInteractable switchButton, otherButton;
    private Battery battery;

    /// <summary>
    /// How much the clock drains from the battery
    /// </summary>
    private float batteryDrainRate = 0.01f;
    /// <summary>
    /// If the clock is powered on or not.(Can be true if there isn't any power in the battery)
    /// </summary>
    private bool powered;
    /// <summary>
    /// The current value of the timer in milliseconds;
    /// </summary>
    private float currentTimer;
    /// <summary>
    /// If the stopwatch is currently running;
    /// </summary>
    private bool stopwatchRunning;

    public override void ModLoaded()
    {
        updateText = false;
        VTOLAPI.SceneLoaded += SceneLoaded;
        StartCoroutine(LoadAssetBundle());

        ReadyRoom();

        use24Callback += Use24HChanged;
        useBatteryCallback += UseBatteryChanged;
        Settings setting = new Settings(this);
        setting.CreateBoolSetting("Use 24 hour clock", use24Callback);
        setting.CreateBoolSetting("Use vehicles battery to power clock", useBatteryCallback, true);
        VTOLAPI.CreateSettingsMenu(setting);
        Log("Loaded!");
    }

    private IEnumerator LoadAssetBundle()
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Directory.GetCurrentDirectory() + assetBundlePath);
        while (!request.isDone)
            yield return null;
        assetBundle = request.assetBundle;
        if (assetBundle != null)
        {
            Log("Loaded Asset Bundle");
            clockPrefab = assetBundle.LoadAsset<GameObject>("Vehicle Clock Prefab");
            f45ClockPrefab = assetBundle.LoadAsset<GameObject>("Vehicle Clock Prefab F45");
            if (clockPrefab == null)
                LogError("Failed finding prefab of VTClock");
            if (f45ClockPrefab == null)
                LogError("Failed finding f45 prefab of VTClock");
        }
        else
        {
            LogError("Errrow when loading asset bundle from path\n" + Directory.GetCurrentDirectory() + assetBundlePath);
            LogError("Does that file exist?");
        }

    }

    private void Use24HChanged(bool arg0)
    {
        Log("Changed use24H to " + arg0);
        use24H = arg0;
    }
    private void UseBatteryChanged(bool state)
    {
        Log("Changed useBattery to " + state);
        useBattery = state;
    }
    /// <summary>
    /// In the ready room we just create a basic empty gameobject with the time
    /// </summary>
    /// <returns>The gameobject in the scene</returns>
    private GameObject CreatePrefab()
    {
        GameObject TimePrefab = new GameObject("Clock", typeof(TextMeshPro));
        TextMeshPro t = TimePrefab.GetComponent<TextMeshPro>();
        t.text = "TIME";
        t.enableAutoSizing = true;
        t.fontSizeMin = 0;
        t.alignment = TextAlignmentOptions.Center;
        return TimePrefab;
    }

    private void Update()
    {
        if (VTOLAPI.currentScene == VTOLScenes.ReadyRoom && updateText)
        {
            UpdateText();
        }
        else if (useBattery && powered && updateText && battery != null && battery.Drain(batteryDrainRate))
        {
            UpdateText();
        }
        else if (!useBattery && updateText)
        {
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (stopwatchRunning)
            currentTimer += Time.deltaTime;

        if (onTime || VTOLAPI.currentScene == VTOLScenes.ReadyRoom)
        {
            lastTime = DateTime.Now.TimeOfDay;
            if (use24H)
                currentText.text = $"{((lastTime.Hours) < 10 ? "0" : "")}{lastTime.Hours}:{(lastTime.Minutes < 10 ? "0" : "")}{lastTime.Minutes}";
            else
                currentText.text = $"{((lastTime.Hours % 12) < 10 ? "0" : "")}{lastTime.Hours % 12}:{(lastTime.Minutes < 10 ? "0" : "")}{lastTime.Minutes} {(lastTime.Hours > 11 ? "PM" : "AM")}";
        }
        else if (!onTime)
        {
            lastTimer = TimeSpan.FromSeconds(currentTimer);
            currentText.text = $"{lastTimer.Hours}h:{lastTimer.Minutes}m:{lastTimer.Seconds}s";
        }
        
    }
    /// <summary>
    /// Changes if the screen is on or not on the clock 
    /// in the players vehicle.
    /// </summary>
    /// <param name="isVisable"></param>
    private void SetDisplay(bool isVisable)
    {
        currentClock.transform.GetChild(1).gameObject.SetActive(isVisable);//Time
        currentClock.transform.GetChild(2).gameObject.SetActive(isVisable);//Title
    }

    private void SceneLoaded(VTOLScenes scene)
    {
        updateText = false;
        switch (scene)
        {
            case VTOLScenes.ReadyRoom:
                ReadyRoom();
                break;
            case VTOLScenes.Akutan:
                SpawnClock();
                break;
            case VTOLScenes.CustomMapBase:
                SpawnClock();
                break;
        }
    }

    private void ReadyRoom()
    {
        Log("Creating ReadyRoom Clock");
        currentClock = CreatePrefab();
        currentText = currentClock.GetComponent<TextMeshPro>();
        RectTransform rect = currentClock.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(3.7321f, 0.1873f);
        rect.anchoredPosition3D = new Vector3(-5.992f, -0.9029f, -0.9087f);
        rect.rotation = Quaternion.Euler(0, -90, 0);
        updateText = true;
        Log("Created Clock in ReadyRoom!");
        currentTimer = 0;
    }

    private void SpawnClock()
    {
        Log("Spawning Clock");
        VTOLVehicles currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
        GameObject vehicle = VTOLAPI.instance.GetPlayersVehicleGameObject();

        if (currentVehicle == VTOLVehicles.F45A)
            currentClock = Instantiate(f45ClockPrefab);
        else if (currentVehicle == VTOLVehicles.AV42C || currentVehicle == VTOLVehicles.FA26B)
            currentClock = Instantiate(clockPrefab);

        if (vehicle == null)
            LogError("Vehicle from AIP is null");

        battery = vehicle.GetComponentInChildren<Battery>();
        currentClock.transform.SetParent(vehicle.transform);
        switch (currentVehicle)
        {
            case VTOLVehicles.None:
                LogError("API found no vehicle for us");
                break;
            case VTOLVehicles.AV42C:
                currentClock.transform.localPosition = new Vector3(0.5641f, 0.7865f, 0.5356f);
                currentClock.transform.localRotation = Quaternion.Euler(0, -64.219f,0);
                break;
            case VTOLVehicles.FA26B:
                currentClock.transform.localPosition = new Vector3(0.4612f, 1.0646f, 5.8649f);
                currentClock.transform.localRotation = Quaternion.Euler(0, -77.672f, 0);
                
                break;
            case VTOLVehicles.F45A:
                currentClock.transform.localPosition = new Vector3(-0.1426f, 0.6219f, 6.1838f);
                currentClock.transform.localRotation = Quaternion.Euler(0, -90, -23.756f);
                break;
        }
        currentTimer = 0;
        SetUpClock(currentClock);
    }

    private void SetUpClock(GameObject clockGO)
    {
        Transform clockT = clockGO.transform;

        currentText = clockT.GetChild(2).GetComponent<TextMeshPro>();
        //Power Button
        VRInteractable powerButton = clockT.GetChild(3).gameObject.GetComponent<VRInteractable>();
        powerButton.interactableName = "Power Clock";
        powerButton.OnInteract = new UnityEvent();
        powerButton.OnInteract.AddListener(PowerClock);
        //Switch Button
        switchButton = clockT.GetChild(5).gameObject.GetComponent<VRInteractable>();
        switchButton.interactableName = "Stopwatch";
        switchButton.OnInteract = new UnityEvent();
        switchButton.OnInteract.AddListener(SwitchButton);
        //Other Button
        otherButton = clockT.GetChild(4).gameObject.GetComponent<VRInteractable>();
        otherButton.interactableName = "Switch clock to 24 hour";
        otherButton.OnInteract = new UnityEvent();
        otherButton.OnInteract.AddListener(OtherButton);

        if (battery == null)
        {
            Debug.LogWarning("Battery is still null, searching again");
            battery = clockGO.GetComponentInParent<Battery>();
        }

        if (!useBattery)
            SetDisplay(true);
        else
            SetDisplay(false);

        updateText = true;
    }

    public void PowerClock()
    {
        if (useBattery && battery != null && !battery.Drain(batteryDrainRate))
            return;
        powered = !powered;
        SetDisplay(powered);
    }
    public void SwitchButton()
    {
        onTime = !onTime;
        if (onTime)
        {
            otherButton.interactableName = $"Switch to {(use24H? "12" : "24")} hour";
            switchButton.interactableName = "Switch to Stopwatch";
        }
        else
        {
            otherButton.interactableName = $"{(stopwatchRunning? "Stop" : "Start")} Stopwatch";
            switchButton.interactableName = "Switch to clock";
        }
    }
    public void OtherButton()
    {
        if (onTime)
            Use24HChanged(!use24H);
        else
        {
            if (stopwatchRunning)
            {
                stopwatchRunning = false;
                otherButton.interactableName = "Start Stopwatch";
            }
            else
            {
                otherButton.interactableName = "Stop Stopwatch";
                currentTimer = 0;
                stopwatchRunning = true;
            }
        }
    }
    public void OnDestory()
    {
        VTOLAPI.SceneLoaded -= SceneLoaded;
    }
}
