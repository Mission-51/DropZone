using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPinController : MonoBehaviour
{
    public Transform playerPos;

    private void Update()
    {
        this.transform.position = playerPos.position + new Vector3(0, 80, 0);
    }
}
