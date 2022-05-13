using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCombat : MonoBehaviour
{
    public GameObject targetedEnemy;
    public enum HeroAttackType { Melee, Range };
    public HeroAttackType heroAttackType;
    public float attackRange;
    public float rotateSpeedForAttack;
    public bool basicAttackIdle = false;
    public bool isHeroAlive;
    public bool performMeleeAttack = true;
    private Movement moveScript;
    private Stats statsScript;
    private Animator anim;
    public bool performRangedAttack = true;
    public GameObject projPrefab;
    public Transform projSpawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        moveScript = GetComponent<Movement>();
        statsScript = GetComponent<Stats>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetedEnemy != null) {
            if(Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) > attackRange) {
                moveScript.agent.SetDestination(targetedEnemy.transform.position);
                moveScript.agent.stoppingDistance = attackRange;
                //rotation
                Quaternion rotationToLookAt = Quaternion.LookRotation(targetedEnemy.transform.position - transform.position);
                float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationToLookAt.eulerAngles.y, ref moveScript.rotateVelocity, rotateSpeedForAttack * (Time.deltaTime * 5));

                transform.eulerAngles = new Vector3(0, rotationY, 0);
            }
            else {
                if(heroAttackType == HeroAttackType.Melee) {
                    if(performMeleeAttack) {
                        Debug.Log("Attack Creep");
                    }
                }
            }
        }
    }

    IEnumerator RangedAttackInterval() {
        performRangedAttack = false;
        anim.SetBool("Basic Attack", true);
        yield return new WaitForSeconds(statsScript.attackTime / ((100 + statsScript.attackTime) * 0.01f));

        if (targetedEnemy == null) {
            anim.SetBool("Basic Attack", false);
            performRangedAttack = true;
        }
    }

    public void MeleeAttack() {
        if(targetedEnemy != null)  {
            if(targetedEnemy.GetComponent<Targetable>().enemyType == Targetable.EnemyType.Creep) {
                targetedEnemy.GetComponent<Stats>().health -= statsScript.attackDamage;
            }
        }
        performMeleeAttack = true;
    }
}
