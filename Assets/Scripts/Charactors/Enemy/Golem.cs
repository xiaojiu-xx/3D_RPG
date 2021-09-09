using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;

    public GameObject rockPrefab;

    public Transform handPos;
    public void KickOff()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStates = attackTarget.GetComponent<CharacterState>();
            
            Vector3 direction = (targetStates.transform.position - transform.position).normalized;

            targetStates.GetComponent<NavMeshAgent>().isStopped = true;
            targetStates.GetComponent<NavMeshAgent>().velocity = direction * kickForce;

            // TODO:被攻击者眩晕状态, 在TakeDamage中有判断如果有暴击播放受伤动画 是不是应该修改?
            targetStates.GetComponent<Animator>().SetTrigger("Dizzy");
            // 产生伤害
            characterState.TakeDamage(characterState, targetStates); 
        }
    }

    public void ThrowRock()
    {
        if(attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }
}
