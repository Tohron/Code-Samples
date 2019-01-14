using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightPanel : MonoBehaviour {

    public bool isHighlighted = false;
    public Image img;
    public Text text;


    public Color backDim;
    public Color backHigh;
    public Color textDim;
    public Color textHigh;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isHighlighted)
        {
            img.color = backHigh;
            text.color = textHigh;
        }
        else
        {
            img.color = backDim;
            text.color = textDim;
        }
        isHighlighted = false;
    }
}
