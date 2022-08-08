using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBuff : MonoBehaviour
{
    public BossMonster boss;
    public float bossDeamageReturn;
    public float bossDefenseReturn;

    void DragonBuff() //보스 자버프
    {
        bossDeamageReturn = boss.redDragonDamage;
        bossDefenseReturn = boss.redDragonDefense;

        boss.redDragonDamage  *= 1.1f; //보스 공격력 10% 증가
        boss.redDragonDefense *= 1.1f; //보스 방어력 10% 증가

        StartCoroutine(CoolTime());
    }
    IEnumerator CoolTime() //8초 후 버프 사라짐
    {
        yield return new WaitForSeconds(8);
        boss.redDragonDamage = bossDeamageReturn;  //보스 공격력 원래대로 돌려줌
        boss.redDragonDefense = bossDefenseReturn; //보스 방어력 원래대로 돌려줌
    }
}