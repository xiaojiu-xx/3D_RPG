using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OptionUI : MonoBehaviour
{
    public Text optionText;
    private Button thisBtn;
    private DialoguePiece currentPiece;

    private string nextPieceID;
    private bool takeQuest;

    private void Awake()
    {
        thisBtn = GetComponent<Button>();
        thisBtn.onClick.AddListener(OnOptionClicked);
    }

    public void UpdateOption(DialoguePiece piece, DialogueOption option)
    {
        currentPiece = piece;
        optionText.text = option.text;
        nextPieceID = option.targetID;
        takeQuest = option.isTask;
    }

    public void OnOptionClicked()
    {
        if (currentPiece.quest != null)
        {
            var newTask = new QuestManage.QuestTask
            {
                questData = Instantiate(currentPiece.quest)
            };

            if (takeQuest)
            {
                if (QuestManage.Instance.HadQuest(newTask.questData))
                {
                    // 判断是否完成给与奖励
                    if(QuestManage.Instance.GetTask(newTask.questData).IsComplete)
                    {
                        newTask.questData.GiveRewards();
                        QuestManage.Instance.GetTask(newTask.questData).IsFinished = true;
                    }
                }
                else
                {
                    // 没有任务 接受任务
                    QuestManage.Instance.tasks.Add(newTask);
                    QuestManage.Instance.GetTask(newTask.questData).IsStarted = true; 

                    // 遍历任务需求物列表
                    foreach (var requireItem in newTask.questData.GetRequireTargetNameList())
                    {
                        // 如果存在则更新到任务当中
                        InventoryManager.Instance.CheckQuestItemInBagOrAction(requireItem);
                    }
                }
            }
        }

        if (nextPieceID == "")
        {
            DialogueUI.Instance.dialoguePanel.SetActive(false);
            return;
        }
        else
        {// 在字典当中根据ID找到对应的DialoguePiece
            DialogueUI.Instance.UpdateMainDialogue(DialogueUI.Instance.currentData.dialoguePiecesDict[nextPieceID]);
        }
    }
}
