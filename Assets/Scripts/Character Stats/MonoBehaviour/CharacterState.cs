using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_OS templateData;
    [HideInInspector]
    public CharacterData_OS characterData;
    public AttackData_SO tempAttackData;
    [HideInInspector]
    public AttackData_SO baseAttackData;   // Player的基础属性
    private RuntimeAnimatorController baseAnimator;
    [Header("Weapon")]
    public Transform WeaponSolt;    // 武器装备点

    // 此特性让变量不出现在Inspectors面板
    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }

        baseAttackData = Instantiate(tempAttackData);
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region 读取数据
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }

    #endregion

    #region  Character Combat

    public void TakeDamage(CharacterState attacker, CharacterState defener)
    {
        // 计算伤害
        int nDamage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);
        // 被攻击者当前的血量
        defener.CurrentHealth = Mathf.Max(defener.CurrentHealth - nDamage, 0);

        if (attacker.isCritical)
        {
            // 暴击伤害时播放受伤动画
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        // TODO:Update UI
        if (attacker.gameObject.tag == "Player")
        {
            defener.UpdateHealthBarOnAttack?.Invoke(defener.CurrentHealth, defener.MaxHealth);
        }
        // TODO:经验Update
        if (defener.CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(defener.characterData.killPoint);
        }
    }

    // 石头怪被石头攻击后调用
    public void TakeDamage(int damage, CharacterState defener)
    {
        int nCurrentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);
        defener.CurrentHealth = Mathf.Max(CurrentHealth - nCurrentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(defener.CurrentHealth, defener.MaxHealth);

        // 石头怪死亡,Player增加经验值
        if (defener.CurrentHealth <= 0)
        {
            GameManager.Instance.playerState.characterData.UpdateExp(defener.characterData.killPoint);
        }

    }

    private int CurrentDamage()
    {
        float coreDamage = baseAttackData.currDamage;
        if (isCritical)
        {
            coreDamage *= baseAttackData.criticalMultiplier;
            // Debug.Log("暴击伤害翻倍");
        }
        return (int)coreDamage;
    }
    #endregion

    #region 

    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipment();
        EquipWeapon(weapon);
    }
    public void EquipWeapon(ItemData_SO weapon)
    {
        // 给骑士换装
        if (weapon.weaponPrefab != null)
        {
            Instantiate(weapon.weaponPrefab, WeaponSolt);
        }
        // 切换动画控制器
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
        // TODO：给骑士更新属性
        baseAttackData.ApplyWeaponData(weapon.weaponData);
    }
    #endregion

    // 卸下武器
    public void UnEquipment()
    {
        // 有武器，销毁武器（子物体）
        if (WeaponSolt.transform.childCount != 0)
        {
            for (int i = 0; i < WeaponSolt.transform.childCount; i++)
            {
                Destroy(WeaponSolt.transform.GetChild(i).gameObject);
            }
        }
        // 回到初始攻击力 做的比较粗糙
        baseAttackData.ApplyWeaponData(tempAttackData);
        // TODO:切换动画
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }

    public void ApplyHealth(int hp)
    {
        if (CurrentHealth + hp <= MaxHealth)
        {
            CurrentHealth += hp;
        }
        else{
            CurrentHealth = MaxHealth;
        }
    }
}
