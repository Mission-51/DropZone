using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoBillboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 메인 카메라를 찾음
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // UI가 항상 카메라를 바라보도록 설정
        if (mainCamera != null)
        {
            // LookAt을 사용하여 UI가 카메라 방향을 향하게 만듦
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}
