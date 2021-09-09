using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManage : Singleton<QuestManage>
{
    [System.Serializable]
    public class QuestTask
    {
        public QuestData_SO questData;

        public bool IsStarted { get { return questData.isStart; } set { questData.isStart = value; } }
        public bool IsComplete { get { return questData.isComplete; } set { questData.isComplete = value; } }
        public bool IsFinished { get { return questData.isFinished; } set { questData.isFinished = value; } }
    }

    public List<QuestTask> tasks = new List<QuestTask>();

    // 敌人死亡， 拾取物品时被调用
    public void UpdateQuestProgress(string requireName, int amout)
    {
        foreach (var task in tasks)
        {
            // 查询QuestRequire中name是否匹配传入的值
            var matchTask = task.questData.questRequires.Find(r => r.name == requireName);
            if (matchTask != null)
                matchTask.currentAmout += amout;

            // 检测任务当中是否满足完成要求
            task.questData.CheckQuestProgress();
            
        }
    }

    public bool HadQuest(QuestData_SO data)
    {
        if (data != null)
            return tasks.Any(q => q.questData.questName == data.questName);
        else
            return false;
    }

    public QuestTask GetTask(QuestData_SO data)
    {
        return tasks.Find(q => q.questData.questName == data.questName);
    }

}
