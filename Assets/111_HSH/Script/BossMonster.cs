using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMonster : MonoBehaviour
{
    public Transform target;            //플레이어

    public GameObject windBuffEffect;         //회오리 스킬
    public GameObject screamEffect;           //보스 버프

    [SerializeField] float currentHP;  //현재 HP      
    [SerializeField] float maxHP;      // 최대 HP

    public float redDragonDamage;       //드래곤 공격력
    public float redDragonDefense;      //드래곤 방어력
    public float redDragonSpeed;        //드래곤 이동속도
    public int meteorNumber;            //메테오 수

    private bool isDead;                //보스 죽었는지 확인
    private bool isSpawn;               //조우 연출 제어
    private bool isWalk;                //드래곤 움직임 제어
    private bool isSkill;               //드래곤 스킬 제어

    private float distanceFromPlayer;           //플레이어와의 거리
    private float dragonSpawnFlyTime;           //드래곤 등장 비행시간

    [SerializeField] float breathRange;         //브레스 사거리
    [SerializeField] float breathCooltime;      //브레스 쿨타임
    bool isBreathReady;                         //브레스 쿨타임 여부

    [SerializeField] float flyBreathRange;      //공중 브레스사거리
    [SerializeField] float flyBreathCooltime;   //공중 브레스쿨타임
    bool isFlyBreathReady;                      //공중 브레스쿨타임 여부

    [SerializeField] float windRange;        //디펜스 사거리
    [SerializeField] float windCooltime;     //디펜스 쿨타임
    bool isWindReady;                       //디펜스 공중 브레스쿨타임 여부

    [SerializeField] float basicAttackRange;    //기본 공격 사거리
    [SerializeField] float basicAttackCoolTime; //기본 공격 쿨타임
    bool isBasicAttackReady;                    //기본 공격 쿨타임 여부

    [SerializeField] float clawAttackRange;     //돌진 공격 사거리
    [SerializeField] float clawAttackCooltime;  //돌진 공격 쿨타임
    bool isClawAttackReady;                     //돌진 공격 쿨타임 여부

    [SerializeField] float screamRange;          //무력화 사거리
    [SerializeField] float screamCoolTime;       //무력화 쿨타임
    bool isScreamReady;                          //무력화 쿨타임 여부

    Animator dragonAnimator;           //애니메이션
    Rigidbody dragonRigidbody;         //드래곤 리지드
    BoxCollider dragonCollider;        //드래곤 박스콜라이더

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
        // 스킬 사용중
        if (target == null || isSkill || isSpawn || isDead)
        {
            return;
        }

        // 페이즈 체크
        PhaseCheck();

        // 추적 필요가 있는지 검사
        MoveDragon();
        RotateDragon();

        // 추적중이면 스킬 사용 x
        if (isWalk == false)
        {
            DragonSkill();
        }
    }

    // ================= Spawn Motion =================
    void SpawnTakeOff() //보스 등장 씬, 준비 동작
    {
        isSpawn = true;
        dragonAnimator.SetTrigger("doSpawnTakeOff");
        StartCoroutine(SpawnMoveUP());
    }
    IEnumerator SpawnMoveUP() // 보스 등장 씬, 높이 올라감
    {
        float time = 0;

        while (time < 3.0f)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.up * 9.0f * Time.deltaTime);
            yield return null;
        }
    }

    void SpawnTakeOffOut() //보스 등장씬, 나는 중
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);
        dragonAnimator.SetBool("isSpawnFly", true);
        StartCoroutine(SpawnMoveRotate());
        StartCoroutine(SpawnMoveFront());
        StartCoroutine(SpawnFlyTime());
    }
    
    IEnumerator SpawnMoveRotate() // 보스 등장 씬, 공중에서 회전
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
    IEnumerator SpawnMoveFront() //보스 등장 씬, 날면서 앞으로 이동
    {
        float time = 0;

        while(time < dragonSpawnFlyTime)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.forward * 40.0f * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator SpawnFlyTime() // 보스 등장 씬, 공중에 있는 시간
    {
        yield return new WaitForSeconds(dragonSpawnFlyTime);
        dragonAnimator.SetBool("isSpawnFly", false);
        dragonAnimator.SetTrigger("doSpawnLand");
        StartCoroutine(SpawnMoveLand());
    }
    IEnumerator SpawnMoveLand() //보스 등장 씬, 올라간 만큼 내려옴, 사운드 : BossMove - Boss
    {
        float time = 0;

        while (time < 3.0f)
        {
            time += Time.deltaTime;
            transform.Translate(Vector3.down * 9.0f * Time.deltaTime);
            yield return null;
        }
    }
    void SpawnActionScream() // 보스 등장 씬, 포효
    {
        dragonAnimator.SetTrigger("doSpawnScream");
    }
    void SpawnOut()
    {
        isSpawn = false;
    }

    // ================= Dragon Move Function =================
    void RotateDragon() //플레이어를 향해 방향 전환
    {
        Vector3 dir = target.position - transform.position;
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation(dir), 5 * Time.deltaTime);
    }
    void MoveDragon()
    {
        //플레이어와의 거리
        distanceFromPlayer = (target.position - transform.position).magnitude;
        // 보스와 플레어이와의 거리가 10이상 일때 추격
        if (distanceFromPlayer >= basicAttackRange)
        {
            isWalk = true;
            dragonAnimator.SetBool("isWalk", isWalk);
            transform.Translate(Vector3.forward * redDragonSpeed * Time.deltaTime, Space.Self);
        }

        // 거리가 10미만 일때 멈춤
        if (distanceFromPlayer < basicAttackRange)
        {
            isWalk = false;
            dragonAnimator.SetBool("isWalk", isWalk);
        }
    } //플레이어를 향해 이동, 사운드 : BossMove - BossWalk

    // ================= Dragon Attack Function =================
    void SkillOut() //스킬 종료
    {
        isSkill = false;
    }
    void StartMeteorGeneration() //메테오 생성 시작
    {
        meteorNumber = 3;
        MeteorGeneration meteorGeneration = GetComponent<MeteorGeneration>();
        meteorGeneration.enabled = true;
    }

    void PhaseCheck() //보스 몬스터 페이즈 체크
    {
        float ratio = (CurrentHP / MaxHP) * 100;

        if (ratio <= 60 && phase == 1) //페이즈2, 3개 패턴 추가 등장
        {
            ++phase;
        }
        if (ratio <= 30 && phase == 2) //페이즈3, 운석 추가 생성
        {
            ++phase;
            meteorNumber += 3;
        }
    }
    void DragonSkill() //보스 몬스터 패턴
    {
        // 함수에 들어왔으면 스킬을 사용
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
        //패턴 확인
        return randomPattern[pattern];
    } //패턴 생성

    IEnumerator BreathCoolTime(float _breathCooltime) //브레스 쿨타임
    {
        isBreathReady = false;
        yield return new WaitForSeconds(_breathCooltime);
        isBreathReady = true;
    }
    IEnumerator FlyBreathCoolTime(float _flyBreathCooltime) //공중 브레스 쿨타임
    {
        isFlyBreathReady = false;
        yield return new WaitForSeconds(_flyBreathCooltime);
        isFlyBreathReady = true;
    }
    IEnumerator BasicCoolTime(float _basicAttackCoolTime) //기본 공격 쿨타임
    {
        isBasicAttackReady = false;
        yield return new WaitForSeconds(_basicAttackCoolTime);
        isBasicAttackReady = true;
    }
    IEnumerator WindBuffCoolTime(float _defenseCooltime) //방어 쿨타임
    {
        isWindReady = false;
        yield return new WaitForSeconds(_defenseCooltime);
        isWindReady = true;
    }
    IEnumerator ClawAttackCoolTime(float _clawAttackCooltime) //돌진 쿨타임
    {
        isClawAttackReady = false;      
        yield return new WaitForSeconds(_clawAttackCooltime);
        isClawAttackReady = true;
    }
    IEnumerator ScreamCoolTime(float _screamCoolTime) //무력화 쿨타임
    {
        isScreamReady = false;
        yield return new WaitForSeconds(_screamCoolTime);
        isScreamReady = true;
    }
    void Breath() //브래스 사운드 : BossSkill - BossFlame
    {
        StartCoroutine(BreathCoolTime(breathCooltime));
        isSkill = true;
        dragonAnimator.SetTrigger("doFire"); 
        
    }
    void BasicAttack() //기본공격, 사운드 : BossBasicAttack
    {
        StartCoroutine(BasicCoolTime(basicAttackCoolTime));
        isSkill = true;
        dragonAnimator.SetTrigger("doBasicAttack");
    }
    void WindBuff() //방어패턴 사운드 : BossSkill - BossDefense
    {
        //Defense 사운드
        StartCoroutine(WindBuffCoolTime(windCooltime));
        StartCoroutine(WindEffect());
        isSkill = true;
        dragonAnimator.SetTrigger("doWindBuff");
    }
    IEnumerator WindEffect() //방어 효과 지속시간
    {
        windBuffEffect.SetActive(true);
        yield return new WaitForSeconds(8.0f);
        windBuffEffect.SetActive(false);
    }
    void ClawAttack() //돌진 , 돌진 사운드 : BossSkill - BossClawAttack 돌진 후 바닥 찍을 때 사운드 : BossSkill - BossClawAttackStep
    {
        StartCoroutine(ClawAttackCoolTime(clawAttackCooltime));
        isSkill = true;
        dragonAnimator.SetTrigger("doClawAttack");
    }
    void Scream() //스크림, 보스 버프 사운드 : BossSkill - BossScream
    {
        StartCoroutine(ScreamCoolTime(screamCoolTime));
        StartCoroutine(ScreamBuff());
        isSkill = true;
        dragonAnimator.SetTrigger("doScream");
    }
    IEnumerator ScreamBuff() //버프 지속시간
    {
        screamEffect.SetActive(true);
        yield return new WaitForSeconds(8.0f);
        screamEffect.SetActive(false);
    }
    void FlyBreath() //공중 브레스, 사운드 BossSKill - BossFlyFlameAttack (날때), BossFlame(브레스 쏠 때)
    {
        isSkill = true;
        StartCoroutine(FlyBreathCoolTime(flyBreathCooltime));
        dragonAnimator.SetTrigger("doFly");
        dragonAnimator.SetBool("isFlyFlameAttack", true); 
    } 
    void DrangonFlyEnd() //공중에서 내려옴
    {
        dragonAnimator.SetBool("isFlyFlameAttack", false);
    }

    // ================= Dragon Hit Function =================
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player Attack") //플레이어와 충돌 시 데미지입음
        {
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage() // 사운드. BossHit
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
    
    void Die() //보스 죽음
    {
        isDead = true;
        dragonAnimator.SetTrigger("doDie");
        dragonCollider.enabled = false;

        MeteorGeneration meteorGeneration = GetComponent<MeteorGeneration>();
        meteorGeneration.enabled = false;

        StartCoroutine(GameManager.Instance.GameClear());
    }

    // 대미지 결정 함수: 보스 몬스터의 크리티컬 확률 20% 고정, 크리티컬 대미지 200% 고정
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

    // 최종 데미지
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
                //현재 HP가 최대 체력보다 많으면 최대 체력으로 고정
                if (currentHP >= MaxHP)
                {
                    currentHP = MaxHP;
                }


                //현재 HP가 0이하면 0으로 고정하고 죽은 상태로 변경
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
