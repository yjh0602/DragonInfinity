using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorGeneration : MonoBehaviour
{
    public BossMonster boss;
    public GameObject meteor;
    bool isCoolTime = true;

    private int minDistance = 6;
    private int maxDistance = 15;

    IEnumerator NewMeteor() //운석 생성 및 낙하
    {
        isCoolTime = false;
        yield return new WaitForSeconds(7);

        for (int i = 0; i < boss.meteorNumber; ++i)
        {
            float[] positionX = new float[2];
            positionX[0] = Random.Range(-maxDistance, -minDistance);
            positionX[1] = Random.Range(minDistance, maxDistance);
            float finalRandomPositionX = positionX[(int)Random.Range(0, 2)];
            finalRandomPositionX += transform.position.x;

            float[] positionZ = new float[2];
            positionZ[0] = Random.Range(-maxDistance, -minDistance);
            positionZ[1] = Random.Range(minDistance, maxDistance);
            float finalRandomPositionZ = positionZ[(int)Random.Range(0, 2)];
            finalRandomPositionZ += transform.position.z;

            Vector3 location = new Vector3(finalRandomPositionX, boss.transform.position.y, finalRandomPositionZ);
            Instantiate(meteor, location, transform.rotation);
        }

        isCoolTime = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCoolTime)
        {
            StartCoroutine("NewMeteor");
        }
    }
}