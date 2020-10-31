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
    public GameObject startGame_UI;
    public GameObject mainMenu_UI;
    public GameObject preference_UI;
    public GameObject lookattheAlbum_UI;
    public GameObject jointhePool_UI;

    [Header("위치")]
    public Transform cameraPos;
    public Transform mainmenuPos;

    [Header("플레이어")]
    public GameObject player;
    public GameObject playerPrefab;
    public float jumpP = 5f;

    [Header("카메라")]
    public Transform menuCam;
    public Transform playerCam;
    Vector3 camHolderPos;
    Vector3 camHolderRot;
    public bool useCamera;

    float rotAngleY = 0;
    float rotSpeed = 250f;
    float rotAngleX;
    float camView;
    int scroll = 1; // default is 1

    [Header("진행 단계")]
    // perspective
    float[] fovArray = { 8.5f, 20f, 35f };
    // orthographic
    float[] sizeArray = { 1.25f, 3f, 5f };


    // 플레이 시작!?

    public enum Scenes
    {
        GameStart,
        MainMenu,
        FindMatch,
        MatchFound
    }
    public Scenes currentScene;

    // 기본 세팅
    private void Start()
    {
        // 시작버튼 안 눌린 상태
        currentScene = Scenes.GameStart;

        #region 카메라 설정
        // 카메라 사용 안함!
        useCamera = false;
        // 카메라 활성화 셋팅
        menuCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
        camHolderPos = playerCam.transform.position;
        camHolderRot = playerCam.transform.localEulerAngles;
        #endregion

        #region 플레이어 설정
        // 플레이어 위치 초기화
        ResetPlayer();
        player.SetActive(false);
        playerPrefab = player.transform.GetChild(0).gameObject;
        #endregion

        #region UI 화면 설정
        // UI 화면들 비활성화
        preference_UI.SetActive(false);
        jointhePool_UI.SetActive(false);
        // 앨범 화면 비활성화
        lookattheAlbum_UI.SetActive(false);
        // 시작화면 활성화
        startGame_UI.SetActive(true);
        // 메인메뉴 비활성화
        mainMenu_UI.SetActive(false);
        #endregion
    }

    void ResetPlayer()
    {
        player.transform.position = mainmenuPos.position;
        player.transform.localEulerAngles = new Vector3(0, 180, 0);
        player.transform.GetChild(0).localPosition = Vector3.up;
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
        currentScene = Scenes.MainMenu;

        menuCam.gameObject.SetActive(true);
        playerCam.gameObject.SetActive(false);

        // 시작화면 비활성화
        startGame_UI.SetActive(false);

        // 메인메뉴 활성화
        mainMenu_UI.SetActive(true);

        // 플레이어 캐릭터 활성화
        player.SetActive(true);
    }


    // 메인메뉴 -> [Join_the_Pool] =================== 버튼용 함수
    public void OnJointhePoolClicked()
    {
        // 시작!
        currentScene = Scenes.FindMatch;

        playerCam.gameObject.SetActive(true);
        menuCam.gameObject.SetActive(false);

        // 카메라 사용함!
        useCamera = true;

        // 카메라 홀더 위치조절
        playerCam.SetParent(player.transform.GetChild(0).transform);

        // 메인메뉴 비활성화
        mainMenu_UI.SetActive(false);
        jointhePool_UI.SetActive(true);

        // 점프
        //print("player jumped");
        Rigidbody playerR = player.GetComponent<Rigidbody>();
        //Rigidbody playerR = player.GetComponentInChildren<Rigidbody>();
        Vector3 jumpIn = (player.transform.forward * -jumpP) + (player.transform.up * jumpP);
        playerR.AddForce(jumpIn, ForceMode.VelocityChange);
    }

    // 메인메뉴 -> [Look at the Album] =============== 버튼용 함수
    public void OnLookattheAlbumClicked()
    {
        lookattheAlbum_UI.SetActive(true);
    }

    // 메인메뉴 -> [Back to the Game] ================ 버튼용 함수
    public void OnBacktoGameClicked()
    {
        lookattheAlbum_UI.SetActive(false);
        preference_UI.SetActive(false);
    }

    // 매칭 상황 -> [Escape] ========================= 버튼용 함수
    public void OnEscapeClicked()
    {
        // 카메라 사용 안함!
        useCamera = false;

        #region 플레이어 설정
        // 플레이어 초기화
        Prefab_Float playerPF = player.GetComponent<Prefab_Float>();
        if (playerPF)
        {
            playerPF.EndFloating();
        }
        ResetPlayer();

        playerCam.SetParent(null);
        playerCam.transform.position = camHolderPos;
        playerCam.transform.localEulerAngles = camHolderRot;
        #endregion

        #region UI 화면 설정
        // UI 화면들 비활성화
        preference_UI.SetActive(false);
        jointhePool_UI.SetActive(false);
        // 앨범 화면 비활성화
        lookattheAlbum_UI.SetActive(false);
        // 시작화면 활성화
        startGame_UI.SetActive(true);
        // 메인메뉴 비활성화
        mainMenu_UI.SetActive(false);
        #endregion

        // 플레이어 비활성화
        player.SetActive(false);

        // 만약 rpc가 있었다면, 없앤다!
        if (RPCManager.Instance.currentPlayer > 0)
        {
            RPCManager.Instance.currentPlayer--;
        }

        // 시작버튼 초기화
        currentScene = Scenes.GameStart;
    }


    // 공통 -> [Preferences] ========================= 버튼용 함수
    public void OnPreferenceClicked()
    {
        preference_UI.SetActive(true);
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
