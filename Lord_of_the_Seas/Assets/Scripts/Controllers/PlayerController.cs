using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum Side
{
    Player,
    Enemy,
    Neutral
}

public class PlayerController : MonoBehaviour
{
    Camera palyerCamera;

    [SerializeField] private Side side;
    private SpawnArea spawnArea;
    private int contactLayer = (1 << 9) | (1 << 10) | (1 << 11) | (1 << 11);

    public int shipType = 0;
    [SerializeField] private ShipPool playerShipPool;


    private float money = 99;
    [SerializeField] float moneyPerSecond = 0;
    const float moneyTax = 2.95f;

    public int buildingsInControll = 0;
    public static readonly int[] shipsPrice = { 100, 200, 400 };
    Vector3 spawnPosition;

    private GraphicRaycaster gr;
    PointerEventData ped = new PointerEventData(EventSystem.current);
    List<RaycastResult> results = new List<RaycastResult>();

    RaycastHit hit;
    
    float doubleClickTime = 0.25f;
    float lastClickTime = 0;
    bool byShip = false;
    int price = 0;
    bool isSpawnAreaSelect = false;

    [SerializeField] AudioClip[] audioClip;

    private void Awake()
    {
        palyerCamera = Camera.main;
        gr = GameObject.Find("PlayerUI").GetComponent<GraphicRaycaster>();
        spawnArea = GameObject.Find("/Player/SpawnZone").GetComponent<SpawnArea>();
        playerShipPool = GameObject.Find("/Player/ShipPool").GetComponent<ShipPool>();
        GameObject enemyBase = GameObject.Find("/EnemyBase");
        buildingsInControll += enemyBase.transform.childCount;
    }

