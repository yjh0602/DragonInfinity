using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= RANGE MONSTER CLASS : INHERITANCE MONSTER CLASS =============

public class RangeMonster : Monster
{
    [SerializeField] float targettingRange; // Ÿ���� ����
    [SerializeField] float maxAttackRange;  // �ִ� ���� ����
    [SerializeField] float minAttackRange;  // �ּ� ���� ���� (Start Run Away)
    [SerializeField] float attackCoolDown;  // ��Ÿ ���� �ð�

    // Projectile
    [SerializeField] GameObject monsterProjectile;

    // Monster Component
    [SerializeField] SkinnedMeshRenderer monsterSkinnedMeshRenderer;
    private Animator monsterAnimator;
    private Rigidbody monsterRigidbody;
    private NavMeshAgent monsterNavMeshAgent;
    private Material monsterMaterial;

    // Spawn Transform
    private Vector3 spawnPosition;

    private bool isSpawn;
    private bool isChase;
    private bool isAttack;
    private bool isAttackReady;
    private bool isWalk;
    private bool isReturn;
    private bool isRunAway;

    private void Awake()
    {
        IsDead = false;

        monsterAnimator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        monsterNavMeshAgent = GetComponent<NavMeshAgent>();
        monsterMaterial = monsterSkinnedMeshRenderer.material;

        isAttack = false;       // ������ �ϰ� �ִ� ������ ����
        isAttackReady = true;   // ����� �������� ����

        isSpawn = false;
        isChase = false;        // ���� ����
        isWalk = false;         // ������ ����
        isReturn = false;       // ���� ��ġ�� ���ư��� ������ ����
        isRunAway = false;      // �÷��̾�κ��� �Ÿ��� ������ �ִ��� ����

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
        if (isWalk == true) // // �����̴� ���¸�
        {
            FreezeVelocity();
        }
    }

    private void Update()
    {
        if (IsDead == true) return;

        if (isAttack || isSpawn)
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

        // Second Priority: ����
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

        //  Third Priority: �÷��̾� �ν�
        if (distanceFromTarget <= targettingRange)
        {
            isChase = true;
            isWalk = true;

            Vector3 lookPosition = TargetTransform.position;
            lookPosition.y = 0.0f;

            transform.LookAt(lookPosition);
        }

        // ���� ��Ÿ����� ������ ����
        if (distanceFromTarget < maxAttackRange)
        {
            isWalk = false;
        }

        if (isChase == false)
            return;

        // Last Priority: �Ϲ� ����
        if (distanceFromTarget <= maxAttackRange && isAttack == false && isAttackReady == true)
        {
            isWalk = false;
            Attack();
            return;
        }
    }

    public override void MoveDecision()
    {
        // ������ ����
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

    private void Attack()
    {
        StartCoroutine(AttackCooldown(attackCoolDown));

        isAttack = true;
        monsterProjectile.transform.position = transform.position + Vector3.up;
        monsterProjectile.transform.rotation = transform.rotation;

        monsterAnimator.SetTrigger("doAttack"); // ���� ��� ���
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


    // �ǰ�
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

    //Sound
    void PlayRangeAttackSound()
    {
        AudioManager.Instance.PlaySFX("RANGE_ATTACK");
    }
}
