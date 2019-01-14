#pragma strict

public var startingdir : String = "C:\\Users\\Tohron\\Pictures";
public var dir : String = "C:\\Users\\Tohron\\Pictures";
var startPath : String;

var fileList : List. < String >;

public var scaleFactor = 1f;
var img : RawImage;
var imgCollider : BoxCollider;
var index : int = 0;
var directoryPanel : RectTransform;
var text : Text;

var loadPanel : GameObject;
var loadBar : Image;

var textureLists : Array;

function Start () {
    //PerformLoad();
    dir = startingdir;
}

function Update () {
    if (dir != startingdir) {
        index = 0;
        print("NormalLoad: " + dir);
        PerformLoad();
        dir = startingdir;
    }
    if (Input.GetKeyDown ("left") && index > 0) {
        index--;
        //url = fileList[index];
        ReloadImage();
    } else if (Input.GetKeyDown ("right") && index < fileList.Count - 1) {
        index++;
        //url = fileList[index];
        ReloadImage();
    }
    
    if (Input.GetKey ("d") && scaleFactor > Time.deltaTime / 5) {
        scaleFactor -= Time.deltaTime / 5;
        ResizeImage();
    } else if (Input.GetKey ("a")) {
        scaleFactor += Time.deltaTime / 5;
        ResizeImage();
    }

	if (Input.GetKeyDown("0")) {
		Application.LoadLevel(1);
	}
}

public function LoadText(dirText : String) {
	if (dirText.Length > 50)
    {
        directoryPanel.sizeDelta = new Vector2(4 * dirText.Length, directoryPanel.sizeDelta.y); // works
    }
    dir = dirText;
    print("FieldLoad: " + dir);
    index = 0;
    PerformLoad();
    dir = startingdir;
}

public function PerformLoad() {
    startPath = dir;
    var i : int;
    var obj : GameObject;

    var jpgPaths : Array = new Array(Directory.GetFiles(startPath,"*.jpg"));
    var jpegPaths : Array = new Array(Directory.GetFiles(startPath,"*.jpeg"));
    var pngPaths : Array = new Array(Directory.GetFiles(startPath,"*.png"));
    //var gifPaths : Array = new Array(Directory.GetFiles(startPath,"*.bmp"));

    var filePaths : Array = new Array();
    // ----------------- Want to merge the arrays so they are sorted only by name
    filePaths = filePaths.Concat(jpgPaths);
    filePaths = filePaths.Concat(jpegPaths);
    filePaths = filePaths.Concat(pngPaths);
    //filePaths = filePaths.Concat(gifPaths);
    fileList = new List. < String >(); // init with filePaths causes exception
    //filePaths = filePaths.Concat(bmpPaths);
    for (i = 0; i < filePaths.length; i++) {
        fileList.Add(filePaths[i]);
    }
    fileList.Sort();
    for (i = 0; i < fileList.Count; i++) {
        fileList[i] = "file://" + fileList[i];
        //print("File: " + filePaths[i]);
    }
    textureLists = new Array();
    var www : WWW;

    loadPanel.SetActive(true);
    loadBar.fillAmount = 0;
    for (i = 0; i < fileList.Count; i++) {
        //print("Adding File: " + fileList[i]);
        www = new WWW(fileList[i]);
        yield www;
        //print("Getting Image"); // This stage is reached
        textureLists.Add(www.texture); // --------------------------- www.texture cannot handle .gif or .bmp images
        //print("Added: " + fileList[i]);
        loadBar.fillAmount = i * 1.0 / fileList.Count;
    }
    loadPanel.SetActive(false);
    ReloadImage();
}

public function ReloadImage() {
    img.texture = textureLists[index];
    var sDelta : Vector2 = 0.02f * Mathf.Pow(scaleFactor, 2) * (new Vector2(img.texture.width, img.texture.height));
    img.rectTransform.sizeDelta = sDelta;
    imgCollider.size = new Vector3(sDelta.x, sDelta.y, .01);
}

public function IndexUp() {
    if (index < fileList.Count - 1) {
        index++;
        //url = fileList[index];
        ReloadImage();
    }
}

public function IndexDown() {
    if (index > 0) {
        index--;
        //url = fileList[index];
        ReloadImage();
    }
}

public function ZoomIn(hitPos : Vector3) {
	//print("ZoomIn"); // reached
	if (hitPos != null) {
		var castOffset : Vector3 = hitPos - img.transform.position;
		var offsetRatioX : float = castOffset.x / img.rectTransform.sizeDelta.x;
		var offsetRatioY : float = castOffset.y / img.rectTransform.sizeDelta.y;
		scaleFactor += Time.deltaTime / 5;
		ResizeImage();
		var newOffset : Vector2 = new Vector2(offsetRatioX * img.rectTransform.sizeDelta.x, 
											offsetRatioY * img.rectTransform.sizeDelta.y);
		var posInfo : Vector3 = hitPos - newOffset;
		img.transform.position = new Vector3(posInfo.x, posInfo.y, img.transform.position.z);
		//print("OOld: " + castOffset + ", ONew: " + newOffset); // not reached
	} else {
		scaleFactor += Time.deltaTime / 5;
		ResizeImage();
	}
}

public function ZoomOut(hitPos : Vector3) {
    if (hitPos != null) {
		var castOffset : Vector3 = hitPos - img.transform.position;
		var offsetRatioX : float = castOffset.x / img.rectTransform.sizeDelta.x;
		var offsetRatioY : float = castOffset.y / img.rectTransform.sizeDelta.y;
		scaleFactor -= Time.deltaTime / 5;
		ResizeImage();
		var newOffset : Vector2 = new Vector2(offsetRatioX * img.rectTransform.sizeDelta.x, 
											offsetRatioY * img.rectTransform.sizeDelta.y);
		var posInfo : Vector3 = hitPos - newOffset;
		img.transform.position = new Vector3(posInfo.x, posInfo.y, img.transform.position.z);
	} else {
		scaleFactor -= Time.deltaTime / 5;
		ResizeImage();
	}
}

public function ResizeImage() {
    var sDelta : Vector2 = 0.02f * Mathf.Pow(scaleFactor, 2) * (new Vector2(img.texture.width, img.texture.height));
    img.rectTransform.sizeDelta = sDelta;
    imgCollider.size = new Vector3(sDelta.x, sDelta.y, .01);
}