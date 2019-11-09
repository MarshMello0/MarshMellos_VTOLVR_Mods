using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class TexturePacks : VTOLMOD
{
    Dictionary<string, Material> skins;
    private void AddKey(string key, Material mat)
    {
        if (!skins.ContainsKey(key))
            skins.Add(key, mat);
    }
    private void SetMaterials()
    {
        skins = new Dictionary<string, Material>();

        foreach (Material item in Resources.FindObjectsOfTypeAll(typeof(Material)) as Material[])
        {
            switch (item.name)
            {
                case "mat_vtol4Exterior":
                    AddKey("mat_vtol4Exterior", item);
                    continue;
                case "mat_vtol4Exterior2":
                    AddKey("mat_vtol4Exterior2", item);
                    continue;
                case "mat_vtol4Interior":
                    AddKey("mat_vtol4Interior", item);
                    continue;
                case "mat_vtol4TiltEngine":
                    AddKey("mat_vtol4TiltEngine", item);
                    continue;
                case "mat_cockpitProps":
                    AddKey("mat_cockpitProps", item);
                    continue;
                case "mat_acesSeat":
                    AddKey("mat_acesSeat", item);
                    continue;
                case "mat_bobbleHead":
                    AddKey("mat_bobbleHead", item);
                    continue;
                case "mat_miniMFD":
                    AddKey("mat_miniMFD", item);
                    continue;
                case "mat_mfd":
                    AddKey("mat_mfd", item);
                    continue;
                case "mat_sevtf_CanopyInt":
                    AddKey("mat_sevtf_CanopyInt", item);
                    continue;
                case "mat_sevtf_engine":
                    AddKey("mat_sevtf_engine", item);
                    continue;
                case "mat_sevtf_ext":
                    AddKey("mat_sevtf_ext", item);
                    continue;
                case "mat_sevtf_ext2":
                    AddKey("mat_sevtf_ext2", item);
                    continue;
                case "mat_sevtf_int":
                    AddKey("mat_sevtf_int", item);
                    continue;
                case "mat_sevtf_lowPoly":
                    AddKey("mat_sevtf_lowPoly", item);
                    continue;
                case "mat_aFighterCanopyExt":
                    AddKey("mat_aFighterCanopyExt", item);
                    continue;
                case "mat_aFighterCanopyInt":
                    AddKey("mat_aFighterCanopyInt", item);
                    continue;
                case "mat_afighterExt1":
                    AddKey("mat_afighterExt1", item);
                    continue;
                case "mat_afighterExt2":
                    AddKey("mat_afighterExt2", item);
                    continue;
                case "mat_aFighterInterior":
                    AddKey("mat_aFighterInterior", item);
                    continue;
                case "mat_aFighterInterior2":
                    AddKey("mat_aFighterInterior2", item);
                    continue;
                case "mat_vgLowpoly":
                    AddKey("mat_vgLowpoly", item);
                    continue;
            }
        }
    }
    private void ApplySkin()
    {
        Debug.Log("Applying Skin Number " + selectedSkin);
        if (selectedSkin < 0)
        {
            Debug.Log("Selected Skin was below 0");
            return;
        }

        Skin selected = installedSkins[selectedSkin];

        Debug.Log("\nSkin: " + selected.name + " \nPath: " + selected.folderPath + "\nHasAV42C: " + selected.hasAv42c);
        switch (VTOLAPI.instance.GetPlayersVehicleEnum())
        {
            case VTOLVehicles.AV42C:
                if (File.Exists(selected.folderPath + @"\vtol4Exterior.png") && skins.ContainsKey("mat_vtol4Exterior"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\vtol4Exterior.png", skins["mat_vtol4Exterior"]));
                if (File.Exists(selected.folderPath + @"\vtol4Exterior2.png") && skins.ContainsKey("mat_vtol4Exterior2"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\vtol4Exterior2.png", skins["mat_vtol4Exterior2"]));
                if (File.Exists(selected.folderPath + @"\vtol4Interior.png") && skins.ContainsKey("mat_vtol4Interior"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\vtol4Interior.png", skins["mat_vtol4Interior"]));
                if (File.Exists(selected.folderPath + @"\vtol4TiltEngine.png") && skins.ContainsKey("mat_vtol4TiltEngine"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\vtol4TiltEngine.png", skins["mat_vtol4TiltEngine"]));
                Debug.Log("Loaded AV42C Skins\nvtol4exterior.png" + File.Exists(selected.folderPath + @"\vtol4Exterior.png") +
                    "\nvtol4Exterior2.png: " + File.Exists(selected.folderPath + @"\vtol4Exterior2.png") +
                    "\nvtol4Interior.png: " + File.Exists(selected.folderPath + @"\vtol4Interior.png") +
                    "\nvtol4TiltEngine.png: " + File.Exists(selected.folderPath + @"\vtol4TiltEngine.png"));
                break;
            case VTOLVehicles.FA26B:
                if (File.Exists(selected.folderPath + @"\aFighterCanopyExt.png") && skins.ContainsKey("mat_aFighterCanopyExt"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\aFighterCanopyExt.png", skins["mat_aFighterCanopyExt"]));
                if (File.Exists(selected.folderPath + @"\aFighterCanopyInt.png") && skins.ContainsKey("mat_aFighterCanopyInt"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\aFighterCanopyInt.png", skins["mat_aFighterCanopyInt"]));
                if (File.Exists(selected.folderPath + @"\afighterExt1.png") && skins.ContainsKey("mat_afighterExt1"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\afighterExt1.png", skins["mat_afighterExt1"]));
                if (File.Exists(selected.folderPath + @"\afighterExt2.png") && skins.ContainsKey("mat_afighterExt2"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\afighterExt2.png", skins["mat_afighterExt2"]));
                if (File.Exists(selected.folderPath + @"\aFighterInterior.png") && skins.ContainsKey("mat_aFighterInterior"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\aFighterInterior.png", skins["mat_aFighterInterior"]));
                if (File.Exists(selected.folderPath + @"\aFighterInterior2.png") && skins.ContainsKey("mat_aFighterInterior2"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\aFighterInterior2.png", skins["mat_aFighterInterior2"]));
                if (File.Exists(selected.folderPath + @"\vgLowpoly.png") && skins.ContainsKey("mat_vgLowpoly"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\vgLowpoly.png", skins["mat_vgLowpoly"]));
                Debug.Log("Loaded FA-26B Skins");
                break;
            case VTOLVehicles.F45A:
                if (File.Exists(selected.folderPath + @"\sevtf_CanopyInt.png") && skins.ContainsKey("mat_sevtf_CanopyInt"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\sevtf_CanopyInt.png", skins["mat_sevtf_CanopyInt"]));
                if (File.Exists(selected.folderPath + @"\sevtf_engine.png") && skins.ContainsKey("mat_sevtf_engine"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\sevtf_engine.png", skins["mat_sevtf_engine"]));
                if (File.Exists(selected.folderPath + @"\sevtf_ext.png") && skins.ContainsKey("mat_sevtf_ext"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\sevtf_ext.png", skins["mat_sevtf_ext"]));
                if (File.Exists(selected.folderPath + @"\sevtf_ext2.png") && skins.ContainsKey("mat_sevtf_ext2"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\sevtf_ext2.png", skins["mat_sevtf_ext2"]));
                if (File.Exists(selected.folderPath + @"\sevtf_int.png") && skins.ContainsKey("mat_sevtf_int"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\sevtf_int.png", skins["mat_sevtf_int"]));
                if (File.Exists(selected.folderPath + @"\sevtf_lowPoly.png") && skins.ContainsKey("mat_sevtf_lowPoly"))
                    StartCoroutine(UpdateTexture(selected.folderPath + @"\sevtf_lowPoly.png", skins["mat_sevtf_lowPoly"]));
                Debug.Log("Loaded F-45A Skins");
                break;
            case VTOLVehicles.None:
                Debug.LogError("API FAILED");
                break;
        }

        if (File.Exists(selected.folderPath + @"\cockpitProps.png") && skins.ContainsKey("mat_cockpitProps"))
            StartCoroutine(UpdateTexture(selected.folderPath + @"\cockpitProps.png", skins["mat_cockpitProps"]));
        if (File.Exists(selected.folderPath + @"\acesSeat.png") && skins.ContainsKey("mat_acesSeat"))
            StartCoroutine(UpdateTexture(selected.folderPath + @"\acesSeat.png", skins["mat_acesSeat"]));
        if (File.Exists(selected.folderPath + @"\bobbleHead.png") && skins.ContainsKey("mat_bobbleHead"))
            StartCoroutine(UpdateTexture(selected.folderPath + @"\bobbleHead.png", skins["mat_bobbleHead"]));
        if (File.Exists(selected.folderPath + @"\miniMFD.png") && skins.ContainsKey("mat_miniMFD"))
            StartCoroutine(UpdateTexture(selected.folderPath + @"\miniMFD.png", skins["mat_miniMFD"]));
        if (File.Exists(selected.folderPath + @"\mfd.png") && skins.ContainsKey("mat_mfd"))
            StartCoroutine(UpdateTexture(selected.folderPath + @"\mfd.png", skins["mat_mfd"]));


    }
    private IEnumerator UpdateTexture(string path, Material material)
    {
        Debug.Log("Updating Texture from path: " + path);
        if (material == null)
        {
            Debug.LogError("Material was null, not updating texture");
        }
        else
        {
            WWW www = new WWW("file:///" + path);
            while (!www.isDone)
                yield return null;
            material.SetTexture("_MainTex", www.texture);
        }
    }
}
