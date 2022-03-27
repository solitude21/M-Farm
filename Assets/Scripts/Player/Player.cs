using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;

    private bool inputDisable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }

    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        // TODO: 

        EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails);
    }

    private void OnMoveToPosition(Vector3 targetPosition) 
    {
        //throw new System.NotImplementedException();
        transform.position = targetPosition;
    }

    private void OnAfterSceneLoadEvent()
    {
        //throw new System.NotImplementedException();
        inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        //throw new System.NotImplementedException();
        inputDisable = true;
    }

    private void Update()
    {
        if (inputDisable == false)
            PlayerInput();
        else
            isMoving = false;
        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        if (inputDisable == false)
            Movement();
    }


    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        movementInput = new Vector2(inputX, inputY).normalized;

        // ��·״̬�ٶ�
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
            movementInput *= 0.5f;
        }


        isMoving = movementInput != Vector2.zero;
    }

    private void Movement() // ��ɫ�����ƶ�
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        foreach (var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

}