    void Update()
    {

        moneyPerSecond = moneyTax * buildingsInControll * Time.deltaTime;
        money += moneyPerSecond;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touchPosition = touch.position;
                Ray ray = palyerCamera.ScreenPointToRay(touchPosition);

                ped.position = touchPosition;

                gr.Raycast(ped, results);
                if (results.Count == 0)
                {
                    if (Physics.Raycast(ray, out hit, 100f, contactLayer))
                    {
                        LayerMask layerMask = hit.collider.gameObject.layer;
                        if (layerMask == LayerMask.NameToLayer("City"))
                        {
                            if (hit.collider.tag == "Player")
                            {
                                if (Time.time - lastClickTime < doubleClickTime)
                                {
                                    Building building = hit.collider.gameObject.GetComponent<Building>();
                                    price = Building.buildingUpgradePrice[building.buildingLevel - 1];
                                    if (price < money)
                                    {

                                        money -= price;
                                        UIManager.instance.SpendMoney(price);
                                        building.Upgrade();
                                        AudioManager.instance.PlayEffect(audioClip[0]);
                                    }
                                }
                                lastClickTime = Time.time;
                                byShip = false;
                            }
                        }
                        else if (layerMask == LayerMask.NameToLayer("SpawnArea"))
                        {
                            if (hit.collider.tag == "Player")
                            {
                                if (isSpawnAreaSelect == true)
                                {
                                    isSpawnAreaSelect = false;
                                    spawnArea.DeselectSpawnArea();
                                }
                                spawnPosition = hit.point;
                                byShip = true;
                            }
                        }
                        else if (layerMask == LayerMask.NameToLayer("NavMesh"))
                        {
                            if (isSpawnAreaSelect == true)
                            {
                                isSpawnAreaSelect = false;
                                spawnArea.DeselectSpawnArea();
                            }
                            float xPos = Mathf.Clamp(hit.point.x, spawnArea.verticesPos[1].x, spawnArea.verticesPos[0].x);
                            float zPos = Mathf.Clamp(hit.point.z, spawnArea.verticesPos[0].z, spawnArea.verticesPos[3].z);
                            spawnPosition = new Vector3(xPos, 0, zPos);

                            byShip = true;
                        }
                        else if (layerMask == LayerMask.NameToLayer("Ocean"))
                        {
                            if (isSpawnAreaSelect == false)
                            {
                                isSpawnAreaSelect = true;
                                spawnArea.SelectSpawnArea();
                            }
                        }
                    }

                }
                results.Clear();
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                Vector3 touchPosition = touch.position;
                Ray ray = palyerCamera.ScreenPointToRay(touchPosition);

                if (Physics.Raycast(ray, out RaycastHit hitCurrent, 100f, contactLayer))
                {
                    if (byShip == true)
                    {
                        if (Vector3.Distance(hit.point, hitCurrent.point) <= 0.7f)
                        {
                            if (shipsPrice[shipType] <= money)
                            {
                                if (playerShipPool.GetShip(shipType, side, spawnPosition) == true)
                                {
                                    AudioManager.instance.PlayEffect(audioClip[Random.Range(1, 3)]);
                                    byShip = false;
                                    price = shipsPrice[shipType];
                                    money -= price;
                                    UIManager.instance.SpendMoney(price);
                                }
                            }
                            if (isSpawnAreaSelect == true)
                            {
                                isSpawnAreaSelect = false;
                                spawnArea.DeselectSpawnArea();
                            }

                        }
                    }
                }

            }
        }

        if(Input.GetMouseButtonDown(0))
        {

            Vector3 mousePos = Input.mousePosition;
            Ray ray = palyerCamera.ScreenPointToRay(mousePos);

            ped.position = mousePos;

            gr.Raycast(ped, results);
            if (results.Count == 0)
            {
                if (Physics.Raycast(ray, out hit, 100f, contactLayer))
                {
                    LayerMask layerMask = hit.collider.gameObject.layer;
                    if (layerMask == LayerMask.NameToLayer("City"))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            if (Time.time - lastClickTime < doubleClickTime)
                            {
                                Building building = hit.collider.gameObject.GetComponent<Building>();
                                price = Building.buildingUpgradePrice[building.buildingLevel - 1];
                                if (price < money)
                                {

                                    money -= price;
                                    UIManager.instance.SpendMoney(price);
                                    building.Upgrade();
                                    AudioManager.instance.PlayEffect(audioClip[0]);
                                }
                            }
                            lastClickTime = Time.time;
                            byShip = false;
                        }
                    }
                    else if (layerMask == LayerMask.NameToLayer("SpawnArea"))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            if (isSpawnAreaSelect == true)
                            {
                                isSpawnAreaSelect = false;
                                spawnArea.DeselectSpawnArea();
                            }
                            spawnPosition = hit.point;
                            byShip = true;
                        }
                    }
                    else if (layerMask == LayerMask.NameToLayer("NavMesh"))
                    {
                        if (isSpawnAreaSelect == true)
                        {
                            isSpawnAreaSelect = false;
                            spawnArea.DeselectSpawnArea();
                        }
                        float xPos = Mathf.Clamp(hit.point.x, spawnArea.verticesPos[1].x, spawnArea.verticesPos[0].x);
                        float zPos = Mathf.Clamp(hit.point.z, spawnArea.verticesPos[0].z, spawnArea.verticesPos[3].z);
                        spawnPosition = new Vector3(xPos, 0, zPos);

                        byShip = true;
                    }
                    else if (layerMask == LayerMask.NameToLayer("Ocean"))
                    {
                        if (isSpawnAreaSelect == false)
                        {
                            isSpawnAreaSelect = true;
                            spawnArea.SelectSpawnArea();
                        }
                    }
                }

            }
            results.Clear();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = palyerCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hitCurrent, 100f, contactLayer))
            {
                if (byShip == true)
                {
                    if (Vector3.Distance(hit.point, hitCurrent.point) <= 0.7f)
                    {
                        if (shipsPrice[shipType] <= money)
                        {
                            if (playerShipPool.GetShip(shipType, side, spawnPosition) == true)
                            {
                                AudioManager.instance.PlayEffect(audioClip[Random.Range(1, 3)]);
                                byShip = false;
                                price = shipsPrice[shipType];
                                money -= price;
                                UIManager.instance.SpendMoney(price);
                            }
                        }
                        if (isSpawnAreaSelect == true)
                        {
                            isSpawnAreaSelect = false;
                            spawnArea.DeselectSpawnArea();
                        }

                    }
                }
            }

        }

    }

    public float GetMoney()
    {
        return money;
    }

    public float GetMoneyPerSecond()
    {
        return buildingsInControll;
    }

    public void StopPlayerController()
    {
        this.gameObject.SetActive(false);
    }

    /*if (Input.GetMouseButtonDown(0))
    {

        Vector3 mousePos = Input.mousePosition;
        Ray ray = palyerCamera.ScreenPointToRay(mousePos);

        ped.position = mousePos;

        gr.Raycast(ped, results);
        if (results.Count == 0)
        {
            if (Physics.Raycast(ray, out hit, 100f, contactLayer))
            {
                LayerMask layerMask = hit.collider.gameObject.layer;
                if (layerMask == LayerMask.NameToLayer("City"))
                {
                    if (hit.collider.tag == "Player")
                    {
                        if (Time.time - lastClickTime < doubleClickTime)
                        {
                            Building building = hit.collider.gameObject.GetComponent<Building>();
                            price = Building.buildingUpgradePrice[building.buildingLevel - 1];
                            if (price < money)
                            {

                                money -= price;
                                UIManager.instance.SpendMoney(price);
                                building.Upgrade();
                                AudioManager.instance.PlayEffect(audioClip[0]);
                            }
                        }
                        lastClickTime = Time.time;
                        byShip = false;
                    }
                }
                else if (layerMask == LayerMask.NameToLayer("SpawnArea"))
                {
                    if (hit.collider.tag == "Player")
                    {
                        if (isSpawnAreaSelect == true)
                        {
                            isSpawnAreaSelect = false;
                            spawnArea.DeselectSpawnArea();
                        }
                        spawnPosition = hit.point;
                        byShip = true;
                    }
                }
                else if (layerMask == LayerMask.NameToLayer("NavMesh"))
                {
                    if (isSpawnAreaSelect == true)
                    {
                        isSpawnAreaSelect = false;
                        spawnArea.DeselectSpawnArea();
                    }
                    float xPos = Mathf.Clamp(hit.point.x, spawnArea.verticesPos[1].x, spawnArea.verticesPos[0].x);
                    float zPos = Mathf.Clamp(hit.point.z, spawnArea.verticesPos[0].z, spawnArea.verticesPos[3].z);
                    spawnPosition = new Vector3(xPos, 0, zPos);

                    byShip = true;
                }
                else if (layerMask == LayerMask.NameToLayer("Ocean"))
                {
                    if (isSpawnAreaSelect == false)
                    {
                        isSpawnAreaSelect = true;
                        spawnArea.SelectSpawnArea();
                    }
                }
            }

        }
        results.Clear();
    }

    if (Input.GetMouseButtonUp(0))
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = palyerCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hitCurrent, 100f, contactLayer))
        {
            if (byShip == true)
            {
                if (Vector3.Distance(hit.point, hitCurrent.point) <= 0.7f)
                {
                    if (shipsPrice[shipType] <= money)
                    {
                        if (playerShipPool.GetShip(shipType, side, spawnPosition) == true)
                        {
                            AudioManager.instance.PlayEffect(audioClip[Random.Range(1,3)]);
                            byShip = false;
                            price = shipsPrice[shipType];
                            money -= price;
                            UIManager.instance.SpendMoney(price);
                        }
                    }
                    if (isSpawnAreaSelect == true)
                    {
                        isSpawnAreaSelect = false;
                        spawnArea.DeselectSpawnArea();
                    }

                }
            }
        }

    }    */


}
