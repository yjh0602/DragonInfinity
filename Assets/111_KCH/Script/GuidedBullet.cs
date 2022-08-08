using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedBullet : Bullet
{
    [HideInInspector] public Transform target;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (flashEffect != null)
        {
            GameObject newFlash = Instantiate(flashEffect, transform.position, Quaternion.identity);
            newFlash.transform.forward = gameObject.transform.forward;

            ParticleSystem flashParticleSystem = newFlash.GetComponent<ParticleSystem>();

            if (flashParticleSystem != null)
            {
                Destroy(newFlash, flashParticleSystem.main.duration);
            }
            else
            {
                ParticleSystem flashChildParticleSystem = newFlash.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(newFlash, flashChildParticleSystem.main.duration);
            }
        }
        AutoDestroy();
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            transform.position += (direction * bulletSpeed * Time.deltaTime);
            transform.forward = direction;
        }

        else
        {
            transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.CurrentHP -= player.MaxHP * (bulletDamage / 100f);
        }
    }
}

