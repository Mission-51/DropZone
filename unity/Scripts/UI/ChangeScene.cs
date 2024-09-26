using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ChangeScene : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
