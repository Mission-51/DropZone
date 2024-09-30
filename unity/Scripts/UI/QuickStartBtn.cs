using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickStartBtn : MonoBehaviour, IPointerClickHandler
{
    public Button button;
    public GameObject activeobject;
    public CanvasGroup active1Group;  
    public CanvasGroup active2Group;
    void Start()
    {
        activeobject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        activeobject.SetActive(true);
        active1Group.gameObject.SetActive(false);
        active2Group.gameObject.SetActive(false);
    }

    // Update is called once per frame
    
}
