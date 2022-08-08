using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// =================== UI MANAGER CLASS (Singleton) ===================================
// 게임내에서 사용하는 모든 UI 정보와 제어 함수를 담고 있는 클래스
// =====================================================================================

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    [SerializeField] Texture2D cursorImage;

    [SerializeField] GameObject backgroundUI;

    // Main Title UI
    [SerializeField] GameObject mainTitleUI;

    // Help UI
    [SerializeField] GameObject helpUI;

    // Player UI
    [SerializeField] GameObject playerUI;
    [SerializeField] Image playerHPFluid;
    [SerializeField] Image playerMPFluid;

    [SerializeField] Image playerSkillImageQ;
    [SerializeField] Image playerSkillImageW;
    [SerializeField] Image playerSkillImageE;
    [SerializeField] Image playerSkillImageR;

    [SerializeField] Player playerScript;
    private Material playerHPMaterial;
    private Material playerMPMaterial;

    [SerializeField] TextMeshProUGUI playerHPText;
    [SerializeField] TextMeshProUGUI playerMPText;
    [SerializeField] TextMeshProUGUI playerDamageText;
    [SerializeField] TextMeshProUGUI playerDefenseText;
    [SerializeField] TextMeshProUGUI playerCriticalChanceText;
    [SerializeField] TextMeshProUGUI playerCriticalDamageText;

    // Setting UI
    [SerializeField] GameObject settingButton;
    [SerializeField] GameObject settingUI;

    // Sound UI
    [SerializeField] GameObject soundUI;

    // Monster UI
    [SerializeField] GameObject monsterHPBarUI;
    [SerializeField] GameObject monsterHPBarCamera; // 체력바 카메라
    public GameObject monsterHPBarCanvas; // 체력바 캔버스
    public GameObject monsterHPBarPrefab; // 체력바 프리팹

    // Result UI
    [SerializeField] GameObject resultUI;
    [SerializeField] TextMeshProUGUI resultTimeText;
    [SerializeField] TextMeshProUGUI resultScoreText;

    // Infinity Room UI
    [SerializeField] GameObject infinityRoomUI;
    [SerializeField] TextMeshProUGUI infinityRoomWaveText;
    [SerializeField] TextMeshProUGUI infinityRoomTimeText;
    [SerializeField] TextMeshProUGUI infinityRoomScoreText;

    // Game Clear UI
    [SerializeField] GameObject gameClearUI;
    [SerializeField] TextMeshProUGUI gameClearTimeText;
    [SerializeField] TextMeshProUGUI gameClearScoreText;

    // Notice UI
    [SerializeField] GameObject noticeUI;
    [SerializeField] TextMeshProUGUI noticeText;
    private Queue<string> noticeQueue;
    private bool isNotice;

    // Boss HP UI
    [SerializeField] GameObject bossUI;
    [SerializeField] Image bossHPBar;

    private void Awake()
    {
        // 인스턴스가 없다면 인스턴스를 담아준다.
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this);
        }

        // 씬이 넘어 갔을때 만약 새로 생성하려고 시도한다면 파괴한다.
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Cursor.SetCursor(cursorImage, Vector2.zero, CursorMode.ForceSoftware);
        playerHPMaterial = playerHPFluid.material;
        playerMPMaterial = playerMPFluid.material;

        noticeQueue = new Queue<string>();
        isNotice = false;
    }

    private void Update()
    {
        // ================== SNIFF NOTICE REQUEST ==================
        if(isNotice == false && noticeQueue.Count != 0)
        {
            isNotice = true;
            noticeText.text = noticeQueue.Dequeue();
            OnNoticeUI();
            StartCoroutine(WaitForNotice());
        }

        if (PlayerScript != null)
        {
            // ================== UPDATE PLAYER STATUS UI ==================
            SetPlayerHPStatus(PlayerScript.MaxHP, PlayerScript.CurrentHP);
            SetPlayerMPStatus(PlayerScript.MaxMP, PlayerScript.CurrentMP);
            SetPlayerDamageStatus(PlayerScript.damage);
            SetPlayerDefenseStatus(PlayerScript.defense);
            SetPlayerCriticalChanceStatus(PlayerScript.CriticalChance);
            SetPlayerCriticalDamageStatus(PlayerScript.CriticalDamage);

            // ================== UPDATE PLAYER COOLDOWN UI  ==================
            // Q Skill
            if (PlayerScript.isQskillCool == false)
            {
                playerSkillImageQ.fillAmount += 1 / PlayerScript.qCoolTime * Time.deltaTime;
            }

            else
            {
                playerSkillImageQ.fillAmount = 0;
            }

            // W Skill
            if (PlayerScript.isWskillCool == false)
            {
                playerSkillImageW.fillAmount += 1 / PlayerScript.wCoolTime * Time.deltaTime;
            }

            else
            {
                playerSkillImageW.fillAmount = 0;
            }

            // E Skill
            if (PlayerScript.isEskillCool == false)
            {
                playerSkillImageE.fillAmount += 1 / PlayerScript.eCoolTime * Time.deltaTime;
            }

            else
            {
                playerSkillImageE.fillAmount = 0;
            }

            // R Skill
            if (PlayerScript.isRskillCool == false)
            {
                playerSkillImageR.fillAmount += 1 / PlayerScript.rCoolTime * Time.deltaTime;
            }

            else
            {
                playerSkillImageR.fillAmount = 0;
            }
        }
    }

    //
    public void OnBackgroundUI()
    {
        OnUI(backgroundUI);
    }

    public void OffBackgroundUI()
    {
        OffUI(backgroundUI);
    }

    // ================== PLAYER UI ==================
    public void OnPlayerUI()
    {
        OnUI(playerUI);
    }
    public void OffPlayerUI()
    {
        OffUI(playerUI);
    }
    public void UpdatePlayerHPUI(float _ratio)
    {
        if (playerHPMaterial != null)
        {
            playerHPMaterial.SetFloat("_FillLevel", _ratio);
        }
    }

    public void UpdatePlayerMPUI(float _ratio)
    {
        if(playerMPMaterial != null)
        {
            playerMPMaterial.SetFloat("_FillLevel", _ratio);
        }
    }

    public void SetPlayerHPStatus(float _maxHP, float _currentHP)
    {
        int maxHP = (int)_maxHP;
        int currentHP = (int)_currentHP;

        playerHPText.text = currentHP.ToString() + " / " + maxHP.ToString();
    }

    public void SetPlayerMPStatus(float _maxMP, float _currentMP)
    {
        int maxMP = (int)_maxMP;
        int currentMP = (int)_currentMP;

        playerMPText.text = currentMP.ToString() + " / " + maxMP.ToString();
    }

    public void SetPlayerDamageStatus(float _damage)
    {
        playerDamageText.text = _damage.ToString();
    }

    public void SetPlayerDefenseStatus(float _defense)
    {
        playerDefenseText.text = _defense.ToString();
    }

    public void SetPlayerCriticalChanceStatus(float _criticalChance)
    {
        playerCriticalChanceText.text = _criticalChance.ToString();
    }
    public void SetPlayerCriticalDamageStatus(float _criticalDamage)
    {
        playerCriticalDamageText.text = _criticalDamage.ToString();
    }

    // ================== MAIN TITLE UI ==================
    public void OnMainTitleUI()
    {
        OnUI(mainTitleUI);
    }
    public void OffMainTitleUI()
    {
        OffUI(mainTitleUI);
    }
    // Main Story Button
    public void LoadMainStoryScene()
    {
        GameManager.Instance.GamePlayer.SetActive(false);
        GameManager.Instance.ResetTimer();
        AudioManager.Instance.StopBGM();

        OffAllUI();
        OnPlayerUI();
        OnMonsterHPBarUI();
        OnSettingButton();

        GameManager.Instance.GamePlayer.SetActive(true);
        GameManager.Instance.OnTimer();
        AudioManager.Instance.PlayBGM("Main Story");

        GameManager.Instance.GamePlayer.GetComponent<Player>().InitState();
        GameManager.Instance.GamePlayer.transform.position = new Vector3(65f, -3.5f, 103);

        GameManager.Instance.gameMode = GAME_MODE.MAIN_STORY_MODE;

        LoadingManager.LoadScene("Main Story");
    }

    // Infinity Room Button
    public void LoadInfinityRoomScene()
    {
        GameManager.Instance.GamePlayer.SetActive(false);
        GameManager.Instance.ResetTimer();
        AudioManager.Instance.StopBGM();

        OffAllUI();
        OnPlayerUI();
        OnMonsterHPBarUI();
        OnSettingButton();
        OnInfinityRoomUI();

        GameManager.Instance.GamePlayer.SetActive(true);
        GameManager.Instance.OnTimer();
        AudioManager.Instance.PlayBGM("Infinity Room");

        GameManager.Instance.GamePlayer.GetComponent<Player>().InitState();
        GameManager.Instance.GamePlayer.transform.position = new Vector3(-37, 4, 30);

        GameManager.Instance.gameMode = GAME_MODE.INFINITY_ROOM_MODE;

        LoadingManager.LoadScene("Infinity Room");
    }

    // Help Button
    public void OnHelpUI()
    {
        OnUI(helpUI);
    }
    public void OffHelpUI()
    {
        OffUI(helpUI);
    }

    // Quit Button
    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    // ================== SETTING UI ==================
    // Setting Button
    public void OnSettingButton()
    {
        OnUI(settingButton);
    }

    public void OffSettingButton()
    {
        OffUI(settingButton);
    }

    // Setting Menu
    public void OnSettingUI()
    {
        GameManager.Instance.PauseGame();
        OnUI(settingUI);
    }

    public void OffSettingUI()
    {
        GameManager.Instance.ResumeGame();
        OffUI(settingUI);
    }

    // Sound Menu Button
    public void OnSoundUI()
    {
        OnUI(soundUI);
    }

    public void OffSoundUI()
    {
        OffUI(soundUI);
    }

    // Return to Main Button
    public void ReturnMainTitle()
    {
        GameManager.Instance.GamePlayer.SetActive(false);
        GameManager.Instance.ResetTimer();
        GameManager.Instance.ResumeGame();
        AudioManager.Instance.StopBGM();

        OffAllUI();
        OnMainTitleUI();
        OnSettingButton();

        AudioManager.Instance.PlayBGM("Main Title");
        GameManager.Instance.gameMode = GAME_MODE.MAIN_TITLE;

        LoadingManager.LoadScene("Main Title");
    }

    public void OnMonsterHPBarUI()
    {
        monsterHPBarUI.SetActive(true);
    }

    public void OffMonsterHPBarUI()
    {
        monsterHPBarUI.SetActive(false);
    }

    // ================== Infinity Room UI ==================
    public void OnInfinityRoomUI()
    {
        OnUI(infinityRoomUI);
    }
    public void OffInfinityRoomUI()
    {
        OffUI(infinityRoomUI);
    }
    public void SetInfinityWave(int _wave)
    {
        infinityRoomWaveText.text = "WAVE: " + _wave;
    }

    public void SetInfinityRoomTime()
    {
        infinityRoomTimeText.text =
            "TIME: " + GameManager.Instance.GetPlayTime();
    }

    public void SetInfinityRoomScore()
    {
        infinityRoomScoreText.text =
            "SCORE: " + GameManager.Instance.infinityRoomScore.ToString();
    }

    // ================== RESULT UI ==================
    public void OnResultUI()
    {
        OnUI(resultUI);
    }
    public void OffResultUI()
    {
        OffUI(resultUI);
    }

    public void SetResultTime()
    {
        resultTimeText.text =
            "TIME: " + GameManager.Instance.GetPlayTime();
    }

    // ================== GAME CLEAR UI ==================
    public void OnGameClearUI()
    {
        OnUI(gameClearUI);
    }
    public void OffGameClearUI()
    {
        OffUI(gameClearUI);
    }

    public void SetGameClearTime()
    {
        gameClearTimeText.text =
            "TIME: " + GameManager.Instance.GetPlayTime();
    }

    // ================== NOTICE UI ==================
    public void OnNoticeUI()
    {
        OnUI(noticeUI);
    }
    public void OffNoticeUI()
    {
        OffUI(noticeUI);
    }

    public void RequestNotice(string _content)
    {
        noticeQueue.Enqueue(_content);
    }

    IEnumerator WaitForNotice()
    {
        yield return new WaitForSeconds(2.0f);
        isNotice = false;
        OffNoticeUI();
    }
    // ================== NOTICE UI ==================
    public void OnBossUI()
    {
        OnUI(bossUI);
    }
    public void OffBossUI()
    {
        OffUI(bossUI);
    }

    public void UpdateBossHPUI(float _ratio)
    {
        if (bossHPBar != null)
        {
            bossHPBar.fillAmount = _ratio;
        }
    }

    // ================== UI CONTROL FUNCTION ==================
    // Open UI
    public void OnUI(GameObject _UI)
    {
        _UI.SetActive(true);
    }

    // Close UI
    public void OffUI (GameObject _UI)
    {
        _UI.SetActive(false);
    }


    public void OffAllUI()
    {
        OffUI(mainTitleUI);
        OffUI(playerUI);
        OffUI(settingUI);
        OffUI(helpUI);
        OffUI(soundUI);
        OffUI(resultUI);
        OffUI(monsterHPBarUI);
        OffUI(infinityRoomUI);
        OffUI(gameClearUI);
        OffUI(bossUI);
    }

    // ================== PROPERTY ==================
    public static UIManager Instance
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

    public Player PlayerScript
    {
        get { return playerScript; }
        set { playerScript = value; }
    }
}
