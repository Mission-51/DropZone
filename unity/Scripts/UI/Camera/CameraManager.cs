using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;       // 메인 카메라
    public Camera secondaryCamera;  // 추가 카메라 (보조 카메라)

    void Start()
    {
        // 메인 카메라의 뷰포트는 전체 화면 차지
        mainCamera.rect = new Rect(0, 0, 1, 1);

        // 보조 카메라의 뷰포트는 화면의 오른쪽 하단에 25% 크기로 배치
        secondaryCamera.rect = new Rect(0.75f, 0f, 0.25f, 0.25f);
    }
}
