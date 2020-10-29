using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<Transform> npcWait = new List<Transform>();
    public List<Vector3> npcPos = new List<Vector3>();

    public float JumpP = 5f;
    public float MoveP = 2.5f;

    int n_idx = 0;
    float timer = 0;

    enum Sequence
    {
        jump,
        jumped,
        moveOn,
        loop,
        reset
    }
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
        if (GameSceneManager.Instance.startButtonClicked) Step = Sequence.reset;

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
                npcWait[n_idx].transform.position = Vector3.Lerp(npcWait[n_idx].transform.position, npcPos[n_idx], MoveP * Time.deltaTime);

                if (Vector3.Distance(npcWait[n_idx].position, npcPos[n_idx]) < 1)
                {
                    npcWait[n_idx].position = npcWait[n_idx].position;
                    n_idx++;

                    // 다음 단계로 넘기기
                    Step = n_idx >= npcWait.Count ? Step++ : Step;
                }
                break;

            case Sequence.loop:
                // 5. 점프한 첫번째 맨 뒤로 옮기기
                npcWait.Add(npcJ_T);
                npcJ_T.position = npcPos[npcWait.IndexOf(npcJ_T)];

                // 6. 인덱스 리셋
                n_idx = 0;
                Step = Sequence.jump;
                break;

            case Sequence.reset:
                // 점프 뛴 녀석이 있다면
                if (npcJ_T != null)
                {
                    // 다시 대기열에 넣어주고
                    npcWait.Add(npcJ_T);
                    // 위치 리셋!
                    foreach (Transform n in npcWait) n.position = npcPos[npcWait.IndexOf(n)];
                    // 뛴 녀석은 이제 없다
                    npcJ_T = null;
                }
                break;
        }
    }

    void MainMenu()
    {
        if (GameSceneManager.Instance.startButtonClicked)
        {

        }
    }

    void Update()
    {
        NPCJUMP();
    }
}
