using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Block
{
    public bool existBlock;
    public Dictionary<string, FaceType> blockFaceTypeDictionary;

    // TODO: コンストラクタの最適化を行う
    public Block(bool existBlock = false, FaceType[] faceTypeArray = null)
    {
        this.existBlock = existBlock;
        this.blockFaceTypeDictionary = new Dictionary<string, FaceType>();

        if (!existBlock)
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
        else if (faceTypeArray == null)
        {
            // 面を指定しない場合はランダムで色を決める
            faceTypeArray = new FaceType[6]
            {
                (FaceType)Random.Range(0, 6),
                (FaceType)Random.Range(0, 6),
                (FaceType)Random.Range(0, 6),
                (FaceType)Random.Range(0, 6),
                (FaceType)Random.Range(0, 6),
                (FaceType)Random.Range(0, 6)
            };
        }
        this.blockFaceTypeDictionary.Add("PositiveX", faceTypeArray[0]);
        this.blockFaceTypeDictionary.Add("NegativeX", faceTypeArray[1]);
        this.blockFaceTypeDictionary.Add("PositiveY", faceTypeArray[2]);
        this.blockFaceTypeDictionary.Add("NegativeY", faceTypeArray[3]);
        this.blockFaceTypeDictionary.Add("PositiveZ", faceTypeArray[4]);
        this.blockFaceTypeDictionary.Add("NegativeZ", faceTypeArray[5]);
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