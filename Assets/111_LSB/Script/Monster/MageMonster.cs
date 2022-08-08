using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= MAGE MONSTER CLASS : INHERITANCE MONSTER CLASS =============

public class MageMonster : Monster
{
    [SerializeField] float targettingRange; // 타게팅 범위
    [SerializeField] float maxAttackRange;  // 최대 공격 범위
    [SerializeField] float minAttackRange;  // 최소 공격 범위 (Start Run Away)
    [SerializeField] float attackCoolDown;  // 평타 재사용 시간
    [SerializeField] float skillCooldown;   // 스킬 재사용 시간
    [SerializeField] float healAmount;

    // Projectile
    [SerializeField] Transform monsterProjectile;

    // Monster Component
    [SerializeField] SkinnedMeshRenderer monsterSkinnedMeshRenderer;
    private Animator monsterAnimator;
    private Rigidbody monsterRigidbody;
    private NavMeshAgent monsterNavMeshAgent;
    private Material monsterMaterial;

    // Spawn Transform
    private Vector3 spawnPosition;

    // Drop Item
    //public Item[] dropItem;

    private bool isSpawn;
    private bool isChase;
    private bool isPattern;     // 패턴 중인지 여부 (공격 / 스킬)
    private bool isAttackReady; // 기본 공격이 준비되었는지 여부
    private bool isWalk;
    private bool isReturn;      // 스폰 위치로 돌아가는지 여부
    private bool isRunAway;     // 도망중인지 여부
    private bool isSkillReady;  // 스킬이 준비되었는지 여부

    private void Awake()
    {
        IsDead = false;

        monsterAnimator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        monsterNavMeshAgent = GetComponent<NavMeshAgent>();
        monsterMaterial = monsterSkinnedMeshRenderer.material;

        isPattern = false;
        isAttackReady = true;
        isSkillReady = true;

        isSpawn = false;
        isChase = false;
        isWalk = false;
        isReturn = false;
        isRunAway = false;

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
        if (IsDead == true) return;

        if (isPattern || isSpawn)
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
            isRunAway = false;
        }

        else if (maxDistance <= 0.1f)
        {
            isReturn = false;
            isWalk = false;
            isChase = false;
            isRunAway = false;
        }

        if (isReturn == true)
            return;

        // Second Priority: 도망
        float distanceFromTarget = (transform.position - TargetTransform.position).magnitude;

        if (distanceFromTarget < minAttackRange)
        {
            isRunAway = true;
            isWalk = true;
            isChase = false;
        }

        if (distanceFromTarget >= minAttackRange)
        {
            isRunAway = false;
            isWalk = false;
            isChase = false;
        }

        if (isRunAway == true)
            return;

        //  Third Priority: 플레이어 인식
        if (distanceFromTarget <= targettingRange)
        {
            isChase = true;
            isWalk = true;

            Vector3 lookPosition = TargetTransform.position;
            lookPosition.y = 0.0f;

            transform.LookAt(lookPosition);
        }

        // 공격 사거리보다 가까우면 멈춤
        if (distanceFromTarget < maxAttackRange)
        {
            isWalk = false;
        }

        if (isChase == false)
            return;

        // Fourth Priority: 스킬A - Heal
        if (distanceFromTarget <= maxAttackRange && isSkillReady == true)
        {
            RaycastHit[] raycastForSkill
                = Physics.SphereCastAll(transform.position, maxAttackRange
                , Vector3.up, 0, LayerMask.GetMask("Monster"));

            if (raycastForSkill.Length > 0)
            {
                isWalk = false;
                SkillIn();
                return;
            }
        }

        // Last Priority: 일반 공격
        if (distanceFromTarget <= maxAttackRange && isAttackReady == true)
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

        // Move Priority: Return > RunAway > Chase
        // Return
        if (isReturn == true)
        {
            monsterNavMeshAgent.SetDestination(spawnPosition);
        }

        // RunAway
        else if (isRunAway == true)
        {
            Vector3 runAwayDestination = (transform.position - TargetTransform.position).normalized * 2.0f;
            runAwayDestination -= transform.position;

            monsterNavMeshAgent.SetDestination(runAwayDestination);
        }

        // Chase
        else if (isChase == true)
        {
            monsterNavMeshAgent.SetDestination(TargetTransform.position);
        }
    }

    private void SkillIn()
    {
        StartCoroutine(SkillCoolDown(skillCooldown));
        isPattern = true;
        monsterAnimator.SetTrigger("doSkill"); // 스킬 모션 재생
    }

    private void SkillOn()
    {
        RaycastHit[] raycastForSkill
                = Physics.SphereCastAll(transform.position, maxAttackRange
                , Vector3.up, 0, LayerMask.GetMask("Monster"));

        if (raycastForSkill.Length > 0)
        {
            foreach (RaycastHit hitObject in raycastForSkill)
            {
                hitObject.transform.GetComponent<Monster>().CurrentHP = MaxHP * healAmount;
            }
        }
    }

    private void Attack()
    {
        StartCoroutine(AttackCooldown(attackCoolDown));

        isPattern = true;

        monsterProjectile.position = transform.position + Vector3.up;
        monsterProjectile.rotation = transform.rotation;

        monsterAnimator.SetTrigger("doAttack"); // 공격 모션 재생
    }

    private void PatternOut()
    {
        isSpawn = false;
        isPattern = false;
    }

    private IEnumerator AttackCooldown(float _delay)
    {
        isAttackReady = false;
        yield return new WaitForSeconds(_delay);
        isAttackReady = true;
    }

    private IEnumerator SkillCoolDown(float _delay)
    {
        isSkillReady = false;
        yield return new WaitForSeconds(_delay);
        isSkillReady = true;

        Debug.Log("Skill Cooldown");
    }

    // 피격
    public override IEnumerator OnDamage(Vector3 reactVector)
    {
        monsterMaterial.color = Color.red;
        monsterAnimator.SetTrigger("doHit");

        yield return new WaitForSeconds(0.1f);

        PatternOut();

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
        GameManager.Instance.NormalMonsterDrop(transform);
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
    void PlayMageAttackSound()
    {
        AudioManager.Instance.PlaySFX("MAGE_ATTACK");
    }
}
