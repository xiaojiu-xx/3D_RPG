using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="New Data", menuName = "Character Stats/Data")]
public class CharacterData_OS : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;    
    public int maxLevel;
    public int baseExp;             // 当前升级所需要的经验值
    public int currentExp;
    public float levelBuff;

    public float levelMultiplier
    {
        get{ return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int point)
    {
        currentExp += point;

        if(currentExp >= baseExp)
            LevelUp();
    }

    private void LevelUp()
    {
        // TODO:升级所有的数据
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);

        // 升级后还剩余的经验值
        currentExp = currentExp - baseExp;
        // 下一次升级所需要的经验值
        baseExp += (int)( baseExp  * levelMultiplier );
        // 提升最大生命值
        maxHealth = (int)( maxHealth * levelMultiplier);
        currentHealth = maxHealth;
        
        Debug.Log("LEVEL UP!" + currentLevel + "Max Health: " + maxHealth  );

    }
}
