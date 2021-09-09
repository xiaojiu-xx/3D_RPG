using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueControl))]
public class QuestGiver : MonoBehaviour
{
    private DialogueControl dialogueControl;
    private QuestData_SO currentQuest;

    public DialogueData_SO startDialogue;
    public DialogueData_SO progressDialogue;
    public DialogueData_SO completeDialogue;
    public DialogueData_SO finishDialogue;

    // 获得任务状态 任务是否开始
    public bool IsStarted
    {
        get
        {
            if (QuestManage.Instance.HadQuest(currentQuest))
            {
                return QuestManage.Instance.GetTask(currentQuest).IsStarted;
            }
            else return false;
        }
    }

    // 获得任务状态 任务是否完成
    public bool IsComplete
    {
        get
        {
            if (QuestManage.Instance.HadQuest(currentQuest))
            {
                return QuestManage.Instance.GetTask(currentQuest).IsComplete;
            }
            else return false;
        }
    }
    
    // 获得任务状态 任务是否结束
    public bool IsFinish
    {
        get
        {
            if (QuestManage.Instance.HadQuest(currentQuest))
            {
                return QuestManage.Instance.GetTask(currentQuest).IsFinished;
            }
            else return false;
        }
    }
    private void Awake()
    {
        dialogueControl = GetComponent<DialogueControl>();
    }

    private void Start() 
    {
        dialogueControl.currentData = startDialogue;
        currentQuest = dialogueControl.currentData.GetQuest();
    }

    private void Update() 
    {
        if(IsStarted)
        {
            if(IsComplete)
            {
                dialogueControl.currentData = completeDialogue;
            }
            else
            {
                dialogueControl.currentData = progressDialogue;
            }
        }
        
        if(IsFinish)
        {
            dialogueControl.currentData = finishDialogue;
        }
    }
}



