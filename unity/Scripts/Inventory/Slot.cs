using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지
    public Transform playerPos; // 플레이어 위치 (아이템 버렸을 때 해당 위치에 아이템이 instantiate 되게 하기 위해)
    public GameObject throwConfirm; // 아이템 드롭 확인창
    public Button confirmBtn, cancelBtn; // 확인 버튼, 취소 버튼

    // 가방 영역 Rect 정보
    private Rect bagRect;
    // 아이템 드롭 확인창 인풋필드
    private TMP_InputField throwInput;

    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;
    [SerializeField]
    private SlotToolTip slotTooltip;

    private void Start()
    {
        bagRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        throwInput = throwConfirm.GetComponentInChildren<TMP_InputField>();

        confirmBtn.onClick.AddListener(OnConfirmBtnClick);
        cancelBtn.onClick.AddListener(OnCancelBtnClick);
    }

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        // 수량 나타내는 숫자 나타나게
        go_CountImage.SetActive(true);
        text_Count.text = itemCount.ToString();
        // 아이템 이미지 불투명하게(보이게)
        SetColor(1);
    }

    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
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
                // 소비
                Debug.Log(item.itemName + " 을 사용했습니다.");
                SetSlotCount(-1);
            }
        }
    }

    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragSlot.instance.transform.localPosition.x < bagRect.xMin
            || DragSlot.instance.transform.localPosition.x > bagRect.xMax
            || DragSlot.instance.transform.localPosition.y < bagRect.yMin
            || DragSlot.instance.transform.localPosition.y > bagRect.yMax)
        {
            // (주석) 아이템 떨어트리는 로직
            //Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, playerPos.position, Quaternion.identity);
            if (DragSlot.instance.dragSlot != null && item != null)
            {
                // 아이템 드롭 확인 창 띄우기
                throwConfirm.SetActive(true);
            }
        }

        DragSlot.instance.SetColor(0);
    }

    // 해당 슬롯에 무언가가 마우스 드롭 됐을 때 발생하는 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        // DragSlot.instance.dragSlot.item >> 내가 현재 드래그 중인 아이템
        // item >> 각 슬롯의 자리에 있던 아이템

        // 원래 있던 아이템, 아이템 개수
        ItemData _tempItem = item;
        int _tempItemCount = itemCount;
        Debug.Log("변경 전 아이템" + item);

        // 드래그 슬롯이 가지고 있는 아이템, 아이템 개수를 현재 슬롯에 추가
        // -> 현재 슬롯의 아이템을 드래그 슬롯의 아이템으로 변경하는 로직
        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
        Debug.Log("변경 후 아이템" + item);

        if (_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

    // 아이템 버리기 확인 버튼 클릭시 함수
    public void OnConfirmBtnClick()
    {
        if (DragSlot.instance.dragSlot != null)
        {
            if (int.TryParse(throwInput.text, out int result))
            {
                // throwInput이 비어있지 않고, 입력값이 0보다 크고 현재 가진 양보다 작거나 같다면(유효성 검사)
                if (throwInput.text != null && int.Parse(throwInput.text) > 0 && int.Parse(throwInput.text) <= DragSlot.instance.dragSlot.itemCount)
                {
                    // 사용자가 입력한 개수만큼 버리기
                    DragSlot.instance.dragSlot.SetSlotCount(-int.Parse(throwInput.text));

                    // 텍스트 초기화
                    throwInput.text = null;
                    // 창 닫기
                    throwConfirm.SetActive(false);
                    // 드래그 슬롯 초기화
                    DragSlot.instance.dragSlot = null;

                    // 플레이스홀더 초기화
                    if (throwInput.placeholder is TextMeshProUGUI placeholderText)
                    {
                        placeholderText.text = "수량";
                    }
                }
                else
                {
                    if (throwInput.placeholder is TextMeshProUGUI placeholderText)
                    {
                        throwInput.text = null;
                        placeholderText.text = "올바른 값이 아닙니다.";
                    }
                }
            }
        }
    }

    // 아이템 버리기 취소 버튼 클릭시 함수
    public void OnCancelBtnClick()
    {
        throwConfirm.SetActive(false);
        // 드래그 슬롯 초기화
        DragSlot.instance.dragSlot = null;

        // 플레이스홀더 초기화
        if (throwInput.placeholder is TextMeshProUGUI placeholderText)
        {
            placeholderText.text = "수량";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            slotTooltip.ShowToolTip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotTooltip.HideToolTip();
    }
}
