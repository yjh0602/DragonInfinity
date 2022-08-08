using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform bulletPosition;
    public GameObject bullet;

    [SerializeField] Transform transformGunBody;
    [SerializeField] LayerMask layerMask; //Ÿ�� ���̾�

    [SerializeField] float attackRange; //�ͷ������Ÿ�
    [SerializeField] float fireRate; //����ӵ�

    Transform targetTransform = null; //���ݴ��

    bool isChase = false;

    private void Awake()
    {
    }

    void Start()
    {
        StartCoroutine(ShotCoroutine());
        InvokeRepeating("SearchEnemy", 0f, 0.5f);
    }

    void Update()
    {
        // ���� �ȿ� Ÿ���� ���� ���
        if (targetTransform == null)
        {
            if(isChase == true)
            {
                isChase = false;
                AudioManager.Instance.PlaySFX("TURRET_OFF");
            }

            transformGunBody.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        }

        // ���� �ȿ� Ÿ���� ���� ���
        else
        {
            if (isChase == false)
            {
                isChase = true;
                AudioManager.Instance.PlaySFX("TURRET_ON");
            }

            transformGunBody.LookAt(targetTransform.transform.position);
        }
    }
    IEnumerator ShotCoroutine()
    {
        while (true)
        {
            if (targetTransform != null)
            {
                GameObject newBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);

                AudioManager.Instance.PlaySFX("BULLET_FIRE1");

                yield return new WaitForSeconds(fireRate);
            }

            yield return null;
        }
    }

    // ���� ������� ��ġ
   void SearchEnemy()
    {
        Collider[] targetColliderList = Physics.OverlapSphere(transform.position, attackRange, layerMask);
        Transform shortestTarget = null;

        if (targetColliderList.Length > 0)
        {
            isChase = true;
            float shorttestDistance = Mathf.Infinity;
            foreach (Collider targetCollider in targetColliderList)
            {
                float distance = Vector3.SqrMagnitude(transform.position - targetCollider.transform.position);
                if (shorttestDistance > distance)
                {
                    shorttestDistance = distance;
                    shortestTarget = targetCollider.transform;
                }
            }
        }
        targetTransform = shortestTarget;
    }
}






