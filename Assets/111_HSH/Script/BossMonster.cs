using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMonster : MonoBehaviour
{
    public Transform target;            //�÷��̾�

    public GameObject windBuffEffect;         //ȸ���� ��ų
    public GameObject screamEffect;           //���� ����

    [SerializeField] float currentHP;  //���� HP      
    [SerializeField] float maxHP;      // �ִ� HP

    public float redDragonDamage;       //�巡�� ���ݷ�
    public float redDragonDefense;      //�巡�� ����
    public float redDragonSpeed;        //�巡�� �̵��ӵ�
    public int meteorNumber;            //���׿� ��

    private bool isDead;                //���� �׾����� Ȯ��
    private bool isSpawn;               //���� ���� ����
    private bool isWalk;                //�巡�� ������ ����
    private bool isSkill;               //�巡�� ��ų ����

    private float distanceFromPlayer;           //�÷��̾���� �Ÿ�
    private float dragonSpawnFlyTime;           //�巡�� ���� ����ð�

    [SerializeField] float breathRange;         //�극�� ��Ÿ�
    [SerializeField] float breathCooltime;      //�극�� ��Ÿ��
    bool isBreathReady;                         //�극�� ��Ÿ�� ����

    [SerializeField] float flyBreathRange;      //���� �극����Ÿ�
    [SerializeField] float flyBreathCooltime;   //���� �극����Ÿ��
    bool isFlyBreathReady;                      //���� �극����Ÿ�� ����

    [SerializeField] float windRange;        //���潺 ��Ÿ�
    [SerializeField] float windCooltime;     //���潺 ��Ÿ��
    bool isWindReady;                       //���潺 ���� �극����Ÿ�� ����

    [SerializeField] float basicAttackRange;    //�⺻ ���� ��Ÿ�
    [SerializeField] float basicAttackCoolTime; //�⺻ ���� ��Ÿ��
    bool isBasicAttackReady;                    //�⺻ ���� ��Ÿ�� ����

    [SerializeField] float clawAttackRange;     //���� ���� ��Ÿ�
    [SerializeField] float clawAttackCooltime;  //���� ���� ��Ÿ��
    bool isClawAttackReady;                     //���� ���� ��Ÿ�� ����

    [SerializeField] float screamRange;          //����ȭ ��Ÿ�
    [SerializeField] float screamCoolTime;       //����ȭ ��Ÿ��
    bool isScreamReady;                          //����ȭ ��Ÿ�� ����

    Animator dragonAnimator;           //�ִϸ��̼�
    Rigidbody dragonRigidbody;         //�巡�� ������
    BoxCollider dragonCollider;        //�巡�� �ڽ��ݶ��̴�

    private int phase = 1;

    private void Awake()
    {
        isDead = false;
        isSpawn = false;
        isWalk = false;
        isSkill = false;

        isBreathReady = true;
        isFlyBreathReady = true;
        isWindReady = true;
        isBasicAttackReady = true;
        isClawAttackReady = true;
        isScreamReady = true;

        dragonSpawnFlyTime = 6.0f;

        dragonAnimator = GetComponent<Animator>();
        dragonRigidbody = GetComponent<Rigidbody>();
        dragonCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        CurrentHP = MaxHP;
        meteorNumber = 0;
        SpawnTakeOff();
    }

    private void Update()
    {
        // ��ų �����
        if (target == null || isSkill || isSpawn || isDead)
        {
            return;
        }

        // ������ üũ
        PhaseCheck();

        // ���� �ʿ䰡 �ִ��� �˻�
        MoveDragon();
        RotateDragon();

        // �������̸� ��ų ��� x
        if (isWalk == false)
        {
            DragonSkill();
        }
    }

    // ================= Spawn Motion =================
    void SpawnTakeOff() //���� ���� ��, �غ� ����
    {
        isSpawn = true;
        dragonAnimator.SetTrigger("doSpawnTakeOff");
        StartCoroutine(SpawnMoveUP());
    }
    IEnumerator SpawnMoveUP() // ���� ���� ��, ���� �ö�
    {
        float time = 0;

        while (time < 3.0f)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.up * 9.0f * Time.deltaTime);
            yield return null;
        }
    }

    void SpawnTakeOffOut() //���� �����, ���� ��
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);
        dragonAnimator.SetBool("isSpawnFly", true);
        StartCoroutine(SpawnMoveRotate());
        StartCoroutine(SpawnMoveFront());
        StartCoroutine(SpawnFlyTime());
    }
    
    IEnumerator SpawnMoveRotate() // ���� ���� ��, ���߿��� ȸ��
    {
        float time = 0;

        while (time < dragonSpawnFlyTime)
        {
            time += Time.deltaTime;
            transform.Rotate(Vector3.up * -(360/ dragonSpawnFlyTime) * Time.deltaTime, Space.Self);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    IEnumerator SpawnMoveFront() //���� ���� ��, ���鼭 ������ �̵�
    {
        float time = 0;

        while(time < dragonSpawnFlyTime)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.forward * 40.0f * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator SpawnFlyTime() // ���� ���� ��, ���߿� �ִ� �ð�
    {
        yield return new WaitForSeconds(dragonSpawnFlyTime);
        dragonAnimator.SetBool("isSpawnFly", false);
        dragonAnimator.SetTrigger("doSpawnLand");
        StartCoroutine(SpawnMoveLand());
    }
    IEnumerator SpawnMoveLand() //���� ���� ��, �ö� ��ŭ ������, ���� : BossMove - Boss
    {
        float time = 0;

        while (time < 3.0f)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.down * 9.0f * Time.deltaTime);
            yield return null;
        }
    }
    void SpawnActionScream() // ���� ���� ��, ��ȿ
    {
        dragonAnimator.SetTrigger("doSpawnScream");
    }
    void SpawnOut()
    {
        isSpawn = false;
    }

    // ================= Dragon Move Function =================
    void RotateDragon() //�÷��̾ ���� ���� ��ȯ
    {
        Vector3 dir = target.position - transform.position;
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation(dir), 5 * Time.deltaTime);
    }
    void MoveDragon()
    {
        //�÷��̾���� �Ÿ�
        distanceFromPlayer = (target.position - transform.position).magnitude;
        // ������ �÷����̿��� �Ÿ��� 10�̻� �϶� �߰�
        if (distanceFromPlayer >= basicAttackRange)
        {
            isWalk = true;
            dragonAnimator.SetBool("isWalk", isWalk);
            transform.Translate(Vector3.forward * redDragonSpeed * Time.deltaTime, Space.Self);
        }

        // �Ÿ��� 10�̸� �϶� ����
        if (distanceFromPlayer < basicAttackRange)
        {
            isWalk = false;
            dragonAnimator.SetBool("isWalk", isWalk);
        }
    } //�÷��̾ ���� �̵�, ���� : BossMove - BossWalk

    // ================= Dragon Attack Function =================
    void SkillOut() //��ų ����
    {
        isSkill = false;
    }
    void StartMeteorGeneration() //���׿� ���� ����
    {
        meteorNumber = 3;
        MeteorGeneration meteorGeneration = GetComponent<MeteorGeneration>();
        meteorGeneration.enabled = true;
    }

    void PhaseCheck() //���� ���� ������ üũ
    {
        float ratio = (CurrentHP / MaxHP) * 100;

        if (ratio <= 60 && phase == 1) //������2, 3�� ���� �߰� ����
        {
            ++phase;
        }
        if (ratio <= 30 && phase == 2) //������3, � �߰� ����
        {
            ++phase;
            meteorNumber += 3;
        }
    }
    void DragonSkill() //���� ���� ����
    {
        // �Լ��� �������� ��ų�� ���
        isSkill = true;

        switch (RandomPattern())
        {
            case -1:
                {
                    isSkill = false;
                    return;
                }
            case 0:
                {
                    BasicAttack();
                    return;
                }
            case 1:
                {
                    Scream();
                    return;
                }
            case 2:
                {
                    ClawAttack();
                    return;
                }
            case 3:
                {
                    WindBuff();
                    return;
                }
            case 4:
                {
                    Breath();
                    return;
                }
            case 5:
                {
                    FlyBreath();
                    return;
                }
        }
    }

    public int RandomPattern()
    {
        List<int> randomPattern = new List<int>();

        if(distanceFromPlayer <= basicAttackRange && isBasicAttackReady == true)
        {
            randomPattern.Add(0);
        }
        if (distanceFromPlayer <= screamRange && isScreamReady == true)
        {
            randomPattern.Add(1);
        }
        if (distanceFromPlayer <= clawAttackRange && isClawAttackReady == true)
        {
            randomPattern.Add(2);
        }
        if (distanceFromPlayer <= windRange && isWindReady == true && phase >= 2)
        {
            randomPattern.Add(3);
        }
        if (distanceFromPlayer <= breathRange && isBreathReady == true && phase >= 2)
        {
            randomPattern.Add(4);
        }
        if (distanceFromPlayer <= flyBreathRange && isFlyBreathReady == true && phase >= 2)
        {
            randomPattern.Add(5);
        }
        if(randomPattern.Count == 0)
        {
            return -1;
        }
        int pattern = Random.Range(0, randomPattern.Count);
        //���� Ȯ��
        return randomPattern[pattern];
    } //���� ����

    IEnumerator BreathCoolTime(float _breathCooltime) //�극�� ��Ÿ��
    {
        isBreathReady = false;
        yield return new WaitForSeconds(_breathCooltime);
        isBreathReady = true;
    }
    IEnumerator FlyBreathCoolTime(float _flyBreathCooltime) //���� �극�� ��Ÿ��
    {
        isFlyBreathReady = false;
        yield return new WaitForSeconds(_flyBreathCooltime);
        isFlyBreathReady = true;
    }
    IEnumerator BasicCoolTime(float _basicAttackCoolTime) //�⺻ ���� ��Ÿ��
    {
        isBasicAttackReady = false;
        yield return new WaitForSeconds(_basicAttackCoolTime);
        isBasicAttackReady = true;
    }
    IEnumerator WindBuffCoolTime(float _defenseCooltime) //��� ��Ÿ��
    {
        isWindReady = false;
        yield return new WaitForSeconds(_defenseCooltime);
        isWindReady = true;
    }
    IEnumerator ClawAttackCoolTime(float _clawAttackCooltime) //���� ��Ÿ��
    {
        isClawAttackReady = false;      
        yield return new WaitForSeconds(_clawAttackCooltime);
        isClawAttackReady = true;
    }
    IEnumerator ScreamCoolTime(float _screamCoolTime) //����ȭ ��Ÿ��
    {
        isScreamReady = false;
        yield return new WaitForSeconds(_screamCoolTime);
        isScreamReady = true;
    }
    void Breath() //�귡�� ���� : BossSkill - BossFlame
    {
        StartCoroutine(BreathCoolTime(breathCooltime));
        isSkill = true;
        dragonAnimator.SetTrigger("doFire"); 
        
    }
    void BasicAttack() //�⺻����, ���� : BossBasicAttack
    {
        StartCoroutine(BasicCoolTime(basicAttackCoolTime));
        isSkill = true;
        dragonAnimator.SetTrigger("doBasicAttack");
    }
    void WindBuff() //������� ���� : BossSkill - BossDefense
    {
        //Defense ����
        StartCoroutine(WindBuffCoolTime(windCooltime));
        StartCoroutine(WindEffect());
        isSkill = true;
        dragonAnimator.SetTrigger("doWindBuff");
    }
    IEnumerator WindEffect() //��� ȿ�� ���ӽð�
    {
        windBuffEffect.SetActive(true);
        yield return new WaitForSeconds(8.0f);
        windBuffEffect.SetActive(false);
    }
    void ClawAttack() //���� , ���� ���� : BossSkill - BossClawAttack ���� �� �ٴ� ���� �� ���� : BossSkill - BossClawAttackStep
    {
        StartCoroutine(ClawAttackCoolTime(clawAttackCooltime));
        isSkill = true;
        dragonAnimator.SetTrigger("doClawAttack");
    }
    void Scream() //��ũ��, ���� ���� ���� : BossSkill - BossScream
    {
        StartCoroutine(ScreamCoolTime(screamCoolTime));
        StartCoroutine(ScreamBuff());
        isSkill = true;
        dragonAnimator.SetTrigger("doScream");
    }
    IEnumerator ScreamBuff() //���� ���ӽð�
    {
        screamEffect.SetActive(true);
        yield return new WaitForSeconds(8.0f);
        screamEffect.SetActive(false);
    }
    void FlyBreath() //���� �극��, ���� BossSKill - BossFlyFlameAttack (����), BossFlame(�극�� �� ��)
    {
        isSkill = true;
        StartCoroutine(FlyBreathCoolTime(flyBreathCooltime));
        dragonAnimator.SetTrigger("doFly");
        dragonAnimator.SetBool("isFlyFlameAttack", true); 
    } 
    void DrangonFlyEnd() //���߿��� ������
    {
        dragonAnimator.SetBool("isFlyFlameAttack", false);
    }

    // ================= Dragon Hit Function =================
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player Attack") //�÷��̾�� �浹 �� ����������
        {
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage() // ����. BossHit
    {
        yield return new WaitForSeconds(0.1f);

        if (CurrentHP > 0)
        {
            PlayHitSound();
        }

        else
        {
            Die();
        }
    }
    
    void Die() //���� ����
    {
        isDead = true;
        dragonAnimator.SetTrigger("doDie");
        dragonCollider.enabled = false;

        MeteorGeneration meteorGeneration = GetComponent<MeteorGeneration>();
        meteorGeneration.enabled = false;

        StartCoroutine(GameManager.Instance.GameClear());
    }

    // ����� ���� �Լ�: ���� ������ ũ��Ƽ�� Ȯ�� 20% ����, ũ��Ƽ�� ����� 200% ����
    bool DecideCritical()
    {
        int critical = Random.Range(0, 100);

        if (critical < 20)
            return true;

        else
            return false;
    }
    void OnCriticalDamage(Player _playerScript, float _multiplication)
    {
        float finalDamage = redDragonDamage - _playerScript.defense;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        _playerScript.CurrentHP -= finalDamage * _multiplication * 2f;
    }
    void OffCriticalDamage(Player _playerScript, float _multiplication)
    {
        float finalDamage = redDragonDamage - _playerScript.defense;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        _playerScript.CurrentHP -= finalDamage * _multiplication;
    }

    // ���� ������
    public void DecideFinalDamage(Collider other, float _multiplication)
    {
        Player playerScript = other.GetComponent<Player>();
        bool isCritical = DecideCritical();

        switch (isCritical)
        {
            case true:
                OnCriticalDamage(playerScript, _multiplication);
                break;
            case false:
                OffCriticalDamage(playerScript, _multiplication);
                break;
        }
    }

    // Sound
    void PlayWalkSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_WALK");
    }
    void PlayBasicAttackSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_BASIC_ATTACK");
    }
    void PlayHitSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_HIT");
    }
    void PlaySpawnGroundSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_SPAWN_GROUND");
    }
    void PlayBreathSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_BREATH");
    }
    void PlayFlyBreathSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_FLY_BREATH");
    }
    void PlayClawSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_CLAW");
    }
    void PlayClawStepSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_CLAW_STEP");
    }
    void PlayScreamSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_SCREAM");
    }
    void PlayWindSound()
    {
        AudioManager.Instance.PlaySFX("BOSS_WIND");
    }

    public float CurrentHP
    {
        get { return currentHP; }
        set
        {
            currentHP = value;

            if (isDead == false)
            {
                //���� HP�� �ִ� ü�º��� ������ �ִ� ü������ ����
                if (currentHP >= MaxHP)
                {
                    currentHP = MaxHP;
                }


                //���� HP�� 0���ϸ� 0���� �����ϰ� ���� ���·� ����
                if (currentHP <= 0)
                {
                    currentHP = 0;
                }

                float ratio = currentHP / MaxHP;
                UIManager.Instance.UpdateBossHPUI(ratio);
            }
        }
    }
    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }
}
