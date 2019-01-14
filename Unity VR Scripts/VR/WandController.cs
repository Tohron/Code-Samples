using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandController : MonoBehaviour
{
    public WandController otherController;
    public LayerMask modeLayer;
    public LayerMask quitLayer;
    public LayerMask loadLayer;
    public LayerMask vFileLayer;
    public LayerMask folderLayer;
    public LayerMask folderScrollLayer;
    public LayerMask proportionLayer;
    public LayerMask arrangementLayer;
    public LayerMask centerLayer;
    private Vector3 castDir;
    [SerializeField]
    public GameObject dirScriptObject;
    public ImageScript iScript;
    //[SerializeField]
    //public InputField inputField;
    public GameObject startupOverlay;
    [SerializeField]
    public Transform imageTransform;
    //public LoadSpecificDir dirScript;

    public GameObject loadManager;
    public Text loadText;
    private bool fileMode = true;

    private bool holdingFolderScroll = false;
    public Vector3 oldPos;
    public ScrollButton scrollScript;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId dButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    private Vector3 castOffset;
    private bool triggerPressed = false;

    private bool zoomMode = true;
    public Material zoomModeMat;
    public float proximityFactor = 3;

	// Use this for initialization
	void Start () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        zoomModeMat.SetColor("_TintColor", Color.magenta);
        castDir = Quaternion.AngleAxis(-30, Vector3.right) * (-Vector3.up);
        //EventSystemManager.currentSystem.SetSelectedGameObject(inputField.gameObject, null);
        //EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
        //inputField.OnPointerClick(null);
        //print("Set R Controller");
    }
	
	// Update is called once per frame
	void Update () {
		if (controller == null)
        {
            Debug.Log("Null Controller!");
            return;
        }
        
        triggerPressed = controller.GetPress(triggerButton);

        if (controller.GetPressDown(menuButton))
        {
            zoomMode = !zoomMode;
            if (zoomMode)
            {
                zoomModeMat.SetColor("_TintColor", Color.magenta);
            } else
            {
                zoomModeMat.SetColor("_TintColor", Color.blue);
            }
        }

        RaycastHit hit;
        if (controller.GetPressDown(triggerButton))
        {
            //print("GotPress, Looking Forward: " + (transform.rotation * -Vector3.up));
            if (fileMode)
            {
                if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, vFileLayer))
                {
                    ImageFilePanel ifp = hit.collider.gameObject.GetComponent<ImageFilePanel>();
                    ifp.Select();
                    iScript.GoToIndex(ifp.index);
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, vFileLayer))
                {
                    FolderPanel fp = hit.collider.gameObject.GetComponent<FolderPanel>();
                    fp.Select();
                    iScript.fScript.OpenDirectory(fp.filePath);
                }
                // Check for clicking Video Mode or Quit
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, modeLayer))
                {
                    UnityEngine.Application.LoadLevel(1);
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, quitLayer))
                {
                    UnityEngine.Application.Quit();
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, proportionLayer))
                {
                    HighlightButton hb = hit.collider.gameObject.GetComponent<HighlightButton>();
                    hb.EngageModeCommand();
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, arrangementLayer))
                {
                    HighlightButton hb = hit.collider.gameObject.GetComponent<HighlightButton>();
                    hb.EngageModeCommand();
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, centerLayer))
                {
                    iScript.ResetPosition();
                    proximityFactor = 1.5f;
                    otherController.proximityFactor = proximityFactor;

                }
                // Registers click on "Load Copied Directory"
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, loadLayer))
                {
                    castOffset = Vector3.zero;
                    if (Clipboard.ContainsText())
                    {
                        string text = Clipboard.GetText();
                        loadText.text = text;
                        //print("Text: " + text);
                        if (text.Length > 0)
                        {
                            dirScriptObject.SendMessage("LoadText", text);
                            startupOverlay.SetActive(false);
                        }
                    }
                }
                // Registers click on folder
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, folderLayer))
                {

                }
                // Registers click on scroll button - needs to manage movement afterward while held!
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, folderScrollLayer))
                {
                    if (!holdingFolderScroll)
                    {
                        oldPos = hit.point - scrollScript.transform.position;
                    } else
                    {
                        scrollScript.Scroll((hit.point - scrollScript.transform.position - oldPos).x);
                        oldPos = hit.point - scrollScript.transform.position;
                    }

                    holdingFolderScroll = true;
                }

            } else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000))
            {
                
                castOffset = hit.point - imageTransform.position;
                //print("Offset: " + castOffset);
            } else
            {
                castOffset = Vector3.zero;
                holdingFolderScroll = false;
            }
        }

        if (triggerPressed && castOffset.magnitude > 0 && !fileMode)
        {
            // Enables dragging of the image
            if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000))
            {

                //castOffset = hit.point - imageTransform.position;
                //print("Offset: " + castOffset);
                imageTransform.position = hit.point - castOffset;
                iScript.UpdateActiveImage();
            }
        }
        if(controller.GetPressDown(dButton))
        {
            Vector2 padDir = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            if (padDir.x > 0.7f) {
                dirScriptObject.SendMessage("IndexUp");
            } else if (padDir.x < -0.7f)
            {
                dirScriptObject.SendMessage("IndexDown");
            }
        }
        if (controller.GetPress(dButton))
        {
            Vector2 padDir = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            if (zoomMode)
            {
                if (padDir.y > 0.7f)
                {
                    if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000))
                    {
                        print("HitPoint: " + hit.collider.gameObject); // is triggered
                        
                        dirScriptObject.SendMessage("ZoomIn", hit.point);
                    } else
                    {
                        dirScriptObject.SendMessage("ZoomIn", imageTransform.position);
                    }
                }
                else if (padDir.y < -0.7f)
                {
                    if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000))
                    {
                        print("HitPoint: " + hit.collider.gameObject); // is triggered
                        dirScriptObject.SendMessage("ZoomOut", hit.point);
                    }
                    else
                    {
                        dirScriptObject.SendMessage("ZoomOut", imageTransform.position);
                    }
                }
            } else
            {
                // Next to player at -29
                if (padDir.y > 0.7f)
                {
                    proximityFactor += 1.25f * Time.deltaTime;
                    otherController.proximityFactor = proximityFactor;
                    //imageTransform.position = new Vector3(imageTransform.position.x, imageTransform.position.y, 
                    //                                imageTransform.position.z + 4 * Time.deltaTime);
                }
                else if (padDir.y < -0.7f)
                {
                    proximityFactor -= 1.25f * Time.deltaTime;
                    otherController.proximityFactor = proximityFactor;
                    //imageTransform.position = new Vector3(imageTransform.position.x, imageTransform.position.y, 
                    //                                imageTransform.position.z - 4 * Time.deltaTime);
                }
                // default proximityFactor is 1.5
                float zPos = 0.2963f * Mathf.Pow(proximityFactor, 3) - 29;
                imageTransform.localPosition = new Vector3(imageTransform.localPosition.x, imageTransform.localPosition.y,
                                                    zPos);
            }
        }

        if (controller.GetPressDown(gripButton))
        {
            //dirScriptObject.SendMessage("LoadText");
            fileMode = !fileMode;
            loadManager.SetActive(fileMode);
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, modeLayer))
        {
            HighlightPanel hp = hit.collider.gameObject.GetComponent<HighlightPanel>();
            hp.isHighlighted = true;
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, quitLayer))
        {
            HighlightPanel hp = hit.collider.gameObject.GetComponent<HighlightPanel>();
            hp.isHighlighted = true;
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, loadLayer))
        {
            HighlightPanel hp = hit.collider.gameObject.GetComponent<HighlightPanel>();
            hp.isHighlighted = true;
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, vFileLayer))
        {
            ImageFilePanel ifp = hit.collider.gameObject.GetComponent<ImageFilePanel>();
            ifp.isHighlighted = true;
            ifp.text.color = Color.white;
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, proportionLayer))
        {
            HighlightButton hb = hit.collider.gameObject.GetComponent<HighlightButton>();
            hb.isHighlighted = true;
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, arrangementLayer))
        {
            HighlightButton hb = hit.collider.gameObject.GetComponent<HighlightButton>();
            hb.isHighlighted = true;
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, centerLayer))
        {
            HighlightButton hb = hit.collider.gameObject.GetComponent<HighlightButton>();
            hb.isHighlighted = true;
        }
    }
}
