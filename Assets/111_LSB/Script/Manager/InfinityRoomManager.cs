using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// =================== INFINITY ROOM MANAGER CLASS ====================================
// ������ �� ����� ���õ� ������ ��� �ִ� Ŭ����.
// �̸� ��ϵ� ���̺� ������ ���� ���̺긦 �����ϰ� �����Ѵ�.
// =====================================================================================

// ���̺� ������ ��� �ִ� ����ü
[System.Serializable]
public struct Wave
{
    public float    spawnTimeInterval;              // ���� ����
    public int      maxSpawnNormalMonsterNumber;    // ���� �Ϲ� ���� ��
    public int      maxSpawnNamedMonsterNumber;     // ���� ���ӵ� ���� ��
    public GameObject[] normalMonsterPrefabs;       // ������ ���� ������
    public GameObject[] namedMonsterPrefabs;
}

public class InfinityRoomManager : MonoBehaviour
{
    [SerializeField] Transform[]    spawnPoint;             // ���� ����Ʈ
    [SerializeField] Wave           waveInfo;               // ���̺� ���� ����
    List<Monster>                   currentWaveMonsterList; // ���� ���̺��� ���� ����Ʈ

    [SerializeField] int totalWaveNumber;           // �� ���̺� ��
    [SerializeField] int currentWaveNumber;         // ���� ���̺� �ܰ�
    private Wave currentWaveInformation;   // ���� ���̺� ����

    private float waveWeight;       // ���̺� ����ġ (���� ��ȭ��)
    private bool isWaveStart;       // ���̺� ���� ����

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
        // ���� ���̺꿡�� ��ȯ�� ���� ��
        int currentSpawnMonsterNumber = 0;

        // ���ӵ� ���̺� (5�ܰ� ����)
        if(currentWaveNumber % 5 == 0)
        {
            UIManager.Instance.RequestNotice("NAMED WAVE");
            // ���̺� ����ü�� ��� �ִ� ���������� �ݺ�
            while (currentSpawnMonsterNumber < currentWaveInformation.maxSpawnNamedMonsterNumber)
            {
                // ��ϵ� ���� ��ġ�� ����������
                int spawnPointIndex = currentSpawnMonsterNumber % spawnPoint.Length;

                // ���� ����, ���� ��ġ ����
                int randomMonsterIndex = Random.Range(0, currentWaveInformation.namedMonsterPrefabs.Length);

                // ���� ����
                GameObject monsterObject
                    = Instantiate(currentWaveInformation.namedMonsterPrefabs[randomMonsterIndex], spawnPoint[spawnPointIndex].position, spawnPoint[spawnPointIndex].rotation);

                Monster monsterScript = monsterObject.GetComponent<Monster>();

                // ���� ��ȭ
                monsterScript.MaxHP *= waveWeight;
                monsterScript.CurrentHP *= waveWeight;
                monsterScript.Damage *= waveWeight;
                monsterScript.Defense *= waveWeight;
                monsterScript.attachedList = currentWaveMonsterList;

                // ���� ���̺��� ���� ����Ʈ�� �߰�
                currentWaveMonsterList.Add(monsterScript);

                ++currentSpawnMonsterNumber;

                // ���� ���� ��ŭ Wait
                yield return new WaitForSeconds(currentWaveInformation.spawnTimeInterval);
            }
        }

        // �븻 ���� �ܰ�
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


    // ��ŸƮ ���� �÷��̾ ������ ���̺� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isWaveStart == false)
        {
            UIManager.Instance.RequestNotice("WAVE START!");
            isWaveStart = true;
        }
    }
}
