using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== MONSTER ATTACK AREA CLASS ======================================
// ����ü�� ������ �Ϲ�/���ӵ� ������ ���� ������ MonsterAttackArea ��ũ��Ʈ�� ���� �̷����
// ������ ���� ������Ʈ���� Ʈ���� ���θ� �Ǵ��ϰ�, �Է��ص� ����� ���� �������� ��
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
