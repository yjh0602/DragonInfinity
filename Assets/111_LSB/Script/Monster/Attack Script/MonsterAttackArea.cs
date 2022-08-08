using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== MONSTER ATTACK AREA CLASS ======================================
// 투사체를 제외한 일반/네임드 몬스터의 공격 판정은 MonsterAttackArea 스크립트에 의해 이루어짐
// 부착된 게임 오브젝트와의 트리거 여부를 판단하고, 입력해둔 계수에 따라 데미지를 줌
// =====================================================================================

public class MonsterAttackArea : MonoBehaviour
{
    [SerializeField] Monster ownerMonster;
    [SerializeField] float multiplication;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ownerMonster.DecideDamage(other, multiplication);
        }
    }

    public Monster OwnerMonster
    {
        get { return ownerMonster; }
        set { ownerMonster = value; }
    }
}
