using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= ICE DEMON CLASS : INHERITANCE MONSTER CLASS =============
// Named Monster

public class IceDemon : Monster
{
    // Monster Status
    [SerializeField] float targettingRange; // 타게팅 범위

    [SerializeField] float attackRange;     // 평타 공격 범위
    [SerializeField] float smashRange;      // 스매시 공격 범위
    [SerializeField] float roarRange;       // 스킬 로어 공격 범위

    [SerializeField] float attackCoolDown;  // 평타 재사용 시간
    [SerializeField] float smashCoolDown;   // 스매시 재사용 시간
    [SerializeField] float roarCooldown;    // 로어 재사용 시간

    // Monster Component
    [SerializeField] SkinnedMeshRenderer monsterSkinnedMeshRenderer;
    private Animator monsterAnimator;
    private Rigidbody monsterRigidbody;
    private NavMeshAgent monsterNavMeshAgent;
    private Material monsterMaterial;

    // 타겟과의 거리
    private float distanceFromTarget;

    // Spawn Transform
    private Vector3 spawnPosition;

    private bool isSpawn;
    private bool isChase;
    private bool isPattern;
    private bool isWalk;
    private bool isReturn;

    private bool isAttackReady;
    private bool isSmashReady;
    private bool isRoarReady;

    private void Awake()
    {
        IsDead = false;

        monsterAnimator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        monsterNavMeshAgent = GetComponent<NavMeshAgent>();
        monsterMaterial = monsterSkinnedMeshRenderer.material;

        distanceFromTarget = 0.0f;

        isPattern = false;
        isAttackReady = true;

        isSmashReady = true;
        isRoarReady = true;

        isSpawn = false;
        isChase = false;
        isWalk = false;
        isReturn = false;

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
            InitHP();
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
        distanceFromTarget = (transform.position - TargetTransform.position).magnitude;

        if (distanceFromTarget <= targettingRange)
        {
            isChase = true;
            isWalk = true;

            Vector3 lookPosition = TargetTransform.position;
            lookPosition.y = 0.0f;

            transform.LookAt(lookPosition);
        }

        // 기본 공격 사거리보다 가까우면 추격 중지
        if (distanceFromTarget < attackRange)
        {
            isWalk = false;
        }

        if (isChase == false)
            return;

        // Third Priority: 랜덤 공격 패턴
        switch (RandomPatternGenerator())
        {
            // 전부 쿨다운 중
            case -1:
                {
                    return;
                }

            // 평타
            case 0:
                {
                    isWalk = false;
                    Attack();
                    return;
                }

            // 스매시
            case 1:
                {
                    isWalk = false;
                    Smash();
                    return;
                }

            // 스킬 A: 로어
            case 2:
                {
                    isWalk = false;
                    SkillRoar();
                    return;
                }
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

        isPattern = true;

        monsterAnimator.SetTrigger("doAttack");
    }

    private void Smash()
    {
        StartCoroutine(SmashCooldown(smashCoolDown));

        isPattern = true;

        monsterAnimator.SetTrigger("doSmash");
    }

    private void SkillRoar()
    {
        StartCoroutine(RoarCoolDown(roarCooldown));

        isPattern = true;

        monsterAnimator.SetTrigger("doRoar");
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

    private IEnumerator SmashCooldown(float _delay)
    {
        isSmashReady = false;
        yield return new WaitForSeconds(_delay);
        isSmashReady = true;
    }
    private IEnumerator RoarCoolDown(float _delay)
    {
        isRoarReady = false;
        yield return new WaitForSeconds(_delay);
        isRoarReady = true;
    }

    private int RandomPatternGenerator()
    {
        // 0: 평타, 1: 스매시, 2: 로어
        List<int> readyPattern = new List<int>();

        if (distanceFromTarget <= attackRange && isAttackReady == true)
            readyPattern.Add(0);

        if (distanceFromTarget <= smashRange && isSmashReady == true)
            readyPattern.Add(1);

        if (distanceFromTarget <= roarRange && isRoarReady == true)
            readyPattern.Add(2);

        if (readyPattern.Count == 0)
        {
            return -1;
        }

        int randomNumber = Random.Range(0, readyPattern.Count);

        return readyPattern[randomNumber];
    }

    // 피격
    public override IEnumerator OnDamage(Vector3 reactVector)
    {
        monsterMaterial.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (CurrentHP > 0)
        {
            PlayDemonHitSound();
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
        GameManager.Instance.NamedMonsterDrop(gameObject.transform);
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
    void PlayDemonHitSound()
    {
        AudioManager.Instance.PlaySFX("DEMON_HIT");
    }
    void PlayDemonBreathSound()
    {
        AudioManager.Instance.PlaySFX("DEMON_BREATH");
    }
    void PlayIceDemonSmash()
    {
        AudioManager.Instance.PlaySFX("ICE_DEMON_SMASH");
    }
    void PlayMeleeAttackSound()
    {
        AudioManager.Instance.PlaySFX("MELEE_ATTACK");
    }
}
