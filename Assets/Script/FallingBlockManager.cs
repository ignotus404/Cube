using System.Dynamic;
using UnityEditor.Build.Content;
using UnityEngine;

public class FallingBlockManager : MonoBehaviour
{
    public BoardManager boardManager;
    public float fallInterval = 1.0f;
    float fallIntervalCount = 0;
    void FixedUpdate()
    {
        if (fallIntervalCount < fallInterval)
        {
            fallIntervalCount += Time.deltaTime;
        }
        else
        {
            fallIntervalCount = 0;
            DecideFallingBlockPosition();
        }
    }
    public void DecideFallingBlockPosition()
    {
        // 1. 落下位置を決定(x,z)
        // 2. 落下位置のy座標にあるブロックをすべて取得
        // 3. そのブロックのうち、一番上にあるブロックのy座標を取得
        // 4. そのy座標の一つ上に落下ブロックを配置
        // 5. 落下させようとしてfalseが返ってきたら、ゲームオーバー処理を行う

        Random.InitState(System.DateTime.Now.Millisecond);
        int xIndex = Random.Range(0, 3);
        int zIndex = Random.Range(0, 3);

        for (int depth = 2; depth >= 0; depth--)
        {
            BoardManager.Block checkBlock = boardManager.GetBoardBlock(xIndex, depth, zIndex);
            if (!checkBlock.existBlock)
            {
                // Debug.Log("x:" + xIndex + " z:" + zIndex + " depth:" + depth);
                BoardManager.Block fallingBlock = new BoardManager.Block();
                fallingBlock.existBlock = true;
                fallingBlock.blockFaceTypeArray = new BoardManager.FaceType[6];

                Random.InitState(System.DateTime.Now.Millisecond);
                for (int i = 0; i < 6; i++)
                {
                    fallingBlock.blockFaceTypeArray[i] = (BoardManager.FaceType)Random.Range(0, 6);
                }

                bool isSucceed = boardManager.SetBoardBlock(xIndex, depth, zIndex, fallingBlock);
                if (!isSucceed)
                {
                    // ゲームオーバー処理
                    Debug.Log("Game Over");
                }
                break;
            }
        }

    }
}
