using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Harmony;
using System.Reflection;
using Random = System.Random;

public class MusicShuffle : VTOLMOD
{
    private static readonly string harmonyID = "marsh.musicshuffle";

    private void Start()
    {
        HarmonyInstance instance = HarmonyInstance.Create(harmonyID);
        instance.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        Random rnd = new Random();
        for (int i = 0; i < list.Count; i++)
        {
            int k = rnd.Next(0, i);
            T value = list[k];
            list[k] = list[i];
            list[i] = value;
        }
        return list;
    }
}



[HarmonyPatch(typeof(CockpitRadio))]
[HarmonyPatch("Start")]
public class Patch
{
    public static void Postfix()
    {
        CockpitRadio cockpitRadio = GameObject.FindObjectOfType<CockpitRadio>();
        if (!cockpitRadio)
        {
            Debug.Log("Couldn't find the cockpit radio");
            return;
        }
        List<string> songs = (List<string>)Traverse.Create(cockpitRadio).Field("origSongs").GetValue();

        Random rnd = new Random();
        songs = MusicShuffle.Shuffle(songs);
        Traverse.Create(cockpitRadio).Field("shuffledSongs").SetValue(songs);
        Traverse.Create(cockpitRadio).Field("origSongs").SetValue(songs);
        Debug.Log("Music Shuffle: Reordered " + songs.Count + " songs");
    }
}