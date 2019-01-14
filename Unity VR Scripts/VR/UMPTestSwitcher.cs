using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMPTestSwitcher : MonoBehaviour {
    public string file1;
    public string file2;
    public UniversalMediaPlayer ump;

    public bool using2 = false;
    public bool switchCued = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (switchCued)
        {
            ump.Play();
            switchCued = false;
            
        }

        if (Input.GetKeyUp("1"))
        {
            if (using2)
            {
                ump.Path = file1;
            }
            else
            {
                ump.Path = file2;
            }
            using2 = !using2;
            ump.Stop();
            switchCued = true;
        }
	}
}
