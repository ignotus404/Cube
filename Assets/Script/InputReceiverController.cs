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
    [LabelText("回転方向判定用コライダーのレイヤー")]
    public LayerMask directionDecideLayer;
    [LabelText("盤面上に存在するブロックのレイヤー")]
    public LayerMask blockLayer;
    [LabelText("メインカメラ")]
    public Camera mainCamera;

    [LabelText("ブロックの回転にかかる時間")]
    public float turnBlockTime = 0.5f;

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
                /* TODO:  ここで動くオブジェクト群を取得するようにする
                 3x3の配列を取得して、それを回転させる
                 その後、回転させたものをセットする */

            }
            // Index取り忘れてる
            (List<Vector3> getObjectIndex, List<GameObject> getObject) targetObjectsArray = (new List<Vector3>(), new List<GameObject>());
            List<UniTask> targetObjectTurnCommands = new List<UniTask>();

            foreach (var item in targetObjectsArray.getObject)
            {
                targetObjectTurnCommands.Add(item.GetComponent<BlockManager>().TurnBlock(rotateAxis, turnBlockTime));
            }

            await UniTask.WhenAll(targetObjectTurnCommands);

            Debug.Log(targetObjectsArray.getObjectIndex.Count);

            foreach (var item in targetObjectsArray.getObjectIndex)
            {
                Debug.Log("ChangeBlock");
                Block changedBlock = TurnBlockFaces(item, rotateAxis);
                Vector3 changedBlockIndex = TurnBlockIndex(item, rotateAxis);
                // Debug.Log("ChangedBlockIndex: " + changedBlockIndex);
                // Debug.Log("ChangedBlock: " + changedBlock.blockFaceTypeDictionary["PositiveX"]);
                BoardManager.instance.SetBoardBlock(changedBlockIndex, changedBlock);
            }
        }, AwaitOperation.Drop)
        .AddTo(this);
    }

    void FixedUpdate()
    {


        // TODO:  lookable 起動時wasdでカメラ90度回転できるようにする
        if (playerInput.Player.Lookable.IsPressed())
        {
            Vector2 look = playerInput.Player.Look.ReadValue<Vector2>();
            transform.Rotate(look.y, look.x, 0);
        }


        // TODO:  マウスオーバー時UIが光る機能のみ搭載
        // 動く配列を取得するのは実際に動かす直前にやる

    }

    void CastMoveDirectionDecideRay(Vector3 direction)
    {
        // TODO:  何かしらの手段でどこの軸か、正負どっちかを取得する
        // TODO:  それをもとに回転方向をVector3で返す
        // TODO:  引数としてどの面にマウスオーバーしたかを取得する

    }


    // TODO:  3x3の配列を回転させるやつ作る
    // こっから

    // TODO:  こいつ自体はBlockManagerに移動する
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
                changedBlock.blockFaceTypeDictionary["NegativeZ"] = block.blockFaceTypeDictionary["PositiveY"];
                changedBlock.blockFaceTypeDictionary["PositiveZ"] = block.blockFaceTypeDictionary["NegativeY"];
                changedBlock.blockFaceTypeDictionary["NegativeY"] = block.blockFaceTypeDictionary["NegativeZ"];
                changedBlock.blockFaceTypeDictionary["PositiveY"] = block.blockFaceTypeDictionary["PositiveZ"];
                break;
            case (var x, _, _) when x > 0:
                changedBlock.blockFaceTypeDictionary["NegativeZ"] = block.blockFaceTypeDictionary["NegativeY"];
                changedBlock.blockFaceTypeDictionary["PositiveZ"] = block.blockFaceTypeDictionary["PositiveY"];
                changedBlock.blockFaceTypeDictionary["NegativeY"] = block.blockFaceTypeDictionary["PositiveZ"];
                changedBlock.blockFaceTypeDictionary["PositiveY"] = block.blockFaceTypeDictionary["NegativeZ"];
                break;
            case (_, var y, _) when y < 0:
                changedBlock.blockFaceTypeDictionary["NegativeX"] = block.blockFaceTypeDictionary["PositiveZ"];
                changedBlock.blockFaceTypeDictionary["PositiveX"] = block.blockFaceTypeDictionary["NegativeZ"];
                changedBlock.blockFaceTypeDictionary["NegativeZ"] = block.blockFaceTypeDictionary["NegativeX"];
                changedBlock.blockFaceTypeDictionary["PositiveZ"] = block.blockFaceTypeDictionary["PositiveX"];
                break;
            case (_, var y, _) when y > 0:
                changedBlock.blockFaceTypeDictionary["NegativeX"] = block.blockFaceTypeDictionary["NegativeZ"];
                changedBlock.blockFaceTypeDictionary["PositiveX"] = block.blockFaceTypeDictionary["PositiveZ"];
                changedBlock.blockFaceTypeDictionary["NegativeZ"] = block.blockFaceTypeDictionary["PositiveX"];
                changedBlock.blockFaceTypeDictionary["PositiveZ"] = block.blockFaceTypeDictionary["NegativeX"];
                break;
            case (_, _, var z) when z < 0:
                changedBlock.blockFaceTypeDictionary["NegativeX"] = block.blockFaceTypeDictionary["NegativeY"];
                changedBlock.blockFaceTypeDictionary["PositiveX"] = block.blockFaceTypeDictionary["PositiveY"];
                changedBlock.blockFaceTypeDictionary["NegativeY"] = block.blockFaceTypeDictionary["PositiveX"];
                changedBlock.blockFaceTypeDictionary["PositiveY"] = block.blockFaceTypeDictionary["NegativeX"];
                break;
            case (_, _, var z) when z > 0:
                changedBlock.blockFaceTypeDictionary["NegativeX"] = block.blockFaceTypeDictionary["PositiveY"];
                changedBlock.blockFaceTypeDictionary["PositiveX"] = block.blockFaceTypeDictionary["NegativeY"];
                changedBlock.blockFaceTypeDictionary["NegativeY"] = block.blockFaceTypeDictionary["NegativeX"];
                changedBlock.blockFaceTypeDictionary["PositiveY"] = block.blockFaceTypeDictionary["PositiveX"];
                break;
        }

        return changedBlock;
    }

    // これはこっちでやる
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
    // ここまで


}