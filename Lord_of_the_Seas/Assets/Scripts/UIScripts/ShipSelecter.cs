using UnityEngine;

public class ShipSelecter : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    public void SelectShip1()
    {
        playerController.shipType = 0;
    }
    public void SelectShip2()
    {
        playerController.shipType = 1;
    }
    public void SelectShip3()
    {
        playerController.shipType = 2;
    }
}
