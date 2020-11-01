using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCManager : MonoBehaviour
{
    public static RPCManager Instance;
    RPCManager()
    {
        Instance = this;
    }
    // 한 방에 들어올 수 있는 다른 플레이어의 수 최대값
    // 전체 플레이어의 수 - 로컬 플레이어 = 3명
    public int MaxRPCs = 3;

    // 현재 모든 플레이어의 수
    public int currentPlayer = 0;

    // 이전 모든 플레이어의 수
    int previousPlayer = 0;

    // 다른 플레이어의 캐릭터
    public GameObject RPC_prefab;

    // 플레이어
    Transform playerObj;

    // 다른 플레이어의 캐릭터
    GameObject rpc;

    // 따라갈 녀석
    Transform follower;

    public enum npc_State
    {
        None,
        GetCloser,
        Stay,
        TakePhoto,
        GetAway
    }
    public npc_State npc_A;

    float approachSpeed = 1.5f;

    public Transform spawnPoint;
    public Transform targetPoint;

    void Start()
    {
        // 다른 플레이어 캐릭터 만들어두기
        for (int i = 0; i < MaxRPCs; i++)
        {
            // 1. 다른 플레이어의 캐릭터를 만들어서,
            rpc = Instantiate(RPC_prefab);

            // 2. 비활성화하기
            rpc.SetActive(false);
        }

        // 오브젝트 가져오기
        playerObj = GameSceneManager.Instance.player.transform;
        follower = GameObject.FindGameObjectWithTag("Respawn").transform;


        npc_A = npc_State.None;
    }

    // 플레이어에게 접근하기!!
    void Approach()
    {
        // 타겟과의 거리가 일정 거리 이상이면 접근하고, 일정 거리 미만이면 그만 접근하기
        float distance = Vector3.Distance(playerObj.position, rpc.transform.position);

        if (distance >= 3f)
        {
            rpc.transform.position = Vector3.Lerp(rpc.transform.position, playerObj.position, approachSpeed * Time.deltaTime);
        }
        else
        {
            rpc.transform.SetParent(follower);
            // 그만 접근하기
            npc_A = npc_State.Stay;
            GameSceneManager.Instance.currentScene = GameSceneManager.Scenes.MatchFound;
        }
    }


    void RunAway()
    {
        // 탈출 포인트 가져오기
        Transform leavePoint = follower.GetChild(1);

        // 타겟과의 거리가 일정 거리 이상이면 접근하고, 일정 거리 미만이면 비활성화하기
        float distance = Vector3.Distance(leavePoint.position, rpc.transform.position);
        if (distance >= 2f)
        {
            rpc.transform.position = Vector3.Lerp(rpc.transform.position, leavePoint.position, approachSpeed * Time.deltaTime);
        }
        else
        {
            Prefab_Float rpcPF = rpc.GetComponentInChildren<Prefab_Float>();
            rpcPF.EndFloating();
            rpc.transform.SetParent(null);
            rpc.SetActive(false);

            // 그만 접근하기
            npc_A = npc_State.None;
            GameSceneManager.Instance.currentScene = GameSceneManager.Scenes.FindMatch;
        }
    }

    void Update()
    {
        if (currentPlayer < 0) currentPlayer = 0;


        // 모든 플레이어의 숫자 확인
        if (currentPlayer != previousPlayer)
        {
            // 다른 플레이어가 입장했을때
            if (currentPlayer > previousPlayer)
            {
                // 적당한 위치에 놓고
                rpc.transform.position = spawnPoint.position;

                // 활성화 
                rpc.SetActive(true);

                // 상태 전환하기
                npc_A = npc_State.GetCloser;
            }

            // 다른 플레이어가 빠져나갔을때
            if (currentPlayer < previousPlayer)
            {
                // 플레이어에게 접근 취소
                npc_A = npc_State.TakePhoto;
            }

            // 이전 플레이어 수 동기화
            previousPlayer = currentPlayer;
        }

        // RPC를 플레이어에게 접근시키기
        switch (npc_A)
        {
            case npc_State.GetCloser:
                Approach();
                break;

            case npc_State.TakePhoto:
                GameSceneManager.Instance.Take_A_Photo();
                break;

            case npc_State.GetAway:
                RunAway();
                break;
        }
    }
}
