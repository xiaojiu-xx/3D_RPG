using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{
    public class DragData
    {
        public SlotHolder originalSlotholder;
        public RectTransform originalSlotholderTransform;
    }

    [Header("Inventory Data")]
    public InventoryData_OS inventoryTemplate;
    [HideInInspector]
    public InventoryData_OS inventoryData;          // 背包数据库
    public InventoryData_OS actionTemplate;
    [HideInInspector]
    public InventoryData_OS actionData;             // 快捷栏数据库
    public InventoryData_OS equipmentTemplate;
    [HideInInspector]
    public InventoryData_OS equipmentData;          // 状态面板武器数据库


    [Header("ContainerS")]
    public ContainerUI inventoryUI;
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;

    public DragData currDragData;

    bool isOpen = false;
    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statesPanel;

    [Header("State Text")]
    public Text hpText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemTooltip itemTooltip;

    protected override void Awake()
    {
        base.Awake();
        if (inventoryTemplate != null)
            inventoryData = Instantiate(inventoryTemplate);

        if (actionTemplate != null)
            actionData = Instantiate(actionTemplate);

        if (equipmentTemplate != null)
            equipmentData = Instantiate(equipmentTemplate);

    }
    private void Start()
    {
        // 加载数据后刷新图标
        LoadData();
        inventoryUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isOpen = !isOpen;
            bagPanel.SetActive(isOpen);
            statesPanel.SetActive(isOpen);
        }

        UpdateStateText(GameManager.Instance.playerState.CurrentHealth,
        GameManager.Instance.playerState.baseAttackData.currDamage);
    }

    // 保存数据
    public void SaveData()
    {
        SaveManager.Instance.Save(inventoryData, inventoryData.name);
        SaveManager.Instance.Save(actionData, actionData.name);
        SaveManager.Instance.Save(equipmentData, equipmentData.name);
    }

    public void LoadData()
    {
        SaveManager.Instance.Load(inventoryData, inventoryData.name);
        SaveManager.Instance.Load(actionData, actionData.name);
        SaveManager.Instance.Load(equipmentData, equipmentData.name);
    }


    public void UpdateStateText(int health, int nDamage)
    {
        hpText.text = health.ToString();
        attackText.text = nDamage.ToString();

    }

    public bool CheckInInventoryUI(Vector3 pos)
    {
        for (int i = 0; i < inventoryUI.slotHolders.Length; i++)
        {
            RectTransform rectTransform = inventoryUI.slotHolders[i].transform as RectTransform;
            // 检测点pos(鼠标)是否在ItemUI内
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos))
            {
                return true;
            }

        }
        return false;
    }

    public bool CheckInActionUI(Vector3 pos)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform rectTransform = actionUI.slotHolders[i].transform as RectTransform;
            // 检测点pos(鼠标)是否在ItemUI内
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 pos)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform rectTransform = equipmentUI.slotHolders[i].transform as RectTransform;
            // 检测点pos(鼠标)是否在ItemUI内
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos))
            {
                return true;
            }
        }
        return false;
    }

    // 检测是否存在任务物品 存在则更新任务重的Require
    public void CheckQuestItemInBagOrAction(string questItemName)
    {
        foreach (var item in inventoryData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.ItemName == questItemName)
                {
                    QuestManage.Instance.UpdateQuestProgress(item.itemData.ItemName, item.amount);
                }
            }
        }

        foreach (var item in actionData.items)
        {
            if (item.itemData != null)
            {
                if (item.itemData.ItemName == questItemName)
                {
                    QuestManage.Instance.UpdateQuestProgress(item.itemData.ItemName, item.amount);
                }
            }
        }

    }

    // 检测背包和快捷栏物品
    public InventoryItem QuestItemInBag(ItemData_SO questItem)
    {
        return inventoryData.items.Find(i => i.itemData == questItem);
    }
    public InventoryItem QuestItemInAction(ItemData_SO questItem)
    {
        return actionData.items.Find(i => i.itemData == questItem);
    }
}
