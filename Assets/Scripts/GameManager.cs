using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class GameManager : MonoBehaviour
{
    static GameManager _S;
    static public GameManager S
    {
        get
        {
            if (_S != null) return _S;
            Debug.LogError("GameManager.cs : Trying to get singleton instance but it has not been set.");
            return null;
        }
        set
        {
            if (_S == null) _S = value;
            else Debug.LogError("GameManager.cs : Trying to set singleton instance but it already has a value.");
        }
    }


    UIManager uiManager;
    PlayerInput playerInput;
    InputAction touchPositionAction;
    public static Vector2 touchPosition
    {
        get
        {
            return S.mainCamera.cam.ScreenToWorldPoint(S.touchPositionAction.ReadValue<Vector2>());
        }
    }


    [Header("Scene Objects")]
    [SerializeField] CameraController mainCamera = null;
    [SerializeField] BackgroundManager bgManager = null;
    [SerializeField] MeshGenerator meshGen = null;
    [SerializeField] GameObjectPool ropePickupPool = null;
    [Space]
    [SerializeField] Ball ball = null;
    [SerializeField] Rope rope = null;
    


    [Header("Ropes")]
    [SerializeField] int nbRopesStart = 5;
    int nbRopes = 0;


    public enum GameState
    {
        none,
        menu,
        waitingToStart,
        touching,
        notTouching,
        gameOver
    }
    GameState _state;
    public static GameState State 
    {
        get { return S._state; }
        private set
        {
            S._state = value;

            switch (value)
            {
                case GameState.waitingToStart:
                    S.uiManager.ShowStartCanvas();
                    break;
                case GameState.touching:
                    S.uiManager.ShowGameCanvas();
                    break;
                case GameState.notTouching:
                    S.uiManager.ShowGameCanvas();
                    break;
                case GameState.gameOver:
                    S.uiManager.ShowGameOverCanvas();
                    break;
            }
        }

    }



    float maxBallHeight = 0.0f;

    [Space]
    [SerializeField] float distanceBetweenRopePickups = 10.0f;
    [SerializeField] float ropePickupSpawnHeightAboveBall = 5.0f;

    float lastRopePickupHeight = 0.0f;


    private void Awake()
    {
        //Singleton assignment
        S = this;

        playerInput = GetComponent<PlayerInput>();
        uiManager = GetComponent<UIManager>();
    }


    void Start()
    {
        // input setup , wire events and such
        playerInput.actions["Touch"].started += OnTouch;
        playerInput.actions["Touch"].canceled += OnTouchRelease;

        touchPositionAction = playerInput.actions["Position"];


        // init ropes
        nbRopes = nbRopesStart;

        // initial meshgen state (camera size, start position, etc.)
        InitMeshGeneratorState();

        // initial state
        State = GameState.waitingToStart;

        // setup the object pool for the rope pickups with a size of 10
        ropePickupPool.SetupPool(10);


    }



    /// <summary>
    /// Called when game is over to reset the game state.
    /// </summary>
    void ResetGame()
    {
        // position ball
        ball.gameObject.SetActive(true);
        ball.InitBall();

        // position rope 
        rope.InitializeRope(Vector3.zero, Vector3.zero);
        rope.gameObject.SetActive(false);

        // init nb of ropes
        nbRopes = nbRopesStart;

        // reset height
        maxBallHeight = 0;

        // Camera reset
        mainCamera.SetTargetPosition(Vector2.zero);

        // Reset backgrounds
        bgManager.InitBackgrounds();

        // Reset the rope pickup pool and height
        ropePickupPool.ReturnAllToPool();
        lastRopePickupHeight = 0.0f;

        // initial state
        State = GameState.waitingToStart;

        // reset display
        uiManager.DisplayRopes(nbRopesStart);
        uiManager.DisplayScore(0);

        // init mesh generator
        InitMeshGeneratorState();
        meshGen.SetupFirstObstacles();

    }



    void Update()
    {
        switch (State)
        {
            case GameState.waitingToStart:
                
                break;


            case GameState.touching:
                rope.ropeStart = touchPosition;
                break;


            case GameState.notTouching:
                if (ball.transform.position.y > maxBallHeight)
                {
                    maxBallHeight = ball.transform.position.y;
                    uiManager.DisplayScore((int)maxBallHeight);
                }
                mainCamera.SetTargetPosition(Vector2.up * maxBallHeight + Vector2.up);


                if (maxBallHeight + ropePickupSpawnHeightAboveBall > lastRopePickupHeight + distanceBetweenRopePickups)
                {
                    // get a new pickup from the pool
                    GameObject pickup = ropePickupPool.TakeFromPool();
                    if (pickup == null)
                    {
                        // return all pickups to pool and take from pool again   
                        ropePickupPool.ReturnAllToPool();
                        pickup = ropePickupPool.TakeFromPool();
                    }

                    // set pickup position to a suitable point, 10.0f units above the ball
                    pickup.transform.position = meshGen.GetPointBetweenObstacles(maxBallHeight + ropePickupSpawnHeightAboveBall, Random.Range(0.25f, 0.75f));

                    lastRopePickupHeight = maxBallHeight + ropePickupSpawnHeightAboveBall;
                }
                UpdateMeshGeneratorState();
                break;


            case GameState.gameOver:
                break;



            case GameState.none:
            default:
                Debug.LogError("GameManager.cs : Invalid or unaccounted for game state.", this);
                break;
        }


    }

    void UpdateMeshGeneratorState()
    {
        meshGen.cameraCurrentHeight = mainCamera.transform.position.y;

        // update difficulty variables here
    }

    void InitMeshGeneratorState()
    {
        meshGen.cameraCurrentHeight = 0.0f;
        meshGen.cameraSize = new Vector2(mainCamera.cam.orthographicSize * mainCamera.cam.aspect, mainCamera.cam.orthographicSize);
        meshGen.startingHeight = -mainCamera.cam.orthographicSize;

        // initialize the difficulty variables here
    }

    


    void OnTouch(InputAction.CallbackContext ctx)
    {
        if (nbRopes > 0 && (State == GameState.notTouching || State == GameState.waitingToStart))
        {
            rope.gameObject.SetActive(true);
            rope.InitializeRope(touchPosition, ball.transform.position);
            ball.AttachToRope(rope);


            State = GameState.touching;

            nbRopes--;
            uiManager.DisplayRopes(nbRopes);
        }
    }

    void OnTouchRelease(InputAction.CallbackContext ctx)
    {
        if (State != GameState.gameOver && State != GameState.waitingToStart)
        {
            rope.gameObject.SetActive(false);
            ball.DetachFromRope();

            State = GameState.notTouching;
        }
    }




    static public void RestartGame()
    {
        S.ResetGame();
    }



    public static void GainRopes(int nb, RopePickup pickup)
    {
        S.nbRopes += nb;
        S.uiManager.DisplayRopes(S.nbRopes);

        // return pickup to pool
        S.ropePickupPool.ReturnToPool(pickup.gameObject, true);
    }

    public static void GameOver()
    {
        State = GameState.gameOver;

        // deactivate ball
        S.ball.gameObject.SetActive(false);
    }




    private void OnGUI()
    {
        //GUIStyle labelStyle = GUI.skin.GetStyle("label");
        //labelStyle.fontSize = 50;
        //
        //GUIStyle buttonStyle = GUI.skin.GetStyle("button");
        //buttonStyle.fontSize = 50;
        //
        //GUILayout.Label("Remaining Ropes : " + nbRopes);
        //if (State == GameState.gameOver && GUILayout.Button("Restart game"))
        //{
        //    RestartGame();
        //}
        //
        //if (State == GameState.waitingToStart)
        //{
        //    GUILayout.Label("Touch to Start");
        //}

    }
}
