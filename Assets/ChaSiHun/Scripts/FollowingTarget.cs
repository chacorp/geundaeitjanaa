using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingTarget : MonoBehaviour
{
    // 따라갈 타겟
    public Transform target;
    //float moveSpeed = 3f;

    public enum pType
    {
        isLocal, // 로컬 플레이어
        isRPC,   // 다른 플레이어
        isNPC    // 그냥 NPC
    }
    public pType playType;



    void Localfollow()
    {
        // 이동할 방향
        Vector3 direction = target.position - transform.position;
        // 타겟과의 거리
        float distance = direction.magnitude;

        // 타겟과의 거리에 따라 이동
        if (distance > 2.2f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
            // transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void RPCsFollow()
    {
        // 이동할 방향
        Vector3 direction = target.position - transform.position;
        // 타겟과의 거리
        float distance = direction.magnitude;

        // 타겟과의 거리에 따라 이동
        if (distance > 4f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
            //transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void NPCsFollow()
    {
        // 이동할 방향
        Vector3 direction = GameSceneManager.Instance.player.transform.position - transform.position;
        // 타겟과의 거리
        float distance = direction.magnitude;

        // 타겟과의 거리에 따라 이동
        if (distance > 3)
        {
            // 속도에 랜덤한 변화 적용하기
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime);
            //transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void Update()
    {
        if (!GameSceneManager.Instance.playStart) { return; }

        // player인지 NPC인지 확인!
        switch (playType)
        {
            case pType.isLocal:
                Localfollow();
                break;

            case pType.isRPC:
                RPCsFollow();
                break;

            case pType.isNPC:
                NPCsFollow();
                break;

            default:
                break;
        }
    }
}
