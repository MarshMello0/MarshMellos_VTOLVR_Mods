using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Harmony;
using System.Reflection;

public class TestingMod : VTOLMOD
{
    private void Start()
    {
        var harmony = HarmonyInstance.Create("com.asdasdas.asdads.adsdsa");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPatch(typeof(VRHandController))]
    [HarmonyPatch("Update")]
    class Patch
    {
        static void Postfix()
        {
            Debug.Log("Hello World");
        }
    }
}

