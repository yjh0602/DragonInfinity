using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform bulletPosition;
    public GameObject bullet;

    [SerializeField] Transform transformGunBody;
    [SerializeField] LayerMask layerMask; //타겟 레이어

    [SerializeField] float attackRange; //터렛사정거리
    [SerializeField] float fireRate; //연사속도

    Transform targetTransform = null; //공격대상

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
        // 범위 안에 타겟이 없을 경우
        if (targetTransform == null)
        {
            if(isChase == true)
            {
                isChase = false;
                AudioManager.Instance.PlaySFX("TURRET_OFF");
            }

            transformGunBody.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        }

        // 범위 안에 타겟이 있을 경우
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

    // 가장 가까운적 서치
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






