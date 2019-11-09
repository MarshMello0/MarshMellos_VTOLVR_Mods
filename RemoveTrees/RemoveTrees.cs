using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;

public class RemoveTrees : VTOLMOD
{
    private static readonly string harmonyID = "marsh.notree";

    private void Start()
    {
        HarmonyInstance instance = HarmonyInstance.Create(harmonyID);
        instance.PatchAll(Assembly.GetExecutingAssembly());
    }
}

[HarmonyPatch(typeof(TreeParticleMaster))]
[HarmonyPatch("Start")]
class Patch
{
    static bool Prefix(TreeParticleMaster treeParticleMaster)
    {
        treeParticleMaster.treeBlocksPerChunk = 0;
        return false;
    }
}