using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Kogane;
using Alchemy.Inspector;
using R3;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks;

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
    (List<Vector3> getObjectIndex, List<GameObject> getObject) rowTargetObjects = new(new List<Vector3>(), new List<GameObject>());
    (List<Vector3> getObjectIndex, List<GameObject> getObject) columnTargetObjects = new(new List<Vector3>(), new List<GameObject>());

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
                    Debug.Log("Left");
                    (turnDirectionIsVertical, rotateAxis) = CastMoveDirectionDecideRay(-transform.right);
                    break;
                case var (x, _) when x > 0:
                    Debug.Log("Right");
                    (turnDirectionIsVertical, rotateAxis) = CastMoveDirectionDecideRay(transform.right);
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
            // Index取り忘れてる
            (List<Vector3> getObjectIndex, List<GameObject> getObject) targetObjectsArray = (new List<Vector3>(), new List<GameObject>());
            List<UniTask> targetObjectTurnCommands = new List<UniTask>();

            if (turnDirectionIsVertical) targetObjectsArray = rowTargetObjects;
            else targetObjectsArray = columnTargetObjects;

            foreach (var item in targetObjectsArray.getObject)
            {
                targetObjectTurnCommands.Add(item.GetComponent<BlockManager>().TurnBlock(rotateAxis));
            }

            await UniTask.WhenAll(targetObjectTurnCommands);

            Debug.Log(targetObjectsArray.getObjectIndex.Count);

            foreach (var item in targetObjectsArray.getObjectIndex)
            {
                Debug.Log("ChangeBlock");
                Block changedBlock = TurnBlockFaces(item, rotateAxis);
                Vector3 changedBlockIndex = TurnBlockIndex(item, rotateAxis);
                // Debug.Log("ChangedBlockIndex: " + changedBlockIndex);
                // Debug.Log("ChangedBlock: " + changedBlock.blockFaceTypeDictionary["X"]["PositiveX"]);
                BoardManager.instance.SetBoardBlock(changedBlockIndex, changedBlock);
            }
        }, AwaitOperation.Drop)
        .AddTo(this);
    }

    void FixedUpdate()
    {
        rowTargetObjects.Item1.Clear();
        columnTargetObjects.Item1.Clear();
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
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            rowTargetObjects.getObjectIndex.Add(new Vector3(i, j, (int)pivotPosition.z));
                        }
                    }
                    columnTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, (int)pivotPosition.y, -1);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            columnTargetObjects.getObjectIndex.Add(new Vector3(i, (int)pivotPosition.y, j));
                        }
                    }
                    break;
                case var (_, y, _) when y < 0:
                    // Positive Y or Negative Y
                    rowTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray((int)pivotPosition.x, -1, -1);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            rowTargetObjects.getObjectIndex.Add(new Vector3((int)pivotPosition.x, i, j));
                        }
                    }
                    columnTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, -1, (int)pivotPosition.z);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            columnTargetObjects.getObjectIndex.Add(new Vector3(i, j, (int)pivotPosition.z));
                        }
                    }
                    break;
                case var (_, _, z) when z < 0:
                    // Positive Z or Negative Z
                    rowTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray((int)pivotPosition.x, -1, -1);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            rowTargetObjects.getObjectIndex.Add(new Vector3((int)pivotPosition.x, i, j));
                        }
                    }
                    columnTargetObjects.getObject = BoardManager.instance.GetTurnTargetObjectsArray(-1, (int)pivotPosition.y, -1);
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            columnTargetObjects.getObjectIndex.Add(new Vector3(i, (int)pivotPosition.y, j));
                        }
                    }
                    break;
            }
        }
    }

    (bool, Vector3) CastMoveDirectionDecideRay(Vector3 direction)
    {
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, directionDecideLayer.value))
        {
            return selectedFace.CaughtTurnDirection(hit.collider.gameObject);
        }

        else
        {
            Debug.LogError("CastMoveDirectionDecideRay: コライダーにヒットしませんでした");
            return (false, Vector3.zero);
        }
    }

    Block TurnBlockFaces(Vector3 pivotPosition, Vector3 rotateAxis)
    {
        Block block = BoardManager.instance.GetBoardBlock(pivotPosition);
        Block changedBlock = new Block(true);
        changedBlock.existBlock = block.existBlock;
        changedBlock.blockFaceTypeDictionary = block.blockFaceTypeDictionary;

        if (changedBlock.existBlock == false) return changedBlock;

        switch (rotateAxis)
        {
            case (var x, _, _) when x < 0:
                changedBlock.blockFaceTypeDictionary["Z"]["NegativeZ"] = block.blockFaceTypeDictionary["Y"]["PositiveY"];
                changedBlock.blockFaceTypeDictionary["Z"]["PositiveZ"] = block.blockFaceTypeDictionary["Y"]["NegativeY"];
                changedBlock.blockFaceTypeDictionary["Y"]["NegativeY"] = block.blockFaceTypeDictionary["Z"]["NegativeZ"];
                changedBlock.blockFaceTypeDictionary["Y"]["PositiveY"] = block.blockFaceTypeDictionary["Z"]["PositiveZ"];
                break;
            case (var x, _, _) when x > 0:
                changedBlock.blockFaceTypeDictionary["Z"]["NegativeZ"] = block.blockFaceTypeDictionary["Y"]["NegativeY"];
                changedBlock.blockFaceTypeDictionary["Z"]["PositiveZ"] = block.blockFaceTypeDictionary["Y"]["PositiveY"];
                changedBlock.blockFaceTypeDictionary["Y"]["NegativeY"] = block.blockFaceTypeDictionary["Z"]["PositiveZ"];
                changedBlock.blockFaceTypeDictionary["Y"]["PositiveY"] = block.blockFaceTypeDictionary["Z"]["NegativeZ"];
                break;
            case (_, var y, _) when y < 0:
                changedBlock.blockFaceTypeDictionary["X"]["NegativeX"] = block.blockFaceTypeDictionary["Z"]["PositiveZ"];
                changedBlock.blockFaceTypeDictionary["X"]["PositiveX"] = block.blockFaceTypeDictionary["Z"]["NegativeZ"];
                changedBlock.blockFaceTypeDictionary["Z"]["NegativeZ"] = block.blockFaceTypeDictionary["X"]["NegativeX"];
                changedBlock.blockFaceTypeDictionary["Z"]["PositiveZ"] = block.blockFaceTypeDictionary["X"]["PositiveX"];
                break;
            case (_, var y, _) when y > 0:
                changedBlock.blockFaceTypeDictionary["X"]["NegativeX"] = block.blockFaceTypeDictionary["Z"]["NegativeZ"];
                changedBlock.blockFaceTypeDictionary["X"]["PositiveX"] = block.blockFaceTypeDictionary["Z"]["PositiveZ"];
                changedBlock.blockFaceTypeDictionary["Z"]["NegativeZ"] = block.blockFaceTypeDictionary["X"]["PositiveX"];
                changedBlock.blockFaceTypeDictionary["Z"]["PositiveZ"] = block.blockFaceTypeDictionary["X"]["NegativeX"];
                break;
            case (_, _, var z) when z < 0:
                changedBlock.blockFaceTypeDictionary["X"]["NegativeX"] = block.blockFaceTypeDictionary["Y"]["NegativeY"];
                changedBlock.blockFaceTypeDictionary["X"]["PositiveX"] = block.blockFaceTypeDictionary["Y"]["PositiveY"];
                changedBlock.blockFaceTypeDictionary["Y"]["NegativeY"] = block.blockFaceTypeDictionary["X"]["PositiveX"];
                changedBlock.blockFaceTypeDictionary["Y"]["PositiveY"] = block.blockFaceTypeDictionary["X"]["NegativeX"];
                break;
            case (_, _, var z) when z > 0:
                changedBlock.blockFaceTypeDictionary["X"]["NegativeX"] = block.blockFaceTypeDictionary["Y"]["PositiveY"];
                changedBlock.blockFaceTypeDictionary["X"]["PositiveX"] = block.blockFaceTypeDictionary["Y"]["NegativeY"];
                changedBlock.blockFaceTypeDictionary["Y"]["NegativeY"] = block.blockFaceTypeDictionary["X"]["NegativeX"];
                changedBlock.blockFaceTypeDictionary["Y"]["PositiveY"] = block.blockFaceTypeDictionary["X"]["PositiveX"];
                break;
        }

        return changedBlock;
    }

    Vector3 TurnBlockIndex(Vector3 pivotPosition, Vector3 rotateAxis)
    {
        Vector3 changedBlockIndex = Vector3.zero;
        switch (rotateAxis)
        {
            case var (x, _, _) when x < 0:
                changedBlockIndex = new Vector3(pivotPosition.x, 2 - pivotPosition.z, pivotPosition.y);
                break;
            case var (x, _, _) when x > 0:
                changedBlockIndex = new Vector3(pivotPosition.x, pivotPosition.z, 2 - pivotPosition.y);
                break;
            case var (_, y, _) when y < 0:
                changedBlockIndex = new Vector3(2 - pivotPosition.z, pivotPosition.y, pivotPosition.x);
                break;
            case var (_, y, _) when y > 0:
                changedBlockIndex = new Vector3(pivotPosition.z, pivotPosition.y, 2 - pivotPosition.x);
                break;
            case var (_, _, z) when z < 0:
                changedBlockIndex = new Vector3(pivotPosition.y, 2 - pivotPosition.x, pivotPosition.z);
                break;
            case var (_, _, z) when z > 0:
                changedBlockIndex = new Vector3(2 - pivotPosition.y, pivotPosition.x, pivotPosition.z);
                break;
        }

        Debug.Log("PivotPosition" + pivotPosition + "ChangedBlockIndex: " + changedBlockIndex);

        return changedBlockIndex;
    }


}