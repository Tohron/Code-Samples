using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoFilePanel : MonoBehaviour {

    public Text text;
    public Image panelBack;
    public Color backNormal;
    public Color backSelect;
    public string filePath;
    public GameObject highlightBox;

    public bool isHighlighted = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isHighlighted)
        {
            highlightBox.SetActive(true);
        } else
        {
            highlightBox.SetActive(false);
            text.color = new Color(0.5f, 1, 1);
        }
        isHighlighted = false;

    }

    public void Select()
    {
        //VideoFilePanel[] panels = GetComponents<VideoFilePanel>();
        VideoFilePanel[] panels = GameObject.FindObjectsOfType<VideoFilePanel>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].panelBack.color = backNormal;
        }
        panelBack.color = backSelect;
    }
}
