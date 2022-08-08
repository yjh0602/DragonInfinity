using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackArea : MonoBehaviour
{
    [SerializeField] BossMonster bossMonster;
    [SerializeField] float multiplication;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bossMonster.DecideFinalDamage(other, multiplication);
        }
    }

    public BossMonster BossMonster
    {
        get { return bossMonster; }
        set { bossMonster = value; }
    }
}
