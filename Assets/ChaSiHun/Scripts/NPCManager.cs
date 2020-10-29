using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<Transform> npcWait = new List<Transform>();
    public List<Vector3> npcPos = new List<Vector3>();

    public float JumpP = 5f;
    public float MoveP = 5f;
    int n_idx = 0;

    float timer = 0;
    enum Sequence
    {
        jump,
        jumped,
        moveOn,
        reset,
    }
    [SerializeField]
    Sequence Step;
    Transform npcJ_T;
    Rigidbody npcJ_R;

    void Start()
    {
        // 각 위치 저장해두기
        for (int i = 0; i < npcWait.Count; i++)
        {
            npcPos.Add(npcWait[i].position);
        }
        Step = Sequence.jump;
    }
    void NPCJUMP()
    {
        if (GameSceneManager.Instance.startButtonClicked) return;

        switch (Step)
        {
            case Sequence.jump:
                // 1. 첫번째 녀석 점프
                npcJ_T = npcWait[0];
                npcJ_R = npcJ_T.GetComponent<Rigidbody>();
                npcJ_R.AddForce(transform.forward * JumpP + transform.up * JumpP, ForceMode.VelocityChange);
                Step = Sequence.jumped;
                break;

            case Sequence.jumped:
                // 2. 기다리기
                timer += Time.deltaTime;
                if (timer > 1)
                {
                    Step = Sequence.moveOn;
                    timer = 0;
                }
                break;

            case Sequence.moveOn:
                // 3. 리스트에서 옮기기
                npcWait.Remove(npcJ_T);

                // 4. 뒤에 녀석들 하나씩 앞으로 땡기기
                //Vector3 dir = npcPos[n_idx] - npcWait[n_idx].position;
                //npcWait[n_idx].transform.position += dir * Time.deltaTime;
                // npcWait[n_idx].Translate(dir * Time.deltaTime);
                npcWait[n_idx].transform.position = Vector3.Lerp(npcWait[n_idx].transform.position, npcWait[n_idx].position, MoveP * Time.deltaTime);
                if (npcWait[n_idx].position == npcPos[n_idx])
                {
                    npcWait[n_idx].position = npcWait[n_idx].position;
                    n_idx++;
                    Step = n_idx >= npcWait.Count ? Sequence.reset : Sequence.moveOn;
                }
                break;

            case Sequence.reset:
                // 5. 점프한 첫번째 맨 뒤로 옮기기
                npcWait.Add(npcJ_T);
                npcJ_T.position = npcPos[npcWait.IndexOf(npcJ_T)];
                break;
        }
    }


    void Update()
    {
        NPCJUMP();
    }
}
