using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealTower : MonoBehaviour
{
    public GameObject healTowerEffect;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (healTowerEffect != null)
            {
                AudioManager.Instance.PlaySFX("HEAL_TOWER");
                Player player = other.GetComponent<Player>();
                player.CurrentHP += player.MaxHP * 0.3f;

                Destroy(healTowerEffect);
            }
        }
    }
}
