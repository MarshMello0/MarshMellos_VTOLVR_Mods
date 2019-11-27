using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Harmony;
using System.Reflection;

public class TestingMod : VTOLMOD
{
    Gun GAU1Gun;
    Gun GAU2Gun;
    Gun GAU3Gun;
    Gun GAU4Gun, GAUGun;
    bool isFiring;
    bool previous = true;
    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 7)
            StartCoroutine(SpawnGuns());
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 7 && GAUGun != null)
        {
            isFiring = (bool)Traverse.Create(GAUGun).Field("firing").GetValue();
            if (isFiring != previous)
            {
                previous = isFiring;
                Fire(isFiring);
            }
        }
    }


    private IEnumerator SpawnGuns()
    {
        yield return new WaitForSeconds(2);
        GameObject HP1 = GameObject.Find("HP1");
        GameObject HP2 = GameObject.Find("HP2");
        GameObject HP3 = GameObject.Find("HP3");
        GameObject HP4 = GameObject.Find("HP4");
        GameObject GAU = GameObject.Find("gau-8");

        Log($"HP1={HP1} HP2={HP2} HP3={HP3} HP4={HP4} GAU={GAU}");

        GameObject newGAU1 = Instantiate(GAU, HP1.transform);
        GameObject newGAU2 = Instantiate(GAU, HP2.transform);
        GameObject newGAU3 = Instantiate(GAU, HP3.transform);
        GameObject newGAU4 = Instantiate(GAU, HP4.transform);

        GAU1Gun = newGAU1.GetComponent<Gun>();
        GAU2Gun = newGAU2.GetComponent<Gun>();
        GAU3Gun = newGAU3.GetComponent<Gun>();
        GAU4Gun = newGAU4.GetComponent<Gun>();

        GAUGun = GAU.GetComponent<Gun>();

        newGAU1.transform.localPosition = new Vector3(0,-0.738f, 0);
        newGAU2.transform.localPosition = new Vector3(0, -0.738f, 0);
        newGAU3.transform.localPosition = new Vector3(0, -0.738f, 0);
        newGAU4.transform.localPosition = new Vector3(0, -0.738f, 0);
    }

    public void Fire(bool firing)
    {
        Log("Firing!!!");
        GAU1Gun.SetFire(firing);
        GAU2Gun.SetFire(firing);
        GAU3Gun.SetFire(firing);
        GAU4Gun.SetFire(firing);
    }

}

