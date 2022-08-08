using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// =================== GAME MANAGER CLASS (Singleton) =================================
// 플레이어 정보나 게임 진행과 관련된 변수, 함수를 담고 있는 클래스.
// 게임 시작, 게임 일시정지, 게임 종료, 타이머, 아이템 드랍 결정 관련 역할을 수행중
// =====================================================================================

// PlayerInformation: 현재 사용하지 않는 클래스
// (가능하다면 플레이저 정보 저장 용도로 사용할 예정)
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
            Debug.Log("플레이어 정보를 저장할 수 없습니다.");
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

        Debug.Log("플레이어 정보를 저장했습니다.");
    }

    public static void LoadPlayerInformation(Player _player)
    {
        if (_player == null)
        {
            Debug.Log("플레이어 정보를 불러올 수 없습니다.");
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

        Debug.Log("플레이어 정보를 불러왔습니다.");
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

    private bool isPause;                           // 일시정지 여부
    private bool isTimerOn;                         // 타이머 작동 여부
    private float countTime;                        // 타이머 시간

    public int infinityRoomScore;

    [SerializeField] GameObject gamePlayer;         // 플레이어
    [SerializeField] GameObject[] dropItemList;     // 드랍 아이템 리스트

    // 게임 모드: 0. 게임 타이틀, 1. 메인 스토리, 2. 무한의 방
    public GAME_MODE gameMode; 

    private void Awake()
    {
        // 인스턴스가 없다면 인스턴스를 담아준다.
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        // 씬이 넘어 갔을때 만약 새로 생성하려고 시도한다면 파괴한다.
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
    // 게임 클리어
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

    // 게임 오버
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

    // 게임 일시 정지
    public void PauseGame()
    {
        if(isPause == false)
        {
            isPause = true;
            Time.timeScale = 0;
        }
    }

    // 게임 재개
    public void ResumeGame()
    {
        if (isPause == true)
        {
            isPause = false;
            Time.timeScale = 1;
        }
    }

    // 게임 종료
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
            = ((int)countTime / 3600).ToString() + ":"      // 시
            + ((int)countTime / 60 % 60).ToString() + ":"   // 분
            + ((int)countTime % 60).ToString();             // 초

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
