using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCManager : MonoBehaviour
{
    // 한 방에 들어올 수 있는 다른 플레이어의 수 최대값
    // 전체 플레이어의 수 - 로컬 플레이어 = 3명
    public int MaxRPCs = 3;

    // 현재 다른 플레이어의 수
    public int currentRPC = 0;
    // 이전 다른 플레이어의 수
    int previousRPC = 0;

    // 다른 플레이어의 캐릭터
    public GameObject RPCprefab;
    // 다른 플레이어의 캐릭터들 담아둘 리스트
    List<GameObject> RPC_activeFalse = new List<GameObject>();

    public Transform spawnPoint;
    public Transform targetPoint;

    void Start()
    {
        for (int i = 0; i < MaxRPCs; i++)
        {
            // 1. 다른 플레이어의 캐릭터 만들어서,
            GameObject rpc = Instantiate(RPCprefab);

            // 2. 비활성화(안보이게하기)하고
            rpc.SetActive(false);

            // 3. 리스트에 넣어두기
            RPC_activeFalse.Add(rpc);
        }
    }

    void Update()
    {
        // currentRPC = 참여하고 있는 플레이어 수 가져오기
        if (Input.GetMouseButtonDown(1))
        {
            currentRPC++;
        }
        
        // 코루틴 사용해서 RPC 추가하는 것으로 바꾸기
        

        // 다른 플레이어의 숫자 확인
        if (currentRPC != previousRPC)
        {
            // 처음에 다 들어왔을때
            if (currentRPC > previousRPC)
            {
                // 리스트에서 캐릭터 하나 빼와서 
                GameObject rpc = RPC_activeFalse[0];
                RPC_activeFalse.Remove(rpc);
                // 적당한 위치에 가져오고
                rpc.transform.position = spawnPoint.position;
                // 활성화 
                rpc.SetActive(true);

                // 따라갈 타겟 설정해주기
                FollowingTarget ft = rpc.GetComponent<FollowingTarget>();
                ft.target = targetPoint.GetChild(currentRPC).transform;

                // 이전 플레이어 수 동기화
                previousRPC = currentRPC;
            }

            // 빠져나갔을때
            if (currentRPC < previousRPC)
            {
                // 나가기 -> 비활성화
            }

            // 이전 플레이어 수 동기화
            previousRPC = currentRPC;
        }
    }
}
