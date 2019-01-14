using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WandControllerVideo : MonoBehaviour {
    public WandControllerVideo otherController;
    public LayerMask uiLayer;
    public LayerMask progLayer;
    public LayerMask vFileLayer;
    public LayerMask modeLayer;
    public LayerMask quitLayer;
    public LayerMask loopLayer;
    public LayerMask centerLayer;
    private Vector3 castDir;

    [SerializeField]
    public VideoScript vScript;
    //public GameObject vScriptObject;
    //[SerializeField]
    //public InputField inputField;
    public GameObject startupOverlay;
    [SerializeField]
    public Transform imageTransform;
    //public LoadSpecificDir dirScript;
    public GameObject loadManager;
    public Text loadText;
    private bool fileMode = true;
    public Image volumeBar;

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
        vScript.progHover = false;
        fileMode = vScript.fileMode;

        //print("Axis0: " + controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)); // axis0 is 2D, has (x,y) on D-pad
        triggerPressed = controller.GetPress(triggerButton);

        // pos coordinates are local
        //print("RController Coords" + controller.transform.pos); // .6 to 1.4 vert(y), -.1 to .4 forward (z), -.3 to .7 sideways (x)

        // Both have multiple values change during tilting
        //print("RController Tilts" + controller.transform.rot.eulerAngles);
        //print("RController QTilts" + controller.transform.rot);
        //print("RController XTilts" + controller.transform.rot.eulerAngles.x); // Never seems to go beyond 270 to 50
        //print("RController YTilts" + controller.transform.rot.eulerAngles.y); // Changed by motion on all 3 axes
        //print("RController ZTilts" + controller.transform.rot.eulerAngles.z); // Ranges from 270 to 90 on desired axis when normal
        // ranges from 90 to 270 when pulled back

        // Controller default rotation is laying flat
        //print("RController Angle:" + Quaternion.Angle(Quaternion.identity, controller.transform.rot));
        //Quaternion rotFrame = Quaternion.AngleAxis(90, Vector3.right) * controller.transform.rot;
        //print("RController FrameAngle:" + Quaternion.Angle(Quaternion.identity, rotFrame));
        // y ranges consitently from 270 to 90 for rotation on correct axis
        //print("RController FrameTilts" + rotFrame.eulerAngles);

        if (controller.GetPressDown(menuButton))
        {
            zoomMode = !zoomMode;
        }
        if (fileMode)
        {
            zoomModeMat.SetColor("_TintColor", new Color(1, 0.4f, 0));
        } else if(zoomMode)
            {
            zoomModeMat.SetColor("_TintColor", Color.magenta);
        } else
            {
            zoomModeMat.SetColor("_TintColor", Color.blue);
        }
        RaycastHit hit;
        // Need to replace "forward" with neg. "up"
        if (controller.GetPressDown(triggerButton))
        {
            //print("GotPress, Looking Forward: " + (transform.rotation * -Vector3.up));
            if (fileMode)
            {

                if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, vFileLayer))
                {
                    VideoFilePanel vfp = hit.collider.gameObject.GetComponent<VideoFilePanel>();
                    vfp.Select();
                    string fileDir = vfp.filePath;
                    vScript.file = fileDir;
                    vScript.LoadVideo();
                    startupOverlay.SetActive(false);
                } else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, modeLayer))
                {
                    UnityEngine.Application.LoadLevel(0);
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, quitLayer))
                {
                    UnityEngine.Application.Quit();
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, loopLayer))
                {
                    HighlightButton hb = hit.collider.gameObject.GetComponent<HighlightButton>();
                    hb.EngageModeCommand();
                }
                else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, centerLayer))
                {
                    vScript.ResetPosition();
                    proximityFactor = 2;
                    otherController.proximityFactor = 2;
                }
                else
                {
                    castOffset = Vector3.zero;
                    if (Clipboard.ContainsText())
                    {
                        string text = Clipboard.GetText();
                        loadText.text = text;
                        print("Text: " + text);
                        if (text.Length > 0)
                        {
                            vScript.text = text;
                            vScript.LoadVideoDir();
                        }
                    }
                }

            }
            else if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000))
            {
                if (hit.collider.gameObject.layer == 5) // Plane is in UI layer
                {
                    castOffset = hit.point - imageTransform.position;
                } else
                {
                    castOffset = Vector3.zero;
                }
                //print("Offset: " + castOffset);
            } else
            {
                castOffset = Vector3.zero;
            }
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, progLayer))
        {
            vScript.progHover = true;
            vScript.ProgBarHover(hit.point);
        }
        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, vFileLayer))
        {
            VideoFilePanel vfp = hit.collider.gameObject.GetComponent<VideoFilePanel>();
            vfp.isHighlighted = true;
            vfp.text.color = Color.white;
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

        if (triggerPressed && castOffset.magnitude > 0)
        {
            
            if (!vScript.progHover && Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, uiLayer))
            {

                //castOffset = hit.point - imageTransform.position;
                //print("Offset: " + castOffset);
                imageTransform.position = hit.point - castOffset;
            } 
            
        }
        else if (triggerPressed && vScript.progHover)
        {
            vScript.ProgBarClick(hit.point);
        }
        if (controller.GetPressDown(dButton))
        {
            // nothing
        }
        if (controller.GetPress(dButton))
        {
            Vector2 padDir = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            if (padDir.x > 0.7f)
            {
                //vScriptObject.SendMessage("IndexUp");
                vScript.timeShifting = 1;
            }
            else if (padDir.x < -0.7f)
            {
                //vScriptObject.SendMessage("IndexDown");
                vScript.timeShifting = -1;
            }
            //Vector2 padDir = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
            if (fileMode)
            {
                if (padDir.y > 0.7f)
                {
                    volumeBar.fillAmount += 0.33f * Time.deltaTime;
                    //vScript.vPlayer.SetDirectAudioVolume(0, volumeBar.fillAmount); // NO EFFECT
                    vScript.umPlayer.Volume = 100 * volumeBar.fillAmount;
                }
                else if (padDir.y < -0.7f)
                {
                    volumeBar.fillAmount -= 0.33f * Time.deltaTime;
                    //vScript.vPlayer.SetDirectAudioVolume(0, volumeBar.fillAmount); // NO EFFECT
                    vScript.umPlayer.Volume = 100 * volumeBar.fillAmount;
                }
            }
            else if (zoomMode)
            {
                Vector3 hitPass = imageTransform.position;
                if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000)) {
                    hitPass = hit.point;
                }
                if (padDir.y > 0.7f)
                {
                    vScript.Zoom(true, hitPass);
                }
                else if (padDir.y < -0.7f)
                {
                    vScript.Zoom(false, hitPass);
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
                // default proximityFactor is 1
                float zPos = 0.5f * Mathf.Pow(proximityFactor, 3) + 0.5f;
                imageTransform.localPosition = new Vector3(imageTransform.localPosition.x, imageTransform.localPosition.y,
                                                    zPos);
            }
        }

        if (controller.GetPressDown(gripButton))
        {
            //vScript.LoadVideo();
            fileMode = !fileMode;
            vScript.fileMode = fileMode;
            loadManager.SetActive(fileMode);
            if (fileMode)
            {
                vScript.umPlayer.Pause();
            } else
            {
                if (!vScript.umPlayer.IsPlaying)
                    vScript.umPlayer.Play();
                vScript.overlayBlocker.SetActive(false);
                print("W: " + vScript.umPlayer.VideoWidth + ", H: " + vScript.umPlayer.VideoHeight);
                vScript.umPlayer.RenderingObjects[0].transform.localScale
                    = new Vector3(1, (float)vScript.umPlayer.VideoHeight / vScript.umPlayer.VideoWidth, 1);
                vScript.LoadLengthString();
                //vScript.umPlayer.RenderingObjects[0].transform.localScale = new Vector3(1, 0.1f, 4);
            }
        }


        if (Physics.Raycast(transform.position, transform.rotation * castDir, out hit, 10000, loopLayer))
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
