using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 挂载在ItemSolt上
[RequireComponent(typeof(ItemUI))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ItemUI currentitemUI;
    private SlotHolder currHolder, targetHolder;

    private void Awake()
    {
        currentitemUI = GetComponent<ItemUI>();
        currHolder = GetComponentInParent<SlotHolder>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        InventoryManager.Instance.currDragData = new InventoryManager.DragData();

        // 保存这个物品原本所属的格子，也就是父级
        InventoryManager.Instance.currDragData.originalSlotholder = GetComponentInParent<SlotHolder>();
        InventoryManager.Instance.currDragData.originalSlotholderTransform = (RectTransform)transform.parent;

        // 设置父级，使其总是在最上层显示
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // IsPointerOverGameObject指针是否位于EventSystem对象上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // 是否指向
            if (InventoryManager.Instance.CheckInActionUI(eventData.position) ||
                InventoryManager.Instance.CheckInEquipmentUI(eventData.position) ||
                InventoryManager.Instance.CheckInInventoryUI(eventData.position))
            {
                if (eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    // 目标格子当中没有物品
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    // 目标格子当中有物品
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }
                
                // 判断目标hold是否为我原hold
                if(targetHolder != InventoryManager.Instance.currDragData.originalSlotholder)
                    switch (targetHolder.slotType)
                    {
                        case SlotType.BAG:
                            SwapItem();     // 交换物品(交换数据库中的数据)
                            break;
                        case SlotType.WEAPON:
                            // 目标格子为ACTION 类型 只能容纳Weapon类型的物品
                            if(currHolder.DataBase.items[currHolder.Index].itemData.itemType == ItemType.Weapon)
                                SwapItem();
                            break;
                        case SlotType.ARMOR:
                            // 目标格子为ACTION 类型 只能容纳Armor类型的数据物品
                            if(currHolder.DataBase.items[currHolder.Index].itemData.itemType == ItemType.Armor)
                                SwapItem();
                            break;          
                        case SlotType.ACTION:
                            // 目标格子为ACTION 类型 只能容纳Useable类型的数据物品
                            if(currHolder.DataBase.items[currHolder.Index].itemData.itemType == ItemType.Useable)
                                SwapItem();
                            break;             
                    }

                currHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }

        // 回到初始的父级下
        transform.SetParent(InventoryManager.Instance.currDragData.originalSlotholderTransform);
        
        // 回到父级时解决不能回到正确位置问题
        RectTransform t = transform as RectTransform;
        t.offsetMax = Vector2.zero;
        t.offsetMin = Vector2.zero;
    }

    public void SwapItem()
    {// 交换物品

        var targetItem = targetHolder.DataBase.items[targetHolder.Index];
        var tempItem = currHolder.DataBase.items[currHolder.Index];

        // 判断是否为同样物品
        bool isSameItem = false;
        if (targetItem.itemData != null )
            isSameItem = tempItem.itemData.ItemName == targetItem.itemData.ItemName;

        // 交换物品，交换数据库中的数据
        // 是相同物品而且是可堆叠
        if(isSameItem && targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {// 不可堆叠
            currHolder.DataBase.items[currHolder.Index] = targetItem;
            targetHolder.DataBase.items[targetHolder.Index] = tempItem;
        }
    }
}
