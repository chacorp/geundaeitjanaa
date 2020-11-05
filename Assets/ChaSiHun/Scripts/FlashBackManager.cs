using Boo.Lang;
using UnityEngine;
using UnityEngine.UI;

public class FlashBackManager : MonoBehaviour
{
    public GameObject flachBack_UI;
    public RectTransform filmRoll;
    public GameObject filmShot_Prefab;

    public bool playFlashBack = false;

    public List<GameObject> filmShots = new List<GameObject>();
    int limit = 700;
    // 올라가는 속도
    public float rollUpSpeed = 10f;


    private void Start()
    {
        flachBack_UI.SetActive(false);

        // 필름 롤의 시작 위치 잡기
        filmRoll.anchoredPosition = new Vector2(0, -limit);
    }

    public void AddMemory(Sprite memory)
    {
        // 사진 만들어서 필름 롤에 추가해놓기
        GameObject n_shot = Instantiate(filmShot_Prefab, filmRoll.transform);
        RectTransform n_shot_rt = n_shot.GetComponent<RectTransform>();

        // 자식(ScreenShot) 가져오기
        Transform n_frame = n_shot.transform.GetChild(0);

        // 위치 잡아두기
        n_shot_rt.anchoredPosition = new Vector2(0, (int)n_frame.GetComponent<RectTransform>().rect.height * -filmShots.Count);
        n_shot.transform.GetComponent<Image>().sprite = memory;

        // 리스트에 넣어두기
        filmShots.Add(n_shot);
    }

    public void FlashBack()
    {
        flachBack_UI.SetActive(true);

        filmRoll.anchoredPosition += new Vector2(0, rollUpSpeed * Time.deltaTime);

        // 일정 높이 만큼 올라갔다면, 꺼버리기
        if (filmRoll.anchoredPosition.y > (600 * filmShots.Count) + limit)
        {
            playFlashBack = false;
            filmRoll.anchoredPosition = Vector2.zero;

            flachBack_UI.SetActive(false);
        }
    }

    void Update()
    {
        if (playFlashBack) FlashBack();
    }
}
