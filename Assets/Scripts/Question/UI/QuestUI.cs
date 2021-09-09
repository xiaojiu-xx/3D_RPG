using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : Singleton<QuestUI>
{
    [Header("Elements")]
    public GameObject questPanel;
    public ItemTooltip tooltip;
    private bool isOpen;

    [Header("Quest Name")]
    public RectTransform questListTransform;
    public QuestNameBtn questNameBtn;

    [Header("Text Content")]
    public Text questContentText;

    [Header("Requirement")]
    public RectTransform requireTransform;
    public QuestRequirement requirement;

    [Header("ReWard Panel")]
    public RectTransform rewardTransform;
    public ItemUI rewardUI;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questPanel.SetActive(isOpen);
            questContentText.text = "";
            // 显示面板
            SetupQuestList();

            if (!isOpen)
            {
                tooltip.gameObject.SetActive(false);
            }
        }
    }

    public void SetupQuestList()
    {
        foreach (RectTransform item in questListTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in QuestManage.Instance.tasks)
        {
            var newTask = Instantiate(questNameBtn, questListTransform);
            newTask.SetupNameBtn(item.questData);
        }
    }

    public void SetupRequireList(QuestData_SO data)
    {
        questContentText.text = data.description;

        foreach (Transform item in requireTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var require in data.questRequires)
        {
            var temp = Instantiate(requirement, requireTransform);
            if (data.isFinished)
                temp.SetupRequirement(temp.name, data.isFinished);
            else
                temp.SetupRequirement(require.name, require.requireAmount, require.currentAmout);
        }
    }

    public void SetupRewardItem(ItemData_SO itemData, int amount)
    {
        var item = Instantiate(rewardUI, rewardTransform);
        item.SetupItemUI(ref itemData, ref amount);
    }
}
