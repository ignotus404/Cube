using R3;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inspector;
using DG.Tweening;
using SerializeDictionary;
using Kogane;
using TMPro;
using System.Security.Cryptography.X509Certificates;

public class Block
{
    public int blockXIndex;
    public int blockYIndex;
    public int blockZIndex;
    public bool existBlock;
    public Dictionary<string, Dictionary<string, FaceType>> blockFaceTypeDictionary;

    public Block(bool existBlock = false, FaceType[] faceTypeArray = null)
    {
        this.existBlock = existBlock;
        this.blockFaceTypeDictionary = new Dictionary<string, Dictionary<string, FaceType>>();
        if (faceTypeArray == null)
        {
            faceTypeArray = new FaceType[6]
            {
                FaceType.Blank,
                FaceType.Blank,
                FaceType.Blank,
                FaceType.Blank,
                FaceType.Blank,
                FaceType.Blank
            };
        }
        this.blockFaceTypeDictionary.Add("X", new Dictionary<string, FaceType> { { "PositiveX", faceTypeArray[0] }, { "NegativeX", faceTypeArray[1] } });
        this.blockFaceTypeDictionary.Add("Y", new Dictionary<string, FaceType> { { "PositiveY", faceTypeArray[2] }, { "NegativeY", faceTypeArray[3] } });
        this.blockFaceTypeDictionary.Add("Z", new Dictionary<string, FaceType> { { "PositiveZ", faceTypeArray[4] }, { "NegativeZ", faceTypeArray[5] } });
    }
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

public class BlockManager : MonoBehaviour
{
    #region ブロックのオブジェクト、マテリアル
    [LabelText("ブロックが存在しないときのオブジェクト")]
    public GameObject emptyBlockObject;
    [LabelText("ブロックが存在するときのオブジェクト")]
    public GameObject existBlockObject;
    [FoldoutGroup("ブロックの色のマテリアル")]
    [LabelText("赤色のマテリアル")]
    public Material redMaterial;
    [FoldoutGroup("ブロックの色のマテリアル")]
    [LabelText("青色のマテリアル")]
    public Material blueMaterial;
    [FoldoutGroup("ブロックの色のマテリアル")]
    [LabelText("緑色のマテリアル")]
    public Material greenMaterial;
    [FoldoutGroup("ブロックの色のマテリアル")]
    [LabelText("黄色のマテリアル")]
    public Material yellowMaterial;
    [FoldoutGroup("ブロックの色のマテリアル")]
    [LabelText("オレンジ色のマテリアル")]
    public Material orangeMaterial;
    [FoldoutGroup("ブロックの色のマテリアル")]
    [LabelText("白色のマテリアル")]
    public Material whiteMaterial;
    # endregion

    # region ブロックの面オブジェクト
    [LabelText("ブロックの面オブジェクトのDictionary")]
    public SerializableDictionary<string, GameObject> blockFaceObjectDictionary = new SerializableDictionary<string, GameObject>
    {
        { "PositiveX", null },
        { "NegativeX", null },
        { "PositiveY", null },
        { "NegativeY", null },
        { "PositiveZ", null },
        { "NegativeZ", null },
    };
    #endregion

    [LabelText("公転するブロックの座標")]
    public Transform pivotObjectPosition;
    public Subject<Vector3> TurnBlockSubject = new Subject<Vector3>();
    Observable<Vector3> TurnBlockObservable => TurnBlockSubject;
    float prevVal;

    void Start()
    {
        TurnBlockObservable
            .SubscribeAwait(async (rotateAxis, ct) =>
                {
                    Vector3 defaultPosition = transform.position;
                    Vector3 defaultRotation = transform.rotation.eulerAngles;
                    await DOTween.To(x => RotatePivotAround(x, rotateAxis), 0, 90, 1.0f)
                        .AsyncWaitForCompletion();
                    transform.position = defaultPosition;
                    transform.rotation = Quaternion.Euler(defaultRotation);
                },
                AwaitOperation.Drop)
                .AddTo(this);

    }

    void RotatePivotAround(float rotateAngle, Vector3 rotateAxis)
    {
        float delta = rotateAngle - prevVal;
        transform.RotateAround(pivotObjectPosition.position, rotateAxis, delta);
        prevVal = rotateAngle;
    }

    public void ReflectBlockColor(Block block)
    {
        // ブロックの色を設定する処理
        if (block.existBlock)
        {
            // ブロックが存在する場合の処理
            emptyBlockObject.SetActive(false);
            existBlockObject.SetActive(true);

            foreach (KeyValuePair<string, Dictionary<string, FaceType>> dictionary in block.blockFaceTypeDictionary)
            {
                foreach (KeyValuePair<string, FaceType> faceType in dictionary.Value)
                {
                    FaceType faceTypeValue = faceType.Value;
                    string faceTypeIndex = faceType.Key;
                    switch (faceTypeValue)
                    {
                        case FaceType.Red:
                            blockFaceObjectDictionary[faceTypeIndex].GetComponent<MeshRenderer>().material = redMaterial;
                            break;
                        case FaceType.Blue:
                            blockFaceObjectDictionary[faceTypeIndex].GetComponent<MeshRenderer>().material = blueMaterial;
                            break;
                        case FaceType.Green:
                            blockFaceObjectDictionary[faceTypeIndex].GetComponent<MeshRenderer>().material = greenMaterial;
                            break;
                        case FaceType.Yellow:
                            blockFaceObjectDictionary[faceTypeIndex].GetComponent<MeshRenderer>().material = yellowMaterial;
                            break;
                        case FaceType.Orange:
                            blockFaceObjectDictionary[faceTypeIndex].GetComponent<MeshRenderer>().material = orangeMaterial;
                            break;
                        case FaceType.White:
                            blockFaceObjectDictionary[faceTypeIndex].GetComponent<MeshRenderer>().material = whiteMaterial;
                            break;
                        default:
                            Debug.LogError("FaceTypeが不正です");
                            break;
                    }
                }
            }
        }
        else
        {
            // ブロックが存在しないなら emptyBlockObject を表示する
            emptyBlockObject.SetActive(true);
            existBlockObject.SetActive(false);
        }
    }

}
