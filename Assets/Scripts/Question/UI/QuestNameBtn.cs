using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameBtn : MonoBehaviour
{
    public Text questNametext;
    public QuestData_SO currentData;

    private void Awake() 
    {
        GetComponent<Button>().onClick.AddListener(UpdateQuestContent);
    }

    private void UpdateQuestContent()
    {
        QuestUI.Instance.SetupRequireList(currentData);

        // 更新奖励物品时，要先清除之前的奖励物品
        foreach (Transform item in QuestUI.Instance.rewardTransform)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in currentData.rewards)
        {
            //QuestUI.Instance.SetupRewardItem(item.itemData, item.amount);
            
        }
    }
 
    public void SetupNameBtn(QuestData_SO questData)
    {
        currentData = questData;

        if(questData.isComplete)
        {
            questNametext.text = questData.questName + "(完成)";
        }
        else
        {
            questNametext.text = questData.questName;
        }
    }

}
