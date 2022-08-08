using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum PET_TYPE
{
    MOVE_SPEED,  
    DAMAGE,
    DEFENSE,   
    CRITICAL,
    ALL
}



public class Pet : MonoBehaviour
{  
    [SerializeField] Transform target;
    [SerializeField] Player player;
    [SerializeField] PET_TYPE petType;

    NavMeshAgent navMeshAgent;
    Animator animator;
     
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();            
        animator = GetComponent<Animator>();        
        Buff();
    }
  
    private void Update()
    {
        navMeshAgent.SetDestination(target.position);     
    }           
    void Buff()
    {      
       switch(petType)
        {
            case PET_TYPE.MOVE_SPEED:
                player.moveSpeed += 1;
                break;

            case PET_TYPE.DAMAGE:
                player.damage += 5;
                break;

            case PET_TYPE.DEFENSE:
                player.defense += 5;
                break;

            case PET_TYPE.CRITICAL:
                player.CriticalChance += 0.1f;
                player.CriticalDamage += 10;
                break;

            case PET_TYPE.ALL:
                player.moveSpeed += 2;
                player.damage += 10;
                player.defense += 10;
                player.CriticalChance += 0.5f;
                player.CriticalDamage += 20;
                player.MaxHP += 100;
                player.MaxMP += 100;
                break;
        }
    }
   
}
