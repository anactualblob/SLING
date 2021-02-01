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

    [SerializeField] Ball ball = null;
    [SerializeField] Rope rope = null;
    [SerializeField] Camera mainCamera = null;


    bool touching = false;
    public static Vector2 touchPosition
    {
        get
        {
            return S.mainCamera.ScreenToWorldPoint(S.touchPositionAction.ReadValue<Vector2>());
        }
    }


    InputAction touchPositionAction;

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
    }



    void Update()
    {
        // if state = swinging, rope.ropeStart = touchPosition
        if (touching)
        {
            rope.ropeStart = touchPosition;
        }
    }



    void OnTouch(InputAction.CallbackContext ctx)
    {

        //Debug.Log("touch");
        
        rope.gameObject.SetActive(true);
        rope.InitializeRope(Vector3.zero, ball.transform.position);
        ball.AttachToRope(rope);
        
        touching = true;

        // substract 1 from remaining ropes
        // set state to swinging
        // etc.
    }


    void OnTouchRelease(InputAction.CallbackContext ctx)
    {
        //Debug.Log("touch release");
        rope.gameObject.SetActive(false);
        ball.DetachFromRope();

        touching = false;
    }



}
