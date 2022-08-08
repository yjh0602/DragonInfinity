using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    BoxCollider boxCollider; //�ڽ� �ݶ��̴�
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
        // ���� �ð� ���� ���
        yield return new WaitForSeconds(1f);
        // ���Ͻ� �ݶ��̴� on
        boxCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        //� ����
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other) //�÷��̾�� � �浹 �� -hp
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
