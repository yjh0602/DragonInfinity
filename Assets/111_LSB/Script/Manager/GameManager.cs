using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// =================== GAME MANAGER CLASS (Singleton) =================================
// �÷��̾� ������ ���� ����� ���õ� ����, �Լ��� ��� �ִ� Ŭ����.
// ���� ����, ���� �Ͻ�����, ���� ����, Ÿ�̸�, ������ ��� ���� ���� ������ ������
// =====================================================================================

// PlayerInformation: ���� ������� �ʴ� Ŭ����
// (�����ϴٸ� �÷����� ���� ���� �뵵�� ����� ����)
public static class PlayerInformation
{
    public static bool isFirstLoad = true;

    public static float currentHP;
    public static float currentMP;
    public static float maxHP;
    public static float maxMP;
    public static float criticalChance;
    public static float criticalDamage;

    public static float qSkillMP;
    public static float wSkillMP;
    public static float eSkillMP;
    public static float rSkillMP;

    public static float moveSpeed;              
    public static float dodgePower;            
    public static float damage;                    
    public static float defense;
    public static float healingTime; 
    public static float healingAmount;

    public static float wSkillDamage;    

    public static void SavePlayerInformation(Player _player)
    {
        if (_player == null)
        {
            Debug.Log("�÷��̾� ������ ������ �� �����ϴ�.");
            return;
        }

        isFirstLoad = false;

        currentHP       =   _player.CurrentHP;
        currentMP       =   _player.CurrentMP;

        maxHP           =   _player.MaxHP;
        maxMP           =   _player.MaxMP;

        criticalChance  =   _player.CriticalChance;
        criticalDamage  =   _player.CriticalDamage;

        moveSpeed       =   _player.moveSpeed;
        dodgePower      =   _player.dodgePower;

        damage          =   _player.damage;
        defense         =   _player.defense;

        healingTime     =   _player.healingTime;
        healingAmount   =   _player.healingAmount;

        wSkillDamage    =   _player.wSkillDamageIncrease;

        Debug.Log("�÷��̾� ������ �����߽��ϴ�.");
    }

    public static void LoadPlayerInformation(Player _player)
    {
        if (_player == null)
        {
            Debug.Log("�÷��̾� ������ �ҷ��� �� �����ϴ�.");
            return;
        }

        if (isFirstLoad == true)
            return;

        _player.CurrentHP = currentHP;
        _player.CurrentMP = currentMP;

        _player.MaxHP = maxHP;
        _player.MaxMP = maxMP;

        _player.CriticalChance = criticalChance;
        _player.CriticalDamage = criticalDamage;

        _player.moveSpeed = moveSpeed;
        _player.dodgePower = dodgePower;

        _player.damage = damage;
        _player.defense = defense;

        _player.healingTime = healingTime;
        _player.healingAmount = healingAmount;

        _player.wSkillDamageIncrease = wSkillDamage;

        Debug.Log("�÷��̾� ������ �ҷ��Խ��ϴ�.");
    }
}

public enum GAME_MODE
{
    MAIN_TITLE,
    MAIN_STORY_MODE,
    INFINITY_ROOM_MODE,

    SIZE
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    private bool isPause;                           // �Ͻ����� ����
    private bool isTimerOn;                         // Ÿ�̸� �۵� ����
    private float countTime;                        // Ÿ�̸� �ð�

    public int infinityRoomScore;

    [SerializeField] GameObject gamePlayer;         // �÷��̾�
    [SerializeField] GameObject[] dropItemList;     // ��� ������ ����Ʈ

    // ���� ���: 0. ���� Ÿ��Ʋ, 1. ���� ���丮, 2. ������ ��
    public GAME_MODE gameMode; 

    private void Awake()
    {
        // �ν��Ͻ��� ���ٸ� �ν��Ͻ��� ����ش�.
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        // ���� �Ѿ� ������ ���� ���� �����Ϸ��� �õ��Ѵٸ� �ı��Ѵ�.
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameMode = GAME_MODE.MAIN_TITLE;

        infinityRoomScore = 0;

        isTimerOn = false;
        isPause = false;

        countTime = 0.0f;

        AudioManager.Instance.PlayBGM("Main Title");

        UIManager.Instance.OffAllUI();
        UIManager.Instance.OnMainTitleUI();
        UIManager.Instance.OnSettingButton();

        GamePlayer.SetActive(false);
        ResetTimer();
    }

    private void Update()
    {
        if(isTimerOn == true)
        {
            countTime += Time.deltaTime;
        }
    }

    // ================== GAME FUNCTION ==================
    // ���� Ŭ����
    public IEnumerator GameClear()
    {
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.RequestNotice(" GAME CLEAR!");

        yield return new WaitForSeconds(5.0f);
        UIManager.Instance.OffAllUI();

        // Clear UI
        UIManager.Instance.SetGameClearTime();
        UIManager.Instance.OnGameClearUI();
    }

