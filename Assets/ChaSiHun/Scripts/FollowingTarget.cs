using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingTarget : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 6.8f;
    public bool isPlayer;
    GameObject player;

    void Start()
    {
        // 만약 이게 플레이어라면 이걸 저장
        if (isPlayer)
        {
            player = this.gameObject;
        }
        else
        { 
            //만약 플레이어가 아니라면 플레이어를 찾아서 저장
            player = GameObject.Find("Player");
        }
    }

    void playerflow()
    {
        if (!GameSceneManager.Instance.playStart) { return; }

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (distance > 1)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void npcFlow()
    {
        if (!GameSceneManager.Instance.playStart) { return; }

        Vector3 direction = player.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance > 3)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    void Update()
    {
        if (isPlayer)
        {
            playerflow();
        }
        else
        {
            npcFlow();
        }
    }
}
