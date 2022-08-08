using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ITEM_TYPE
{
    MAX_HP,
    MAX_MP,

    DAMAGE,
    DEFENSE,

    CRITICAL_CHANCE,
    CRITICAL_DAMAGE,
}

public enum ITEM_GRADE
{
    NORMAL,
    RARE,
    MAGIC,
    EPIC,
    UNIQUE,
    LEGEND
}


public class Item : MonoBehaviour
{
    [SerializeField] ITEM_GRADE itemGrade;
    [SerializeField] float value;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            AudioManager.Instance.PlaySFX("ITEM_GET");
            UseItem(other);
            Destroy(gameObject);
        }
        
    }

    private void UseItem(Collider other)
    {
        
        switch (itemGrade)
        {          
            case ITEM_GRADE.NORMAL:
                {
                    DecideType(value, other);
                    break;
                }
            case ITEM_GRADE.RARE:
                {
                    DecideType(value, other);
                    break;
                }
            case ITEM_GRADE.MAGIC:
                {
                    DecideType(value, other);
                    break;
                }
            case ITEM_GRADE.EPIC:
                {
                    DecideType(value, other);
                    break;
                }
            case ITEM_GRADE.UNIQUE:
                {
                    DecideType(value, other);
                    break;
                }
            case ITEM_GRADE.LEGEND:
                {
                    DecideType(value, other);
                    break;
                }
        }
    }
    private void DecideType(float _value, Collider other)
    {
        Player playerScript = other.GetComponent<Player>();
        ITEM_TYPE randomNumber = (ITEM_TYPE)Random.Range(0, 6);
        switch (randomNumber)
        {
            case ITEM_TYPE.MAX_HP:
                {
                    playerScript.MaxHP += _value;
                    UIManager.Instance.RequestNotice("Max HP + " + _value);
                    break;
                }
            case ITEM_TYPE.MAX_MP:
                {
                    playerScript.MaxMP += _value;
                    UIManager.Instance.RequestNotice("Max MP + " + _value);
                    break;
                }
            case ITEM_TYPE.DAMAGE:
                {
                    playerScript.damage += _value;
                    UIManager.Instance.RequestNotice("Damage + " + _value);
                    break;
                }
            case ITEM_TYPE.DEFENSE:
                {
                    playerScript.defense += _value;
                    UIManager.Instance.RequestNotice("Defense + " + _value);
                    break;
                }
            case ITEM_TYPE.CRITICAL_CHANCE:
                {
                    playerScript.CriticalChance += (_value / 10);
                    UIManager.Instance.RequestNotice("Critical Chance + " + _value / 10);
                    break;
                }
            case ITEM_TYPE.CRITICAL_DAMAGE:
                {
                    playerScript.CriticalDamage += _value;
                    UIManager.Instance.RequestNotice("Critical Damage + " + _value);
                    break;
                }
        }

    }
}
