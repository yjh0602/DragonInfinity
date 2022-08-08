using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= ICE DEMON CLASS : INHERITANCE MONSTER CLASS =============
// Named Monster

public class IceDemon : Monster
{
    // Monster Status
    [SerializeField] float targettingRange; // Ÿ���� ����

    [SerializeField] float attackRange;     // ��Ÿ ���� ����
    [SerializeField] float smashRange;      // ���Ž� ���� ����
    [SerializeField] float roarRange;       // ��ų �ξ� ���� ����

    [SerializeField] float attackCoolDown;  // ��Ÿ ���� �ð�
    [SerializeField] float smashCoolDown;   // ���Ž� ���� �ð�
    [SerializeField] float roarCooldown;    // �ξ� ���� �ð�

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
        if (distanceFromTarget < attackRange)
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

            // ��Ÿ
            case 0:
                {
                    isWalk = false;
                    Attack();
                    return;
                }

            // ���Ž�
            case 1:
                {
                    isWalk = false;
                    Smash();
                    return;
                }

            // ��ų A: �ξ�
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
        // ������ ����
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
        // 0: ��Ÿ, 1: ���Ž�, 2: �ξ�
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

    // �ǰ�
    public override IEnumerator OnDamage(Vector3 reactVector)
    {
        monsterMaterial.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (CurrentHP > 0)
        {
            PlayDemonHitSound();
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
