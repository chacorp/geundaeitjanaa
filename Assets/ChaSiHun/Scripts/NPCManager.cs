using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public Transform NPCs;
    public List<Transform> npcWait = new List<Transform>();
    public List<Vector3> npcPos = new List<Vector3>();

    public float JumpP = 5f;
    public float MoveP = 2.5f;
    public float delayTime = 1f;

    //[SerializeField]
    int n_idx = 0;
    float timer = 0;

    enum Sequence
    {
        jump,
        delay,
        moveOn,
        loop,
        reset
    }
    Sequence Step;

    Transform npcJ_T;
    Rigidbody npcJ_R;

    void Start()
    {
        // NPC의 자식 오브젝트들 가져오기
        for (int i = 0; i < NPCs.childCount; i++) npcWait.Add(NPCs.GetChild(i));

        // 각 위치 저장해두기
        for (int i = 0; i < npcWait.Count; i++) npcPos.Add(npcWait[i].position);

        Step = Sequence.jump;
    }
    void NPCJUMP()
    {
        // [StartGame] 버튼이 눌렸다면, 리셋하기
        if (GameSceneManager.Instance.currentScene == GameSceneManager.Scenes.MainMenu) Step = Sequence.reset;

        // 만약 [Escape] 버튼을 눌렀고, 스텝이 '리셋'이라면, 다시 루프 돌리기 바꾸기
        else if (GameSceneManager.Instance.currentScene == GameSceneManager.Scenes.GameStart && Step == Sequence.reset) Step = Sequence.jump;

        Prefab_Float nFwP;

        // NPC 애니매이션 스텝!!
        switch (Step)
        {
            case Sequence.jump:
                // 1. 첫번째 녀석 점프
                npcJ_T = npcWait[0];
                npcJ_R = npcJ_T.GetComponentInChildren<Rigidbody>();
                npcJ_R.AddForce(transform.forward * JumpP + transform.up * JumpP, ForceMode.VelocityChange);

                // 2. 리스트에서 옮기기
                npcWait.Remove(npcJ_T);
                Step = Sequence.delay;
                break;

            case Sequence.delay:
                // 3. 기다리기
                timer += Time.deltaTime;
                if (timer > delayTime)
                {
                    Step = Sequence.moveOn;
                    timer = 0;
                }
                break;

            case Sequence.moveOn:

                // 4. 뒤에 녀석들 하나씩 앞으로 땡기기
                npcWait[n_idx].transform.position = Vector3.Lerp(npcWait[n_idx].transform.position, npcPos[n_idx], MoveP * Time.deltaTime);

                if (Vector3.Distance(npcWait[n_idx].position, npcPos[n_idx]) < .1f)
                {
                    npcWait[n_idx].position = npcWait[n_idx].position;
                    n_idx++;
                    // 이터레이션이 끝났다면, 다음 단계로 넘기기
                    Step = n_idx >= npcWait.Count ? Sequence.loop : Step;
                }
                break;

            case Sequence.loop:
                // 5. 점프한 첫번째 맨 뒤로 옮기기
                npcWait.Add(npcJ_T);
                npcJ_T.position = npcPos[npcWait.IndexOf(npcJ_T)];
                npcJ_T.GetChild(0).localPosition = Vector3.up;

                nFwP = npcJ_T.GetComponentInChildren<Prefab_Float>();
                if (nFwP) nFwP.EndFloating();

                // 6. 인덱스 리셋
                n_idx = 0;
                Step = Sequence.jump;
                break;

            case Sequence.reset:
                // 점프 뛴 녀석이 있다면
                if (npcJ_T)
                {
                    // 두둥실 없애기
                    nFwP = npcJ_T.GetComponentInChildren<Prefab_Float>();
                    if (nFwP) nFwP.EndFloating();

                    // 다시 대기열에 넣어주고
                    npcWait.Add(npcJ_T);

                    // 뛴 녀석은 이제 없다
                    npcJ_T = null;
                }
                // 위치 리셋!
                foreach (Transform n in npcWait)
                {
                    n.GetChild(0).localPosition = Vector3.up;
                    n.position = npcPos[npcWait.IndexOf(n)];
                }
                n_idx = 0;
                break;
        }
    }

    // 맨 앞에 있는 npc 녀석을 안 보이게 하고, 그 자리에 플레이어를 표시하기 위한 함수
    void NPC_ACTIVE_CONTROLL()
    {
        if ((int)GameSceneManager.Instance.currentScene > 0)
        {
            // 맨 앞자리 녀석 비활성화
            if (npcWait[0].gameObject.activeSelf) npcWait[0].gameObject.SetActive(false);
        }
        else
        {
            // 맨 앞자리 녀석 활성화
            if (!npcWait[0].gameObject.activeSelf) npcWait[0].gameObject.SetActive(true);
        }
    }

    void Update()
    {
        NPC_ACTIVE_CONTROLL();
        NPCJUMP();
    }
}
