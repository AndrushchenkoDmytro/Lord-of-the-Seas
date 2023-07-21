using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] ShipPool enemyShipPool;
    [SerializeField] ShipPool playerShipPool;
    [SerializeField] SpawnArea spawnArea;

    [SerializeField] float money = 99;
    [SerializeField] float moneyPerSecond = 0;
    private float moneyTax = 3.765f;
    [SerializeField] List<Building> controllBuildings = new List<Building>();
    
    public int buildingsInControll;

    public static readonly int[] shipsPrice = { 100, 200, 400 };

    [SerializeField] private List<Vector3> positionsOfShipsNearBase = new List<Vector3>();
    private int i = 0;

    bool isPlayerWin = false;
    [SerializeField] int playerShipsCount = 0;
    [SerializeField] int enemyShipsCount = 0;

    int playerShipsNearToBase = 0;
    bool isPlayerShipsNearToBase = false;

    int chance = 0;
    
    IEnumerator currentEnemyAction;
    IEnumerator mainCoroutine;


    private void Awake()
    {
        moneyTax = Random.Range(3, 3.2f);
        GameObject enemyBase = GameObject.Find("/EnemyBase");

        for (int i = 0; i < enemyBase.transform.childCount; i++)
        {
            Building building = enemyBase.transform.GetChild(i).GetChild(0).GetComponent<Building>();
            controllBuildings.Add(building);
        }
        buildingsInControll += enemyBase.transform.childCount;
    }

    void Start()
    {
        mainCoroutine = LogicScenario1();
        StartCoroutine(mainCoroutine);
    }

    void Update()
    {
        moneyPerSecond = moneyTax * buildingsInControll * Time.deltaTime;
        money += moneyPerSecond;
    }

    public void ChekBuildingSide(Building building)
    {
        if (building.currentSide == Side.Enemy)
        {
            controllBuildings.Add(building);
        }
        else
        {
            controllBuildings.Remove(building);
        }
    }

    IEnumerator LogicScenario1()
    {
        while (isPlayerWin == false)
        {
            enemyShipsCount = enemyShipPool.maxShipsCount - enemyShipPool.availableShipsCount;
            playerShipsCount = playerShipPool.maxShipsCount - playerShipPool.availableShipsCount;

            if (playerShipsCount < 1)
            {
                chance = Random.Range(0, 10);
                if(chance < 8)   // spawnShip
                {
                    if(buildingsInControll < 6)
                    {
                        currentEnemyAction = SpawnShipInRandomPosition(0);
                    }
                    else if(buildingsInControll < 9)
                    {
                        currentEnemyAction = SpawnShipInRandomPosition(1);
                    }
                    else
                    {
                        currentEnemyAction = SpawnShipInRandomPosition(2);
                    }
                    yield return StartCoroutine(currentEnemyAction);
                }
                else
                {
                    if(enemyShipsCount > 2)
                    {
                        currentEnemyAction = UpgradeControllBuilding();
                        yield return StartCoroutine(currentEnemyAction);
                    }
                }
            }
            else
            {
                if(playerShipsNearToBase == 0)
                {
                    if (playerShipsCount >= enemyShipsCount)
                    {
                        if (playerShipsCount - enemyShipsCount < 2 )
                        {
                            chance = Random.Range(0, 10);
                            if(chance < 3)
                            {
                                currentEnemyAction = UpgradeControllBuilding();
                                yield return StartCoroutine(currentEnemyAction);
                            }
                            else
                            {
                                chance = Random.Range(0, 10);
                                if (buildingsInControll < 6)
                                {
                                    if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                }
                                else if (buildingsInControll < 9)
                                {
                                    if (chance < 3)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                else
                                {
                                    if (chance < 2)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 6)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                yield return StartCoroutine(currentEnemyAction);
                                
                            }
                        }
                        else 
                        {
                            chance = Random.Range(0, 10);
                            if (chance < 1)
                            {
                                currentEnemyAction = UpgradeControllBuilding();
                                yield return StartCoroutine(currentEnemyAction);
                            }
                            else
                            {
                                chance = Random.Range(0, 10);
                                if (buildingsInControll < 6)
                                {
                                    if (chance < 7)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                }
                                else if (buildingsInControll < 9)
                                {
                                    if (chance < 4)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 9)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                else
                                {
                                    if (chance < 2)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                yield return StartCoroutine(currentEnemyAction);
                            }
                        }
                    }
                    else
                    {
                        if (enemyShipsCount - playerShipsCount < 2)
                        {
                            chance = Random.Range(0, 10);
                            if(chance < 5)
                            {
                                currentEnemyAction = UpgradeControllBuilding();
                                yield return StartCoroutine(currentEnemyAction);
                            }
                            else
                            {
                                chance = Random.Range(0, 10);
                                if (buildingsInControll < 6)
                                {
                                    if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                }
                                else if (buildingsInControll < 9)
                                {
                                    if (chance < 3)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                else
                                {
                                    if (chance < 1)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 6)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                yield return StartCoroutine(currentEnemyAction);

                            }

                        }
                        else
                        {
                            chance = Random.Range(0, 10);
                            if(chance < 8)
                            {
                                currentEnemyAction = UpgradeControllBuilding();
                                yield return StartCoroutine(currentEnemyAction);
                            }
                            else
                            {
                                chance = Random.Range(0, 10);
                                if (buildingsInControll < 6)
                                {
                                    if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                }
                                else if (buildingsInControll < 9)
                                {
                                    if (chance < 3)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 8)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                else
                                {
                                    if (chance < 1)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(0);
                                    }
                                    else if (chance < 6)
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(1);
                                    }
                                    else
                                    {
                                        currentEnemyAction = SpawnShipInRandomPosition(2);
                                    }
                                }
                                yield return StartCoroutine(currentEnemyAction);

                            }
                        }

                    }
                }
                else
                {

                    if (i < positionsOfShipsNearBase.Count)
                    {

                        chance = Random.Range(0, 10);
                        if (buildingsInControll < 6)
                        {
                            if (chance < 8)
                            {
                                currentEnemyAction = SpawnShipNearBase(0, i);
                            }
                            else
                            {
                                currentEnemyAction = SpawnShipNearBase(1, i);
                            }
                        }
                        else if (buildingsInControll < 9)
                        {
                            if (chance < 3)
                            {
                                currentEnemyAction = SpawnShipNearBase(0, i);
                            }
                            else if (chance < 8)
                            {
                                currentEnemyAction = SpawnShipNearBase(1, i);
                            }
                            else
                            {
                                currentEnemyAction = SpawnShipNearBase(2, i);
                            }
                        }
                        else
                        {
                            if (chance < 1)
                            {
                                currentEnemyAction = SpawnShipNearBase(0, i);
                            }
                            else if (chance < 6)
                            {
                                currentEnemyAction = SpawnShipNearBase(1, i);
                            }
                            else
                            {
                                currentEnemyAction = SpawnShipNearBase(2, i);
                            }
                        }
                        yield return StartCoroutine(currentEnemyAction);
                        i++;
                        if (i > positionsOfShipsNearBase.Count) i = 0;
                    }
                    yield return new WaitForEndOfFrame();
                    
                }
            }
        }
    }

    IEnumerator SpawnShipInRandomPosition(int type)
    {

        while (shipsPrice[type] > money)
        {
            if(isPlayerShipsNearToBase == true)
            {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        Vector3 spawnPosition = new Vector3(Random.Range(spawnArea.verticesPos[1].x, spawnArea.verticesPos[0].x), 0.133f, Random.Range(spawnArea.verticesPos[0].z, spawnArea.verticesPos[2].z));
        if (enemyShipPool.GetShip(type, Side.Enemy, spawnPosition) == true)
        {
            money -= shipsPrice[type];
            enemyShipsCount += 1;
        }
        yield return new WaitForFixedUpdate();
    }
    IEnumerator SpawnShipNearBase(int type, int positionIndex)
    {
        while (shipsPrice[type] > money)
        {
            yield return new WaitForFixedUpdate();
        }

        Vector3 spawnPosition = new Vector3( positionsOfShipsNearBase[positionIndex].x + Random.Range(-5,5), 0.133f, spawnArea.verticesPos[0].z - Random.Range(2, 10));
        if (enemyShipPool.GetShip(type, Side.Enemy, spawnPosition) == true)
        {
            money -= shipsPrice[type];
            enemyShipsCount += 1;
        }
        yield return new WaitForFixedUpdate();
    }

    IEnumerator UpgradeControllBuilding()
    {
        for (int i = 1; i < 4; i++)
        {
            if(isPlayerShipsNearToBase == false)
            {
                for (int j = 0; j < controllBuildings.Count; j++)
                {
                    if (isPlayerShipsNearToBase == false)
                    {
                        if (controllBuildings[j].buildingLevel == i)
                        {
                            while (money < Building.buildingUpgradePrice[i - 1])
                            {
                                if (isPlayerShipsNearToBase == false)
                                {
                                    yield return new WaitForFixedUpdate();
                                }
                                else
                                {
                                    yield break;
                                }
                            }
                            controllBuildings[j].Upgrade();
                            money -= Building.buildingUpgradePrice[i - 1];
                            i += controllBuildings.Count;
                            j += 4;
                            yield break;
                        }
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            else
            {
                yield break;
            }
        }
        yield return new WaitForFixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ships"))
        {
            if (other.tag == "Player")
            {
                positionsOfShipsNearBase.Add(other.transform.position);
                if (playerShipsNearToBase == 0)
                {
                    isPlayerShipsNearToBase = true;
                }
                playerShipsNearToBase++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ships"))
        {
            if (other.tag == "Player")
            {
                positionsOfShipsNearBase.Remove(other.transform.position);
                if (playerShipsNearToBase == 0)
                {
                    isPlayerShipsNearToBase = false;
                }
                playerShipsNearToBase--;
            }
        }
    }
}
