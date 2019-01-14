using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightButton : MonoBehaviour {
    public bool isHighlighted = false;
    public GameObject tooltip;
    public bool isSelected = false;
    public Image img;
    public string modeCommand = "";
    public HighlightButton[] otherButtons;

    public GameObject scriptObj;

    public Color selected;
    public Color highlighted;
    public Color unselected;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isHighlighted)
        {
            tooltip.SetActive(true);
        } else
        {
            tooltip.SetActive(false);
        }

        if (isHighlighted && !isSelected)
        {
            img.color = highlighted;
        }
        else if (isSelected)
        {
            img.color = selected;
        }
        else
        {
            img.color = unselected;
        }
        isHighlighted = false;
    }

    public void EngageModeCommand()
    {
        isSelected = true;
        for (int i = 0; i < otherButtons.Length; i++)
        {
            otherButtons[i].isSelected = false;
        }
        scriptObj.SendMessage(modeCommand);
    }
}
