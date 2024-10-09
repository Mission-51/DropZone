using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProfileClose : MonoBehaviour, IPointerClickHandler
{
    public NicknameUpdate nicknameUpdate;

    public void OnPointerClick(PointerEventData eventData)
    {
        nicknameUpdate.OnProfileClose();
    }
}
