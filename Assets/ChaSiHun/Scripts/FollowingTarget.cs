using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingTarget : MonoBehaviour
{
    // 따라갈 타겟
    public Transform target;

    [Tooltip("플레이어들의 속도는 8이 적당하고, NPC의 속도는 6.8이 적당하다 ")]
    public float moveSpeed = 6.8f;

    // 플레이어인지 아닌지?
    public bool isPlayer;


    void Playerflow()
    {
        // 플레이 시작이 아니라면 무시
        if (!GameSceneManager.Instance.playStart) { return; }

        // 이동할 방향
        Vector3 direction = target.position - transform.position;
        // 타겟과의 거리
        float distance = direction.magnitude;

        // 타겟과의 거리에 따라 이동
        if (distance > 1)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void NPCsFlow()
    {
        if (!GameSceneManager.Instance.playStart) { return; }

        // 이동할 방향
        Vector3 direction = GameSceneManager.Instance.player.transform.position - transform.position;
        // 타겟과의 거리
        float distance = direction.magnitude;

        // 타겟과의 거리에 따라 이동
        if (distance > 3)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void Update()
    {
        // player인지 NPC인지 확인!
        if (isPlayer)
        {
            Playerflow();
        }
        else
        {
            NPCsFlow();
        }
    }
}
