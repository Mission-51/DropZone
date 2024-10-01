using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameObjectAction : MonoBehaviour, IPointerClickHandler
{
    
    public GameObject gameobject;
    
    
    public void OnPointerClick(PointerEventData eventData)
    {
        gameobject.SetActive(false);
    }

    // Update is called once per frame
    
}
