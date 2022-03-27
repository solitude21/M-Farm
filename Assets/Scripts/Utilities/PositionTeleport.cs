using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTeleport : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.MoveToPosition += OnMoveToPosition;
    }

    private void OnDisable()
    {
        EventHandler.MoveToPosition -= OnMoveToPosition;
    }

    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
}
