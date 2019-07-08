using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Info("Testing Mod","Just Testing","","0.0.0")]
public class TestingMod : VTOLMOD
{

    private void Start()
    {
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(2);
        Debug.Log(Application.unityVersion);
    }
}

