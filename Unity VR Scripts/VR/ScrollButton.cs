using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollButton : MonoBehaviour {

    public Transform container;
    public float containerXInit;
    public float xInit;
    public float xMaxOffset = 0;
    public float extraContainerWidth = 0;

    // Use this for initialization
    void Start () {
        xInit = transform.position.x;
        containerXInit = container.position.x;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Scroll(float xOffset)
    {
        if (transform.position.x + xOffset <= xInit)
        {
            transform.position = new Vector3(xInit, transform.position.y, transform.position.z);
            container.position = new Vector3(containerXInit, 
                    container.position.y, container.position.z);
        }
        else if (transform.position.x + xOffset >= xInit + xMaxOffset)
        {
            transform.position = new Vector3(xInit + xMaxOffset, transform.position.y, transform.position.z);
            container.position = new Vector3(containerXInit + extraContainerWidth, 
                    container.position.y, container.position.z);
        }
        else
        {
            float relativeXPosition = (transform.position.x + xOffset - xInit) / xMaxOffset;
            transform.position = new Vector3(transform.position.x + xOffset, transform.position.y, transform.position.z);
            container.position = new Vector3(containerXInit + relativeXPosition * extraContainerWidth, 
                    container.position.y, container.position.z);
        }
    }
}
