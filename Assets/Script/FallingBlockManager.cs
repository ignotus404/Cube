using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingBlockManager : MonoBehaviour
{
    public BoardManager boardManager;
    public float fallInterval = 120.0f;
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
        Random.InitState(System.DateTime.Now.Millisecond);
        int xIndex = Random.Range(0, 3);
        int zIndex = Random.Range(0, 3);

        for (int depth = 2; depth >= 0; depth--)
        {
            Block checkBlock = boardManager.GetBoardBlock(xIndex, depth, zIndex);
            if (!checkBlock.existBlock)
            {
                // Debug.Log("x:" + xIndex + " z:" + zIndex + " depth:" + depth);
                Block fallingBlock = new Block(existBlock: true);
                Block foreachIndexBlock = new Block(existBlock: true);

                Random.InitState(System.DateTime.Now.Millisecond);
                foreach (string dimensionKey in foreachIndexBlock.blockFaceTypeDictionary.Keys)
                {
                    Dictionary<string, FaceType> targetDimension = foreachIndexBlock.blockFaceTypeDictionary[dimensionKey];
                    foreach (string targetFace in targetDimension.Keys)
                    {
                        fallingBlock.blockFaceTypeDictionary[dimensionKey][targetFace] = (FaceType)Random.Range(0, 6);
                    }
                }

                boardManager.SetBoardBlock(xIndex, depth, zIndex, fallingBlock);
                break;
            }

            if (depth == 0)
            {
                Debug.Log("GameOver");
            }
        }

    }
}
