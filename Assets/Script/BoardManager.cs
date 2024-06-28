using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using Array2DEditor;

public class BoardManager : MonoBehaviour
{

    public static BoardManager instance { get; private set; }

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
    List<List<List<GameObject>>> boardInterfaceBlockArray;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        // ブロックの初期化
        cubeBlockArray = new Block[3, 3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    cubeBlockArray[x, y, z] = new Block();
                }
            }
        }

        // 各面のブロック配列の初期化
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

        // インターフェース用ブロックにアクセスする際に使う配列の初期化
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

        // インターフェース用ブロックの初期化
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

    public List<GameObject> GetTurnTargetObjectsArray(in Vector3 index)
    {
        List<GameObject> targetObjectsArray = new List<GameObject>();
        for (int xIndex = 0; xIndex < 3; xIndex++)
        {
            for (int yIndex = 0; yIndex < 3; yIndex++)
            {
                for (int zIndex = 0; zIndex < 3; zIndex++)
                {
                    if ((index.x == -1 || xIndex == index.x) && (index.y == -1 || yIndex == index.y) && (index.z == -1 || zIndex == index.z))
                    {
                        targetObjectsArray.Add(boardInterfaceBlockArray[xIndex][yIndex][zIndex]);
                    }
                }
            }
        }

        return targetObjectsArray;
    }

    // TODO:  2次元配列で取得したいブロックの情報を取得するやつも作る

    public Block GetBoardBlock(in int xIndex, in int yIndex, in int zIndex)
    {
        if (xIndex < 0 || xIndex >= 3 || yIndex < 0 || yIndex >= 3 || zIndex < 0 || zIndex >= 3)
        {
            Debug.LogError("Invalid index");
            return new Block();
        }
        return cubeBlockArray[xIndex, yIndex, zIndex];
    }

    public Block GetBoardBlock(in Vector3 index)
    {
        if (index.x < 0 || index.x >= 3 || index.y < 0 || index.y >= 3 || index.z < 0 || index.z >= 3)
        {
            Debug.LogError("Invalid index");
            return new Block();
        }
        return cubeBlockArray[(int)index.x, (int)index.y, (int)index.z];
    }

    public void SetBoardBlock(in int xIndex, in int yIndex, in int zIndex, in Block block)
    {
        if (xIndex < 0 || xIndex >= 3 || yIndex < 0 || yIndex >= 3 || zIndex < 0 || zIndex >= 3)
        {
            Debug.LogError("Invalid index");
            return;
        }
        cubeBlockArray[xIndex, yIndex, zIndex] = block;
        ReflectFaceBlockArray();
        ReflectBlockDataToInterfaceBlock();
    }

    public void SetBoardBlock(in Vector3 index, in Block block)
    {
        if (index.x < 0 || index.x >= 3 || index.y < 0 || index.y >= 3 || index.z < 0 || index.z >= 3)
        {
            Debug.LogError("Invalid index");
            return;
        }
        cubeBlockArray[(int)index.x, (int)index.y, (int)index.z] = block;
        // Debug.Log("SetBoardBlock");
        ReflectFaceBlockArray();
        ReflectBlockDataToInterfaceBlock();
    }

    // TODO:  配列で読み込んでセットできるやつも作る



    public void ReflectFaceBlockArray()
    {
        // Debug.Log("ReflectFaceBlockArray");
        // TODO:  cubeBlockArrayを元に各面の表面にあたるブロックの配列を作成する
        // TODO:  ここの2次元辞書参照も外す
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                // PositiveXのブロック配列を作成
                for (int depth = 0; depth < 3; depth++)
                {
                    if (cubeBlockArray[depth, row, column].existBlock)
                    {
                        positiveXBlockArray.SetCell(row, column, (int)cubeBlockArray[depth, row, column].blockFaceTypeDictionary["PositiveX"]);
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
                        negativeXBlockArray.SetCell(row, column, (int)cubeBlockArray[depth, row, column].blockFaceTypeDictionary["NegativeX"]);
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
                        positiveYBlockArray.SetCell(row, column, (int)cubeBlockArray[row, depth, column].blockFaceTypeDictionary["PositiveY"]);
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
                        negativeYBlockArray.SetCell(row, column, (int)cubeBlockArray[row, depth, column].blockFaceTypeDictionary["NegativeY"]);
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
                        positiveZBlockArray.SetCell(row, column, (int)cubeBlockArray[row, column, depth].blockFaceTypeDictionary["PositiveZ"]);
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
                        negativeZBlockArray.SetCell(row, column, (int)cubeBlockArray[row, column, depth].blockFaceTypeDictionary["NegativeZ"]);
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
        // Debug.Log("ReflectBlockDataToInterfaceBlock");
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
