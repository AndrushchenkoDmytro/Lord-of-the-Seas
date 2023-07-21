using UnityEngine;

public class TowerDetector : MonoBehaviour
{
    [SerializeField] private CannonTower cannonTower;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Ships"))
        {
            if (other.tag != cannonTower.currentSide.ToString())
            {
                Ship otherShip = other.gameObject.GetComponentInParent<Ship>();

                cannonTower.AddTarget(otherShip);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ships"))
        {
            if (other.tag != cannonTower.currentSide.ToString())
            {
                Ship otherShip = other.gameObject.GetComponentInParent<Ship>();

                cannonTower.RemoveTarget(otherShip);
            }
        }
    }
}
