using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ============= MONSTER : ABSTRACT CLASS =============

public abstract class Monster : MonoBehaviour
{
    // ============= Member Variable =============
    [SerializeField] float maxHP;                   // �ִ� ü��
    [SerializeField] float currentHP;               // ���� ü��

    [SerializeField] float damage;                  // �����
    [SerializeField] float defense;                 // ����
    [SerializeField] float moveSpeed;               // �̵��ӵ�

    [SerializeField] Transform targetTransform;     // ���� ���

    [SerializeField] Vector3 monsterHPBarOffset;    // ü�¹� ������
    [SerializeField] GameObject monsterHPBar;       // ü�¹�

    public List<Monster> attachedList;              // ���� ����Ʈ�� �����Ǿ� ���� ��쿡 ���

    private bool isDead = false;                    // ���� ����

    // ============= Abstract Function =============
    public abstract void ActionDecision();                      // �ൿ ����
    public abstract void MoveDecision();                        // ������ ����
    public abstract void InitStatus();                          // ���� �ʱ�ȭ
    public abstract void SpawnIn();                             // ���� ����
    public abstract void SpawnOut();                            // ���� ����
    public abstract void Dead();                                // ���
    public abstract void DropItem();                            // ������ ���
    public abstract void FreezeVelocity();                      // �浹 ���� ����
    public abstract IEnumerator OnDamage(Vector3 reactVector);  // �ǰ� �Լ�

    // ============= Common Function =============

    // ü�� �ʱ�ȭ
    public void InitHP()
    {
        CurrentHP = MaxHP;
    }

    // ü�¹� ����
    public void CreateHPBar()
    {
        monsterHPBar = Instantiate(UIManager.Instance.monsterHPBarPrefab, UIManager.Instance.monsterHPBarCanvas.transform);

        monsterHPBar.GetComponent<MonsterHPBar>().TargetTransform = gameObject.transform;
        monsterHPBar.GetComponent<MonsterHPBar>().Offset = monsterHPBarOffset;
        monsterHPBar.SetActive(true);
    }

    // ���� ��� ����
    public void SetTargetTransform()
    {
        targetTransform = GameManager.Instance.GamePlayer.transform;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (IsDead == true)
            return;

        // �ǰ� ȿ����, �������� �÷��̾�� ó��
        if (other.tag == "Player Attack")
        {
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVector));
        }
    }

    // ============= Damage Function =============
    // ������ ũ��Ƽ�� Ȯ�� 10%, ũ��Ƽ�� ����� 150% ����
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

    // ü�¹� UI ����
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

            // ���� HP�� �ִ� ü�º��� ������ �ִ� ü������ ����
            if (currentHP > MaxHP)
            {
                currentHP = MaxHP;
            }

            // ���� HP�� 0���ϸ� 0���� ����
            if (currentHP <= 0 && isDead == false)
            {
                currentHP = 0;

                if(GameManager.Instance.gameMode == GAME_MODE.INFINITY_ROOM_MODE)
                {
                    ++GameManager.Instance.infinityRoomScore;
                }
            }

            // ü�¹� ������Ʈ
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
