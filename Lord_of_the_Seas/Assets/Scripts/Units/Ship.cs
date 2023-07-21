using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ship : MonoBehaviour, IDamageble
{
    [SerializeField] private Side side;
    [SerializeField] public float shipHP { get; private set; }
    [SerializeField] private float maxShipHP;
    private bool isDestroy = false;

    [SerializeField] private Vector3 destination = new Vector3(0, 0, 0);
    private NavMeshAgent shipAgent;
    private LineRenderer lineRenderer;
    private SphereCollider sphereCollider;
    private Animator animator;
    private ParticleSystem particleSystem;
    [SerializeField] AudioClip audioClip;

    [SerializeField] private float lightCannonDamage = 15;
    [SerializeField] private int lightCannons = 4;
    [SerializeField] private float lightCannonsRecharge = 4;
    private float timeToRechargeLight;

    [SerializeField] private float heightCannonDamage = 5;
    [SerializeField] private int heightCannons = 2;
    [SerializeField] private float heightCannonsRecharge = 3.5f;
    private float timeToRechargeHeight;

    private Vector3 capturePointOffset = new Vector3(0, 1, 0);
    [SerializeField] private float captureSpeed = 25.55f;

    [SerializeField] private List<Ship> shipTargetsForAttack = new List<Ship>();
    private Ship currentShipTarget;
    [SerializeField] private int shipTargetsForAttackCount;

    [SerializeField] private List<Building> buildingTargetsForAttack = new List<Building>();
    [SerializeField] private Building currentBuildingTarget;
    [SerializeField] private int buildingForAttackCount = 0;
    [SerializeField] private List<Building> buildingTargetsForCapture = new List<Building>();
    private int buildingForCaptureCount = 0;

    private CannonBallPool cannonBallsPool;
    public System.Action<GameObject> OnShipDeActivate;
    public System.Action<Ship> OnShipDestroy;
    private IEnumerator currentCoroutine;

    void Awake()
    {
        shipAgent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        particleSystem = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
        animator.enabled = false;
        lightCannonDamage += Random.Range(0f, 0.25f);
        heightCannonDamage += Random.Range(0f, 0.25f);

        if (side == Side.Player)
        {
            cannonBallsPool = GameObject.Find("/Player/CannonBallPool").GetComponent<CannonBallPool>();
            destination = GameObject.Find("/EnemyBase").transform.position;
        }
        else
        {
            cannonBallsPool = GameObject.Find("/Enemy/CannonBallPool").GetComponent<CannonBallPool>();
            destination = GameObject.Find("/PlayerBase").transform.position;
            heightCannonDamage *= -1;
            captureSpeed *= -1;
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

    }

    public void SetProperties(ref Side side, ref Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        gameObject.SetActive(true);
        StartCoroutine(SetShipProperties(side, spawnPosition));
    }

    IEnumerator SetShipProperties(Side side, Vector3 spawnPosition)
    {
        yield return new WaitForEndOfFrame();
        shipTargetsForAttack.Clear();
        shipTargetsForAttackCount = 0;
        buildingTargetsForAttack.Clear();
        buildingForAttackCount = 0;
        buildingTargetsForCapture.Clear();
        buildingForCaptureCount = 0;

        timeToRechargeLight = lightCannonsRecharge;
        timeToRechargeHeight = heightCannonsRecharge;

        this.side = side;
        //gameObject.tag = (type+1).ToString();
        //transform.GetChild(0).gameObject.tag = this.side.ToString();

        isDestroy = false;
        shipHP = maxShipHP;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;

        if (spawnPosition.x < -8 || spawnPosition.x > 8)
        {
            destination.x = spawnPosition.x;
        }
        else
        {
            if (spawnPosition.x <= 0)
            {
                destination.x = -8;
            }
            else
            {
                destination.x = 8;
            }
        }
        yield return new WaitForEndOfFrame();
        shipAgent.enabled = true;
        shipAgent.SetDestination(destination);
        sphereCollider.enabled = true;
    }

    private void FixedUpdate()
    {

        if (timeToRechargeLight <= lightCannonsRecharge)
        {
            timeToRechargeLight += Time.deltaTime;
        }
        else
        {
            if (shipTargetsForAttackCount > 0)
            {
                if (currentShipTarget != null)
                {
                    AttackTargetFromLightCanons(0);
                }
            }
            else if (buildingForAttackCount > 0)
            {
                if (currentBuildingTarget != null)
                {
                    AttackTargetFromLightCanons(1);
                }
            }
        }

        if (timeToRechargeHeight <= heightCannonsRecharge)
        {
            timeToRechargeHeight += Time.deltaTime;
        }
        else
        {
            if (buildingForAttackCount > 0)
            {
                if (currentBuildingTarget != null)
                {
                    AttackTargetFromHeightCanons();
                }
            }
            
        }

        if (buildingForCaptureCount > 0)
        {
            CaptureBuilding();
        }
        
    }

    private void CheckSpeedMode()
    {
        float acceleration, speed;
        if (buildingForCaptureCount > 0)
        {
            acceleration = 0.4f; speed = 0.5f;
        }
        else if (buildingForAttackCount > 0)
        {
            acceleration = 0.5f; speed = 0.6f;
        }
        else if (shipTargetsForAttackCount > 0)
        {
            acceleration = 0.6f; speed = 0.7f;
        }
        else
        {
            acceleration = 1.6f; speed = 2f;
        }
        shipAgent.acceleration = acceleration;
        shipAgent.speed = speed;
    }

    public void GetDamage(float damage)
    {
        shipHP -= damage;
        if (shipHP < 0)
        {
            DestroyShip();
        }
    }

    private void FindShipToAttack()
    {
        Ship tempShip = null;
        float minHP = 2000f;
        foreach (Ship ship in shipTargetsForAttack)
        {
            float tempHP = ship.shipHP;
            if (minHP > tempHP)
            {
                minHP = tempHP;
                tempShip = ship;
            }
        }
        currentShipTarget = tempShip;
    }

    private void FindBuldingToAttack()
    {
        Building tempTower = null;
        float minDistance = 400f;
        foreach (Building tower in buildingTargetsForAttack)
        {
            if (tower.currentSide != side)
            {
                float tempDistance = Vector3.Distance(transform.position, tower.transform.position);
                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    tempTower = tower;
                }
            }
        }
        currentBuildingTarget = tempTower;
    }

    private void AttackTargetFromLightCanons(byte t)
    {
        currentCoroutine = ShootFromLightCanons(t);
        StartCoroutine(currentCoroutine);
        AudioManager.instance.PlayEffect(audioClip);
        timeToRechargeLight = 0;
    }

    IEnumerator ShootFromLightCanons(byte t)
    {
        Vector3 tempSpawnPos;
        if (t == 0)
        {
            for (int i = 0; i < lightCannons; i++)
            {
                if (currentShipTarget != null)
                {
                    tempSpawnPos = transform.position + new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.05f, 0.4f), Random.Range(-3f, 3f));
                    cannonBallsPool.GetCannonBall(currentShipTarget.gameObject, tempSpawnPos, lightCannonDamage);
                    yield return new WaitForSeconds(0.12f + Random.Range(20, 30) * 0.005f);
                }
                else timeToRechargeLight += Time.deltaTime * i;
            }
        }
        else
        {
            for (int i = 0; i < lightCannons; i++)
            {
                if (currentBuildingTarget != null)
                {
                    tempSpawnPos = transform.position + new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.05f, 0.4f), Random.Range(-3f, 3f));
                    if (currentBuildingTarget.currentSide != Side.Neutral)
                    {
                        cannonBallsPool.GetCannonBall(currentBuildingTarget.gameObject, tempSpawnPos, heightCannonDamage);
                    }
                    else
                    {
                        if (heightCannonDamage >= 0)
                        {
                            cannonBallsPool.GetCannonBall(currentBuildingTarget.gameObject, tempSpawnPos, heightCannonDamage * -1);
                        }
                        else
                        {
                            cannonBallsPool.GetCannonBall(currentBuildingTarget.gameObject, tempSpawnPos, heightCannonDamage);
                        }
                    }
                    yield return new WaitForSeconds(0.12f + Random.Range(20, 30) * 0.005f);
                }
                else timeToRechargeLight += Time.deltaTime * i;

            }
        }
    }

    private void AttackTargetFromHeightCanons()
    {
        currentCoroutine = ShootFromHeightCanons();
        StartCoroutine(currentCoroutine);
        AudioManager.instance.PlayEffect(audioClip);
        timeToRechargeHeight = 0;

    }

    IEnumerator ShootFromHeightCanons()
    {
        Vector3 tempSpawnPos = transform.position;
        for (int i = 0; i < heightCannons; i++)
        {
            if (currentBuildingTarget != null)
            {
                tempSpawnPos += new Vector3(Random.Range(-0.6f, 0.6f), Random.Range(0.2f, 0.4f), Random.Range(-0.6f, 0.6f));
                if(currentBuildingTarget.currentSide != Side.Neutral)
                {

                    cannonBallsPool.GetCannonBall(currentBuildingTarget.gameObject, tempSpawnPos, heightCannonDamage);
                }
                else
                {
                    if(heightCannonDamage >= 0)
                    {

                        cannonBallsPool.GetCannonBall(currentBuildingTarget.gameObject, tempSpawnPos, heightCannonDamage * -1);
                    }
                    else
                    {

                        cannonBallsPool.GetCannonBall(currentBuildingTarget.gameObject, tempSpawnPos, heightCannonDamage);
                    }
                }
                yield return new WaitForSeconds(0.1f + Random.Range(20, 30) * 0.005f);
            }
            else timeToRechargeHeight += Time.deltaTime * i * 3;
        }
    }

    private void CaptureBuilding()
    {
        if(lineRenderer.enabled == true)
        {
            for (int i = 0; i < buildingForCaptureCount; i++)
            {
                lineRenderer.SetPosition(i * 2, transform.position + capturePointOffset);
                buildingTargetsForCapture[i].Capture(ref captureSpeed);
            }
        }
    }

    private void SetCaptureBuildingPoints()
    {
        if (buildingForCaptureCount > 0)
        {
            lineRenderer.positionCount = buildingForCaptureCount * 2;
            for (int i = 0; i < buildingForCaptureCount; i++)
            {
                lineRenderer.SetPosition(i * 2 + 1, buildingTargetsForCapture[i].buildingPos);
            }
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }
    public void DestroyShip()
    {
        isDestroy = true;
        StopCoroutine(currentCoroutine);
        currentCoroutine = PrepareShipForReturningToThePool();
        StartCoroutine(currentCoroutine);
    }

    public Side GetSide()
    {
        return side;
    }

    IEnumerator PrepareShipForReturningToThePool()
    {
        sphereCollider.enabled = false;
        lineRenderer.enabled = false;
        shipAgent.enabled = false;

        for (int i = 0; i < buildingTargetsForAttack.Count; i++)
        {
            buildingTargetsForAttack[i].OnChangeSide -= CheckCurrentBuildingState;
        }

        for (int i = 0; i < buildingTargetsForCapture.Count; i++)
        {
            buildingTargetsForCapture[i].OnChangeSide -= CheckCurrentBuildingState;
        }

        shipTargetsForAttack.Clear();
        shipTargetsForAttackCount = 0;
        buildingTargetsForAttack.Clear();
        buildingForAttackCount = 0;
        buildingTargetsForCapture.Clear();
        buildingForCaptureCount = 0;

        SetCaptureBuildingPoints();
        CheckSpeedMode();

        yield return new WaitForEndOfFrame();

        OnShipDestroy?.Invoke(this);

        yield return new WaitForEndOfFrame();

        particleSystem.Play();
        yield return new WaitForSeconds(0.1f);
        animator.enabled = true;
        animator.Play("ShipDestroy");
        yield return new WaitForSeconds(2.51f);

        animator.enabled = false;
        OnShipDeActivate?.Invoke(gameObject);
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }

    private void RemoveShipFromTargetList(Ship destroyedShip)
    {
        if (currentShipTarget != null)
        {
            if (currentShipTarget.GetInstanceID() == destroyedShip.GetInstanceID())
                currentShipTarget = null;
        }
        if(shipTargetsForAttack.Remove(destroyedShip) == true)
        {
            shipTargetsForAttackCount--;
            FindShipToAttack();
            CheckSpeedMode();
        }
            
    }

    public void CheckCurrentBuildingState(Building tower)
    {
        if (tower.currentSide != Side.Neutral)
        {
            if (buildingTargetsForAttack.Contains(tower) == false)
            {
                buildingTargetsForAttack.Add(tower);
                if (tower.currentSide != side)
                {
                    FindBuldingToAttack();
                    buildingForAttackCount++;
                }
            }
            if (buildingTargetsForCapture.Contains(tower) == true)
            {
                buildingTargetsForCapture.Remove(tower);
                buildingForCaptureCount--;
                SetCaptureBuildingPoints();
            }
        }
        else
        {
            if (buildingTargetsForAttack.Contains(tower) == true)
            {
                buildingTargetsForAttack.Remove(tower);
                if (tower.lastSide != side)
                    buildingForAttackCount--;
                if (ReferenceEquals(currentBuildingTarget, tower))
                {
                    FindBuldingToAttack();
                }
            }
            if (buildingTargetsForCapture.Contains(tower) == false)
            {
                buildingTargetsForCapture.Add(tower);
                buildingForCaptureCount++;
                SetCaptureBuildingPoints();
            }
        }
        CheckSpeedMode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ships"))
        {

            if (other.tag != side.ToString())
            {
                Ship otherShip = other.gameObject.GetComponentInParent<Ship>();
                shipTargetsForAttack.Add(otherShip);
                otherShip.OnShipDestroy += RemoveShipFromTargetList;
                shipTargetsForAttackCount++;
                FindShipToAttack();
                CheckSpeedMode();
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Detectors"))
        {
            Building otherTower = other.gameObject.GetComponentInChildren<Building>();
            if (otherTower.currentSide == Side.Neutral)
            {
                if(otherTower.is—aptured == true)
                {
                    buildingTargetsForAttack.Add(otherTower);
                    buildingForAttackCount++;
                    FindBuldingToAttack();
                }
                else
                {
                    buildingTargetsForCapture.Add(otherTower);
                    buildingForCaptureCount++;
                    SetCaptureBuildingPoints();
                }
                otherTower.OnChangeSide += CheckCurrentBuildingState;
                CheckSpeedMode();
            }
            else
            {
                buildingTargetsForAttack.Add(otherTower);
                otherTower.OnChangeSide += CheckCurrentBuildingState;

                if (otherTower.currentSide != side)
                {
                    buildingForAttackCount++;
                    FindBuldingToAttack();
                }
                CheckSpeedMode();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ships"))
        {
            if (other.tag != side.ToString())
            {
                Ship otherShip = other.gameObject.GetComponentInParent<Ship>();
                if (otherShip.isDestroy == false)
                {
                    if (shipTargetsForAttack.Remove(otherShip) == true)
                    {
                        shipTargetsForAttackCount--;
                        otherShip.OnShipDestroy -= RemoveShipFromTargetList;

                        FindShipToAttack();
                        CheckSpeedMode();
                    }
                }
            }
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Detectors"))
        {
            Building otherTower = other.gameObject.GetComponentInChildren<Building>();

            if (buildingTargetsForAttack.Contains(otherTower) == true)
            {
                buildingTargetsForAttack.Remove(otherTower);
                if (otherTower.currentSide != side)
                    buildingForAttackCount--;
                FindBuldingToAttack();
            }
            if (buildingTargetsForCapture.Contains(otherTower) == true)
            {
                buildingTargetsForCapture.Remove(otherTower);
                buildingForCaptureCount--;
                SetCaptureBuildingPoints();
            }
            otherTower.OnChangeSide -= CheckCurrentBuildingState;
            CheckSpeedMode();
        }
    }

}
