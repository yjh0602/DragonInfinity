using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public Vector3 angle;

    private void Awake()
    {
        target = GameManager.Instance.GamePlayer.transform;
        target.GetComponent<Player>().mainCamera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.rotation = Quaternion.Euler(angle);
            transform.position = target.position + offset;
        }
    }
}
