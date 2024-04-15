using Alchemy.Inspector;
using R3;
using UnityEngine;

public class TurnDirectionDecideColliderManager : MonoBehaviour
{
    public Subject<GameObject> turnDirectionDecidedSubject = new Subject<GameObject>();
    public InputReceiverController inputReceiverController;
    Observable<GameObject> turnDirectionDecidedObservable => turnDirectionDecidedSubject;

    [TabGroup("TurnDirection", "North")]
    [SerializeField]
    private GameObject northCollider;
    [TabGroup("TurnDirection", "North")]
    [SerializeField]
    private Vector3 turnDirectionNorth = new Vector3(90, 90, 90);
    [TabGroup("TurnDirection", "East")]
    [SerializeField]
    private GameObject eastCollider;
    [TabGroup("TurnDirection", "East")]
    [SerializeField]
    private Vector3 turnDirectionEast = new Vector3(90, 90, 90);
    [TabGroup("TurnDirection", "South")]
    [SerializeField]
    private GameObject southCollider;
    [TabGroup("TurnDirection", "South")]
    [SerializeField]
    private Vector3 turnDirectionSouth = -new Vector3(90, 90, 90);
    [TabGroup("TurnDirection", "West")]
    [SerializeField]
    private GameObject westCollider;
    [TabGroup("TurnDirection", "West")]
    [SerializeField]
    private Vector3 turnDirectionWest = -new Vector3(90, 90, 90);

    void Start()
    {
        turnDirectionDecidedObservable
            .Subscribe
            (
                x =>
                {
                    if (x == northCollider)
                    {
                        inputReceiverController.receiveRotateAngleSubject.OnNext(turnDirectionNorth);
                    }
                    else if (x == eastCollider)
                    {
                        inputReceiverController.receiveRotateAngleSubject.OnNext(turnDirectionEast);
                    }
                    else if (x == southCollider)
                    {
                        inputReceiverController.receiveRotateAngleSubject.OnNext(turnDirectionSouth);
                    }
                    else if (x == westCollider)
                    {
                        inputReceiverController.receiveRotateAngleSubject.OnNext(turnDirectionWest);
                    }
                }
            );
    }
}