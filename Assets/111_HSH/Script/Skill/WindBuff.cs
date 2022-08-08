using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBuff : MonoBehaviour
{
    public BossMonster boss;
    public float bossDeamageReturn;
    public float bossDefenseReturn;

    void DragonBuff() //���� �ڹ���
    {
        bossDeamageReturn = boss.redDragonDamage;
        bossDefenseReturn = boss.redDragonDefense;

        boss.redDragonDamage  *= 1.1f; //���� ���ݷ� 10% ����
        boss.redDragonDefense *= 1.1f; //���� ���� 10% ����

        StartCoroutine(CoolTime());
    }
    IEnumerator CoolTime() //8�� �� ���� �����
    {
        yield return new WaitForSeconds(8);
        boss.redDragonDamage = bossDeamageReturn;  //���� ���ݷ� ������� ������
        boss.redDragonDefense = bossDefenseReturn; //���� ���� ������� ������
    }
}