using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    public struct Block
    {
        public bool existBlock;
        public FaceType[] blockFaceTypeArray;
    }

    public enum FaceType
    {
        Blank = -1,
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
        Orange = 4,
        White = 5,
    }

    #region BoardBlockArray
    public static Block[,,] cubeBlockArray { get; private set; } = new Block[3, 3, 3];
    public static int[,] positiveXBlockArray { get; private set; } = new int[3, 3];
    public static int[,] negativeXBlockArray { get; private set; } = new int[3, 3];
    public static int[,] positiveYBlockArray { get; private set; } = new int[3, 3];
    public static int[,] negativeYBlockArray { get; private set; } = new int[3, 3];
    public static int[,] positiveZBlockArray { get; private set; } = new int[3, 3];
    public static int[,] negativeZBlockArray { get; private set; } = new int[3, 3];
    #endregion

    [LabelText("Interface用ブロックを子に持つオブジェクト")]
    [SerializeField]
    GameObject interfaceBlockParent;
    List<List<List<GameObject>>> boardInterfaceBlockArray = new List<List<List<GameObject>>>();

    void Start()
    {
        boardInterfaceBlockArray = new List<List<List<GameObject>>>();
        for (int x = 0; x < 3; x++)
        {
            List<List<GameObject>> xItems = new List<List<GameObject>>();
            for (int y = 0; y < 3; y++)
            {
                List<GameObject> yItems = new List<GameObject>
                {
                    interfaceBlockParent.transform.GetChild(x).GetChild(y).GetChild(0).gameObject,
                    interfaceBlockParent.transform.GetChild(x).GetChild(y).GetChild(1).gameObject,
                    interfaceBlockParent.transform.GetChild(x).GetChild(y).GetChild(2).gameObject
                };
                xItems.Add(yItems);
            }
            boardInterfaceBlockArray.Add(xItems);
        }
    }

    public List<GameObject> GetTurnTargetObjectsArray(int x, int y, int z)
    {
        List<GameObject> targetObjectsArray = new List<GameObject>();
        for (int xIndex = 0; xIndex < 3; xIndex++)
        {
            for (int yIndex = 0; yIndex < 3; yIndex++)
            {
                for (int zIndex = 0; zIndex < 3; zIndex++)
                {
                    if ((x == -1 || xIndex == x) && (y == -1 || yIndex == y) && (z == -1 || zIndex == z))
                    {
                        targetObjectsArray.Add(boardInterfaceBlockArray[xIndex][yIndex][zIndex]);
                    }
                }
            }
        }

        return targetObjectsArray;
    }

    public bool SetBoardBlock(in int xIndex, in int yIndex, in int zIndex, in Block block)
    {
        if (xIndex < 0 || xIndex >= 3 || yIndex < 0 || yIndex >= 3 || zIndex < 0 || zIndex >= 3)
        {
            return false;
        }
        cubeBlockArray[xIndex, yIndex, zIndex] = block;
        ReflectBlockDataToInterfaceBlock();
        return true;
    }

    public void SetFaceBlockArray()
    {
        //  #####    #######  ######   ##   ##    ####            ##  ##     ##     ######   ##   ##  ######    #####   ###  ##   #####
        //   ## ##    ##   #   ##  ##  ##   ##   ##  ##           ##  ##    ####     ##  ##  ##   ##  # ## #   ##   ##   ##  ##  ##   ##
        //   ##  ##   ## #     ##  ##  ##   ##  ##                ##  ##   ##  ##    ##  ##  ##   ##    ##     ##   ##   ## ##   ##   ##
        //   ##  ##   ####     #####   ##   ##  ##                 ####    ##  ##    #####   ##   ##    ##     ##   ##   ####    ##   ##
        //   ##  ##   ## #     ##  ##  ##   ##  ##  ###             ##     ######    ## ##   ##   ##    ##     ##   ##   ## ##   ##   ##
        //   ## ##    ##   #   ##  ##  ##   ##   ##  ##             ##     ##  ##    ##  ##  ##   ##    ##     ##   ##   ##  ##  ##   ##
        //  #####    #######  ######    #####     #####            ####    ##  ##   #### ##   #####    ####     #####   ###  ##   #####
        // cubeBlockArrayを元に各面の表面にあたるブロックの配列を作成する
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                // PositiveXのブロック配列を作成
                for (int depth = 0; depth < 3; depth++)
                {
                    if (cubeBlockArray[depth, row, column].existBlock)
                    {
                        positiveXBlockArray[row, column] = (int)cubeBlockArray[depth, row, column].blockFaceTypeArray[0];
                        break;
                    }

                    else if (depth == 2)
                    {
                        positiveXBlockArray[row, column] = (int)FaceType.Blank;
                    }
                }


                // NegativeXのブロック配列を作成
                for (int depth = 2; depth >= 0; depth--)
                {
                    if (cubeBlockArray[depth, row, column].existBlock)
                    {
                        negativeXBlockArray[row, column] = (int)cubeBlockArray[depth, row, column].blockFaceTypeArray[1];
                        break;
                    }

                    else if (depth == 0)
                    {
                        negativeXBlockArray[row, column] = (int)FaceType.Blank;
                    }
                }


                // PositiveYのブロック配列を作成
                for (int depth = 0; depth < 3; depth++)
                {
                    if (cubeBlockArray[row, depth, column].existBlock)
                    {
                        positiveYBlockArray[row, column] = (int)cubeBlockArray[row, depth, column].blockFaceTypeArray[2];
                        break;
                    }

                    else if (depth == 2)
                    {
                        positiveYBlockArray[row, column] = (int)FaceType.Blank;
                    }
                }

                // NegativeYのブロック配列を作成
                for (int depth = 2; depth >= 0; depth--)
                {
                    if (cubeBlockArray[row, depth, column].existBlock)
                    {
                        negativeYBlockArray[row, column] = (int)cubeBlockArray[row, depth, column].blockFaceTypeArray[3];
                        break;
                    }

                    else if (depth == 0)
                    {
                        negativeYBlockArray[row, column] = (int)FaceType.Blank;
                    }
                }

                // PositiveZのブロック配列を作成
                for (int depth = 0; depth < 3; depth++)
                {
                    if (cubeBlockArray[row, column, depth].existBlock)
                    {
                        positiveZBlockArray[row, column] = (int)cubeBlockArray[row, column, depth].blockFaceTypeArray[4];
                        break;
                    }

                    else if (depth == 2)
                    {
                        positiveZBlockArray[row, column] = (int)FaceType.Blank;
                    }
                }

                // NegativeZのブロック配列を作成
                for (int depth = 2; depth >= 0; depth--)
                {
                    if (cubeBlockArray[row, column, depth].existBlock)
                    {
                        negativeZBlockArray[row, column] = (int)cubeBlockArray[row, column, depth].blockFaceTypeArray[5];
                        break;
                    }

                    else if (depth == 0)
                    {
                        negativeZBlockArray[row, column] = (int)FaceType.Blank;
                    }
                }
            }
        }
    }

    public void ReflectBlockDataToInterfaceBlock()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    Block block = cubeBlockArray[x, y, z];
                    boardInterfaceBlockArray[x][y][z].GetComponent<BlockManager>().ReflectBlockColor(block);
                }
            }
        }
    }
}
