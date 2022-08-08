using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    BoxCollider boxCollider; //박스 콜라이더
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        StartCoroutine(WaitForFall());
    }
    IEnumerator WaitForFall()
    {
        // 낙하 시간 동안 대기
        yield return new WaitForSeconds(1f);
        // 낙하시 콜라이더 on
        boxCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        //운석 삭제
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other) //플레이어와 운석 충돌 시 -hp
    {
        if (other.gameObject.tag == "Player")
        {
            if (MainStoryManager.Instance.bossMonster != null)
            {
                MainStoryManager.Instance.bossMonster.DecideFinalDamage(other, 0.5f);
            }
        }
    }
}
