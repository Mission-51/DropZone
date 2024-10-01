using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 10, -10);
    public float smoothSpeed = 0.125f;

    void Awake()
    {
        target = GameObject.FindWithTag("Player").transform; // Player를 찾아 카메라 고정
    }

    // Update is called once per frame
    void Update()
    {
        // 목표 위치를 계산: 캐릭터의 위치 + 오프셋(상단과 뒤쪽으로 고정)
        Vector3 desiredPosition = target.position + offset;

        // 카메라의 현재 위치를 목표 위치로 부드럽게 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 카메라의 위치를 업데이트
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
