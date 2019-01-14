using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageScript : MonoBehaviour {
    public string startingdir = "C:\\Users\\Tohron\\Pictures";
    public string dir = "C:\\Users\\Tohron\\Pictures";
    string startPath;

    List<string> fileList;

    private int proportionMode = 0; // 0 for relative, 1 for same width, 2 for same height
    private int arrangementMode = 0; // 0 for single, 1 for row, 2 for column
    public GameObject imageContainer; // used for horizontal & vertical list viewing
    public Transform mainCamPos;

    public float scaleFactor = 1f;
    public RawImage img;
    public Vector3 imgBasePos;
    //public BoxCollider imgCollider;
    public int index = 0;
    public RectTransform directoryPanel;
    public Text text;

    public GameObject loadPanel;
    public Image loadBar;

    //List<Texture2D> textureLists;
    //List<UniGifImage> gifLists;
    public GameObject imagePrefab;
    public GameObject gifPrefab;
    List<GameObject> imageList = new List<GameObject>();

    private Vector3 firstImagePos;
    private bool loadCompletionQueued = false;
    //private bool imageLoaded = false;
    public int imagesLoaded = 0;

    public Transform filePanelBase;
    public List<ImageFilePanel> filePanelScripts;
    public GameObject filePanelObject;
    public FolderScript fScript;

    void Start()
    {
        //PerformLoad();
        dir = startingdir;
        imageList = new List<GameObject>();
        filePanelScripts = new List<ImageFilePanel>();
        firstImagePos = img.transform.position;
    }

    void Update()
    {
        //print("IPosActive: " + img.transform.position); // change happens AFTER column switch completes
        if (dir != startingdir)
        {
            index = 0;
            print("NormalLoad: " + dir);
            PerformLoad();
            dir = startingdir;
        }

        if (loadCompletionQueued && imagesLoaded == fileList.Count)
        {
            loadCompletionQueued = false;
            loadPanel.SetActive(false);
            //ReloadImage();
            index = 0;
            EnableImage(index);
            if (proportionMode == 1)
            {
                proportionMode = 0;
                SameWidthSwitch(); // handles arrangement internally
            } else if (proportionMode == 2)
            {
                proportionMode = 0;
                SameHeightSwitch(); // handles arrangement internally
            } else {
                if (arrangementMode == 1)
                {
                    arrangementMode = 0; // allows function to run
                    RowSwitch();
                }
                else if (arrangementMode == 2)
                {
                    arrangementMode = 0; // allows function to run
                    ColumnSwitch();
                }
            }
            CenterToVector();
            //firstImagePos = imgBasePos;
            loadFileObjs();
        }
        else if (loadCompletionQueued)
        {
            loadBar.fillAmount = imagesLoaded * 1.0f / fileList.Count;
        }
    }

    private void loadFileObjs()
    {
        // Now needs to list out all the valid files
        int levelCounter = -1;
        for (int i = 0; i < fileList.Count; i++)
        {
            if (i % 20 == 0)
                levelCounter++;
            GameObject g = Instantiate(filePanelObject, Vector3.zero, Quaternion.identity);
            g.transform.SetParent(filePanelBase);
            g.transform.rotation = filePanelBase.rotation;
            g.transform.position = new Vector3(filePanelBase.position.x,
                filePanelBase.position.y - 0.05f * i + 1f * levelCounter, filePanelBase.position.z - .75f * levelCounter);
            ImageFilePanel ifp = g.GetComponent<ImageFilePanel>();
            ifp.filePath = fileList[i];
            ifp.text.text = ifp.filePath.Replace(startPath, "").Remove(0, 7); // leaves only the file name
            ifp.index = i;
            ifp.iScript = this;
            filePanelScripts.Add(ifp);
        }
        if (fileList.Count > 0)
            filePanelScripts[0].Select();
    }

    /**
     * Called whenever the active image might be changed
     */
    public void UpdateActiveImage()
    {
        getIndex();
        //print("UpActive#" + index);
        filePanelScripts[index].Select();
    }
    public void LoadText(string dirText)
    {
        if (dirText.Length > 50)
        {
            directoryPanel.sizeDelta = new Vector2(4 * dirText.Length, directoryPanel.sizeDelta.y); // works
        }
        dir = dirText;
        //print("FieldLoad: " + dir);
        index = 0;
        fScript.OpenDirectory(dirText);
        PerformLoad();
        dir = startingdir;
    }

    public void PerformLoad()
    {
        startPath = dir;
        int i;
        GameObject obj;
        imagesLoaded = 0;

        for (i = 0; i < filePanelScripts.Count; i++)
        {
            Destroy(filePanelScripts[i].gameObject);
        }
        filePanelScripts = new List<ImageFilePanel>();

        string[] jpgPaths = Directory.GetFiles(startPath, "*.jpg");
        string[] jpegPaths = Directory.GetFiles(startPath, "*.jpeg");
        string[] pngPaths = Directory.GetFiles(startPath, "*.png");
        string[] gifPaths = Directory.GetFiles(startPath, "*.gif");

        fileList = new List<string>();
        // ----------------- Want to merge the arrays so they are sorted only by name
        fileList.AddRange(jpgPaths);
        fileList.AddRange(jpegPaths);
        fileList.AddRange(pngPaths);
        fileList.AddRange(gifPaths);
        fileList.Sort();
        for (i = 0; i < fileList.Count; i++)
        {
            if (!fileList[i].Contains(".gif")) { // don't need to append file:// for .gif files
                fileList[i] = "file://" + fileList[i];
            }
            //print("File: " + fileList[i]);
        }
        //textureLists = new List<Texture2D>();
        //gifLists = new List<UniGifImage>();
        //var www : WWW;
        firstImagePos = imgBasePos;
        if (imageList.Count > 0)
        {
            getIndex();
            firstImagePos = imageList[index].transform.position;
            print("IndexImagePos: " + firstImagePos);
        }
        //img.transform.position = imgBasePos;

        ClearImages();
        ResizeImageContainer();
        loadPanel.SetActive(true);
        loadBar.fillAmount = 0;
        for (i = 0; i < fileList.Count; i++)
        {
            //print("Adding File: " + fileList[i]);
            //www = new WWW(fileList[i]);
            //yield www;
            //print("Getting Image"); // This stage is reached
            //textureLists.Add(www.texture); // --------------------------- www.texture cannot handle .gif or .bmp images
            //print("Added: " + fileList[i]);
            imageList.Add(null);
            StartCoroutine(ImageLoadCoroutine(i));
            //loadBar.fillAmount = i * 1.0f / fileList.Count;
        }
        loadCompletionQueued = true;
        //loadPanel.SetActive(false);
        //ReloadImage();
    }

    private IEnumerator ImageLoadCoroutine(int i)
    {
        GameObject imgObj;
        if (fileList[i].Contains(".gif"))
        {
            imgObj = Instantiate(gifPrefab);
            imgObj.transform.SetParent(imageContainer.transform);
            imgObj.transform.position = firstImagePos;
            imgObj.transform.localRotation = Quaternion.identity;
            imgObj.transform.localScale = Vector3.one;
            /*
            UniGifImage gifImage = new UniGifImage();
            gifImage.m_rawImage = img; // ISSUE: Should only run the animation if .gif is the active image
            yield return StartCoroutine(gifImage.SetGifFromUrlCoroutine(fileList[i]));
            gifLists.Add(gifImage);
            */
            UniGifTest test = imgObj.GetComponent<UniGifTest>();
            test.dir = fileList[i];
            if (i == 0)
            {
                //test.LoadGif(true);
                yield return StartCoroutine(test.ViewGifCoroutine(true));
            } else
            {
                //test.LoadGif(false);
                yield return StartCoroutine(test.ViewGifCoroutine(false));
                imgObj.GetComponent<RawImage>().enabled = false;
                imgObj.GetComponent<BoxCollider>().enabled = false;
            }
            imagesLoaded++;
            //loadBar.fillAmount = imagesLoaded * 1.0f / fileList.Count;
        }
        else
        {
            WWW www = new WWW(fileList[i]);
            yield return www;
            // Needs to add differently if the texture is a .gif
            //textureLists.Add(www.texture);
            imgObj = Instantiate(imagePrefab);
            imgObj.transform.SetParent(imageContainer.transform);
            imgObj.transform.localPosition = Vector3.zero;
            imgObj.transform.localRotation = Quaternion.identity;
            imgObj.transform.localScale = Vector3.one;
            imgObj.GetComponent<RawImage>().texture = www.texture;
            imgObj.GetComponent<RawImage>().enabled = false;
            imgObj.GetComponent<BoxCollider>().enabled = false;
            imagesLoaded++;
        }
        //imageList.Add(imgObj);
        imageList[i] = imgObj;
    }

    private void ClearImages()
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            Destroy(imageList[i]);
        }
        imageList = new List<GameObject>();
    }

    /*
    public void ReloadImage()
    {
        // Needs special check for whether image is a .gif
        print("Index: " + index); // 0 remains out of range
        //while (textureLists.Count <= index)
        //    print("Waiting for load..."); // this approach causes Unity to freeze
        img.texture = textureLists[index];
        Vector2 sDelta = 0.02f * Mathf.Pow(scaleFactor, 2) * (new Vector2(img.texture.width, img.texture.height));
        img.rectTransform.sizeDelta = sDelta;
        imgCollider.size = new Vector3(sDelta.x, sDelta.y, .01f);
    }
    */
    public void DisableImage(int imageIndex)
    {
        GameObject curImgObj = imageList[imageIndex];
        UniGifImage gifScript = curImgObj.GetComponent<UniGifImage>();
        if (gifScript != null)
        {
            print("StoppingGif");
            gifScript.Stop();
        }
        RawImage curImg = curImgObj.GetComponent<RawImage>();
        BoxCollider curColl = curImgObj.GetComponent<BoxCollider>();
        curColl.enabled = false;
        curImg.enabled = false;
    }
    public void EnableImage(int imageIndex)
    {
        GameObject curImgObj = imageList[imageIndex];
        UniGifImage gifScript = curImgObj.GetComponent<UniGifImage>();
        if (gifScript != null)
        {
            print("PlayingGif");
            gifScript.Play();
        }
        RawImage curImg = curImgObj.GetComponent<RawImage>();
        BoxCollider curColl = curImgObj.GetComponent<BoxCollider>();
        //Vector2 sDelta = 0.02f * Mathf.Pow(scaleFactor, 2) * (new Vector2(curImg.texture.width, curImg.texture.height));
        Vector2 sDelta = new Vector2(curImg.texture.width, curImg.texture.height);
        curImg.rectTransform.sizeDelta = sDelta;
        curColl.size = new Vector3(sDelta.x, sDelta.y, .01f);
        curColl.enabled = true;
        curImg.enabled = true;
    }
    
    public void IndexUp()
    {
        if (arrangementMode == 0)
        {
            if (index < fileList.Count - 1)
            {
                DisableImage(index);
                index++;
                //url = fileList[index];
                //ReloadImage();
                EnableImage(index);
            }
        } else
        {
            getIndex();
            if (index < fileList.Count - 1)
            {
                Vector3 pos1 = imageList[index].transform.position;
                Vector3 pos2 = imageList[index + 1].transform.position;
                img.transform.position += (pos1 - pos2);
            }
        }
        UpdateActiveImage();
    }

    public void IndexDown()
    {
        if (arrangementMode == 0)
        {
            if (index > 0)
            {
                DisableImage(index);
                index--;
                //url = fileList[index];
                //ReloadImage();
                EnableImage(index);
            }
        }
        else
        {
            getIndex();
            if (index > 0)
            {
                Vector3 pos1 = imageList[index].transform.position;
                Vector3 pos2 = imageList[index - 1].transform.position;
                img.transform.position += (pos1 - pos2);
            }
        }
        UpdateActiveImage();
    }

    public void GoToIndex(int goIndex)
    {
        if (arrangementMode == 0)
        {
            DisableImage(index);
            index = goIndex;
            //url = fileList[index];
            //ReloadImage();
            EnableImage(index);
        }
        else
        {
            Vector3 pos1 = imageList[index].transform.position;
            index = goIndex;
            Vector3 pos2 = imageList[index].transform.position;
            img.transform.position += (pos1 - pos2);
        }
        // UpdateActiveImage was already handled previously
    }

    public void ResetPosition()
    {
        
        index = 0;
        filePanelScripts[index].Select();
        if (arrangementMode == 0)
        {
            img.transform.position = imgBasePos;
            imageContainer.transform.position = imgBasePos;
            for (int i = 0; i < imageList.Count; i++)
            {
                imageList[i].transform.position = imgBasePos;
            }
        } else if (imageList.Count > 0)
        {
            Vector3 containerSeparation = imageContainer.transform.position - imageList[0].transform.position;
            img.transform.position = imgBasePos;
            imageContainer.transform.position = imgBasePos + containerSeparation;
        }
    }

    public void CenterToVector()
    {
        if (arrangementMode == 0)
        {
            img.transform.position = firstImagePos;
            imageContainer.transform.position = firstImagePos;
            for (int i = 0; i < imageList.Count; i++)
            {
                imageList[i].transform.position = firstImagePos;
            }
        }
        else if (imageList.Count > 0)
        {
            Vector3 containerSeparation = imageContainer.transform.position - imageList[0].transform.position;
            img.transform.position = firstImagePos;
            imageContainer.transform.position = firstImagePos + containerSeparation;
        }
    }

    public void ZoomIn(Vector3 hitPos)
    {
        scaleFactor += Time.deltaTime / 5;
        Zoom(hitPos);
    }

    public void ZoomOut(Vector3 hitPos)
    {
        scaleFactor -= Time.deltaTime / 5;
        Zoom(hitPos);
    }

    private void Zoom(Vector3 hitPos)
    {
        float oldScalar = imageContainer.transform.localScale.x;
        float scalar = 0.02f * Mathf.Pow(scaleFactor, 2); // ISSUE: scalar may be based on previous scale
        if (arrangementMode == 0)
        {
            GameObject curImgObj = imageList[index];
            RawImage curImg = curImgObj.GetComponent<RawImage>();
            if (hitPos != null  && hitPos != img.transform.position)
            {
                Vector3 castOffset = hitPos - curImg.transform.position;
                ResizeImageContainer();
                Vector3 newOffset = (scalar / oldScalar) * castOffset;
                Vector3 posInfo = hitPos - newOffset;
                img.transform.position = new Vector3(posInfo.x, posInfo.y, img.transform.position.z);
            }
            else
            {
                //ResizeImage();
                ResizeImageContainer();
            }
        } else
        {
            BoxCollider curColl = imageContainer.GetComponent<BoxCollider>();
            if (hitPos != null && hitPos != img.transform.position)
            {
                Vector3 castOffset = hitPos - imageContainer.transform.position;
                ResizeImageContainer();
                Vector3 newOffset = (scalar / oldScalar) * castOffset;
                Vector3 posInfo = hitPos - newOffset;
                imageContainer.transform.position = new Vector3(posInfo.x, posInfo.y, img.transform.position.z);
            }
            else
            {
                //ResizeImage();
                ResizeImageContainer();
            }
        }
    }

    public void ResizeImage()
    {
        GameObject curImgObj = imageList[index];
        RawImage curImg = curImgObj.GetComponent<RawImage>();
        BoxCollider curColl = curImgObj.GetComponent<BoxCollider>();
        Vector2 sDelta = 0.02f * Mathf.Pow(scaleFactor, 2) * (new Vector2(curImg.texture.width, curImg.texture.height));
        curImg.rectTransform.sizeDelta = sDelta;
        curColl.size = new Vector3(sDelta.x, sDelta.y, .01f);
        /*
        Vector2 sDelta = 0.02f * Mathf.Pow(scaleFactor, 2) * (new Vector2(img.texture.width, img.texture.height));
        img.rectTransform.sizeDelta = sDelta;
        imgCollider.size = new Vector3(sDelta.x, sDelta.y, .01f);
        */
    }

    private void ResizeImageContainer()
    {
        imageContainer.transform.localScale = 0.02f * Mathf.Pow(scaleFactor, 2) * Vector3.one;
    }

    public void ProportionalSwitch()
    {
        if (proportionMode == 0)
            return;
        proportionMode = 0;
        if (imageList.Count == 0)
            return;
        for (int i = 0; i < imageList.Count; i++)
        {
            imageList[i].transform.localScale = Vector3.one;
        }

        RecalculateArrangement();
    }
    public void SameWidthSwitch()
    {
        if (proportionMode == 1)
            return;
        proportionMode = 1;
        if (imageList.Count == 0)
            return;

        getIndex();
        RawImage ri = imageList[index].GetComponent<RawImage>();
        float baseWidth = imageList[index].transform.localScale.x * ri.texture.width;

        for (int i = 0; i < imageList.Count; i++)
        {
            UniGifImage gifScript = imageList[i].GetComponent<UniGifImage>();
            if (gifScript != null)
            {
                gifScript.Play();
            }
            ri = imageList[i].GetComponent<RawImage>();
            // Rescales images so they all have the same width as baseWidth
            float newScale = baseWidth / ri.texture.width;
            if (gifScript != null)
            {
                gifScript.Stop();
            }
            imageList[i].transform.localScale = newScale * Vector3.one;
        }

        RecalculateArrangement();
    }

    public void SameHeightSwitch()
    {
        if (proportionMode == 2)
            return;
        proportionMode = 2;
        if (imageList.Count == 0)
            return;

        getIndex();
        RawImage ri = imageList[index].GetComponent<RawImage>();
        float baseHeight = imageList[index].transform.localScale.x * ri.texture.height;

        for (int i = 0; i < imageList.Count; i++)
        {
            UniGifImage gifScript = imageList[i].GetComponent<UniGifImage>();
            if (gifScript != null)
            {
                gifScript.Play();
            }
            ri = imageList[i].GetComponent<RawImage>();
            // Rescales images so they all have the same height as baseHeight
            float newScale = baseHeight / ri.texture.height;
            if (gifScript != null)
            {
                gifScript.Stop();
            }
            imageList[i].transform.localScale = newScale * Vector3.one;
        }

        RecalculateArrangement();
    }

    private void RecalculateArrangement()
    {
        if (arrangementMode == 1)
        {
            arrangementMode = 0;
            RowSwitch();
        }
        if (arrangementMode == 2)
        {
            arrangementMode = 0;
            ColumnSwitch();
        }
    }

    public void SingleSwitch()
    {
        if (arrangementMode == 0)
            return;
        
        getIndex();
        arrangementMode = 0;
        if (imageList.Count == 0)
            return;
        Vector3 imagePos = imageList[index].transform.position;
        setImageStatus(false);
        img.transform.position = imagePos;
        imageContainer.transform.position = imagePos;
        for (int i = 0; i < imageList.Count; i++)
        {
            //imageList[i].transform.SetParent(img.transform);
            imageList[i].transform.position = imagePos;
        }
        imageContainer.GetComponent<BoxCollider>().enabled = false;
        //Destroy(imageContainer);
    }

    public void RowSwitch()
    {
        if (arrangementMode == 1)
            return;
        if (arrangementMode == 0)
            setImageStatus(true);

        getIndex();
        arrangementMode = 1;
        if (imageList.Count == 0)
            return;

        //float scaleAdjuster = 0.00125f; // ----------- May want to switch to involving square of scaleFactor
        //float scaleAdjuster = 1;

        imageContainer.GetComponent<BoxCollider>().enabled = true;
        imageContainer.transform.localScale = 0.02f * Mathf.Pow(scaleFactor, 2) * Vector3.one;
        float width = getTotalWidth();
        float toIndex = getWidthToIndex();
        BoxCollider curColl = imageContainer.GetComponent<BoxCollider>();
        curColl.size = new Vector3(width, getMaxHeight(), 0.1f);
        float posShift = -0.02f * Mathf.Pow(scaleFactor, 2) * (toIndex - width / 2);
        imageContainer.transform.position = imageList[index].transform.position + new Vector3(posShift, 0, 0);
        Vector3 basePosition = -new Vector3(width / 2, 0, 0);
        for (int i = 0; i < imageList.Count; i++)
        {
            //imageList[i].transform.SetParent(imageContainer.transform);
            RawImage ri = imageList[i].GetComponent<RawImage>();
            imageList[i].transform.localPosition = basePosition + new Vector3(imageList[i].transform.localScale.x * ri.texture.width / 2, 0, 0);
            basePosition += new Vector3(imageList[i].transform.localScale.x * ri.texture.width + 40, 0, 0);
        }
    }

    public void ColumnSwitch()
    {
        if (arrangementMode == 2)
            return;
        if (arrangementMode == 0)
            setImageStatus(true);

        getIndex();
        arrangementMode = 2;
        if (imageList.Count == 0)
            return;

        //float scaleAdjuster = 0.00125f; // ----------- May want to switch to involving square of scaleFactor
        //float scaleAdjuster = 1;

        imageContainer.GetComponent<BoxCollider>().enabled = true;
        imageContainer.transform.localScale = 0.02f * Mathf.Pow(scaleFactor, 2) * Vector3.one;
        float height = getTotalHeight();
        float toIndex = getHeightToIndex();
        BoxCollider curColl = imageContainer.GetComponent<BoxCollider>();
        curColl.size = new Vector3(getMaxWidth(),height, 0.1f);
        float posShift = -0.02f * Mathf.Pow(scaleFactor, 2) * (toIndex - height / 2);
        imageContainer.transform.position = imageList[index].transform.position - new Vector3(0, posShift, 0);
        Vector3 basePosition = new Vector3(0, height / 2, 0);
        for (int i = 0; i < imageList.Count; i++)
        {
            //imageList[i].transform.SetParent(imageContainer.transform);
            RawImage ri = imageList[i].GetComponent<RawImage>();
            imageList[i].transform.localPosition = basePosition - new Vector3(0, imageList[i].transform.localScale.x * ri.texture.height / 2, 0);
            basePosition -= new Vector3(0, imageList[i].transform.localScale.x * ri.texture.height + 40, 0);
        }
        
    }
    /**
     * Sets "index" to be the index of whichever image in imageList is closest to the viewer.
     */
    private int getIndex()
    {
        if (arrangementMode == 0)
            return index;

        float minDistance = 1000f;
        for (int i = 0; i < imageList.Count; i++)
        {
            float distance = (imageList[i].transform.position - mainCamPos.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                index = i;
            } else
            {
                return index; // once distance starts increasing again, no need to iterate further
            }
        }
        return index;
    }

    private void setImageStatus(bool enabled)
    {
        for (int i = 0; i < imageList.Count; i++)
        {
            if (i != index)
            {
                if (enabled)
                {
                    EnableImage(i);
                }
                else
                {
                    DisableImage(i);
                }
            }
        }
    }

    /**
     * Gets the combined width of all the images in ImageList, plus any separators
     * Needs to account for adjustments if width or height is locked
     */
    private float getTotalWidth()
    {
        float totalWidth = 0f;
        for (int i = 0; i < imageList.Count; i++)
        {
            RawImage ri = imageList[i].GetComponent<RawImage>();
            totalWidth += imageList[i].transform.localScale.x * ri.texture.width + 40;
        }

        return totalWidth;
    }
    /**
     * Gets the combined height of all the images in ImageList, plus any separators
     * Needs to account for adjustments if width or height is locked
     */
    private float getTotalHeight()
    {
        float totalHeight = 0f;
        for (int i = 0; i < imageList.Count; i++)
        {
            RawImage ri = imageList[i].GetComponent<RawImage>();
            totalHeight += imageList[i].transform.localScale.x * ri.texture.height + 40;
        }

        return totalHeight;
    }
    /**
     * Gets the width of the widest image in ImageList
     * Needs to account for adjustments if width or height is locked
     */
    private float getMaxWidth()
    {
        float maxWidth = 0f;
        for (int i = 0; i < imageList.Count; i++)
        {
            RawImage ri = imageList[i].GetComponent<RawImage>();
            float w = imageList[i].transform.localScale.x * ri.texture.width;
            if (w > maxWidth)
                maxWidth = w;
        }

        return maxWidth;
    }
    /**
     * Gets the height of the talles image in ImageList
     * Needs to account for adjustments if width or height is locked
     */
    private float getMaxHeight()
    {
        float maxHeight = 0f;
        for (int i = 0; i < imageList.Count; i++)
        {
            RawImage ri = imageList[i].GetComponent<RawImage>();
            float h = imageList[i].transform.localScale.x * ri.texture.height;
            if (h > maxHeight)
                maxHeight = h;
        }

        return maxHeight;
    }

    private float getWidthToIndex()
    {
        float widthTotal = 0f;
        for (int i = 0; i < imageList.Count; i++)
        {
            RawImage ri = imageList[i].GetComponent<RawImage>();
            if (i < index)
            {
                widthTotal += imageList[i].transform.localScale.x * ri.texture.width + 40;
            } else if (i == index)
            {
                widthTotal += 0.5f * imageList[i].transform.localScale.x * ri.texture.width + 0;
                return widthTotal; // should always end here
            }
        }
        return widthTotal;
    }

    private float getHeightToIndex()
    {
        float heightTotal = 0f;
        for (int i = 0; i < imageList.Count; i++)
        {
            RawImage ri = imageList[i].GetComponent<RawImage>();
            if (i < index)
            {
                heightTotal += imageList[i].transform.localScale.x * ri.texture.height + 40;
            }
            else if (i == index)
            {
                heightTotal += 0.5f * imageList[i].transform.localScale.x * ri.texture.height + 0;
                return heightTotal; // should always end here
            }
        }
        return heightTotal;
    }
}
