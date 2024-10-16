using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancleBtn : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        NetworkManager.Instance.OnLeftRoom();
    }
}
