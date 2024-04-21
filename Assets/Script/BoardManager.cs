using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using Array2DEditor;

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
    Block[,,] cubeBlockArray = new Block[3, 3, 3];
    [SerializeField]
    Array2DInt positiveXBlockArray = null;
    [SerializeField]
    Array2DInt negativeXBlockArray = null;
    [SerializeField]
    Array2DInt positiveYBlockArray = null;
    [SerializeField]
    Array2DInt negativeYBlockArray = null;
    [SerializeField]
    Array2DInt positiveZBlockArray = null;
    [SerializeField]
    Array2DInt negativeZBlockArray = null;
    #endregion

    [LabelText("Interface用ブロックを子に持つオブジェクト")]
    [SerializeField]
    GameObject interfaceBlockParent;
    List<List<List<GameObject>>> boardInterfaceBlockArray = new List<List<List<GameObject>>>();

    void Start()
    {
        cubeBlockArray = new Block[3, 3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeBlockArray[x, y, z] = new Block
                    {
                        existBlock = false,
                        blockFaceTypeArray = new FaceType[6]
                    };
                }
            }
        }

        positiveXBlockArray.SetGridSize(new Vector2Int(3, 3));
        negativeXBlockArray.SetGridSize(new Vector2Int(3, 3));
        positiveYBlockArray.SetGridSize(new Vector2Int(3, 3));
        negativeYBlockArray.SetGridSize(new Vector2Int(3, 3));
        positiveZBlockArray.SetGridSize(new Vector2Int(3, 3));
        negativeZBlockArray.SetGridSize(new Vector2Int(3, 3));
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                positiveXBlockArray.SetCell(i, j, (int)FaceType.Blank);
                negativeXBlockArray.SetCell(i, j, (int)FaceType.Blank);
                positiveYBlockArray.SetCell(i, j, (int)FaceType.Blank);
                negativeYBlockArray.SetCell(i, j, (int)FaceType.Blank);
                positiveZBlockArray.SetCell(i, j, (int)FaceType.Blank);
                negativeZBlockArray.SetCell(i, j, (int)FaceType.Blank);
            }
        }

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
        ReflectFaceBlockArray();
        ReflectBlockDataToInterfaceBlock();
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
        ReflectFaceBlockArray();
        ReflectBlockDataToInterfaceBlock();
        return true;
    }

    public void ReflectFaceBlockArray()
    {
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
                        positiveXBlockArray.SetCell(row, column, (int)cubeBlockArray[depth, row, column].blockFaceTypeArray[0]);
                        break;
                    }

                    else if (depth == 2)
                    {
                        positiveXBlockArray.SetCell(row, column, (int)FaceType.Blank);
                    }
                }


                // NegativeXのブロック配列を作成
                for (int depth = 2; depth >= 0; depth--)
                {
                    if (cubeBlockArray[depth, row, column].existBlock)
                    {
                        negativeXBlockArray.SetCell(row, column, (int)cubeBlockArray[depth, row, column].blockFaceTypeArray[1]);
                        break;
                    }

                    else if (depth == 0)
                    {
                        negativeXBlockArray.SetCell(row, column, (int)FaceType.Blank);
                    }
                }


                // PositiveYのブロック配列を作成
                for (int depth = 0; depth < 3; depth++)
                {
                    if (cubeBlockArray[row, depth, column].existBlock)
                    {
                        positiveYBlockArray.SetCell(row, column, (int)cubeBlockArray[row, depth, column].blockFaceTypeArray[2]);
                        break;
                    }

                    else if (depth == 2)
                    {
                        positiveYBlockArray.SetCell(row, column, (int)FaceType.Blank);
                    }
                }

                // NegativeYのブロック配列を作成
                for (int depth = 2; depth >= 0; depth--)
                {
                    if (cubeBlockArray[row, depth, column].existBlock)
                    {
                        negativeYBlockArray.SetCell(row, column, (int)cubeBlockArray[row, depth, column].blockFaceTypeArray[3]);
                        break;
                    }

                    else if (depth == 0)
                    {
                        negativeYBlockArray.SetCell(row, column, (int)FaceType.Blank);
                    }
                }

                // PositiveZのブロック配列を作成
                for (int depth = 0; depth < 3; depth++)
                {
                    if (cubeBlockArray[row, column, depth].existBlock)
                    {
                        positiveZBlockArray.SetCell(row, column, (int)cubeBlockArray[row, column, depth].blockFaceTypeArray[4]);
                        break;
                    }

                    else if (depth == 2)
                    {
                        positiveZBlockArray.SetCell(row, column, (int)FaceType.Blank);
                    }
                }

                // NegativeZのブロック配列を作成
                for (int depth = 2; depth >= 0; depth--)
                {
                    if (cubeBlockArray[row, column, depth].existBlock)
                    {
                        negativeZBlockArray.SetCell(row, column, (int)cubeBlockArray[row, column, depth].blockFaceTypeArray[5]);
                        break;
                    }

                    else if (depth == 0)
                    {
                        negativeZBlockArray.SetCell(row, column, (int)FaceType.Blank);
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

    public Block GetBoardBlock(in int xIndex, in int yIndex, in int zIndex)
    {
        if (xIndex < 0 || xIndex >= 3 || yIndex < 0 || yIndex >= 3 || zIndex < 0 || zIndex >= 3)
        {
            Debug.LogError("Invalid index");
            return new Block();
        }
        return cubeBlockArray[xIndex, yIndex, zIndex];
    }
}
