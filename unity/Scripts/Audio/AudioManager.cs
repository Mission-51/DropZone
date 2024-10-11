using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public AudioSource lobbySound;
    public AudioSource gameSound;

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 등록
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static AudioManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 게임 씬에 들어가면 배경 음악 오브젝트를 삭제
        if (scene.name == "MinigameScene" || scene.name == "MainScene") // 미니게임, 메인게임일 경우
        {
            lobbySound.Stop();
            if (gameSound.isPlaying)
            {
                return;
            }
            gameSound.Play();
        }
        else if (scene.name == "LobbyScene" || scene.name == "LoginScene") // 로비 or 로그인 씬에서 로비 음악 실행
        {
            gameSound.Stop();
            if (lobbySound.isPlaying)
            {
                return;
            }
            lobbySound.Play();
        }
    }
}
