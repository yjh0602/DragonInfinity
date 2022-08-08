using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ============= GRUNT CLASS : INHERITANCE MONSTER CLASS =============
// Named Monster

public class Grunt : Monster
{
    // Monster Status
    [SerializeField] float targettingRange; // Ÿ���� ����

    [SerializeField] float attackRange;  // ��Ÿ ���� ����
    [SerializeField] float skillRangeA;  // ��ųA ���� ����
    [SerializeField] float skillRangeB;  // ��ųB ���� ���� 
    [SerializeField] float skillRangeC;  // ��ųC ���� ����

    [SerializeField] float attackCoolDown;   // ��Ÿ ���� �ð�
    [SerializeField] float skillCooldownA;   // ��ųA ���� �ð�
    [SerializeField] float skillCooldownB;   // ��ųB ���� �ð�
    [SerializeField] float skillCooldownC;   // ��ųC ���� �ð�

    // Monster Component
    [SerializeField] SkinnedMeshRenderer monsterSkinnedMeshRenderer;
    private Animator        monsterAnimator;
    private Rigidbody       monsterRigidbody;
    private NavMeshAgent    monsterNavMeshAgent;
    private Material        monsterMaterial;

    // Ÿ�ٰ��� �Ÿ�
    private float distanceFromTarget;

    // Spawn Transform
    private Vector3 spawnPosition;

    // Drop Item
    //public Item[] dropItem;

    private bool isSpawn;
    private bool isChase;
    private bool isPattern;
    private bool isAttackReady;
    private bool isWalk;
    private bool isReturn;

    private bool isSkillReadyA;
    private bool isSkillReadyB;
    private bool isSkillReadyC;

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

        isSkillReadyA = true;
        isSkillReadyB = true;
        isSkillReadyC = true;

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
        switch(RandomPatternGenerator())
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

                // ��ų A: Slam
            case 1:
                {
                    isWalk = false;
                    SkillA();
                    return;
                }

                // ��ų B: Spin
            case 2:
                {
                    isWalk = false;
                    SkillB();
                    return;
                }

                // ��ų C: Swing
            case 3:
                {
                    isWalk = false;
                    SkillC();
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

        monsterAnimator.SetTrigger("doAttack"); // ���� ��� ���
    }

    private void SkillA()
    {
        StartCoroutine(SkillCoolDownA(skillCooldownA));

        isPattern = true;

        monsterAnimator.SetTrigger("doSkillA"); // ��ų ��� ���
    }

    private void SkillB()
    {
        StartCoroutine(SkillCoolDownB(skillCooldownB));

        isPattern = true;

        monsterAnimator.SetTrigger("doSkillB"); // ��ų ��� ���
    }

    private void SkillC()
    {
        StartCoroutine(SkillCoolDownC(skillCooldownC));

        isPattern = true;

        monsterAnimator.SetTrigger("doSkillC");
    }

    private void PatternOut()
    {
        isSpawn = false;
        isPattern = false;
    }

    private void OnSwingAnimation()
    {
        monsterAnimator.SetBool("isSwing", true);
    }
    private void OffSwingAnimation()
    {
        monsterAnimator.SetBool("isSwing", false);
    }

    private IEnumerator AttackCooldown(float _delay)
    {
        isAttackReady = false;
        yield return new WaitForSeconds(_delay);
        isAttackReady = true;
    }

    private IEnumerator SkillCoolDownA(float _delay)
    {
        isSkillReadyA = false;
        yield return new WaitForSeconds(_delay);
        isSkillReadyA = true;
    }
    private IEnumerator SkillCoolDownB(float _delay)
    {
        isSkillReadyB = false;
        yield return new WaitForSeconds(_delay);
        isSkillReadyB = true;
    }
    private IEnumerator SkillCoolDownC(float _delay)
    {
        isSkillReadyC = false;
        yield return new WaitForSeconds(_delay);
        isSkillReadyC = true;
    }

    private int RandomPatternGenerator()
    {
        // 0: ��Ÿ, 1: ����, 2: ����, 3: ����
        List<int> readyPattern = new List<int>();

        if (distanceFromTarget <= attackRange && isAttackReady == true)
            readyPattern.Add(0);

        if (distanceFromTarget <= skillRangeA && isSkillReadyA == true)
            readyPattern.Add(1);

        if (distanceFromTarget <= skillRangeB && isSkillReadyB == true)
            readyPattern.Add(2);

        if (distanceFromTarget <= skillRangeC && isSkillReadyC == true)
            readyPattern.Add(3);

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
    void PlaySwingSound()
    {
        AudioManager.Instance.PlaySFX("GRUNT_SWING");
    }
    void PlaySpinSound()
    {
        AudioManager.Instance.PlaySFX("GRUNT_SPIN");
    }
    void PlaySmashSound()
    {
        AudioManager.Instance.PlaySFX("GRUNT_SMASH");
    }
    void PlayMeleeAttackSound()
    {
        AudioManager.Instance.PlaySFX("MELEE_ATTACK");
    }
}
