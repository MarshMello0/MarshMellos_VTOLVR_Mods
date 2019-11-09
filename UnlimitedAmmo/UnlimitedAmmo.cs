using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;

class UnlimitedAmmo : VTOLMOD
{
    private static readonly string harmonyID = "marsh.unlimitedammo";

    private void Start()
    {
        HarmonyInstance instance = HarmonyInstance.Create(harmonyID);
        instance.PatchAll(Assembly.GetExecutingAssembly());
    }
}


[HarmonyPatch(typeof(VTOLCannon))]
[HarmonyPatch("Fire")]
class Patch
{
    static void Postfix(VTOLCannon cannon)
    {
        cannon.ammo = cannon.maxAmmo;
    }
}