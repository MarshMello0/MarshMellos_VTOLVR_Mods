using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestingMod
{
    public class Load
    {
        public static void Init()
        {
            new GameObject("Testing Mod", typeof(TestingMod));
        }
    }

    public class TestingMod : MonoBehaviour
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
}
