using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== MONSTER PROJECTILE CLASS ========================================
// 일반/네임드 몬스터의 투사체 판정은 MonsterProjectile 스크립트에 의해 이루어짐
// 부착된 게임 오브젝트와의 트리거 여부를 판단하고, 입력해둔 계수에 따라 데미지를 줌
// MONSTER_PROJECTILE_TYPE 설정을 통해 투사체마다 다른 속도를 줄 수 있도록 함.
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
