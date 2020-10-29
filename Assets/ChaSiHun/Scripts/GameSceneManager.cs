using UnityEngine;

// 1. 시작화면
//    -> 버튼 (메인메뉴)

// 2. 메인 메뉴 (캐릭터도 보여줌)
//    -> 게임 시작하기 버튼
//    -> 캐릭터 설정 버튼 
//       ->> 버튼 누르면 이동할 캐릭터 위치 값

// 3. 게임 시작 씬
//    -> 캐릭터 위치 값
//    -> 애니메이션: 수영장에 퐁당 빠지기

// 4. 매칭 시작하기
// -----------------------------------
//    -> 둥둥 떠다니기
// 5. 매칭 완료               
//    -> 다른 플레이어 다가옴

// 6. 근데있잖아 타임

public class GameSceneManager : MonoBehaviour
{
    // Class 싱글톤으로 만들기
    public static GameSceneManager Instance;
    GameSceneManager()
    {
        Instance = this;
    }

    [Header("메뉴")]
    public GameObject startGame_Scene;
    public GameObject mainMenu_Scene;
    public GameObject preference_Scene;
    public GameObject lookattheAlbum_Scene;

    [Header("위치")]
    public Transform cameraPos;
    public Transform mainmenuPos;
    public Transform joinPoolPos;

    [Header("플레이어")]
    public GameObject player;
    public float jumpP = 5f;

    [Header("카메라")]
    public Transform menuCam;
    public Transform playerCam;
    Vector3 camHolderPos;
    public bool useCamera;

    float rotAngleY = 0;
    float rotSpeed = 250f;
    float rotAngleX;
    float camView;
    int scroll = 1; // default is 1

    // perspective
    float[] fovArray = { 8.5f, 20f, 35f };
    // orthographic
    float[] sizeArray = { 1.25f, 3f, 5f };
    

    [Header("진행 단계")]
    // 플레이 시작!?
    public bool startButtonClicked;
    public bool playStart = false;


    // 기본 세팅
    private void Start()
    {
        // 시작버튼 안 눌린 상태
        startButtonClicked = false;

        // 카메라 사용 안함!
        useCamera = false;

        menuCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);


        // 플레이어 위치 초기화
        player.transform.position = mainmenuPos.position;
        player.transform.localEulerAngles = new Vector3(0, 180, 0);
        player.SetActive(false);


        // UI 셋팅화면 비활성화
        preference_Scene.SetActive(false);
        // 앨범 화면 비활성화
        lookattheAlbum_Scene.SetActive(false);
        // 시작화면 활성화
        startGame_Scene.SetActive(true);
        // 메인메뉴 비활성화
        mainMenu_Scene.SetActive(false);
    }

    // 1. 시작하기
    void OpenState()
    {
    }

    // 2. 메인 메뉴
    void MainMenuState()
    {
    }

    // 3. 매칭 풀장 들어가기
    void JoinMatchState()
    {
    }

    // 4. 다른 플레이어와 매칭 성공
    void MatchFoundState()
    {
    }


    // 시작화면 -> [StartGame] ======================= 버튼용 함수
    public void OnStartGameClicked()
    {
        //state = SceneState.mainMenu_State;
        startButtonClicked = true;

        playStart = false;

        menuCam.gameObject.SetActive(true);
        playerCam.gameObject.SetActive(false);

        // 시작화면 비활성화
        startGame_Scene.SetActive(false);
        // 메인메뉴 활성화
        mainMenu_Scene.SetActive(true);

        player.SetActive(true);
    }


    // 메인메뉴 -> [Join_the_Pool] =================== 버튼용 함수
    public void OnJointhePoolClicked()
    {
        // 시작!
        playStart = true;        
        playerCam.gameObject.SetActive(true);
        menuCam.gameObject.SetActive(false);

        // 카메라 사용함!
        useCamera = true;

        // 카메라 홀더 위치조절
        playerCam.parent = player.transform.GetChild(0).transform;
        playerCam.localPosition = camHolderPos;

        // 메인메뉴 비활성화
        mainMenu_Scene.SetActive(false);

        // 점프
        print("player jumped");
        Rigidbody playerR = player.GetComponent<Rigidbody>();
        Vector3 jumpIn = (player.transform.forward * -jumpP) + (player.transform.up * jumpP);
        playerR.AddForce(jumpIn, ForceMode.VelocityChange);
    }

    // 메인메뉴 -> [Look at the Album] =============== 버튼용 함수
    public void OnLookattheAlbumClicked()
    {
        lookattheAlbum_Scene.SetActive(true);
    }

    // 메인메뉴 -> [Preferences] ===================== 버튼용 함수
    public void OnPreferenceClicked()
    {
        preference_Scene.SetActive(true);
    }

    // 메인메뉴 -> [Back to the Game] ================ 버튼용 함수
    public void OnBacktoGameClicked()
    {
        lookattheAlbum_Scene.SetActive(false);
        preference_Scene.SetActive(false);
    }



    // 마우스 스크롤링
    int ScrollDirection()
    {
        float direction = Input.GetAxis("Mouse ScrollWheel");
        if (direction > 0)
        {
            scroll--;
        }
        else if (direction < 0)
        {
            scroll++;
        }
        // 선택범위 클램프
        scroll = Mathf.Clamp(scroll, 0, fovArray.Length - 1);

        return scroll;
    }

    // 카메라 회전 컨트롤 (1)
    void CameraRotateControl()
    {
        // 마우스 값 가져오기
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");

        rotAngleY += mX * rotSpeed * Time.deltaTime;
        rotAngleX -= mY * rotSpeed * Time.deltaTime;

        // 각도 클립핑
        // 이미 30도에서 시작하기 때문에 각도를 0 ~ 90으로 하고 싶다면 -30한 -30 ~ 60으로 해줘야함
        rotAngleY = rotAngleY > 360 ? 0 : rotAngleY;
        rotAngleX = Mathf.Clamp(rotAngleX, -10, 60);

        // 회전각도 갱신
        playerCam.localEulerAngles = new Vector3(rotAngleX, rotAngleY, playerCam.localEulerAngles.z);
    }
    
    // 카메라 뷰 컨트롤 (2)
    void CameraViewControl()
    {
        // 카메라 시점 받아오기
        camView = Camera.main.orthographic ? Camera.main.orthographicSize : Camera.main.fieldOfView;

        // 적용
        float[] viewArray = Camera.main.orthographic ? sizeArray : fovArray;
        camView = Mathf.Lerp(camView, viewArray[ScrollDirection()], Time.deltaTime);

        // Lerp 끝단 수렴하게 해주기
        if (camView >= viewArray[scroll] - 0.05f && camView <= viewArray[scroll] + .05f)
        {
            camView = viewArray[scroll];
        }

        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = camView;
        }
        else
        {
            Camera.main.fieldOfView = camView;
        }
    }



    private void Update()
    {
        // 카메라 사용여부 확인하기
        if (useCamera)
        {
            CameraRotateControl();
            CameraViewControl();
        }
    }
}
