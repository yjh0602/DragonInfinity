using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= MELEE MONSTER CLASS : INHERITANCE MONSTER CLASS =============

public class MeleeMonster : Monster
{
    [SerializeField] float targettingRange; // 타게팅 범위
    [SerializeField] float attackRange;     // 공격 시작 범위
    [SerializeField] float attackCoolDown;  // 평타 재사용 시간

    // Monster Component
    [SerializeField] SkinnedMeshRenderer monsterSkinnedMeshRenderer;
    private Animator        monsterAnimator;
    private Rigidbody       monsterRigidbody;
    private NavMeshAgent    monsterNavMeshAgent;
    private Material        monsterMaterial;

    private Vector3 spawnPosition;          // 스폰 위치

    private bool isSpawn;
    private bool isChase;
    private bool isAttack;
    private bool isAttackReady;
    private bool isWalk;
    private bool isReturn;

    private void Awake()
    {
        IsDead = false;

        monsterAnimator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        monsterNavMeshAgent = GetComponent<NavMeshAgent>();
        monsterMaterial = monsterSkinnedMeshRenderer.material;

        isAttack = false;       // 공격을 하고 있는 중인지 여부
        isAttackReady = true;   // 재공격 가능한지 여부

        isSpawn = false;
        isChase = false;
        isWalk = false;         // 움직임 여부
        isReturn = false;       // 스폰 위치로 돌아가는 중인지 여부

        monsterNavMeshAgent.speed = MoveSpeed;
    }

    private void Start()
    {
        SpawnIn();
        CreateHPBar();
        SetTargetTransform();
    }

    private void FixedUpdate()
    {
        if (isWalk == true) // 움직이는 상태면
        {
            FreezeVelocity();
        }
    }

    private void Update()
    {
        if (IsDead == true)
            return;


        if (isAttack || isSpawn)
            return;

        // 행동 결정
        ActionDecision();

        // 움직임 결정
        MoveDecision();
    }
    public override void ActionDecision()
    {
        // First Priority: 회귀
        float maxDistance = (spawnPosition - transform.position).magnitude;

        if (maxDistance >= targettingRange)
        {
            isReturn = true;
            isWalk = true;
            isChase = false;
        }

        else if (maxDistance <= 0.1f)
        {
            isReturn = false;
            isWalk = false;
            isChase = false;
        }

        if (isReturn == true)
            return;

        //  Second Priority: 플레이어 인식
        float distanceFromTarget = (transform.position - TargetTransform.position).magnitude;

        if (distanceFromTarget <= targettingRange)
        {
            isChase = true;
            isWalk = true;

            Vector3 lookPosition = TargetTransform.position;
            lookPosition.y = 0.0f;

            transform.LookAt(lookPosition);
        }

        // 공격 사거리보다 가까우면 멈춤
        if (distanceFromTarget < attackRange)
        {
            isWalk = false;
        }

        if (isChase == false)
            return;

        // Last Priority: 일반 공격
        if (distanceFromTarget <= attackRange && isAttack == false && isAttackReady == true)
        {
            isWalk = false;
            Attack();
            return;
        }
    }

    public override void MoveDecision()
    {
        // 움직임 갱신
        monsterNavMeshAgent.isStopped = !isWalk;
        monsterAnimator.SetBool("isWalk", isWalk);

        // Move Priority: Return > Chase
        // Return
        if (isReturn == true)
        {
            monsterNavMeshAgent.SetDestination(spawnPosition);
        }

        // Chase
        else if (isChase == true)
        {
            monsterNavMeshAgent.SetDestination(TargetTransform.position);
        }
    }

    private void Attack()
    {
        StartCoroutine(AttackCooldown(attackCoolDown));

        isAttack = true;

        monsterAnimator.SetTrigger("doAttack"); // 공격 모션 재생
    }
    private void AttackOut()
    {
        isSpawn = false;
        isAttack = false;
    }

    private IEnumerator AttackCooldown(float _delay)
    {
        isAttackReady = false;
        yield return new WaitForSeconds(_delay);
        isAttackReady = true;
    }


    // 피격
    public override IEnumerator OnDamage(Vector3 reactVector)
    {
        monsterMaterial.color = Color.red;
        monsterAnimator.SetTrigger("doHit");

        yield return new WaitForSeconds(0.1f);
        AttackOut();

        if (CurrentHP > 0)
        {
            monsterMaterial.color = Color.white;
        }

        else // CurrenHP <= 0 (사망 상태)
        {
            Dead();
            DropItem();

            monsterMaterial.color = Color.gray;

            reactVector.Normalize();
            reactVector += Vector3.up;

            monsterRigidbody.AddForce(reactVector * 2, ForceMode.Impulse);
            monsterAnimator.SetTrigger("doDie");
            Destroy(gameObject, 4.0f);
        }

    }

    public override void DropItem()
    {
        GameManager.Instance.NormalMonsterDrop(gameObject.transform);
    }

    public override void FreezeVelocity()
    {
        monsterRigidbody.velocity = Vector3.zero;
        monsterRigidbody.angularVelocity = Vector3.zero;
    }

    public override void InitStatus()
    {
        CurrentHP = MaxHP;
    }

    public override void SpawnIn()
    {
        isSpawn = true;
        spawnPosition = gameObject.transform.position;
        monsterAnimator.SetTrigger("doSpawn");
    }
    public override void SpawnOut()
    {
        isSpawn = false;
    }

    public override void Dead()
    {
        IsDead = true;
        isWalk = false;
        isReturn = false;
        isChase = false;

        monsterNavMeshAgent.enabled = false;
    }

    // Sound
    void PlayMeleeAttackSound()
    {
        AudioManager.Instance.PlaySFX("MELEE_ATTACK");
    }
}
