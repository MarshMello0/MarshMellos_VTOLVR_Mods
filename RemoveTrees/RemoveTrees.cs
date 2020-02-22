using HarmonyLib;
using System.Reflection;

public class RemoveTrees : VTOLMOD
{
    private static readonly string harmonyID = "marsh.notree";

    private void Start()
    {
        Harmony harmony = new Harmony(harmonyID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
[HarmonyPatch(typeof(VTTMapTrees.TreeJob), "CreateTree")]
public static class Patch_TreeMaster
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return false;
    }
}