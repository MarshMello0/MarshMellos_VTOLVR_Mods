using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class VTWatch : VTOLMOD
{
    private TextMeshPro currentText;
    private GameObject currentClock;
    private bool updateText;
    private TimeSpan lastTime;
    public static UnityAction<bool> callback;
    private static bool use24H;
    public override void ModLoaded()
    {
        updateText = false;
        VTOLAPI.SceneLoaded += SceneLoaded;
        ReadyRoom();

        callback += Use24HChanged;
        Settings setting = new Settings(this);
        setting.CreateBoolSetting("Use 24 hour clock", callback);
        VTOLAPI.CreateSettingsMenu(setting);
        Log("Loaded!");
    }

    private void Use24HChanged(bool arg0)
    {
        Log("Changed use24H to " + arg0);
        use24H = arg0;
    }

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
        if (updateText)
        {
            lastTime = DateTime.Now.TimeOfDay;
            if (use24H)
                currentText.text = $"{((lastTime.Hours) < 10 ? "0" : "")}{lastTime.Hours}:{(lastTime.Minutes < 10 ? "0" : "")}{lastTime.Minutes}";
            else
                currentText.text = $"{((lastTime.Hours % 12 ) < 10 ? "0" : "")}{lastTime.Hours % 12}:{(lastTime.Minutes < 10 ? "0" : "")}{lastTime.Minutes} {(lastTime.Hours > 11? "PM": "AM")}";
        }
    }

    private void SceneLoaded(VTOLScenes scene)
    {
        switch (scene)
        {
            case VTOLScenes.SplashScene:
                break;
            case VTOLScenes.SamplerScene:
                break;
            case VTOLScenes.ReadyRoom:
                ReadyRoom();
                break;
            case VTOLScenes.VehicleConfiguration:
                break;
            case VTOLScenes.LoadingScene:
                break;
            case VTOLScenes.MeshTerrain:
                break;
            case VTOLScenes.OpenWater:
                break;
            case VTOLScenes.Akutan:
                break;
            case VTOLScenes.VTEditMenu:
                break;
            case VTOLScenes.VTEditLoadingScene:
                break;
            case VTOLScenes.VTMapEditMenu:
                break;
            case VTOLScenes.CustomMapBase:
                break;
            case VTOLScenes.CommRadioTest:
                break;
            case VTOLScenes.ShaderVariantsScene:
                break;
            default:
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
    }

    public void OnDestory()
    {
        VTOLAPI.SceneLoaded -= SceneLoaded;
    }
}
