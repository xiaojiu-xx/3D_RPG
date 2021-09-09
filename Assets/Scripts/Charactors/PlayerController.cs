using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private CharacterState characterState;
    private float lastAttackTime;
    private float stopDistance;
    private bool isDead;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClick += MoveToTarget;
        MouseManager.Instance.OnEnemyClick += EventAttack;
        GameManager.Instance.RigisterPlayer(characterState);
    }

    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();
    }

    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;

        MouseManager.Instance.OnMouseClick -= MoveToTarget;
        MouseManager.Instance.OnEnemyClick -= EventAttack;
    }


    private void Update()
    {
        isDead = characterState.CurrentHealth == 0;
        // 进行广播
        if (isDead)
            GameManager.Instance.NotifyObservers();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        //Debug.Log(agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }


    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    // 参数为鼠标点击到的Enemy
    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterState.isCritical = UnityEngine.Random.value < characterState.baseAttackData.criticalChance;
            StartCoroutine( MoveToAttackTarget() );
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        // 确保移动的时候agent未处于停止状态
        agent.isStopped = false;
        // TODO:当向攻击目标移动时,目标体积较大,大于stoppingDistance将造成Player抖动,这样设置还有些问题
        agent.stoppingDistance = characterState.baseAttackData.attackRange;

        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterState.baseAttackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterState.isCritical);
            anim.SetTrigger("Attack");
            lastAttackTime = characterState.baseAttackData.coolDown;
        }
    }

    // Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("AttackAble"))
        {
            // 目标为石头且石头状态为HitNothing
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                
                // 给石头一个大于1的速度，石头在fixupdate中不会将状态切换回到 HitNothing
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                // FIXME:Player给石头的简单设置为数值
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            // 目标是否在前方120度扇形区域
            if (transform.IsFacingTarget(attackTarget.transform))
            {
                var targetState = attackTarget.GetComponent<CharacterState>();

                characterState.TakeDamage(characterState, targetState);
            }
        }
    }
}
