using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public ItemData item; // ȹ���� ������
    public int itemCount; // ȹ���� �������� ����
    public Image itemImage; // �������� �̹���

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    // ������ �̹����� ������ ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // �κ��丮�� ���ο� ������ ���� �߰�
    public void AddItem(ItemData _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        // ���� ��Ÿ���� ���� ��Ÿ����
        go_CountImage.SetActive(true);
        text_Count.text = itemCount.ToString();
        // ������ �̹��� �������ϰ�(���̰�)
        SetColor(1);
    }

    // �ش� ������ ������ ���� ������Ʈ
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // �ش� ���� �ϳ� ����
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                // �Һ�
                Debug.Log(item.itemName + " �� ����߽��ϴ�.");
                SetSlotCount(-1);
            }
        }
    }

    // ���콺 �巡�װ� ���� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // ���콺 �巡�� ���� �� ��� �߻��ϴ� �̺�Ʈ
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // ���콺 �巡�װ� ������ �� �߻��ϴ� �̺�Ʈ
    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    // �ش� ���Կ� ���𰡰� ���콺 ��� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        ItemData _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        } else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }
}