    // ���� ����
    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3.0f);
        UIManager.Instance.RequestNotice("GAME OVER");

        yield return new WaitForSeconds(5.0f);
        UIManager.Instance.OffAllUI();

        // Result UI
        UIManager.Instance.SetResultTime();
        UIManager.Instance.OnResultUI();
    }

    // ���� �Ͻ� ����
    public void PauseGame()
    {
        if(isPause == false)
        {
            isPause = true;
            Time.timeScale = 0;
        }
    }

    // ���� �簳
    public void ResumeGame()
    {
        if (isPause == true)
        {
            isPause = false;
            Time.timeScale = 1;
        }
    }

    // ���� ����
    public void QuitGame()
    {
        Application.Quit();
    }

    // ================== TIMER ==================
    public void OnTimer()
    {
        isTimerOn = true;
    }

    public void OffTimer()
    {
        isTimerOn = false;
    }

    public void ResetTimer()
    {
        OffTimer();
        countTime = 0.0f;
    }

    public string GetPlayTime()
    {
        string resultTime
            = ((int)countTime / 3600).ToString() + ":"      // ��
            + ((int)countTime / 60 % 60).ToString() + ":"   // ��
            + ((int)countTime % 60).ToString();             // ��

        return resultTime;
    }

    // ================== ITEM DROP ==================
    public void NormalMonsterDrop(Transform _normalMonster)
    {
        if (gameMode == GAME_MODE.INFINITY_ROOM_MODE)
        {
            float randomNumber = Random.Range(0, 100);

            if (randomNumber < 80)
            {
                AudioManager.Instance.PlaySFX("ITEM_LOW_SOUND");
                Instantiate(dropItemList[0], _normalMonster.position, _normalMonster.rotation);
            }
            if (randomNumber >= 80 && randomNumber < 90)
            {
                AudioManager.Instance.PlaySFX("ITEM_LOW_SOUND");
                Instantiate(dropItemList[1], _normalMonster.position, _normalMonster.rotation);
            }
            if (randomNumber >= 90 && randomNumber < 95)
            {
                AudioManager.Instance.PlaySFX("ITEM_LOW_SOUND");
                Instantiate(dropItemList[2], _normalMonster.position, _normalMonster.rotation);
            }
            if (randomNumber >= 95 && randomNumber < 98)
            {
                AudioManager.Instance.PlaySFX("ITEM_HIGH_SOUND");
                Instantiate(dropItemList[3], _normalMonster.position, _normalMonster.rotation);
            }
            if (randomNumber >= 98 && randomNumber < 99.5)
            {
                AudioManager.Instance.PlaySFX("ITEM_HIGH_SOUND");
                Instantiate(dropItemList[4], _normalMonster.position, _normalMonster.rotation);
            }
            if (randomNumber >= 99.5 && randomNumber < 100)
            {
                UIManager.Instance.RequestNotice("REGEND ITEM!");
                AudioManager.Instance.PlaySFX("ITEM_HIGH_SOUND");
                Instantiate(dropItemList[5], _normalMonster.position, _normalMonster.rotation);
            }
        }
    }

    public void NamedMonsterDrop(Transform _namedMonster)
    {
        if (gameMode == GAME_MODE.INFINITY_ROOM_MODE)
        {
            float randomNumber = Random.Range(0, 100);

            if (randomNumber < 40)
            {
                AudioManager.Instance.PlaySFX("ITEM_LOW_SOUND");
                Instantiate(dropItemList[0], _namedMonster.position, _namedMonster.rotation);
            }
            if (randomNumber >= 40 && randomNumber < 65)
            {
                AudioManager.Instance.PlaySFX("ITEM_LOW_SOUND");
                Instantiate(dropItemList[1], _namedMonster.position, _namedMonster.rotation);
            }
            if (randomNumber >= 65 && randomNumber < 80)
            {
                AudioManager.Instance.PlaySFX("ITEM_LOW_SOUND");
                Instantiate(dropItemList[2], _namedMonster.position, _namedMonster.rotation);
            }
            if (randomNumber >= 80 && randomNumber < 90)
            {
                AudioManager.Instance.PlaySFX("ITEM_HIGH_SOUND");
                Instantiate(dropItemList[3], _namedMonster.position, _namedMonster.rotation);
            }
            if (randomNumber >= 90 && randomNumber < 97)
            {
                AudioManager.Instance.PlaySFX("ITEM_HIGH_SOUND");
                Instantiate(dropItemList[4], _namedMonster.position, _namedMonster.rotation);
            }
            if (randomNumber >= 97 && randomNumber < 100)
            {
                UIManager.Instance.RequestNotice("REGEND ITEM!");
                AudioManager.Instance.PlaySFX("ITEM_HIGH_SOUND");
                Instantiate(dropItemList[5], _namedMonster.position, _namedMonster.rotation);
            }
        }
    }

    // ================== PROPERTY ==================
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }

    public GameObject GamePlayer
    {
        get { return gamePlayer; }
        set { gamePlayer = value; }
    }
}
