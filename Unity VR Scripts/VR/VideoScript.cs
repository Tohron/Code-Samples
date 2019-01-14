using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScript : MonoBehaviour {
    public bool progHover = false;
    public bool fileMode = true;

    public bool shouldLoop = false;
    //public VideoPlayer vPlayer;
    //public GameObject umpObject;
    public bool umpSwitchQueued = false;
    public UniversalMediaPlayer umPlayer;
    private Vector3 initVidPos;
    public GameObject vidImage;
    public AudioSource source;
    public RectTransform directoryPanel;
    public string text; // text directory of the folder in use
    public string file; // address of the video file
    //public Text text;
    public GameObject overlayBlocker;
    public GameObject progContainer;
    public Image vidProgress;
    public Image vidProgressChange;
    public Text timeText;
    private string vidLengthString = "";
    public BoxCollider progCollider;
    public Transform videoPlane;
    Vector3 baseScale = new Vector3(-.5f, .5f, -.5f);
    // Use this for initialization
    public string curPath;

    public int timeShifting = 0; // 1 for fast-forward, -1 for fast reverse, 0 for normal play
    public float forwardTime = 0;

    public Transform filePanelBase;
    public List<GameObject> filePanelObjects;
    public GameObject filePanelObject;

    void Start () {
        //curPath = vPlayer.url;
        curPath = umPlayer.Path;
        filePanelObjects = new List<GameObject>();
        initVidPos = vidImage.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (umpSwitchQueued)
        {
            umPlayer.Play();
            umpSwitchQueued = false;
        }

        if (!shouldLoop && umPlayer.Length - umPlayer.Time < 300)
        {
            umPlayer.Pause();
            //print("End Pause");
        }
		if (umPlayer.Path != curPath)
        {
            print("Attempting Change"); // works correctly
            umPlayer.Stop();
            umPlayer.Play();
            curPath = umPlayer.Path;
        }

        if (fileMode)
        {
            umPlayer.Pause();
            //print("File Pause");
        }
        
        if (timeShifting != 0)
        {
            umPlayer.Pause();
            //print("Timeshift Pause");
            progContainer.SetActive(true);
            vidProgress.enabled = true;
            vidProgressChange.enabled = true;
            forwardTime += Time.deltaTime;
            float shift = timeShifting * 25 * Time.deltaTime * Mathf.Pow(forwardTime, 1.5f);
            umPlayer.Position += shift * 1000 / umPlayer.Length;
            //print("Shifting - FTime:" + forwardTime + ", Shift: " + shift);
            source.volume = 0.0f;
        } else
        {
            progContainer.SetActive(false);
            vidProgress.enabled = false;
            vidProgressChange.enabled = false;
            forwardTime = 0;
            source.volume = 1.0f;
            if (!fileMode && umPlayer.Length - umPlayer.Time > 1000 && !umPlayer.IsPlaying)
                umPlayer.Play();
        }
        if (progHover)
        {
            progContainer.SetActive(true);
            vidProgress.enabled = true;
            vidProgressChange.enabled = true;
        }
        timeShifting = 0;
        float videoLength = umPlayer.Length / 1000f;
        float vTime = umPlayer.Time / 1000f;
        vidProgress.fillAmount = vTime / videoLength;
        int minutes = Mathf.FloorToInt(vTime / 60);
        int seconds = Mathf.FloorToInt(vTime - 60 * minutes);
        string vidProgString;
        if (seconds < 10)
        {
            vidProgString = minutes + ":0" + seconds;
        }
        else
        {
            vidProgString = minutes + ":" + seconds;
        }
        timeText.text = vidProgString + "/" + vidLengthString;
        //print("Frame Extension: " + (vPlayer.frameCount / vPlayer.frameRate)); // this method actually works
        //print("ClipLength: " + (vPlayer.clip.length / 60)); // 3.8 min is double recorded amount ------ is the same amount for all videos!!!
        if (Input.GetKeyUp("9"))
        {
            Application.LoadLevel(0);
        }
    }

    public void Zoom(bool zIn, Vector3 zoomPoint)
    {
        Vector3 castOffset = zoomPoint - videoPlane.position;
        float offsetRatioX = castOffset.x / videoPlane.localScale.x;
        float offsetRatioY = castOffset.y / videoPlane.localScale.y;
        if (zIn)
        {
            videoPlane.localScale = (1 + Time.deltaTime) * videoPlane.localScale;
        } else
        {
            videoPlane.localScale = (1 - Time.deltaTime) * videoPlane.localScale;
        }
        Vector3 newOffset = new Vector3(offsetRatioX * videoPlane.localScale.x,
                                            offsetRatioY * videoPlane.localScale.y, 0);
        Vector3 posInfo = zoomPoint - newOffset;
        videoPlane.position = new Vector3(posInfo.x, posInfo.y, videoPlane.position.z);
    }

    public void ResetPosition()
    {
        //vidImage.
        transform.position = initVidPos;
    }

    public void ProgBarClick(Vector3 hitPoint)
    {
        print("ProgBarClick");
        float xBound = progCollider.bounds.extents.x;
        float startX = progCollider.transform.position.x - xBound;
        float progAmount = (hitPoint.x - startX) / (2 * xBound);

        umPlayer.Position = progAmount;
        //umPlayer.Time = (long) (progAmount * umPlayer.Length);
        //umPlayer.Play();
    }
    public void ProgBarHover(Vector3 hitPoint)
    {
        float xBound = progCollider.bounds.extents.x;
        float startX = progCollider.transform.position.x - xBound;
        float progAmount = (hitPoint.x - startX) / (2 * xBound);

        vidProgressChange.fillAmount = progAmount;
    }

    public void LoadVideoDir()
    {
        for (int i = 0; i < filePanelObjects.Count; i++)
        {
            Destroy(filePanelObjects[i]);
        }
        //directoryPanel.sizeDelta = new Vector2(300, directoryPanel.sizeDelta.y); // works
        if (text.Length > 50)
        {
            directoryPanel.sizeDelta = new Vector2(4 * text.Length, directoryPanel.sizeDelta.y); // works
        }

        filePanelObjects = new List<GameObject>();
        string[] files = Directory.GetFiles(text);
        List<string> movieFiles = new List<string>();
        //print("Found " + files.Length + " files...");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Contains(".mp4") || files[i].Contains(".avi") || files[i].Contains(".flv") || files[i].Contains(".wmv")
                || files[i].Contains(".mov") || files[i].Contains(".qt") || files[i].Contains(".3gp")
                || files[i].Contains(".swf") || files[i].Contains(".mkv") || files[i].Contains(".ogg")
                || files[i].Contains(".webm")) {
                movieFiles.Add(files[i]);
                //print("Adding: " + files[i]);
            }
        }
        // Now needs to list out all the valid files
        int levelCounter = -1;
        for (int i = 0; i < movieFiles.Count; i++)
        {
            if (i % 20 == 0)
                levelCounter++;
            GameObject g = Instantiate(filePanelObject, Vector3.zero, Quaternion.identity);
            g.transform.SetParent(filePanelBase);
            g.transform.rotation = filePanelBase.rotation;
            g.transform.position = new Vector3(filePanelBase.position.x, 
                filePanelBase.position.y - 0.05f * i + 1f * levelCounter, filePanelBase.position.z - .75f * levelCounter);
            VideoFilePanel vfp = g.GetComponent<VideoFilePanel>();
            vfp.filePath = movieFiles[i];
            vfp.text.text = vfp.filePath.Replace(text, "").Remove(0,1); // leaves only the file name
            filePanelObjects.Add(g);
        }
    }

    public void LoadLengthString()
    {
        float videoLength = umPlayer.Length / 1000f;
        int minutes = Mathf.FloorToInt(videoLength / 60);
        int seconds = Mathf.FloorToInt(videoLength - 60 * minutes);
        if (seconds < 10)
        {
            vidLengthString = minutes + ":0" + seconds;
        }
        else
        {
            vidLengthString = minutes + ":" + seconds;
        }
    }

    public void LoadVideo()
    {
        /*
        Destroy(umPlayer.gameObject);
        GameObject g = Instantiate(umpObject, Vector3.zero, Quaternion.identity);
        g.transform.SetParent(videoPlane);
        g.transform.position = videoPlane.position;
        g.transform.localScale = Vector3.one;
        umPlayer = g.GetComponent<UniversalMediaPlayer>();
        umPlayer.RenderingObjects[0] = vidImage;
        source = umPlayer.AudioObjects[0].GetComponent<AudioSource>();
        */
        //umPlayer.Release(); // Not the desired functionality
        //umPlayer.
        print("Attempting VFile Load: " + file);
        umPlayer.Path = file;
        //umPlayer.Stop();
        umPlayer.Stop();
        umpSwitchQueued = true;
        curPath = umPlayer.Path;
        //umPlayer.Stop(); // Causes player to get stuck
        //umPlayer.Pause();
        overlayBlocker.SetActive(true);
    }

    public void EnableLoop()
    {
        shouldLoop = true;
        umPlayer.Loop = true;
        //if (!umPlayer.IsPlaying)
        //{
        //    umPlayer.Play();
        //}
    }

    public void DisableLoop()
    {
        shouldLoop = false;
        umPlayer.Loop = false;

    }
}
