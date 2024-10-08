using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour, IPointerClickHandler
{
    public Button button;
    public GameObject activeobject;
    
    void Start()
    {
        activeobject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        activeobject.SetActive(true);
   
    }
}
