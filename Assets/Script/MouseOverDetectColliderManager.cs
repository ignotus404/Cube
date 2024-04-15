using Alchemy.Inspector;
using DG.Tweening;
using UnityEngine;

public class MouseOverDetectColliderManager : MonoBehaviour
{

    [LabelText("マウスオーバー時の色")]
    public Color mouseOverColor = Color.red;
    [LabelText("オブジェクトの配列内座標")]
    public Vector3 objectArrayPosition;

    private Renderer rendererComponent;
    void Start()
    {
        rendererComponent = GetComponent<Renderer>();
    }
    public void MouseOverDetect()
    {
        this.rendererComponent.material.DOColor(mouseOverColor, 0.1f).OnComplete
        (
            () =>
            {
                this.rendererComponent.material.DOColor(Color.white, 0.1f);
            }
        );
    }
}
