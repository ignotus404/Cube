using Alchemy.Inspector;
using R3;
using UnityEngine;

public class TurnDirectionDecideColliderManager : MonoBehaviour
{
    public InputReceiverController inputReceiverController;
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

    public (bool, Vector3) CaughtTurnDirection(GameObject collider)
    {
        if (collider == northCollider)
        {
            return (true, turnDirectionNorth);
        }
        else if (collider == eastCollider)
        {
            return (false, turnDirectionEast);
        }
        else if (collider == southCollider)
        {
            return (true, turnDirectionSouth);
        }
        else if (collider == westCollider)
        {
            return (false, turnDirectionWest);
        }
        else
        {
            Debug.LogError("CaughtTurnDirection: 予期しないコライダーが渡されました");
            return (false, Vector3.zero);
        }
    }
}