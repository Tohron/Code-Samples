using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FolderScript : MonoBehaviour {

    public GameObject folderObj;
    public List<FolderPanel> folders = new List<FolderPanel>();
    public GameObject folderContainer;
    public RectTransform scrollButton;

    public string curDir = "";
    public string parentDir;

    // one of these if left null
    public VideoScript vScript = null;
    public ImageScript iScript = null;

    public Text loadText;
    public GameObject startupOverlay;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadCurDirectory()
    {
        LoadDirectory(curDir);
    }

    public void OpenParentDirectory()
    {
        OpenDirectory(parentDir);
    }

    public void OpenDirectory(string dir)
    {
        curDir = dir;
        parentDir = Directory.GetDirectoryRoot(dir);
        /*
        string[] dirLevels = dir.Split('/');
        parentDir = dirLevels[0];
        for (int i = 1; i < dirLevels.Length - 1; i++)
        {
            parentDir += "/" + dirLevels[i];
        }
        */
        string[] newFolders = Directory.GetDirectories(dir);
        for (int i = 0; i < folders.Count; i++)
        {
            Destroy(folders[i].gameObject);
        }
        folders = new List<FolderPanel>();
        int levelCounter = -1;
        for (int i = 0; i < newFolders.Length; i++)
        {
            if (i % 4 == 0)
                levelCounter++;
            GameObject folder = Instantiate(folderObj, Vector3.zero, Quaternion.identity);
            folder.transform.SetParent(folderContainer.transform);
            folder.transform.rotation = folderContainer.transform.rotation;
            folder.transform.position = new Vector3(folderContainer.transform.position.x + .5f * levelCounter - 0.7f,
                folderContainer.transform.position.y - 0.09f * i + 0.36f * levelCounter + 0.18f, 
                folderContainer.transform.position.z);
            folder.transform.localScale = Vector3.one;
            FolderPanel folderP = folder.GetComponent<FolderPanel>();
            folderP.fScript = this;
            folderP.filePath = newFolders[i];
            string[] fileParts = newFolders[i].Split('\\');
            folderP.text.text = fileParts[fileParts.Length - 1]; // sets the text to the actual file name
        }
        // Calculates width of container and sets up scrolling
        if (levelCounter > 4)
        {
            float containerWidth = 100 * levelCounter; // --- this is an estimate - revise!!!
            float ratio = 400 / containerWidth;
            scrollButton.sizeDelta = new Vector2(400 * ratio, 20);
        } else
        {
            scrollButton.sizeDelta = new Vector2(400, 20);
        }

    }

    public void LoadDirectory(string dir)
    {
        loadText.text = dir;
        if (vScript) // loads directory for video mode
        {
            print("Text: " + dir);
            if (dir.Length > 0)
            {
                vScript.text = dir;
                vScript.LoadVideoDir();
            }
        } else if (iScript) // loads directory for image mode
        {
            if (dir.Length > 0)
            {
                iScript.LoadText(dir);
                startupOverlay.SetActive(false);
            }
        }
    }
}
