using System.Collections.Generic;
using UnityEngine;

public class ShipPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> shipsT1List = new List<GameObject>();
    [SerializeField] private List<GameObject> shipsT2List = new List<GameObject>();
    [SerializeField] private List<GameObject> shipsT3List = new List<GameObject>();

    [SerializeField] private GameObject[] shipPrefabs;
    [SerializeField] private Vector3 preSpawnPosition;
    GameObject shipGameObject;
    Ship ship;

    public int availableShipsCount { get; private set;}
    public int maxShipsCount { get; private set;}

    private void Awake()
    {
        maxShipsCount = 15;
        for (int i = 0; i < 8; i++)
        {
            shipGameObject = Instantiate(shipPrefabs[0], preSpawnPosition, Quaternion.identity);
            shipsT1List.Add(shipGameObject);
            shipGameObject.SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            shipGameObject = Instantiate(shipPrefabs[1], preSpawnPosition, Quaternion.identity);
            shipsT2List.Add(shipGameObject);
            shipGameObject.SetActive(false);
        }
        for (int i = 0; i < 2; i++)
        {
            shipGameObject = Instantiate(shipPrefabs[2], preSpawnPosition, Quaternion.identity);
            shipsT3List.Add(shipGameObject);
            shipGameObject.SetActive(false);
        }
        availableShipsCount = maxShipsCount;
    }

    public bool GetShip(int type, Side side, Vector3 spawnPosition)
    {
        if (availableShipsCount > 0)
        {
            if (type == 0)
            {
                if (shipsT1List.Count > 0)
                {
                    shipGameObject = shipsT1List[0];
                    shipsT1List.Remove(shipGameObject);
                }
                else
                {
                    shipGameObject = Instantiate(shipPrefabs[type], spawnPosition, Quaternion.identity);
                }
            }
            else if (type == 1)
            {
                if (shipsT2List.Count > 0)
                {
                    shipGameObject = shipsT2List[0];
                    shipsT2List.Remove(shipGameObject);
                }
                else
                {
                    shipGameObject = Instantiate(shipPrefabs[type], spawnPosition, Quaternion.identity);
                }
            }
            else
            {
                if (shipsT3List.Count > 0)
                {
                    shipGameObject = shipsT3List[0];
                    shipsT3List.Remove(shipGameObject);

                }
                else
                {
                    shipGameObject = Instantiate(shipPrefabs[type], spawnPosition, Quaternion.identity);
                }
            }
            ship = shipGameObject.GetComponent<Ship>();
            ship.SetProperties(ref side, ref spawnPosition);
            ship.OnShipDeActivate += ReturnShip;
            availableShipsCount--;

            return true;
        }
        else return false;
    }

    public void ReturnShip(GameObject ship)
    {
        ship.GetComponent<Ship>().OnShipDeActivate -= ReturnShip;

        if (ship.tag == "1")
        {
            shipsT1List.Add(ship);
        }
        else if(ship.tag == "2")
        {
            shipsT2List.Add(ship);
        }
        else
        {
            shipsT3List.Add(ship);
        }
        availableShipsCount++;
    }
}
