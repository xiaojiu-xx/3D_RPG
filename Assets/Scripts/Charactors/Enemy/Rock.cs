using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates{HitPlayer, HitEnemy, HitNothing}
    public RockStates rockStates;
    private Rigidbody rb;

    [Header("Basic Setting")]
    public float force;
    public GameObject target;
    public int damage;
    private Vector3 direction;
    public GameObject brakeEffect;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        // 石头刚出来时velocity特别小, 给他一个相对较大值 避免刚出来时velocity小于1
        rb.velocity = Vector3.one;
        FlyToTarget();
    }

    void FixedUpdate() 
    {
        if(rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if(target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if(other.gameObject.CompareTag("Player"))
                {// 石头攻击到Player
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    // 攻击的目标眩晕
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterState>().TakeDamage(damage, other.gameObject.GetComponent<CharacterState>());
                    // 攻击完成后
                    rockStates = RockStates.HitNothing;
                }
                break;
            
            case RockStates.HitEnemy:
                if(other.gameObject.GetComponent<Golem>())
                {// 石头攻击到石头怪
                    // 获得石头怪身上的CharacterState
                    var otherStates = other.gameObject.GetComponent<CharacterState>();
                    otherStates.TakeDamage(damage, otherStates);
                    Instantiate(brakeEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }   
    }
}
