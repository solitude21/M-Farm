using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField]
        private Image slotImage;
        [SerializeField]
        private TextMeshProUGUI amountText;
        
        public Image slotHightlight;
        [SerializeField]
        private Button button;

        [Header("格子类型")]
        public SlotType slotType;

        public bool isSelected;

        public int slotIndex;


        // 物品信息
        public ItemDetails itemDetails;
        //public int itemAmount;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void OnEnable()
        {
            //EventHandler.ItemSelectedEvent += ChangeHighlightState;
        }

        private void OnDisable()
        {
            //EventHandler.ItemSelectedEvent -= ChangeHighlightState;
        }

        /*private void ChangeHighlightState(ItemDetails otherItemDetails, bool otherIsSelected)
        {
            if (isSelected)
                slotHightlight.gameObject.SetActive(true);
            else
                slotHightlight.gameObject.SetActive(false);
        }*/


        private void Start()
        {
            isSelected = false;

            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 更新格子信息
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            //itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }


        /// <summary>
        /// 将Slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHightlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
            //itemAmount = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;

            isSelected = !isSelected;

            inventoryUI.UpdateSlotHightlight(slotIndex);

            if (slotType == SlotType.Bag)
            {
                // 通知物品是否被选中
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails != null)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();
                
                // 是否可以使用Event?
                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
        
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                // 在Player自身背包范围内交换
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetSlot.slotIndex);
                    // 涉及到数据库的变化，需要第一时间修改UI缓存数据
                    //EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList);
                }

                // 设置取消所有高亮
                inventoryUI.UpdateSlotHightlight(-1);
                // 不再举起当前物品
                if (slotType == SlotType.Bag)
                {
                    EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
                }
            }
            /*else
            {
                if (itemDetails.canDropped)
                {
                    // 鼠标对应世界地图坐标
                    var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

                    EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
                }
            }*/

            

        }
    }
}


