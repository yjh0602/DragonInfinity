using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== INFINITY ROOM MANAGER CLASS ====================================
// 무한의 방 진행과 관련된 정보를 담고 있는 클래스.
// 미리 등록된 웨이브 정보를 통해 웨이브를 생성하고 진행한다.
// =====================================================================================

// 웨이브 정보를 담고 있는 구조체
[System.Serializable]
public struct Wave
{
    public float    spawnTimeInterval;              // 스폰 간격
    public int      maxSpawnNormalMonsterNumber;    // 스폰 일반 몬스터 수
    public int      maxSpawnNamedMonsterNumber;     // 스폰 네임드 몬스터 수
    public GameObject[] normalMonsterPrefabs;       // 스폰할 몬스터 프리팹
    public GameObject[] namedMonsterPrefabs;
}

public class InfinityRoomManager : MonoBehaviour
{
    [SerializeField] Transform[]    spawnPoint;             // 스폰 포인트
    [SerializeField] Wave           waveInfo;               // 웨이브 생성 정보
    List<Monster>                   currentWaveMonsterList; // 현재 웨이브의 몬스터 리스트

    [SerializeField] int totalWaveNumber;           // 총 웨이브 수
    [SerializeField] int currentWaveNumber;         // 현재 웨이브 단계
    private Wave currentWaveInformation;   // 현재 웨이브 정보

    private float waveWeight;       // 웨이브 가중치 (몬스터 강화용)
    private bool isWaveStart;       // 웨이브 시작 여부

    private bool isWave;

    private void Awake()
    {
        currentWaveMonsterList = new List<Monster>();
        isWaveStart = false;
        isWave = false;
        currentWaveNumber = 0;
        waveWeight = 1.0f;
    }

    private void Start()
    {
        UIManager.Instance.RequestNotice("INFINITY ROOM");
    }

    private void Update()
    {
        UIManager.Instance.SetInfinityRoomTime();
        UIManager.Instance.SetInfinityRoomScore();

        if (isWaveStart == true)
        {
            if (currentWaveMonsterList.Count == 0 && currentWaveNumber < totalWaveNumber && isWave == false)
            {
                StartNextWave();
            }
        }
    }

    private void StartNextWave()
    {
        isWave = true;
        ++currentWaveNumber;
        UIManager.Instance.SetInfinityWave(currentWaveNumber);

        waveWeight *= 1.1f;
        StartWave(waveInfo);
    }

    private void StartWave(Wave _wave)
    {
        currentWaveInformation = _wave;
        StartCoroutine(SpawnMonster());
    }

    IEnumerator SpawnMonster()
    {
        // 현재 웨이브에서 소환한 몬스터 수
        int currentSpawnMonsterNumber = 0;

        // 네임드 웨이브 (5단계 마다)
        if(currentWaveNumber % 5 == 0)
        {
            UIManager.Instance.RequestNotice("NAMED WAVE");
            // 웨이브 구조체에 담긴 최대 스폰수까지 반복
            while (currentSpawnMonsterNumber < currentWaveInformation.maxSpawnNamedMonsterNumber)
            {
                // 등록된 스폰 위치에 순차적으로
                int spawnPointIndex = currentSpawnMonsterNumber % spawnPoint.Length;

                // 몬스터 종류, 스폰 위치 결정
                int randomMonsterIndex = Random.Range(0, currentWaveInformation.namedMonsterPrefabs.Length);

                // 몬스터 생성
                GameObject monsterObject
                    = Instantiate(currentWaveInformation.namedMonsterPrefabs[randomMonsterIndex], spawnPoint[spawnPointIndex].position, spawnPoint[spawnPointIndex].rotation);

                Monster monsterScript = monsterObject.GetComponent<Monster>();

                // 몬스터 강화
                monsterScript.MaxHP *= waveWeight;
                monsterScript.CurrentHP *= waveWeight;
                monsterScript.Damage *= waveWeight;
                monsterScript.Defense *= waveWeight;
                monsterScript.attachedList = currentWaveMonsterList;

                // 현재 웨이브의 몬스터 리스트에 추가
                currentWaveMonsterList.Add(monsterScript);

                ++currentSpawnMonsterNumber;

                // 스폰 간격 만큼 Wait
                yield return new WaitForSeconds(currentWaveInformation.spawnTimeInterval);
            }
        }

        // 노말 몬스터 단계
        else
        {
            while (currentSpawnMonsterNumber < currentWaveInformation.maxSpawnNormalMonsterNumber)
            {
                int spawnPointIndex = currentSpawnMonsterNumber % spawnPoint.Length;
                int randomMonsterIndex = Random.Range(0, currentWaveInformation.normalMonsterPrefabs.Length);

                GameObject monsterObject
                    = Instantiate(currentWaveInformation.normalMonsterPrefabs[randomMonsterIndex], spawnPoint[spawnPointIndex].position, spawnPoint[spawnPointIndex].rotation);

                Monster monsterScript = monsterObject.GetComponent<Monster>();

                monsterScript.MaxHP *= waveWeight;
                monsterScript.CurrentHP *= waveWeight;
                monsterScript.Damage *= waveWeight;
                monsterScript.Defense *= waveWeight;
                monsterScript.attachedList = currentWaveMonsterList;

                currentWaveMonsterList.Add(monsterScript);

                ++currentSpawnMonsterNumber;

                yield return new WaitForSeconds(currentWaveInformation.spawnTimeInterval);
            }
        }

        isWave = false;
    }


    // 스타트 선에 플레이어가 들어오면 웨이브 시작
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isWaveStart == false)
        {
            UIManager.Instance.RequestNotice("WAVE START!");
            isWaveStart = true;
        }
    }
}
