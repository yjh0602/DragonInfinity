using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== MONSTER PROJECTILE CLASS ========================================
// �Ϲ�/���ӵ� ������ ����ü ������ MonsterProjectile ��ũ��Ʈ�� ���� �̷����
// ������ ���� ������Ʈ���� Ʈ���� ���θ� �Ǵ��ϰ�, �Է��ص� ����� ���� �������� ��
// MONSTER_PROJECTILE_TYPE ������ ���� ����ü���� �ٸ� �ӵ��� �� �� �ֵ��� ��.
// =====================================================================================
public enum MONSTER_PROJECTILE_TYPE
{
    SKELETON_ARROW,
    SKELETON_MAGE_WATER,

    EVIL_MAGE_FIRE,
    GOBLIN_ARROW,

    SIZE
}

public class MonsterProjectile : MonoBehaviour
{
    [SerializeField] MONSTER_PROJECTILE_TYPE monsterProjectileType;
    [SerializeField] Monster ownerMonster;
    [SerializeField] float multiplication;
    private float monsterProjectileSpeed;

    private void Awake()
    {
        switch (monsterProjectileType)
        {
            case MONSTER_PROJECTILE_TYPE.SKELETON_ARROW:
                {
                    monsterProjectileSpeed = 20.0f;
                    break;
                }

            case MONSTER_PROJECTILE_TYPE.SKELETON_MAGE_WATER:
                {
                    monsterProjectileSpeed = 20.0f;

                    break;
                }

            case MONSTER_PROJECTILE_TYPE.EVIL_MAGE_FIRE:
                {
                    monsterProjectileSpeed = 20.0f;

                    break;
                }

            case MONSTER_PROJECTILE_TYPE.GOBLIN_ARROW:
                {
                    monsterProjectileSpeed = 20.0f;

                    break;
                }
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * monsterProjectileSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ownerMonster.DecideDamage(other, multiplication);
        }
    }
}
