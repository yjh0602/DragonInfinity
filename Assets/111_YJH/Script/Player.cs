using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField] float currentHP;
    [SerializeField] float currentMP;
    [SerializeField] float maxHP;
    [SerializeField] float maxMP;
    [SerializeField] float criticalChance;
    [SerializeField] float criticalDamage;

    [SerializeField] float qSkillMP;
    [SerializeField] float wSkillMP;
    [SerializeField] float eSkillMP;
    [SerializeField] float rSkillMP;

    public float moveSpeed; // �̵��ӵ�                
    public float dodgePower; // ������ �ӵ�            
    public float damage; // ���ݷ�                     
    public float defense; //����

    public float healingTime; // ġ���ð�
    public float healingAmount; // ü�� ȸ����
    public float manaAmount; // ���� ȸ����

    public float wSkillDamageIncrease; // W��ų ������ ������         

    public float qCoolTime;
    public float wCoolTime;
    public float eCoolTime;
    public float rCoolTime;

    private float comboFloat; // �޺� ��ø �ð�.

    [SerializeField] GameObject skillEffectBuff; // ������ų
    [SerializeField] GameObject skillEffectQ;
    [SerializeField] GameObject skillEffectW;
    [SerializeField] GameObject skillEffectE;
    [SerializeField] GameObject skillEffectR;
    [SerializeField] GameObject comboEffect1;
    [SerializeField] GameObject comboEffect2;
    [SerializeField] GameObject comboEffect3;
    [SerializeField] GameObject comboEffect4;

    public Camera mainCamera;
    private Animator playerAnimator;
    [HideInInspector] public NavMeshAgent playerNavMeshAgent;
    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;

    private Vector3 destination; // ������ ��

    private bool isRun;     // �����̴���
    private bool isDodge;   // ȸ����
    private bool isAttack;  // ������
    private bool isHit;     // �´���
    private bool isSkill;   // ��ų������
    private bool isBuff;    // ��������
    private bool isStun;    // ���ϻ���
    private bool isDead;    // ����

    [HideInInspector] public bool isDodgeCool; // ȸ�� ��
    [HideInInspector] public bool isQskillCool; // Q��ų ��
    [HideInInspector] public bool isWskillCool; // W��ų ��
    [HideInInspector] public bool isRskillCool; // R��ų ��
    [HideInInspector] public bool isEskillCool; // E��ų ��

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerNavMeshAgent = GetComponent<NavMeshAgent>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();

        playerNavMeshAgent.updateRotation = false; // ĳ���Ͱ� ��Ʈ�� ȸ������ �ʵ���
        playerNavMeshAgent.speed = moveSpeed;
    }

    void Start()
    {
        CurrentHP = MaxHP;
        CurrentMP = MaxMP;

        isRun = false;

        // ���۽� �� �ʱ�ȭ
        isQskillCool = true;
        isWskillCool = true;
        isEskillCool = true;
        isRskillCool = true;
    }

    private void OnEnable()
    {
        StartCoroutine(SelfHealing());
    }

    // Update is called once per frame
    void Update()
    {
        // �ൿ �Ҵ� ����
        if (isDead)
            return;

        Dodge();
        if (isDodge)
            return;

        Skill();
        if (isSkill)
            return;

        Attack();
        if (isAttack)
            return;

        Move();
    }

    private void Move()
    {
        playerNavMeshAgent.speed = moveSpeed;

        if (Input.GetMouseButton(1)) // ���콺 ��Ŭ��
        {
            RaycastHit hit; // Ŭ�� �Ѱ��� ��ǥ
            // collider �� �ִ� ��ü�� ������ �Է� , out hit���� �װ��� ��ȯ
            // physics.Raycast(ray , out hit)  // ray���� ��ǥ�� ���� ray ���������� ��ǥ�� ��ȯ
            if (playerNavMeshAgent.enabled == true && !isHit && !isStun && Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                isRun = true;
                SetDestination(hit.point);
            }
        }

        LookMoveDirection();
    }

    private void SetDestination(Vector3 _destination)
    {
        playerNavMeshAgent.SetDestination(_destination);
        destination = _destination;
        isRun = true;
        playerAnimator.SetBool("isRun", true);
    }

    private void LookMoveDirection()
    {
        if(isRun == true)
        {
            if(playerNavMeshAgent.velocity.magnitude <= 0.2f)
            {
                isRun = false;
                playerAnimator.SetBool("isRun", false);
                return;
            }
            var direction = new Vector3(playerNavMeshAgent.steeringTarget.x, transform.position.y, playerNavMeshAgent.steeringTarget.z) - transform.position;
            playerAnimator.transform.forward = direction;
        }
    }

    void Attack()
    {
        if (Input.GetMouseButton(0)  && playerNavMeshAgent.enabled == true)
        {
            LookMousePosition();

            playerNavMeshAgent.SetDestination(transform.position);
            isAttack = true;
            isRun = false;
            comboFloat += Time.deltaTime * 2f;
            playerAnimator.SetFloat("ComboFloat", comboFloat);
            playerAnimator.SetBool("isAttack", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            isAttack = false;
            comboFloat = 0;
            playerAnimator.SetFloat("ComboFloat", comboFloat);
            playerAnimator.SetBool("isAttack", false);
        }
        if (comboFloat >= 5.94)
        {
            comboFloat = 0;
            playerAnimator.SetFloat("ComboFloat", comboFloat);
            playerAnimator.SetBool("isAttack", false);
        }
    }

    void AttackOut()
    {
        isAttack = false;
        playerAnimator.SetBool("isAttack", false);
    }

    void StopMotion()
    {
        comboFloat = 0;
        isAttack = false;
        isRun = false;
        playerAnimator.SetBool("isAttack", false);
        playerAnimator.SetBool("isRun", false);
        playerNavMeshAgent.SetDestination(transform.position);
    }

    void LookMousePosition()
    {
        RaycastHit mousePoint;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out mousePoint))
        {
            Vector3 mouseDirect = new Vector3(mousePoint.point.x, transform.position.y, mousePoint.point.z) - transform.position;
            playerAnimator.transform.forward = mouseDirect;
        }
    }

    bool CheckCritical()
    {
        int critical = Random.Range(0, 1000);
        float myCriticalChance = (int)criticalChance * 10;

        if (critical <= myCriticalChance)
            return true;

        else
            return false;
    }
    void OnCriticalDamage(Monster _monster, float _mul)
    {
        float finalDamage = damage - _monster.Defense;
        if(finalDamage < 0)
        {
            finalDamage = 1;
        }

        _monster.CurrentHP -= finalDamage * _mul * criticalDamage;
    }
    void OffCriticalDamage(Monster _monster, float _mul)
    {
        float finalDamage = damage - _monster.Defense;
        if(finalDamage < 0)
        {
            finalDamage = 1;
        }

        _monster.CurrentHP -= finalDamage * _mul;
    }
    public void DecideDamage(Collider other, float _mul)
    {
        Monster monster = other.GetComponent<Monster>();
        bool isCritical = CheckCritical();

        switch (isCritical)
        {
            case true:
                OnCriticalDamage(monster, _mul);
                break;
            case false:
                OffCriticalDamage(monster, _mul);
                break;
        }
    }

    // �������� �����
    void OnCriticalDamageForBoss(BossMonster _monster, float _mul)
    {
        float finalDamage = damage - _monster.redDragonDefense;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        _monster.CurrentHP -= finalDamage * _mul * criticalDamage;
    }
    void OffCriticalDamageForBoss(BossMonster _monster, float _mul)
    {
        float finalDamage = damage - _monster.redDragonDefense;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        _monster.CurrentHP -= finalDamage * _mul;
    }

    public void DecideDamageForBoss(Collider other, float _mul)
    {
        BossMonster bossMonster = other.GetComponent<BossMonster>();
        bool isCritical = CheckCritical();

        switch (isCritical)
        {
            case true:
                OnCriticalDamageForBoss(bossMonster, _mul);
                break;
            case false:
                OffCriticalDamageForBoss(bossMonster, _mul);
                break;
        }
    }


    void Dead()
    {
        playerCollider.enabled = false;
        isDead = true;

        isHit = false;
        isAttack = false;
        isRun = false;
        playerNavMeshAgent.isStopped = true;

        playerAnimator.SetTrigger("doDead");
        playerAnimator.SetBool("isHit", false);
        playerAnimator.SetBool("isAttack", false);
        playerAnimator.SetBool("isRun", false);

        comboFloat = 0f;

        StartCoroutine(GameManager.Instance.GameOver());
    }

    IEnumerator SelfHealing()
    {
        while (isDead == false)
        {
            CurrentHP += MaxHP * healingAmount;
            CurrentMP += MaxMP * manaAmount;
            yield return new WaitForSeconds(healingTime);
        }
    }
    void Dodge()
    {
        if (Input.GetButtonDown("Dodge") && !isDodge && isDodgeCool && !isStun)
        {
            AudioManager.Instance.PlaySFX("PLAYER_DODGE");

            DodgeStart();
            StartCoroutine(DodgeCool());
        }
    }
    void DodgeStart()
    {
        StopMotion();
        LookMousePosition();

        isDodge = true;
        playerNavMeshAgent.speed += dodgePower;
        playerCollider.enabled = false;

        Vector3 offset = Vector3.up;
        Ray ray = new Ray(transform.position + offset, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dodgePower))
        {
            playerNavMeshAgent.SetDestination(hit.point);
        }
        else
        {
            Vector3 vector = transform.position + (transform.forward * dodgePower);
            playerNavMeshAgent.SetDestination(vector);
        }

        playerAnimator.SetTrigger("doDodge");
    }

    void DodgeEnd()
    {
        playerNavMeshAgent.speed -= dodgePower;
        isAttack = false;
        isSkill = false;
        isDodge = false;
        playerCollider.enabled = true;
    }


    IEnumerator DodgeCool()
    {
        isDodgeCool = false;
        yield return new WaitForSeconds(2f);
        isDodgeCool = true;
    }

    void Skill()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isSkill && isQskillCool && !((CurrentMP - qSkillMP) < 0))
        {
            StopMotion();
            LookMousePosition();
            SkillQ();
            StartCoroutine(SkillQcoolTime());
        }

        if (Input.GetKeyDown(KeyCode.W) && !isSkill && !isBuff && isWskillCool && !((CurrentMP - wSkillMP) < 0))
        {
            StopMotion();
            LookMousePosition();
            SkillW();
            StartCoroutine(Buff());
            StartCoroutine(SkillWcoolTime());
        }

        if (Input.GetKeyDown(KeyCode.E) && !isSkill && isEskillCool && !((CurrentMP - eSkillMP) < 0))
        {
            StopMotion();
            LookMousePosition();
            SkillE();
            StartCoroutine(SkillEcoolTime());
        }

        if (Input.GetKeyDown(KeyCode.R) && !isSkill && isRskillCool && !((CurrentMP - rSkillMP) < 0))
        {
            StopMotion();
            LookMousePosition();
            SkillR();
            StartCoroutine(SkillRcoolTime());
        }
    }
    void SkillQ()
    {
        isSkill = true;
        CurrentMP -= qSkillMP;
        playerAnimator.SetTrigger("Q");
    }
    IEnumerator SkillQcoolTime()
    {
        isQskillCool = false;
        yield return new WaitForSeconds(qCoolTime);
        isQskillCool = true;
    }
    void SkillW()
    {
        isSkill = true;
        CurrentMP -= wSkillMP;
        playerAnimator.SetTrigger("W");
    }
    IEnumerator Buff()
    {
        moveSpeed += 5;
        isBuff = true;
        damage += wSkillDamageIncrease;
        playerNavMeshAgent.speed = moveSpeed;

        skillEffectBuff.SetActive(true);

        yield return new WaitForSeconds(10f);

        skillEffectBuff.SetActive(false);
        moveSpeed -= 5;
        isBuff = false;
        damage -= wSkillDamageIncrease;
        playerNavMeshAgent.speed = moveSpeed;
    }
    IEnumerator SkillWcoolTime()
    {
        isWskillCool = false;
        yield return new WaitForSeconds(wCoolTime);
        isWskillCool = true;
    }
    void SkillE()
    {
        isSkill = true;
        CurrentMP -= eSkillMP;
        playerAnimator.SetTrigger("E");
    }
    IEnumerator SkillEcoolTime()
    {
        isEskillCool = false;
        yield return new WaitForSeconds(eCoolTime);
        isEskillCool = true;
    }
    void SkillR()
    {
        isSkill = true;
        CurrentMP -= rSkillMP;
        playerAnimator.SetTrigger("R");
    }
    IEnumerator SkillRcoolTime()
    {
        isRskillCool = false;
        yield return new WaitForSeconds(rCoolTime);
        isRskillCool = true;
    }
    public void SkillOut()
    {
        isSkill = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHit || !isDodge || !isSkill) // �ǰ�, ȸ��, ��ų �߿��� ��� X
        {
            // �ǰ� (�´� ȿ����, ������� ���Ϳ��� ó��)
            if (other.tag == "Monster Attack" || other.tag == "Trap Attack")
            {
                OnHit();
            }

            if (other.tag == "Hammer" && !isStun)
            {
                playerNavMeshAgent.SetDestination(transform.position);
                OnStun();
            }
        }
    }
    void OnHit()
    {
        float HitFloat = Random.Range(1, 4);

        isHit = true;
        isRun = false;
        isSkill = false;

        playerAnimator.SetBool("isHit", true);
        playerAnimator.SetBool("isRun", false);
        playerAnimator.SetFloat("HitFloat", HitFloat);

        if(CurrentHP <= 0 && isDead == false)
        {
            Dead();
        }
    }

    void OffHit()
    {
        isHit = false;
        playerAnimator.SetBool("isHit", isHit);
    }

    void OnStun()
    {
        if (CurrentHP <= 0 && isDead == false)
        {
            Dead();
        }
        isStun = true;
        playerAnimator.SetTrigger("doStun");
    }
    void OffStun()
    {
        isStun = false;
    }

    public void InitState()
    {
        playerCollider.enabled = true;

        skillEffectBuff.SetActive(false);
        comboEffect1.SetActive(false);
        comboEffect2.SetActive(false);
        comboEffect3.SetActive(false);
        comboEffect4.SetActive(false);
        skillEffectQ.SetActive(false);
        skillEffectW.SetActive(false);
        skillEffectE.SetActive(false);
        skillEffectR.SetActive(false);

        isRun = false;     // �����̴���
        isDodge = false;   // ȸ����
        isAttack = false;  // ������
        isHit = false;     // �´���
        isSkill = false;   // ��ų������
        isBuff = false;    // ��������
        isStun = false;    // ���ϻ���
        isDead = false;    // ����

        isDodgeCool = true; // ȸ�� ��
        isQskillCool = true; // Q��ų ��
        isWskillCool = true; // W��ų ��
        isRskillCool = true; // R��ų ��
        isEskillCool = true; // E��ų ��

        playerAnimator.SetBool("isRun", false);
        playerAnimator.SetBool("isHit", false);
        playerAnimator.SetBool("isAttack", false);

        CurrentHP = MaxHP;
        CurrentMP = MaxMP;
    }

    // Sound
    void PlayWalkSound()
    {
        AudioManager.Instance.PlaySFX("PLAYER_WALK");
    }

    void PlaySkillSoundQ()
    {
        AudioManager.Instance.PlaySFX("PLAYER_Q_SKILL");
    }

    void PlaySkillSoundW()
    {
        AudioManager.Instance.PlaySFX("PLAYER_W_SKILL");
    }

    void PlaySkillSoundE()
    {
        AudioManager.Instance.PlaySFX("PLAYER_E_SKILL");
    }

    void PlaySkillSoundR()
    {
        AudioManager.Instance.PlaySFX("PLAYER_R_SKILL");
    }

    void PlayComboSound1()
    {
        AudioManager.Instance.PlaySFX("PLAYER_COMBO_1");
    }
    void PlayComboSound2()
    {
        AudioManager.Instance.PlaySFX("PLAYER_COMBO_2");
    }
    void PlayComboSound3()
    {
        AudioManager.Instance.PlaySFX("PLAYER_COMBO_3");
    }
    void PlayComboSound4()
    {
        AudioManager.Instance.PlaySFX("PLAYER_COMBO_4");
    }

    //
    public float CurrentHP
    {
        get { return currentHP; }
        set
        {
            currentHP = value;

            if (isDead == false)
            {
                // ���� HP�� �ִ� ü�º��� ������ �ִ� ü������ ����
                if (currentHP > MaxHP)
                {
                    currentHP = MaxHP;
                }

                // ���� HP�� 0���ϸ� 0���� �����ϰ� ���� ���·� ����
                if (currentHP <= 0)
                {
                    currentHP = 0;
                }

                if (MaxHP > 0)
                {
                    float ratio = currentHP / MaxHP;
                    UIManager.Instance.UpdatePlayerHPUI(ratio);
                }
            }
        }
    }

    public float CurrentMP
    {
        get { return currentMP; }
        set
        {
            currentMP = value;
            if (currentMP > MaxMP)
            {
                currentMP = MaxMP;
            }

            if (currentMP <= 0)
            {
                currentMP = 0;
            }

            if (MaxMP >= 0)
            {
                float ratio = currentMP / MaxMP;
                UIManager.Instance.UpdatePlayerMPUI(ratio);
            }
        }
    }

    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }
    public float MaxMP
    {
        get { return maxMP; }
        set { maxMP = value; }
    }
    public float CriticalChance
    {
        get { return criticalChance; }
        set { criticalChance = value; }
    }
    public float CriticalDamage
    {
        get { return criticalDamage; }
        set { criticalDamage = value; }
    }

    public Rigidbody PlayerRigidbody
    {
        get { return playerRigidbody; }
    }
}




