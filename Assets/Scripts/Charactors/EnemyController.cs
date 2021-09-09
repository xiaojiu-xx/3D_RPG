using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }  // 守卫    巡逻    追击    死亡
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterState))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private NavMeshAgent agent;

    private Animator anim;

    private Collider coll;

    // Enemy身上的CharacterState
    protected CharacterState characterState;

    private EnemyStates enemyState;         // 怪物的状态

    protected GameObject attackTarget;      // 攻击目标

    private float lastAttackTime;

    [Header("Basic Settings")]
    public float sightRadius;

    public float lookAtTime;
    private float remainlookAtTime;

    public bool isGuard;            // 是否为站桩怪物

    private float speed;

    [Header("Patrol State")]
    public float patrolRange;

    private Vector3 wayPoint;       // 巡逻要到达的点

    private Vector3 guardPos;

    private Quaternion guardRotation;



    // 动画标志值
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterState = GetComponent<CharacterState>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainlookAtTime = lookAtTime;
        coll = GetComponent<Collider>();
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyStates.GUARD;
        }
        else
        {
            enemyState = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        // FIXME:切换场景后修改掉
        GameManager.Instance.AddObserver(this);
    }

    // 切换场景时调用
    void OnEnable()
    {
        // GameManager.Instance.AddObserver(this);
    }

    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);

        // 挂载LootSpawner组件，并且死亡了
        if (GetComponent<LootSpawner>() && isDead)
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }

        if (QuestManage.IsInitialized && isDead)
            QuestManage.Instance.UpdateQuestProgress(this.name, 1);
    }

    private void Update()
    {
        if (characterState.CurrentHealth == 0)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchEnemyState();
            SwitchStates();
            lastAttackTime -= Time.deltaTime;
        }

    }

    private void SwitchStates()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterState.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchEnemyState()
    {
        if (isDead)
        {
            enemyState = EnemyStates.DEAD;
        }
        // 如果发现player，切换到追击状态(CHASE)
        else if (FoundPlayer())
        {
            enemyState = EnemyStates.CHASE;
        }

        switch (enemyState)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }

                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                // 是否已经到达该点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainlookAtTime > 0)
                    {
                        remainlookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }

                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;

                agent.speed = speed;
                if (!FoundPlayer())
                {
                    // 拉托回到上一个状态
                    isFollow = false;
                    if (remainlookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainlookAtTime -= Time.deltaTime;
                    }

                    else
                    {
                        if (isGuard)
                            enemyState = EnemyStates.GUARD;
                        else
                            enemyState = EnemyStates.PATROL;
                    }

                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                // 在攻击范围之内进行攻击（攻击动画）
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterState.baseAttackData.coolDown;

                        // 暴击判断
                        characterState.isCritical = Random.value < characterState.baseAttackData.criticalChance;
                        // 进行攻击
                        Attack();
                    }
                }

                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                // 当Player的攻击导致Enemy死亡的时候 而Enemy执行攻击动画状态机一直调用NavMeshAgent 此时将报错
                // agent.enabled = false; 
                agent.radius = 0;

                Destroy(gameObject, 2f);

                break;
        }
    }



    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //
            anim.SetTrigger("Skill");
        }
    }
    // 在可视范围找到Player 
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                // 找到攻击目标
                attackTarget = target.gameObject;
                // Debug.Log("找到攻击目标" + attackTarget.tag);
                return true;
            }

        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterState.baseAttackData.attackRange;
        }
        else
        {
            return false;
        }
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterState.baseAttackData.skillRange;
        }
        else
        {
            return false;
        }
    }

    // 生成巡逻的点
    private void GetNewWayPoint()
    {
        remainlookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX,
            transform.position.y,
            guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    // Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            // 目标状态 也就是Player
            var targetState = attackTarget.GetComponent<CharacterState>();
            // 参数为 攻击者characterState 、防御者characterState
            // 调用者为制造伤害的人，
            characterState.TakeDamage(characterState, targetState);
        }
    }

    public void EndNotify()
    {
        // 获胜动画
        // 停止所有移动
        // 停止agent
        anim.SetBool("Win", true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
