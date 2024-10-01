using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Transform playerPos;
    public Transform minimap;
    public Camera minimapCam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            minimap.localScale *= 2;
            minimapCam.orthographicSize = 100;
        } else if (Input.GetKeyUp(KeyCode.Tab))
        {
            minimap.localScale /= 2;
            minimapCam.orthographicSize = 50;
        }

        this.transform.position = playerPos.position + new Vector3(0, 100, 0);
    }
}
