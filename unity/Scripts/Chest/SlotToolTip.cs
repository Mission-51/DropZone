using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private TextMeshProUGUI txt_ItemName;

    [SerializeField]
    private TextMeshProUGUI txt_ItemDesc;

    public void ShowToolTip(ItemData _item)
    {
        go_Base.SetActive(true);

        txt_ItemName.text = _item.itemName;
        txt_ItemDesc.text = _item.toolTip;
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
