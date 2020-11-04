using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    public static ScreenShot Instance;
    ScreenShot()
    {
        Instance = this;
    }

    // 게임화면(3D)을 보여주는 카메라
    Camera mainCam;

    // 지난 게임부터 현재까지 모든 게임에서 얻은 사진
    int saved_Photos = 0;

    // 현재 게임에서 얻은 사진
    public int current_Photos { get; private set; }

    public Sprite recentPhoto;// { get; private set; }

    private void Awake()
    {
        saved_Photos = PlayerPrefs.GetInt("photoNum", 0);
        mainCam = Camera.main;
    }

    public bool nowCapturing { get; private set; }

    public void CaptureScreen()
    {
        // 동기화 플래그 설정
        nowCapturing = true;

        // 화면 캡쳐를 위한 코루틴 시작
        StartCoroutine(RenderToTexture(Screen.width, Screen.height));
    }

    private IEnumerator RenderToTexture(int r_width, int r_height)
    {

        // 캡처는 WaitForEndOfFrame 이후에 해야 한다!
        // 그렇지 않으면 다 출력 되지 않은 상태의 화면을 보게 된다!
        yield return new WaitForEndOfFrame();

        //RenderTexture 생성
        RenderTexture rt = new RenderTexture(r_width, r_height, 24);

        //RenderTexture 저장을 위한 Texture2D 생성
        Texture2D screenShot = new Texture2D(r_width, r_height);//, TextureFormat.ARGB32, false);

        // 카메라에 RenderTexture 할당
        mainCam.targetTexture = rt;

        // 각 카메라가 보고 있는 화면을 랜더링 합니다.
        mainCam.Render();

        // read 하기 위해, 랜더링 된 RenderTexture를 RenderTexture.active에 설정
        RenderTexture.active = rt;

        // RenderTexture.active에 설정된 RenderTexture를 read 합니다.
        screenShot.ReadPixels(new Rect(0, 0, r_width, r_height), 0, 0);
        screenShot.Apply();

        // 캡쳐가 완료 되었습니다.
        // 이제 캡쳐된 Texture2D 를 가지고 원하는 행동을 하면 됩니다.
        recentPhoto = Sprite.Create(screenShot,
                                    new Rect(0, 0, r_width, r_height),
                                    new Vector2(0.5f, 0.5f)
                                    );

        GameSceneManager.Instance.ShowPhoto(recentPhoto);

        // File로 쓰고 싶다면 아래처럼 하면 됩니다.
        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + $"/../captured_{saved_Photos}.png", bytes);

        // 사진 갯수 누적!
        PlayerPrefs.SetInt("photoNum", ++saved_Photos);
        current_Photos++;

        // 사용한 것들 해제
        RenderTexture.active = null;
        mainCam.targetTexture = null;
        Destroy(rt);

        // 동기화 플래그 해제
        nowCapturing = false;

        yield return 0;
    }
}
