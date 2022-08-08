using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= SKELETONKING CLASS : INHERITANCE MONSTER CLASS =============
// Named Monster

public class SkeletonKing : Monster
{
    // Monster Status
    [SerializeField] float targettingRange; // Ÿ���� ����

    [SerializeField] float attackRangeA;    // ��ŸA ���� ����
    [SerializeField] float attackRangeB;    // ��ŸB ���� ����
    [SerializeField] float skillRangeA;     // ��ųA ���� ����

    [SerializeField] float attackCoolDownA;  // ��ŸA ���� �ð�
    [SerializeField] float attackCoolDownB;  // ��ƼB ���� �ð�
    [SerializeField] float skillCooldownA;   // ��ųA ���� �ð�

    [SerializeField] GameObject skillChargeEffect;
    [SerializeField] GameObject skillBlackHoleEffect;

    // Monster Component
    [SerializeField] SkinnedMeshRenderer monsterSkinnedMeshRenderer;
    private Animator monsterAnimator;
    private Rigidbody monsterRigidbody;
    private NavMeshAgent monsterNavMeshAgent;
    private Material monsterMaterial;

    // Ÿ�ٰ��� �Ÿ�
    private float distanceFromTarget;

    // Spawn Transform
    private Vector3 spawnPosition;

    private bool isSpawn;
    private bool isChase;
    private bool isWalk;
    private bool isReturn;

    private bool isPattern;
    private bool isAttackReadyA;
    private bool isAttackReadyB;
    private bool isSkillReadyA;

    private void Awake()
    {
        IsDead = false;

        monsterAnimator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        monsterNavMeshAgent = GetComponent<NavMeshAgent>();
        monsterMaterial = monsterSkinnedMeshRenderer.material;

        distanceFromTarget = 0.0f;

        isPattern = false;

        isAttackReadyA = true;
        isAttackReadyB = true;
        isSkillReadyA = true;

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
        if (isWalk == true) // �����̴� ���¸�
        {
            FreezeVelocity();
        }
    }

    private void Update()
    {
        if (IsDead == true) return;

        if (isPattern || isSpawn)
            return;

        // �ൿ ����
        ActionDecision();

        // ������ ����
        MoveDecision();
    }

    public override void ActionDecision()
    {
        // First Priority: ȸ��
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

        //  Second Priority: �÷��̾� �ν�
        distanceFromTarget = (transform.position - TargetTransform.position).magnitude;

        if (distanceFromTarget <= targettingRange)
        {
            isChase = true;
            isWalk = true;

            Vector3 lookPosition = TargetTransform.position;
            lookPosition.y = 0.0f;

            transform.LookAt(lookPosition);
        }

        // �⺻ ���� ��Ÿ����� ������ �߰� ����
        if (distanceFromTarget < attackRangeA)
        {
            isWalk = false;
        }

        if (isChase == false)
            return;

        // Third Priority: ���� ���� ����
        switch (RandomPatternGenerator())
        {
            // ���� ��ٿ� ��
            case -1:
                {
                    return;
                }

            // ��Ÿ1
            case 0:
                {
                    isWalk = false;
                    AttackA();
                    return;
                }

            // ��Ÿ2
            case 1:
                {
                    isWalk = false;
                    AttackB();
                    return;
                }

            // ��ų A: Raise Dead
            case 2:
                {
                    isWalk = false;
                    SkillA();
                    return;
                }
        }
    }

    public override void MoveDecision()
    {
        // ������ ����
        monsterNavMeshAgent.isStopped = !isWalk;
        monsterAnimator.SetBool("isWalk", isWalk);

        // Destination Priority: Return > Chase
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

    private void AttackA()
    {
        StartCoroutine(AttackCooldownA(attackCoolDownA));

        isPattern = true;

        monsterAnimator.SetTrigger("doAttackA"); // ���� ��� ���
    }

    private void AttackB()
    {
        StartCoroutine(AttackCooldownB(attackCoolDownB));

        isPattern = true;

        monsterAnimator.SetTrigger("doAttackB"); // ��ų ��� ���
    }

    private void SkillA()
    {
        StartCoroutine(SkillCoolDownA(skillCooldownA));
        StartCoroutine(OnCastingEffect());
        isPattern = true;
        monsterAnimator.SetTrigger("doSkillA"); // ��ų ��� ���
    }

    private IEnumerator OnCastingEffect()
    {
        skillChargeEffect.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        skillChargeEffect.SetActive(false);
    }

    private void CreateBlackHole()
    {
        Vector3 targetPosition = TargetTransform.position;

        GameObject blackhole = Instantiate(skillBlackHoleEffect, targetPosition, TargetTransform.rotation);
        blackhole.GetComponent<BlackHole>().OwnerMonster = this;
    }

    private void PatternOut()
    {
        isSpawn = false;
        isPattern = false;
    }

    private void OnCastingAnimation()
    {
        monsterAnimator.SetBool("isCasting", true);
    }
    private void OffCastingAnimation()
    {
        monsterAnimator.SetBool("isCasting", false);
    }

    private IEnumerator AttackCooldownA(float _delay)
    {
        isAttackReadyA = false;
        yield return new WaitForSeconds(_delay);
        isAttackReadyA = true;
    }

    private IEnumerator AttackCooldownB(float _delay)
    {
        isAttackReadyB = false;
        yield return new WaitForSeconds(_delay);
        isAttackReadyB = true;
    }

    private IEnumerator SkillCoolDownA(float _delay)
    {
        isSkillReadyA = false;
        yield return new WaitForSeconds(_delay);
        isSkillReadyA = true;
    }

    private int RandomPatternGenerator()
    {
        // 0: ��ŸA, 1: ��ŸB, 2: ������ ����
        List<int> readyPattern = new List<int>();

        if (distanceFromTarget <= attackRangeA && isAttackReadyA == true)
            readyPattern.Add(0);

        if (distanceFromTarget <= attackRangeB && isAttackReadyB == true)
            readyPattern.Add(1);

        if (distanceFromTarget <= skillRangeA && isSkillReadyA == true)
            readyPattern.Add(2);

        if (readyPattern.Count == 0)
        {
            return -1;
        }

        int randomNumber = Random.Range(0, readyPattern.Count);

        return readyPattern[randomNumber];
    }

    // �ǰ�
    public override IEnumerator OnDamage(Vector3 reactVector)
    {
        monsterMaterial.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (CurrentHP > 0)
        {
            PlayMonsterHitSound();
            monsterMaterial.color = Color.white;
        }

        else // CurrenHP <= 0 (��� ����)
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
    void PlaySkeletonKingHitSound()
    {
        AudioManager.Instance.PlaySFX("SKELETON_KING_HIT");
    }
    void PlaySkeletonKingAttackSound()
    {
        AudioManager.Instance.PlaySFX("SKELETON_KING_ATTACK");
    }
}
