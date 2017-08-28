using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_controllerScene : MonoBehaviour {


    public bool on = false;

	// Use this for initialization
	void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!on) { Time.timeScale = 0f; on = true; }
            else { Time.timeScale = 1f; on = false; }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.Quit();
        }

    }

    string something(string fuck)
    {
        if (fuck == "nope")
        {
            return fuck;
        }
        else return null;
    }
}
