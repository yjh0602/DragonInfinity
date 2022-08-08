using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackArea : MonoBehaviour
{
    public BoxCollider attackCollider;
    public float mul;

    [SerializeField] Player player;

    //Monster monster

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Monster")
        {
            player.DecideDamage(other, mul);
        }

        if(other.gameObject.tag == "Boss Monster")
        {
            player.DecideDamageForBoss(other, mul);
        }
    }
}
