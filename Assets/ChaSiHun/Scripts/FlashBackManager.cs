using Boo.Lang;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FlashBackManager : MonoBehaviour
{
    [Header("FlashBack")]
    public GameObject flachBack_UI;
    public RectTransform filmRoll;
    public GameObject filmShot_Prefab;


    [Header("LookAtTheAlbum")]
    public Transform albumContent;
    public GameObject albumImg_prefab;

    public bool playFlashBack = false;

    public List<GameObject> filmShots = new List<GameObject>();
    int limit = 700;
    // 올라가는 속도
    public float rollUpSpeed = 200f;

    float delay;

    private void Start()
    {
        flachBack_UI.SetActive(false);

        // 필름 롤의 시작 위치 잡기
        filmRoll.anchoredPosition = new Vector2(0, -limit);
    }
    
    public void AddAlbum(Sprite memory)
    {
        // 사진을 담을 gameobject 만들기
        GameObject n_album = Instantiate(albumImg_prefab, albumContent);
        RectTransform n_album_rt = n_album.GetComponent<RectTransform>();

        int n_album_H = 900;
        // 위치 잡기
        n_album_rt.anchoredPosition = new Vector2(0, -n_album_H * (albumContent.childCount - 1) - 450);

        // 앨범 크기 늘려주기
        if (albumContent.childCount > 1) albumContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, n_album_H);

        // 사진 넣기
        n_album.transform.GetComponent<Image>().sprite = memory;
    }

    public void AddMemory(Sprite memory)
    {
        // 사진 만들어서 필름 롤에 추가해놓기
        GameObject n_shot = Instantiate(filmShot_Prefab, filmRoll.transform);
        RectTransform n_shot_rt = n_shot.GetComponent<RectTransform>();

        // 이미지 높이 가져오기
        int n_shot_H = (int)n_shot.transform.GetChild(0).GetComponent<RectTransform>().rect.height;

        // 위치 잡아두기
        n_shot_rt.anchoredPosition = new Vector2(0, -n_shot_H * filmShots.Count);         

        // 이미지에 sprite 넣기
        n_shot.transform.GetComponent<Image>().sprite = memory;

        // 리스트에 넣어두기
        filmShots.Add(n_shot);
    }
    public void FlashBack()
    {
        flachBack_UI.SetActive(true);

        if (filmShots.Count == 0)
        {
            delay += Time.deltaTime;
            if (delay > 2)
            {
                EmptyFilms();
            }
        }
        else
        {
            // 속도 조절
            filmRoll.anchoredPosition += new Vector2(0, rollUpSpeed * Time.deltaTime);

            // 일정 높이 만큼 올라갔다면, 꺼버리기
            if (filmRoll.anchoredPosition.y > (600 * filmShots.Count) + limit)
            {
                EmptyFilms();
            }
        }
    }

    void EmptyFilms()
    {
        // 위치 리셋
        filmRoll.anchoredPosition = new Vector2(0, -limit);
        // 비활성화
        flachBack_UI.SetActive(false);
        // 비우기
        if(filmShots.Count > 0) filmShots.Clear();

        // 메인 메뉴로 돌아가!
        GameSceneManager.Instance.OnStartGameClicked();
        //GameSceneManager.Instance.currentScene = GameSceneManager.Scenes.MainMenu;
        playFlashBack = false;
    }

    void Update()
    {
        if (playFlashBack) FlashBack();
    }
}
