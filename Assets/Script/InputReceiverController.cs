using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Kogane;
using Alchemy.Inspector;
using R3;

public class InputReceiverController : MonoBehaviour
{
    PlayerAction playerInput;
    TurnDirectionDecideColliderManager selectedFace;
    [LabelText("回転方向判定用コライダーのレイヤー")]
    public LayerMask directionDecideLayer;
    [LabelText("盤面上に存在するブロックのレイヤー")]
    public LayerMask blockLayer;
    [LabelText("メインカメラ")]
    public Camera mainCamera;
    [LabelText("Interface用ブロックを子に持つオブジェクト")]
    [SerializeField]
    (Vector3 gotObjectIndex, List<GameObject> getObject) rowTargetObjects = new(Vector3.zero, new List<GameObject>());
    (Vector3 gotObjectIndex, List<GameObject> getObject) columnTargetObjects = new(Vector3.zero, new List<GameObject>());

    void Awake()
    {
        playerInput = new PlayerAction();
        playerInput.Enable();

        InputSystemExtensions.PerformedAsObservable(playerInput.Player.Move)
        .SubscribeAwait(async (ctx, ct) =>
        {
            bool turnDirectionIsVertical = false;
            Vector3 rotateAxis = Vector3.zero;
            switch (ctx.ReadValue<Vector2>())
            {
                case var (x, _) when x < 0:
                    Debug.Log("Right");
                    (turnDirectionIsVertical, rotateAxis) = CastMoveDirectionDecideRay(transform.right);
                    break;
                case var (x, _) when x > 0:
                    Debug.Log("Left");
                    (turnDirectionIsVertical, rotateAxis) = CastMoveDirectionDecideRay(-transform.right);
                    break;
                case var (_, y) when y < 0:
                    Debug.Log("Down");
                    (turnDirectionIsVertical, rotateAxis) = CastMoveDirectionDecideRay(-transform.up);
                    break;
                case var (_, y) when y > 0:
                    Debug.Log("Up");
                    (turnDirectionIsVertical, rotateAxis) = CastMoveDirectionDecideRay(transform.up);
                    break;
            }

            if (turnDirectionIsVertical)
            {
                foreach (var item in rowTargetObjects.getObject)
                {
                    await item.GetComponent<BlockManager>().TurnBlock(rotateAxis);
                }
                BoardManager.instance.TurnBlock(rotateAxis, rowTargetObjects.gotObjectIndex);
            }
            else
            {
                foreach (var item in columnTargetObjects.getObject)
                {
                    await item.GetComponent<BlockManager>().TurnBlock(rotateAxis);
                }
                BoardManager.instance.TurnBlock(rotateAxis, columnTargetObjects.gotObjectIndex);
            }

        }, AwaitOperation.Drop)
        .AddTo(this);
    }

    void FixedUpdate()
    {
        rowTargetObjects.Item2.Clear();
        columnTargetObjects.Item2.Clear();

        if (playerInput.Player.Lookable.IsPressed())
        {
            Vector2 look = playerInput.Player.Look.ReadValue<Vector2>();
            transform.Rotate(look.y, look.x, 0);
        }

        Ray pivotRay = new Ray(transform.position, -transform.forward);
        if (Physics.Raycast(pivotRay, out RaycastHit hit, Mathf.Infinity, directionDecideLayer.value))
        {
            selectedFace = hit.collider.GetComponent<TurnDirectionDecideColliderManager>();
        }

        Ray cameraRay = mainCamera.ScreenPointToRay(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
        if (Physics.Raycast(cameraRay, out RaycastHit cameraHit, Mathf.Infinity, blockLayer.value))
        {
            Vector3 pivotPosition = cameraHit.collider.GetComponent<MouseOverDetectColliderManager>().objectArrayPosition;
            switch (pivotPosition)
            {
                case var (x, _, _) when x < 0:
                    // Positive X or Negative X
                    rowTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, -1, (int)pivotPosition.z);
                    columnTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, (int)pivotPosition.y, -1);
                    break;
                case var (_, y, _) when y < 0:
                    // Positive Y or Negative Y
                    rowTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, -1, (int)pivotPosition.z);
                    columnTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray((int)pivotPosition.x, -1, -1);
                    break;
                case var (_, _, z) when z < 0:
                    // Positive Z or Negative Z
                    rowTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray((int)pivotPosition.x, -1, -1);
                    columnTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, (int)pivotPosition.y, -1);
                    break;
            }
        }
    }

    (bool, Vector3) CastMoveDirectionDecideRay(Vector3 direction)
    {

        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, directionDecideLayer.value))
        {
            Debug.Log("Hit: " + hit.collider.name);
            return selectedFace.CaughtTurnDirection(hit.collider.gameObject);
        }

        else
        {
            Debug.LogError("CastMoveDirectionDecideRay: コライダーにヒットしませんでした");
            return (false, Vector3.zero);
        }
    }
}
