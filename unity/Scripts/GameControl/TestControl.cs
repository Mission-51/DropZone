using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestControl : MonoBehaviourPun
{
    // Start is called before the first frame update
    public GameObject panel1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(panel1.activeSelf)
            {
                panel1.SetActive(false);
            }
            else
            {
                panel1.SetActive(true);
            }
        }
    }

    public void OnTestLogin()
    {

    }

    public void GoGameScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GoLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void MiniGameScene()
    {
        SceneManager.LoadScene("MinigameScene");
    }
}
