using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDamage : MonoBehaviour
{   
    [SerializeField] BossMonster bossMonster;
    [SerializeField] float multiplication;
    private bool isIn;
    private IEnumerator windCoroutine;

    private void Awake()
    {
        isIn = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isIn == true)
                return;

            isIn = true;
            windCoroutine = WindAttack(other);
            StartCoroutine(windCoroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Wind Exit");
            isIn = false;
            StopCoroutine(windCoroutine);
        }
    }

    IEnumerator WindAttack(Collider other) //���� �ȿ� ���� ��� �ִ� 3ȸ ���� ���� ������ ����
    {
        if (isIn == true)
        {
            bossMonster.DecideFinalDamage(other, multiplication);
        }
        yield return new WaitForSeconds(2.0f);

        if (isIn == true)
        {
            bossMonster.DecideFinalDamage(other, multiplication);
        }
        yield return new WaitForSeconds(2.0f);

        if (isIn == true)
        {
            bossMonster.DecideFinalDamage(other, multiplication);
        }
        yield return new WaitForSeconds(2.0f);

        isIn = false;
    }
}
