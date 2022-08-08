using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// =================== BLACK HOLE CLASS : USE IN SKELETON KING CLASS ===================
// 플레이어가 범위 안에 있으면, 블랙홀을 향해서 움직이게 함.
// 플레이어가 NavMeshAgent를 사용하기 때문에 Destination을 주기적으로 바꿔주는 방식으로 구현
// =====================================================================================

public class BlackHole : MonoBehaviour
{
    [SerializeField] GameObject skillExplosionEffect;
    private Monster ownerMonster;
    private NavMeshAgent targetNavmeshAgent = null;

    private void Start()
    {
        StartCoroutine(PlayerInBlackHole());
        StartCoroutine(OnBlackHole());
    }

    IEnumerator PlayerInBlackHole()
    {
        while (true)
        {
            if (targetNavmeshAgent != null)
            {
                targetNavmeshAgent.SetDestination(transform.position);
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (targetNavmeshAgent == null)
            {
                targetNavmeshAgent = other.GetComponent<NavMeshAgent>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            targetNavmeshAgent = null;
        }
    }

    IEnumerator OnBlackHole()
    {
        PlaySkeletonKingGravitySound();
        yield return new WaitForSeconds(2f);

        PlaySkeletonKingGravitySound();
        yield return new WaitForSeconds(2f);

        PlaySkeletonKingGravitySound();
        yield return new WaitForSeconds(1f);

        PlaySkeletonKingExplosionSound();
        GameObject explosion = Instantiate(skillExplosionEffect, transform.position, transform.rotation);
        explosion.GetComponent<MonsterAttackArea>().OwnerMonster = OwnerMonster;

        yield return new WaitForSeconds(1f);
        Destroy(explosion);
        Destroy(gameObject);
    }

    void PlaySkeletonKingGravitySound()
    {
        AudioManager.Instance.PlaySFX("SKELETON_KING_GRAVITY");
    }
    void PlaySkeletonKingExplosionSound()
    {
        AudioManager.Instance.PlaySFX("SKELETON_KING_EXPLOSION");
    }

    public Monster OwnerMonster
    {
        get { return ownerMonster; }
        set { ownerMonster = value; }

    }

}
