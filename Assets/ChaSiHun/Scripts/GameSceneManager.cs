using UnityEngine;
using UnityEngine.UI;

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

[RequireComponent(typeof(ScreenShot))]
[RequireComponent(typeof(FlashBackManager))]
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
    public GameObject photoShot_UI;
    public GameObject jointhePool_UI;
    // 버튼
    public GameObject escape_btn;

    [Header("위치")]
    public Transform cameraPos;
    public Transform mainmenuPos;

    [Header("플레이어")]
    public GameObject player;
    public GameObject playerPrefab;
    public float jumpP = 5f;

    [Header("카메라")]
    Vector3 camHolderPos;
    Vector3 camHolderRot;
    public Transform menuCam;
    public Transform playerCam;
    public bool useCamera { get; private set; }

    float rotAngleY = 0;
    float rotSpeed = 250f;
    float rotAngleX;
    float camView;
    int scroll = 1; // default is 1

    [Header("진행 단계")]
    float[] viewArray;
    // perspective
    float[] fovArray = { 8.5f, 20f, 35f };
    // orthographic
    float[] sizeArray = { 1.25f, 3f, 5f };

    [Header("주마등")]
    FlashBackManager FBM;

    // Scene 전환
    public enum Scenes
    {
        GameStart,
        MainMenu,
        FindMatch,
        MatchFound
    }
    public Scenes currentScene;

    // 기본 시작 세팅
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

        #region UI 화면 설정
        // UI 화면들 비활성화
        preference_UI.SetActive(false);
        jointhePool_UI.SetActive(false);
        photoShot_UI.SetActive(false);
        // 앨범 화면 비활성화
        lookattheAlbum_UI.SetActive(false);
        // 시작화면 활성화
        startGame_UI.SetActive(true);
        // 메인메뉴 비활성화
        mainMenu_UI.SetActive(false);
        #endregion

        #region 플레이어 설정
        // 플레이어 위치 초기화
        ResetPlayer();
        player.SetActive(false);
        playerPrefab = player.transform.GetChild(0).gameObject;
        #endregion

        #region 컴포넌트 가져오기
        FBM = GetComponent<FlashBackManager>();
        #endregion
    }


    #region 마우스 컨트롤 및 기초 설정
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

    // 카메라 회전 컨트롤
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
        rotAngleX = Mathf.Clamp(rotAngleX, -15, 60);

        // 회전각도 갱신
        playerCam.localEulerAngles = new Vector3(rotAngleX, rotAngleY, playerCam.localEulerAngles.z);
    }

    // 카메라 뷰 컨트롤
    void CameraViewControl()
    {
        // 카메라 시점
        camView = Camera.main.orthographic ? Camera.main.orthographicSize : Camera.main.fieldOfView;
        // 카메라 시야
        viewArray = Camera.main.orthographic ? sizeArray : fovArray;

        // 값 바꿔주기
        camView = Mathf.Lerp(camView, viewArray[ScrollDirection()], Time.deltaTime);

        // Lerp 끝단에서 수렴하게 해주기
        if (camView >= viewArray[scroll] - 0.05f && camView <= viewArray[scroll] + .05f)
        {
            camView = viewArray[scroll];
        }

        // 바뀐 값 적용해주기
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = camView;
        }
        else
        {
            Camera.main.fieldOfView = camView;
        }
    }

    // 플레이어 리셋하기
    void ResetPlayer()
    {
        player.transform.position = mainmenuPos.position;
        player.transform.localEulerAngles = new Vector3(0, 180, 0);
        player.transform.GetChild(0).localPosition = Vector3.up;
    }
    #endregion

    #region 시작화면
    //======================================================================================
    // [StartGame] << 버튼 
    public void OnStartGameClicked()
    {
        // Scene 전환하기
        currentScene = Scenes.MainMenu;

        #region 카메라 설정
        menuCam.gameObject.SetActive(true);
        playerCam.gameObject.SetActive(false);
        #endregion

        #region UI 활성화
        // 시작화면 비활성화
        startGame_UI.SetActive(false);
        // 메인메뉴 활성화
        mainMenu_UI.SetActive(true);
        #endregion

        // 플레이어 캐릭터 활성화
        player.SetActive(true);
    }
    //======================================================================================
    #endregion

    #region 메인메뉴
    //======================================================================================
    // [Join_the_Pool] << 버튼
    public void OnJointhePoolClicked()
    {
        // Scene 전환하기
        currentScene = Scenes.FindMatch;

        #region 카메라 설정
        // 활성화할 카메라 선택
        playerCam.gameObject.SetActive(true);
        menuCam.gameObject.SetActive(false);

        // 카메라 사용함!
        useCamera = true;

        // 카메라 홀더 위치조절
        playerCam.SetParent(player.transform.GetChild(0).transform);
        playerCam.localEulerAngles = new Vector3(0, 180, 0);
        rotAngleX = playerCam.localEulerAngles.x;
        rotAngleY = playerCam.localEulerAngles.y;
        #endregion

        #region UI 활성화
        // 메인메뉴 비활성화
        mainMenu_UI.SetActive(false);
        jointhePool_UI.SetActive(true);
        #endregion

        #region 플레이어 캐릭터 점프!!
        Rigidbody playerR = player.GetComponent<Rigidbody>();
        //Rigidbody playerR = player.GetComponentInChildren<Rigidbody>();
        Vector3 jumpIn = (player.transform.forward * -jumpP) + (player.transform.up * jumpP);
        playerR.AddForce(jumpIn, ForceMode.VelocityChange);
        #endregion
    }

    // [Look at the Album] << 버튼
    public void OnLookattheAlbumClicked()
    {
        lookattheAlbum_UI.SetActive(true);
    }

    // [Back to the Game] << 버튼
    public void OnBacktoGameClicked()
    {
        lookattheAlbum_UI.SetActive(false);
        preference_UI.SetActive(false);
    }
    //======================================================================================
    #endregion

    #region 매칭 상대 찾기 
    //======================================================================================

    void FlashBack()
    {

    }

    // [Escape] << 버튼
    public void OnEscapeClicked()
    {
        #region 플레이어 설정
        // 플레이어 초기화
        Prefab_Float playerPF = player.GetComponent<Prefab_Float>();
        if (playerPF)
        {
            playerPF.EndFloating();
        }
        ResetPlayer();

        // 플레이어 비활성화
        player.SetActive(false);

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

        #region 카메라 위치 리셋 및 viewport 리셋
        // 카메라 사용 안함!
        useCamera = false;

        // 카메라 위치 리셋
        playerCam.SetParent(null);
        playerCam.transform.position = camHolderPos;
        playerCam.transform.localEulerAngles = camHolderRot;

        // 카메라 시점
        camView = Camera.main.orthographic ? Camera.main.orthographicSize : Camera.main.fieldOfView;
        // 카메라 시야
        viewArray = Camera.main.orthographic ? sizeArray : fovArray;
        // 스크롤 디폴트로 바꾸기
        scroll = 1;
        // 값 바꿔주기
        camView = viewArray[scroll];

        // 바꾼 값 적용하기
        if (Camera.main.orthographic)
        {
            Camera.main.orthographicSize = camView;
        }
        else
        {
            Camera.main.fieldOfView = camView;
        }
        #endregion

        // 만약 rpc가 있었다면, 없앤다!
        if (RPCManager.Instance.currentPlayer > 0)
        {
            RPCManager.Instance.currentPlayer--;
        }

        // 시작화면으로 초기화
        currentScene = Scenes.GameStart;

        // 주마등
        FlashBack();
    }

    public void ShowPhoto(Sprite input)
    {
        Image cap_img = photoShot_UI.transform.GetChild(0).gameObject.GetComponent<Image>();
        cap_img.sprite = input;

        // 사진 캡쳐 UI가 비활성화 되어있다면, 활성화하기
        if (!photoShot_UI.activeSelf && cap_img.sprite != null)
        {
            photoShot_UI.SetActive(true);
        }
    }

    public void Take_A_Photo()
    {
        // 스크린샷!
        // ScreenCapture.CaptureScreenshot($"CapturedPhoto{n_Photos++}.png");
        ScreenShot.Instance.CaptureScreen();
    }

    // [ X ] << 버튼
    public void OnClosePhotoShot()
    {
        RPCManager.Instance.rpc_A = RPCManager.rpc_State.GetAway;
        photoShot_UI.SetActive(false);
    }
    //======================================================================================
    #endregion

    #region 대화시작

    void FirstImpression()
    {

    }

    void KeepTalking()
    {

    }
    #endregion

    #region 전체 공통
    //======================================================================================
    // [Preferences] << 버튼
    public void OnPreferenceClicked()
    {
        preference_UI.SetActive(true);
    }
    //======================================================================================
    #endregion



    private void Update()
    {
        // 카메라 사용여부 확인하기
        if (useCamera)
        {
            CameraViewControl();
            if (Input.GetMouseButton(1))
                CameraRotateControl();
        }

        // 시작했을 때만 입력받기
        if (currentScene == Scenes.FindMatch)
        {
            // currentRPC = 참여하고 있는 플레이어 수 가져오기
            if (Input.GetKeyDown(KeyCode.A) && RPCManager.Instance.currentPlayer < RPCManager.Instance.MaxRPCs)
            {
                RPCManager.Instance.currentPlayer++;
            }
        }
        else if (currentScene == Scenes.MatchFound)
        {
            FirstImpression();

            if (Input.GetKeyDown(KeyCode.D) && RPCManager.Instance.currentPlayer > 0)
            {
                RPCManager.Instance.currentPlayer--;
            }
        }

        // 매칭 대기 <-> 매칭 완료 상황 별 [Escape] 버튼 조정하기 !!!
        if (currentScene == Scenes.MatchFound)
        {
            escape_btn.SetActive(false);
        }
        else if (currentScene == Scenes.FindMatch)
        {
            escape_btn.SetActive(true);
        }
    }
}
