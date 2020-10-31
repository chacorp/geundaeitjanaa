using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefab_Float : MonoBehaviour
{
    public float floatingSpeed = 2f;
    public bool isFloating = false;
    enum Ptype
    {
        player,
        RPC,
        NPC
    }
    [SerializeField]
    Ptype playerType;

    Floater_wo_Physics FwP;
    PathFollower pf;
    GameObject follower;

    private void Start()
    {
        if (playerType == Ptype.NPC)
        {
            FwP = gameObject.GetComponent<Floater_wo_Physics>();
        }
        else
        {
            FwP = gameObject.GetComponentInChildren<Floater_wo_Physics>();
        }
        FwP.amplitude = 0.26f;
        FwP.enabled = false;

        // Pathfollower 가져오기
        follower = GameObject.FindGameObjectWithTag("Respawn");
        pf = follower.GetComponent<PathFollower>();
    }

    void Update()
    {
        if (isFloating)
        {
            switch (playerType)
            {
                case Ptype.player:
                    //위치 리셋
                    transform.SetParent(follower.transform);
                    //속도 설정
                    pf.speed = -floatingSpeed;
                    break;


                case Ptype.RPC:
                    break;


                case Ptype.NPC:
                    transform.parent.position += transform.parent.right * floatingSpeed * Time.deltaTime;
                    break;
            }
        }
    }

    public void EndFloating()
    {
        // 비활성화하기
        FwP.enabled = false;
        isFloating = false;

        switch (playerType)
        {
            case Ptype.player:
                transform.SetParent(null);
                //속도 설정
                pf.speed = 0;
                break;

            case Ptype.RPC:
                transform.localPosition = Vector3.up;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 물 바닥에 부딪혔을때
        if (collision.gameObject.CompareTag("Finish") && !isFloating)
        {
            isFloating = true;
            // <Floater_wo_Physics> 컴포넌트 활성화하기
            FwP.enabled = true;
        }
    }
}
