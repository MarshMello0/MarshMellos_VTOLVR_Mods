using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;
public class TestingMod : VTOLMOD
{
    private static readonly string assetsPath = @"VTOLVR_ModLoader\mods\TestingMod\assets.bundle";
    private static AssetBundle asset;
    public override void ModLoaded()
    {
        base.ModLoaded();
        StartCoroutine(LoadAssetBundle());
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 7 || arg0.buildIndex == 12)
            StartCoroutine(CreatePlane());
    }

    private IEnumerator LoadAssetBundle()
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetsPath);
        yield return request;
        asset = request.assetBundle;
        if (asset == null)
            LogError("Assets is null");
        else
            Log("Assets Bundle is loaded");
    }
    private IEnumerator CreatePlane()
    {
        while (VTMapManager.fetch == null || !VTMapManager.fetch.scenarioReady)
        {
            yield return null;
        }

        if (asset == null)
        {
            LogError("Assets was null when creating plane");
        }
        GameObject player = VTOLAPI.instance.GetPlayersVehicleGameObject();
        GameObject prefab = asset.LoadAsset<GameObject>("A-15F Prefab");
        GameObject vehicle = Instantiate(prefab,player.transform.position + new Vector3(-30,10,-30), player.transform.rotation);
        Log("Spawned");

        GameObject camRig = GameObject.Find("CameraRigParent");
        if (camRig == null)
            LogError("Camera Rig Was Null");
        camRig.transform.parent = vehicle.transform.Find("EjectorSeat");
        //camRig.transform.localPosition = new Vector3(-15.13126f, -3.32916e-06f, 8.728076e-07f);
        Log("Moved the new camera rig");
    }
}

