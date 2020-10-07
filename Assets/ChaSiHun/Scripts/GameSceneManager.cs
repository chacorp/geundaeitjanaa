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
    public Transform characterSettingPos;
    public Transform waterPoolPos;

    [Header("플레이어")]
    public GameObject player;
    public GameObject playerTube;

    [Header("카메라")]
    public Transform CamHolder;
    public bool rotateCamera;
    float rotAngleY = 0;
    float rotSpeed = 250f;
    float rotAngleX;
    float FOV;
    int selectFoV = 1; // default is 1
    float[] fovArray = { 25, 50, 75 };

    [Header("진행 단계")]
    public bool playStart = false;
    // 진행단계
    public enum SceneStage
    {
        openScene,
        mainMenuScene,
        JoinMatchScene,
        matchFound,
    }
    public SceneStage stage;

    // 초기 세팅
    private void Awake()
    {
    }

    // 시작 세팅
    private void Start()
    {
        stage = SceneStage.openScene;
        
    }

    void OpenScene()
    {
        // 시작화면 활성화
        startPage.SetActive(true);
        // 메인메뉴 비활성화
        mainMenu.SetActive(false);

        // 플레이어 프리팹 비활성화
        player.transform.GetChild(0).gameObject.SetActive(false);
        // 화면 돌리기 비활성화
        rotateCamera = false;
    }

    // 시작화면용 메인메뉴 입장 함수
    public void OnGameEnter()
    {
        // 플레이어 프리팹 활성화
        player.transform.GetChild(0).gameObject.SetActive(true);
        // 플레이어 위치 초기화
        player.transform.position = characterSettingPos.position;
        // 플레이어 튜브 비활성화
        playerTube.SetActive(false);

        // 시작화면 비활성화
        startPage.SetActive(false);
        // 메인메유 활성화
        mainMenu.SetActive(true);
    }

    // 메인메뉴용 매칭 입장 함수
    public void OnGameStart()
    {
        // 화면 돌리기 활성화
        rotateCamera = true;
        // 메인메뉴 활성화
        mainMenu.SetActive(false);

        // 플레이어 위치 갱신
        player.transform.position = waterPoolPos.position;
        // 플레이어 튜브 켜두기
        playerTube.SetActive(true);
        // 시작!
        playStart = true;
    }


    // 카메라 돌리기
    void CameraRotateControl()
    {
        // 마우스 값 가져오기
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");

        rotAngleY += mX * rotSpeed * Time.deltaTime;
        rotAngleX -= mY * rotSpeed * Time.deltaTime;

        // 각도 클립핑
        rotAngleY = rotAngleY > 360 ? 0 : rotAngleY;
        rotAngleX = Mathf.Clamp(rotAngleX, -10, 60);

        // 회전각도 갱신
        CamHolder.localEulerAngles = new Vector3(rotAngleX, rotAngleY, CamHolder.localEulerAngles.z);
    }

    void CameraFOVControl()
    {
        // 마우스 스크롤 방향 가져오기
        float direction = Input.GetAxis("Mouse ScrollWheel");
        if (direction > 0)
        {
            selectFoV--;
        }
        else if (direction < 0)
        {
            selectFoV++;
        }
        // 선택범위 클램프
        selectFoV = Mathf.Clamp(selectFoV, 0, fovArray.Length - 1);

        // 적용
        FOV = Camera.main.fieldOfView;
        FOV = Mathf.Lerp(FOV, fovArray[selectFoV], Time.deltaTime);

        // Lerp 끝단 수렴하게 해주기
        if (FOV >= fovArray[selectFoV] - 0.05f && FOV <= fovArray[selectFoV] + .05f)
        {
            FOV = fovArray[selectFoV];
        }
        Camera.main.fieldOfView = FOV;


    }

    // 반복 함수
    private void Update()
    {
        if (rotateCamera)
        {
            CameraRotateControl();
            CameraFOVControl();
        }
    }
}
