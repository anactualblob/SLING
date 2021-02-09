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


    PlayerInput playerInput;
    InputAction touchPositionAction;
    public static Vector2 touchPosition
    {
        get
        {
            return S.mainCamera.cam.ScreenToWorldPoint(S.touchPositionAction.ReadValue<Vector2>());
        }
    }



    [SerializeField] CameraController mainCamera = null;
    [SerializeField] BackgroundManager bgManager = null;
    [Space]
    [SerializeField] Ball ball = null;
    [SerializeField] Rope rope = null;
    


    [Space]
    [SerializeField]
    int nbRopesStart = 5;
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
        }
        
    }



    float maxBallHeight = 0.0f;


    private void Awake()
    {
        //Singleton assignment
        S = this;    
    }


    void Start()
    {
        // input setup , wire events and such
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Touch"].started += OnTouch;
        playerInput.actions["Touch"].canceled += OnTouchRelease;

        touchPositionAction = playerInput.actions["Position"];


        // init ropes
        nbRopes = nbRopesStart;

        // initial state
        State = GameState.waitingToStart;

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

        // initial state
        State = GameState.waitingToStart;



        // etc.
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
                }
                mainCamera.SetTargetPosition(Vector2.up * maxBallHeight);
                break;

            case GameState.gameOver:
                break;



            case GameState.none:
            default:
                Debug.LogError("GameManager.cs : Invalid or unaccounted for game state.", this);
                break;
        }


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




    public void RestartGame()
    {
        ResetGame();
    }



    public static void GainRopes(int nb)
    {
        S.nbRopes += nb;
    }

    public static void GameOver()
    {
        State = GameState.gameOver;

        // deactivate ball
        S.ball.gameObject.SetActive(false);
    }




    private void OnGUI()
    {
        GUIStyle labelStyle = GUI.skin.GetStyle("label");
        labelStyle.fontSize = 50;
        
        GUIStyle buttonStyle = GUI.skin.GetStyle("button");
        buttonStyle.fontSize = 50;
        
        GUILayout.Label("Remaining Ropes : " + nbRopes);
        if (State == GameState.gameOver && GUILayout.Button("Restart game"))
        {
            RestartGame();
        }
        
        if (State == GameState.waitingToStart)
        {
            GUILayout.Label("Touch to Start");
        }

    }
}
