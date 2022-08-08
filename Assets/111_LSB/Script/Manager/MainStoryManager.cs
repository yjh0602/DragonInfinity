using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== MAIN STORY MANAGER CLASS (Singleton) ===========================
// 메인스토리 진행과 관련된 정보를 담고 있는 클래스.
// 네임드 몬스터를 모두 처치할 시 보스방 진입을 가능하게 한다.
// =====================================================================================

public class MainStoryManager : MonoBehaviour
{
    private static MainStoryManager instance = null;

    public BossMonster bossMonster;
    [SerializeField] PlayerCamera playerCamera;

    [SerializeField] GameObject teleport;
    [SerializeField] List<Monster> namedMonsterList;
    [SerializeField] List<Transform> namedSpawnPoint;
    private List<Monster> aliveNamedMonsterList;

    private bool isOpen;

    private void Awake()
    {
        // 인스턴스가 없다면 인스턴스를 담아준다.
        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {        
        UIManager.Instance.RequestNotice("MAIN STORY");
        isOpen = false;
        playerCamera = Camera.main.GetComponent<PlayerCamera>();
        aliveNamedMonsterList = new List<Monster>();

        // 네임드 생성
        for(int i = 0; i < namedSpawnPoint.Count; ++i)
        {
            Monster namedMonster =
                Instantiate(namedMonsterList[i], namedSpawnPoint[i].position, namedSpawnPoint[i].rotation);

            namedMonster.attachedList = aliveNamedMonsterList;
            aliveNamedMonsterList.Add(namedMonster);
        }
    }

    void Update()
    {
        if(isOpen == false && aliveNamedMonsterList.Count <= 0 && aliveNamedMonsterList != null)
        {
            OpenBossRoomPortal();
        }
    }

    // 보스룸 포탈 오픈
    void OpenBossRoomPortal()
    {
        isOpen = true;

        UIManager.Instance.RequestNotice("BOSS ROOM HAS OPENED");
        teleport.SetActive(true);
    }

    // 보스룸 입장
    public void EnterBossRoom()
    {
        UIManager.Instance.RequestNotice("BOSS : RED DRAGON");
        StartCoroutine(WaitForBossRoomCamera());
        GameManager.Instance.GamePlayer.transform.position = new Vector3(-5, 24.4f, -290);

        bossMonster.gameObject.SetActive(true);
        bossMonster.target = GameManager.Instance.GamePlayer.gameObject.transform;

        UIManager.Instance.OnBossUI();

    }

    IEnumerator WaitForBossRoomCamera()
    {
        GameManager.Instance.GamePlayer.GetComponent<Player>().playerNavMeshAgent.enabled = false;
        playerCamera.offset = new Vector3(0, 90, 25);
        playerCamera.angle = new Vector3(50, -180, 0);
        yield return new WaitForSeconds(16.0f);
        playerCamera.offset = new Vector3(0, 20, 7);
        playerCamera.angle = new Vector3(60, -180, 0);
        GameManager.Instance.GamePlayer.GetComponent<Player>().playerNavMeshAgent.enabled = true;
        GameManager.Instance.GamePlayer.GetComponent<Player>().InitState();
    }

    // ================== PROPERTY ==================
    public static MainStoryManager Instance
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
}
