using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TRAP_TYPE
{
    NEEDLE,
    CUTTER,
    SPRING,
    HAMMER,
    OBSTACLE,
    SAW,
    SPEED,
    KEY 
}

public class Trap : MonoBehaviour
{
    [SerializeField] TRAP_TYPE trapType;

    IEnumerator SpeedTrap(Collider other)
    {
        other.GetComponent<Player>().moveSpeed += 2;
        yield return new WaitForSeconds(5.0f);
        other.GetComponent<Player>().moveSpeed += 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            TrapOn(other);
        }

    }
    private void TrapOn(Collider other)
    {
        Player player = other.GetComponent<Player>();
        switch (trapType)
        {
            case TRAP_TYPE.NEEDLE:
                {
                    PlayNeedleUpSound();
                    other.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2, ForceMode.Impulse);
                    player.CurrentHP -= player.MaxHP * 0.05f;
                    break;
                }

            case TRAP_TYPE.CUTTER:
                {
                    PlayCutterSound();
                    other.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2, ForceMode.Impulse);
                    player.CurrentHP -= player.MaxHP * 0.05f;
                    break;
                }

            case TRAP_TYPE.SPRING:
                {
                    PlaySpringUpSound();
                    other.GetComponent<Rigidbody>().AddForce(Vector3.back * 10, ForceMode.Impulse);

                    break;
                }

            case TRAP_TYPE.HAMMER:
                {
                    PlayHammerSound();
                    other.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2, ForceMode.Impulse);
                    player.CurrentHP -= player.MaxHP * 0.05f;
                    break;
                }

            case TRAP_TYPE.OBSTACLE:
                {
                    PlayObstacleSound();
                    other.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2, ForceMode.Impulse);
                    player.CurrentHP -= player.MaxHP * 0.05f;
                    break;
                }

            case TRAP_TYPE.SAW:
                {
                    PlaySawSound();
                    other.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2, ForceMode.Impulse);
                    player.CurrentHP -= player.MaxHP * 0.05f;
                    break;
                }

            case TRAP_TYPE.SPEED:
                {
                    PlaySpeedUpSound();
                    SpeedTrap(other);
                    Destroy(gameObject);
                    break;
                }
        }
    }

    void PlayCutterSound()
    {
        AudioManager.Instance.PlaySFX("TRAP_CUTTER");
    }

    void PlayHammerSound()
    {
        AudioManager.Instance.PlaySFX("TRAP_HAMMER");
    }

    void PlayObstacleSound()
    {
        AudioManager.Instance.PlaySFX("OBSTACLE");
    }

    void PlayNeedleUpSound()
    {
        AudioManager.Instance.PlaySFX("TRAP_NEEDLE_UP");
    }

    void PlaySawSound()
    {
        AudioManager.Instance.PlaySFX("TRAP_SAW1");
    }
    void PlaySpringUpSound()
    {
        AudioManager.Instance.PlaySFX("TRAP_SPRING_UP");
    }

    void PlaySpeedUpSound()
    {
        AudioManager.Instance.PlaySFX("SPEED_UP");
    }
}

