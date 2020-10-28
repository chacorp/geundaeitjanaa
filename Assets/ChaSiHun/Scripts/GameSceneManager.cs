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
    public GameObject startPage;
    public GameObject mainMenu;

    [Header("위치")]
    public Transform cameraPos;
    public Transform mainmenuPos;
    public Transform joinPoolPos;

    [Header("플레이어")]
    public GameObject player;
    public GameObject playerTube;
    public float jumpP = 50;

    [Header("카메라")]
    public Transform menuCam;
    public Transform playerCam;
    Vector3 camHolderPos;
    public bool useCamera;
    float rotAngleY = 0;
    float rotSpeed = 250f;
    float rotAngleX;
    float camView;
    int select = 1; // default is 1
    // perspective
    float[] fovArray = { 8.5f, 20f, 35f };
    // orthographic
    float[] sizeArray = { 1.25f, 3f, 5f };
    

    [Header("진행 단계")]
    // 플레이 시작!?
    public bool playStart = false;
    public bool startButtonClicked;
    // 진행단계
    public enum SceneState
    {
        open_State,         // 시작하기
        mainMenu_State,     // 메인 메뉴
        joinMatch_State,    // 매칭 풀장 들어가기
        matchFound_State,   // 매칭 완료
        endMatch_State      // 매칭 끝내기
    }
    public SceneState state;


    // 기본 세팅
    private void Start()
    {
        startButtonClicked = false;

        // 카메라 사용 안함!
        useCamera = false;

        menuCam.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);

        // 진행 단계 => 시작하기
        state = SceneState.open_State;

        // 플레이어 위치 초기화
        player.transform.position = mainmenuPos.position;
        player.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    // 1. 시작하기
    void OpenState()
    {
        // 시작화면 활성화
        startPage.SetActive(true);

        // 메인메뉴 비활성화
        mainMenu.SetActive(false);

        // 화면 돌리기 비활성화
        useCamera = false;
    }

    // 2. 메인 메뉴
    void MainMenuState()
    {
        playStart = false;

        menuCam.gameObject.SetActive(true);
        playerCam.gameObject.SetActive(false);


        // 시작화면 비활성화
        startPage.SetActive(false);
        // 메인메뉴 활성화
        mainMenu.SetActive(true);
    }

    // 3. 매칭 풀장 들어가기
    void JoinMatchState()
    {
        // 카메라 홀더 위치조절
        playerCam.parent = player.transform.GetChild(0).transform;
        playerCam.localPosition = camHolderPos;

        // 카메라 사용함!
        useCamera = true;

        // 메인메뉴 비활성화
        mainMenu.SetActive(false);

        // 플레이어 튜브 켜두기
        playerTube.SetActive(true);
    }

    // 4. 다른 플레이어와 매칭 성공
    void MatchFoundState()
    {

    }


    // 시작화면 -> [StartGame] 버튼용 함수
    public void OnGameOpen()
    {
        state = SceneState.mainMenu_State;
        startButtonClicked = true;
    }

    // 메인메뉴 -> [Join_the_Pool] 버튼용 함수
    public void OnGameJoin()
    {
        state = SceneState.joinMatch_State;

        // 시작!
        playStart = true;        
        playerCam.gameObject.SetActive(true);
        menuCam.gameObject.SetActive(false);

        // 점프
        print("jump");
        Rigidbody playerR = player.GetComponent<Rigidbody>();
        Vector3 jumpIn = (player.transform.forward * -jumpP) + (player.transform.up * jumpP);
        playerR.AddForce(jumpIn, ForceMode.VelocityChange);
    }

    // 마우스 스크롤링
    int ScrollDirection()
    {
        float direction = Input.GetAxis("Mouse ScrollWheel");
        if (direction > 0)
        {
            select--;
        }
        else if (direction < 0)
        {
            select++;
        }
        // 선택범위 클램프
        select = Mathf.Clamp(select, 0, fovArray.Length - 1);

        return select;
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
        if (camView >= viewArray[select] - 0.05f && camView <= viewArray[select] + .05f)
        {
            camView = viewArray[select];
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

        // 진행단계에 따라 전환하기
        switch (state)
        {
            // 처음 시작할 때
            case SceneState.open_State:
                OpenState();
                break;

            // 메인메뉴 들어갈 때
            case SceneState.mainMenu_State:
                MainMenuState();
                break;

            // 풀장에 들어갈 때 => 매칭시작
            case SceneState.joinMatch_State:
                JoinMatchState();
                break;

            // 매칭 되었을 때
            case SceneState.matchFound_State:
                MatchFoundState();
                break;

            case SceneState.endMatch_State:
                break;

            // 그냥 디폴트(암것도 안함)
            default:
                break;
        }
    }
}
