using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FreeCam : VTOLMOD
{
    private static readonly string assetsPath = @"VTOLVR_ModLoader\mods\FreeCam\freecam.assets";
    private static AssetBundle asset;
    private static GameObject canvasPrefab;
    public static GameObject cameraPrefab;
    private void Awake()
    {
        StartCoroutine(Load());
    }

    private IEnumerator Load()
    {
        Log("Loading Asset Bundle");
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetsPath);
        yield return request;
        asset = request.assetBundle;
        if (asset == null)
        {
            LogError("Assets is null\nFree Cam won't continue");
        }
        else
        {
            Log("Assets Bundle is loaded");
            //Storing the prefab in memory to spawn
            AssetBundleRequest canvasRequest = asset.LoadAssetAsync<GameObject>("Free Cam - Canvas");
            AssetBundleRequest cameraRequest = asset.LoadAssetAsync<GameObject>("Free Cam - Camera");
            yield return canvasRequest;
            yield return cameraRequest;
            canvasPrefab = canvasRequest.asset as GameObject;
            cameraPrefab = cameraRequest.asset as GameObject;
            //Adding the scripts to the prefab;
            Log("Adding Scripts");
            UIManager manager = canvasPrefab.AddComponent<UIManager>();
            manager.openPos = canvasPrefab.transform.GetChild(0).GetComponent<RectTransform>();
            manager.closePos = canvasPrefab.transform.GetChild(1).GetComponent<RectTransform>();
            manager.rectTransform = canvasPrefab.transform.GetChild(2).GetComponent<RectTransform>();
            manager.movementCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
            manager.time = 1f;
            Log("Adding mouse overs");
            manager.toggleUI = canvasPrefab.transform.GetChild(2).GetChild(1).GetChild(0).gameObject.AddComponent<MouseOver>();
            manager.freecamOver = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(1).gameObject.AddComponent<MouseOver>();
            manager.freecamText = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            manager.mouselookOver = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(2).gameObject.AddComponent<MouseOver>();
            manager.mouselookText = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            manager.refreshActorsOver = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(9).gameObject.AddComponent<MouseOver>();
            Log("Getting Texts");
            manager.speedText = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>();
            manager.sSpeedText = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>();
            manager.sensitivityText = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(6).GetComponent<TextMeshProUGUI>();
            manager.fovText = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(7).GetComponent<TextMeshProUGUI>();
            Log("Getting Sliders");
            manager.speedSlider = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(4).GetComponentInChildren<Slider>();
            manager.sSpeedSlider = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(5).GetComponentInChildren<Slider>();
            manager.sensitivitySlider = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(6).GetComponentInChildren<Slider>();
            manager.fovSlider = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(7).GetComponentInChildren<Slider>();
            Log("Adding Dropdown");
            manager.actorDropdown = canvasPrefab.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(8).GetComponentInChildren<TMP_Dropdown>();

            UIManager.speed = manager.speedSlider.value;
            UIManager.sSpeed = manager.sSpeedSlider.value;
            UIManager.sensitivity = manager.sensitivitySlider.value;
            UIManager.fov = manager.fovSlider.value;

            manager.speedText.text += " = " + UIManager.speed;
            manager.sSpeedText.text += " = " + UIManager.sSpeed;
            manager.sensitivityText.text += " = " + UIManager.sensitivity;
            manager.fovText.text += " = " + UIManager.fov;

            Log("Spawned!");
            DontDestroyOnLoad(Instantiate(canvasPrefab));
        }
    }
}