using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// =================== BLACK HOLE CLASS : USE IN SKELETON KING CLASS ===================
// �÷��̾ ���� �ȿ� ������, ��Ȧ�� ���ؼ� �����̰� ��.
// �÷��̾ NavMeshAgent�� ����ϱ� ������ Destination�� �ֱ������� �ٲ��ִ� ������� ����
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
