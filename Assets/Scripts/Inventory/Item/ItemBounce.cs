using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBounce : MonoBehaviour
{
    private Transform spriteTransform;

    private BoxCollider2D coll;

    public float gravity = -3.5f;

    private bool isGround;

    private float distance;

    private Vector2 direction;

    private Vector3 targetPos;

    private void Awake()
    {
        spriteTransform = transform.GetChild(0);
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
    }

    private void Update()
    {
        Bounce();
    }

    public void InitBounceItem(Vector3 target, Vector2 dir)
    {
        coll.enabled = false;
        direction = dir;
        targetPos = target;
        distance = Vector3.Distance(target, transform.position);

        spriteTransform.position += Vector3.up * 1.5f;
    }

    private void Bounce()
    {
        isGround = spriteTransform.position.y <= transform.position.y;

        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
        }

        if (!isGround)
        {
            spriteTransform.position += Vector3.up * gravity * Time.deltaTime;
        }
        else
        {
            spriteTransform.position = transform.position;
            coll.enabled = true;
        }
    }
}
