using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFilePanel : MonoBehaviour {
    public ImageScript iScript;
    public Text text;
    public Image panelBack;
    public Color backNormal;
    public Color backSelect;
    public int index = 0;
    public string filePath;
    public GameObject highlightBox;

    public bool isHighlighted = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isHighlighted)
        {
            highlightBox.SetActive(true);
        }
        else
        {
            highlightBox.SetActive(false);
            text.color = new Color(0.5f, 1, 1);
        }
        isHighlighted = false;

    }

    public void Select()
    {
        //ImageFilePanel[] panels = GetComponents<ImageFilePanel>(); // only looks on this gameObject
        //ImageFilePanel[] panels = GameObject.FindObjectsOfType<ImageFilePanel>(); // ISSUE: Finds nothing when file pane disabled
        //print("Panel#: " + panels.Length); 
        for (int i = 0; i < iScript.filePanelScripts.Count; i++)
        {
            iScript.filePanelScripts[i].panelBack.color = backNormal;
        }
        panelBack.color = backSelect;
    }
}
