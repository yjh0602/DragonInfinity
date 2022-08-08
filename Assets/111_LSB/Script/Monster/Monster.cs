using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ============= MONSTER : ABSTRACT CLASS =============

public abstract class Monster : MonoBehaviour
{
    // ============= Member Variable =============
    [SerializeField] float maxHP;                   // 최대 체력
    [SerializeField] float currentHP;               // 현재 체력

    [SerializeField] float damage;                  // 대미지
    [SerializeField] float defense;                 // 방어력
    [SerializeField] float moveSpeed;               // 이동속도

    [SerializeField] Transform targetTransform;     // 추적 대상

    [SerializeField] Vector3 monsterHPBarOffset;    // 체력바 오프셋
    [SerializeField] GameObject monsterHPBar;       // 체력바

    public List<Monster> attachedList;              // 몬스터 리스트에 부착되어 있을 경우에 사용

    private bool isDead = false;                    // 생존 여부

    // ============= Abstract Function =============
    public abstract void ActionDecision();                      // 행동 결정
    public abstract void MoveDecision();                        // 움직임 결정
    public abstract void InitStatus();                          // 상태 초기화
    public abstract void SpawnIn();                             // 스폰 시작
    public abstract void SpawnOut();                            // 스폰 종료
    public abstract void Dead();                                // 사망
    public abstract void DropItem();                            // 아이템 드랍
    public abstract void FreezeVelocity();                      // 충돌 버그 방지
    public abstract IEnumerator OnDamage(Vector3 reactVector);  // 피격 함수

    // ============= Common Function =============

    // 체력 초기화
    public void InitHP()
    {
        CurrentHP = MaxHP;
    }

    // 체력바 생성
    public void CreateHPBar()
    {
        monsterHPBar = Instantiate(UIManager.Instance.monsterHPBarPrefab, UIManager.Instance.monsterHPBarCanvas.transform);

        monsterHPBar.GetComponent<MonsterHPBar>().TargetTransform = gameObject.transform;
        monsterHPBar.GetComponent<MonsterHPBar>().Offset = monsterHPBarOffset;
        monsterHPBar.SetActive(true);
    }

    // 추적 대상 설정
    public void SetTargetTransform()
    {
        targetTransform = GameManager.Instance.GamePlayer.transform;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsDead == true)
            return;

        // 피격 효과만, 데미지는 플레이어에서 처리
        if (other.tag == "Player Attack")
        {
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector));
        }
    }

    // ============= Damage Function =============
    // 몬스터의 크리티컬 확률 10%, 크리티컬 대미지 150% 고정
    bool DecideCritical()
    {
        int critical = Random.Range(0, 100);

        if (critical < 10)
            return true;

        else
            return false;
    }

    // On Critical
    void OnCriticalDamage(Player _playerScript, float _multiplication)
    {
        float finalDamage = damage - _playerScript.defense;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        _playerScript.CurrentHP -= finalDamage * _multiplication * 1.5f;
    }

    // Off Critical
    void OffCriticalDamage(Player _playerScript, float _multiplication)
    {
        float finalDamage = damage - _playerScript.defense;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        _playerScript.CurrentHP -= finalDamage * _multiplication;
    }

    // Damage
    public void DecideDamage(Collider other, float _multiplication)
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

    // 체력바 UI 제거
    private void OnDestroy()
    {
        if (attachedList != null)
        {
            attachedList.Remove(this);
        }
        Destroy(monsterHPBar);
    }

    // ============= Property =============
    public float CurrentHP
    {
        get { return currentHP; }
        set
        {
            currentHP = value;

            // 현재 HP가 최대 체력보다 많으면 최대 체력으로 고정
            if (currentHP > MaxHP)
            {
                currentHP = MaxHP;
            }

            // 현재 HP가 0이하면 0으로 고정
            if (currentHP <= 0 && isDead == false)
            {
                currentHP = 0;

                if(GameManager.Instance.gameMode == GAME_MODE.INFINITY_ROOM_MODE)
                {
                    ++GameManager.Instance.infinityRoomScore;
                }
            }

            // 체력바 업데이트
            if (monsterHPBar != null)
            {
                monsterHPBar.GetComponent<MonsterHPBar>().UpdateMonsterHP(MaxHP, currentHP);
            }
        }
    }

    // Sound
    public void PlayMonsterHitSound()
    {
        AudioManager.Instance.PlaySFX("MONSTER_HIT");
    }

    // ============= Property =============
    public float MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    public float Defense
    {
        get { return defense; }
        set { defense = value; }
    }
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public Transform TargetTransform
    {
        get { return targetTransform; }
        set { targetTransform = value; }
    }
}
