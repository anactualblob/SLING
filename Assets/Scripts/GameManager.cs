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


    [SerializeField] CameraController mainCamera = null;
    [Space]
    [SerializeField] Ball ball = null;
    [SerializeField] Rope rope = null;
    

    public static Vector2 touchPosition
    {
        get
        {
            return S.mainCamera.cam.ScreenToWorldPoint(S.touchPositionAction.ReadValue<Vector2>());
        }
    }


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
        S = this;    
    }


    void Start()
    {
        // input setup , wire events and such
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Touch"].started += OnTouch;
        playerInput.actions["Touch"].canceled += OnTouchRelease;

        touchPositionAction = playerInput.actions["Position"];


        nbRopes = nbRopesStart;


        State = GameState.notTouching;
    }



    void Update()
    {
        // if state = swinging, rope.ropeStart = touchPosition
        //if (State == GameState.touching)
        //{
        //    rope.ropeStart = touchPosition;
        //}
        //
        //if (State == GameState.notTouching)
        //{
        //
        //}

        Debug.Log(nbRopes);


        switch (State)
        {
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
                Debug.Log("GameOver !");
                break;



            case GameState.none:
            default:
                Debug.LogError("GameManager.cs : Invalid or unaccounted for game state.", this);
                break;
        }
    }



    void OnTouch(InputAction.CallbackContext ctx)
    {
        if (nbRopes > 0)
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
        if (State != GameState.gameOver)
        {
            rope.gameObject.SetActive(false);
            ball.DetachFromRope();

            State = GameState.notTouching;
        }
    }


    public static void GainRopes(int nb)
    {
        S.nbRopes += nb;
    }

    public static void GameOver()
    {
        State = GameState.gameOver;
    }
}
