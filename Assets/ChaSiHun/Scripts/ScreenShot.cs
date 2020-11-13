using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ScreenShot : MonoBehaviour
{
    public static ScreenShot Instance;
    ScreenShot()
    {
        Instance = this;
    }

    // 스크린 샷 저장할 경로
    static string directoryName = "Album";

    // 이번 게임에서 캡쳐한 스크린 샷들
    public List<Sprite> recentPhoto = new List<Sprite>();// { get; private set; }

    // 현재 게임에서 얻은 사진
    public int current_Photos { get; private set; }
    // 지난 게임부터 현재까지 모든 게임에서 찍은 사진의 갯수 (폴더에 있는 사진 갯수 아님)
    int saved_Photos = 0;
    // Album 폴더에 있는 사진의 갯수
    long existing_Photos;

    // 게임화면(3D)을 보여주는 카메라
    Camera mainCam;

    // 주마등을 위한 함수....!
    FlashBackManager FBM;


    // 현재는 사진이 추가될때마다 앨범에 있는거 전부 다 덮어써져서 중복된다
    // 일단 앨범 폴더에 있는 것을 추가하고, 
    // 스크린 샷 찍을 때마다 추가되는 것을 불러와야 한다!



    private void Awake()
    {
        saved_Photos = PlayerPrefs.GetInt("photoNum", 0);
        mainCam = Camera.main;
        FBM = GetComponent<FlashBackManager>();

        // 경로에 있는 png 갯수 가져오기
        existing_Photos = Directory.GetFiles(GetDirPath(), "*.png").Length;

        // 앨범에 png가 있다면 일단 앨범으로 가져오기
        PresetAlbumPNGs();
    }

    public bool nowCapturing { get; private set; }

    // 스크린 캡쳐하기
    public void CaptureScreen()
    {
        VerifyDirectory();

        // 동기화 플래그 설정
        nowCapturing = true;

        // 화면 캡쳐를 위한 코루틴 시작
        StartCoroutine(RenderToTexture());
    }

    // 화면 캡쳐 시작!
    private IEnumerator RenderToTexture()
    {
        int r_width = Screen.width;
        int r_height = Screen.height;
        Rect renderRect = new Rect(0, 0, r_width, r_height);

        // 캡처는 WaitForEndOfFrame 이후에 해야 한다!
        // 그렇지 않으면 다 출력 되지 않은 상태의 화면을 보게 된다!
        yield return new WaitForEndOfFrame();

        //RenderTexture 생성
        RenderTexture rt = new RenderTexture(r_width, r_height, 24);

        //RenderTexture 저장을 위한 Texture2D 생성
        Texture2D screenShot = new Texture2D(r_width, r_height);

        // 카메라에 RenderTexture 할당
        mainCam.targetTexture = rt;

        // 각 카메라가 보고 있는 화면을 랜더링 합니다.
        mainCam.Render();

        // read 하기 위해, 랜더링 된 RenderTexture를 RenderTexture.active에 설정
        RenderTexture.active = rt;

        // RenderTexture.active에 설정된 RenderTexture를 read 합니다.
        screenShot.ReadPixels(renderRect, 0, 0);
        screenShot.Apply();

        // 캡쳐가 완료 되었습니다.
        // 이제 캡쳐된 Texture2D 를 가지고 원하는 행동을 하면 됩니다.
        recentPhoto.Add(Sprite.Create(screenShot, renderRect, new Vector2(0.5f, 0.5f)));

        // 가장 마지막 sprite를 넣어두기
        GameSceneManager.Instance.ShowPhoto(recentPhoto[recentPhoto.Count - 1]);

        // 원하는 경로에 png파일로 저장하기
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(GetDirPath() + $"/captured_{saved_Photos}.png", bytes);

        // 사진 갯수 누적!
        PlayerPrefs.SetInt("photoNum", ++saved_Photos);
        current_Photos++;

        // FlashBack에 추가하기
        FBM.AddMemory(recentPhoto[recentPhoto.Count - 1]);

        // 사용한 것들 해제
        RenderTexture.active = null;
        mainCam.targetTexture = null;
        Destroy(rt);

        // 동기화 플래그 해제
        nowCapturing = false;

        yield return 0;
    }


    private void Update()
    {
    }

    #region 폴더 경로 가져오기

    static void VerifyDirectory()
    {
        string dir = GetDirPath();
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    static string GetDirPath()
    {
        return Application.dataPath + "/" + directoryName;
    }


    // Album 폴더에서 이미지 가져오기
    void PresetAlbumPNGs()
    {
        Rect textRect = new Rect(0, 0, Screen.width, Screen.height);
        byte[] fileData;

        // 경로에 있는 모든 ".png" 파일을 string으로 가져오기
        string[] PNGs = Directory.GetFiles(GetDirPath(), "*.png");

        // PNGs 속에 있는 각각의 string들에 대해서,
        foreach (string PNG in PNGs)
        {
            // 만약 PNG가 파일로 있다면
            if (File.Exists(PNG))
            {
                // PNG를 바이트로 읽어오고
                fileData = File.ReadAllBytes(PNG);

                // 새로운 텍스쳐 만들고
                Texture2D tex = new Texture2D((int)textRect.width, (int)textRect.height);

                // 텍스쳐에서 PNG 읽기
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.

                // FBM에 읽어낸 텍스쳐를 Sprite로 넘기기
                FBM.AddAlbum(Sprite.Create(tex, textRect, new Vector2(0.5f, 0.5f)));
            }
        }
    }
    #endregion
}
