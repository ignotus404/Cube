using R3;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Alchemy.Inspector;
using DG.Tweening;

public class BlockManager : MonoBehaviour
{
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
                    await DOTween.To(x => RotatePivotAround(x, rotateAxis), 0, 90, 0.1f)
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

    public void ReflectBlockColor(BoardManager.Block block)
    {
        // ブロックの色を設定する処理
        if (block.existBlock)
        {
            // ブロックが存在する場合の処理
            emptyBlockObject.SetActive(false);
            existBlockObject.SetActive(true);

            foreach (BoardManager.FaceType faceType in block.blockFaceTypeArray)
            {
                // ブロックの色に応じて、ブロックの色を変更する処理
                switch (faceType)
                {
                    case BoardManager.FaceType.Red:
                        existBlockObject.GetComponent<Renderer>().material = redMaterial;
                        break;
                    case BoardManager.FaceType.Blue:
                        existBlockObject.GetComponent<Renderer>().material = blueMaterial;
                        break;
                    case BoardManager.FaceType.Green:
                        existBlockObject.GetComponent<Renderer>().material = greenMaterial;
                        break;
                    case BoardManager.FaceType.Yellow:
                        existBlockObject.GetComponent<Renderer>().material = yellowMaterial;
                        break;
                    case BoardManager.FaceType.Orange:
                        existBlockObject.GetComponent<Renderer>().material = orangeMaterial;
                        break;
                    case BoardManager.FaceType.White:
                        existBlockObject.GetComponent<Renderer>().material = whiteMaterial;
                        break;
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
