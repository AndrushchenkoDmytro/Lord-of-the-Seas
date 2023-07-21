using UnityEngine;

public class Building : MonoBehaviour, IDamageble
{
    [SerializeField] protected Side startSide = Side.Enemy;
    public Side currentSide { get; protected set; }
    public Side lastSide { get; protected set; }
    public Vector3 buildingPos { get; private set; }
    public bool isÑaptured { get; protected set; }

    public int buildingLevel = 1;
    public static readonly int[] buildingUpgradePrice = { 200, 400 };
    [SerializeField] protected float buildingHP = -100;
    [SerializeField] protected float maxBuildingHP = 200;
    [SerializeField] protected float minBuildingHP = -200;

    protected float totalDamage = 0;
    protected bool isÀttacked = false;
    protected bool isDamaged = false;
    protected float stunTime = 0;

    public event System.Action<Building> OnChangeSide;

    [SerializeField] AudioClip loseControllEffect;
    [SerializeField] protected ParticleEffectController particleEffectController;
    [SerializeField] private GameObject[] settlementGameObjects;

    private PlayerController playerController;
    private EnemyController enemyController;


    protected void Awake()
    {
        currentSide = startSide;
        buildingPos = transform.position + new Vector3(0,0f,0);
        isÑaptured = true;

        if(currentSide == Side.Enemy)
        {
            buildingHP = minBuildingHP;
            gameObject.tag = "Enemy";

        }
        else
        {
            buildingHP = maxBuildingHP;
            if(currentSide == Side.Player)
            {
                gameObject.tag = "Player";
            }
            else
            {
                gameObject.tag = "Neutral";
            }
        }

        playerController = GameObject.Find("/Player/PlayerController").GetComponent<PlayerController>();
        enemyController = GameObject.Find("/Enemy/EnemyController").GetComponent<EnemyController>();
        OnChangeSide += enemyController.ChekBuildingSide;
    }

    protected virtual void GetTotalDamage()
    {
        if(isÀttacked == true)
        {
            buildingHP += totalDamage;
            if(currentSide == Side.Player)
            {
                if(buildingHP <= 0)
                {
                    lastSide = currentSide;
                    currentSide = Side.Neutral;
                    gameObject.tag = "Neutral";
                    buildingHP = 0;
                    isÑaptured = false;
                    ÑhangeBuildingLevelToStart();
                    OnChangeSide?.Invoke(this);
                }
            }
            else if (currentSide == Side.Enemy)
            {
                if (buildingHP >= 0)
                {
                    lastSide = currentSide;
                    currentSide = Side.Neutral;
                    gameObject.tag = "Neutral";
                    buildingHP = 0;
                    isÑaptured = false;
                    ÑhangeBuildingLevelToStart();
                    OnChangeSide?.Invoke(this);
                }
            }
            else // side == Side.Neutral
            {
                if(isÑaptured == true)
                {
                    if(buildingHP <= 0)
                    {
                        lastSide = currentSide;
                        isÑaptured = false;
                        buildingHP = 0;
                        ÑhangeBuildingLevelToStart();
                        OnChangeSide.Invoke(this);
                    }
                }
                else
                {
                    if (buildingHP > maxBuildingHP)
                    {
                        lastSide = currentSide;
                        currentSide = Side.Player;
                        gameObject.tag = "Player";
                        buildingHP = maxBuildingHP;
                        isÑaptured = true;
                        playerController.buildingsInControll += 1;
                        OnChangeSide?.Invoke(this);
                    }
                    else if (buildingHP < minBuildingHP)
                    {
                        lastSide = currentSide;
                        currentSide = Side.Enemy;
                        gameObject.tag = "Enemy";
                        buildingHP = minBuildingHP;
                        isÑaptured = true;
                        enemyController.buildingsInControll += 1;
                        OnChangeSide?.Invoke(this);
                    }
                }
            }
            isDamaged = true;
            stunTime = 0;
            totalDamage = 0;
            isÀttacked = false;
        }
    }

    private void LateUpdate()
    {
        GetTotalDamage();
    }

    void FixedUpdate()
    {

        if(isDamaged == false)
        {
            RepairBuilding();
        }
        else
        {
            if(stunTime > 3)
            {
                isDamaged = false;
                stunTime = 0;
            }
            else
            {
                stunTime += Time.deltaTime * 0.5f;
            }
        }
        //Debug.Log("Side - " + side + " isCaptured - " + isÑaptured + " towerHP - " + towerHP);
    }

    private void RepairBuilding()
    {
        float maxPossibleHP = maxBuildingHP * buildingLevel;
        if (currentSide == Side.Enemy)
            maxPossibleHP *= -1;

        if (currentSide == Side.Player)
        {
            if(buildingHP > 0)
            {
                buildingHP += Time.deltaTime;

                if (buildingHP > maxPossibleHP)
                    buildingHP = maxPossibleHP;
            }
        }
        else if(currentSide == Side.Enemy)
        {
            if (buildingHP < 0)
            {
                buildingHP -= Time.deltaTime;
                if (buildingHP < maxPossibleHP)
                    buildingHP = maxPossibleHP;
            }
        }
        else
        {
            if(isÑaptured == true)
            {
                if (buildingHP > 0)
                {
                    buildingHP += Time.deltaTime;
                    if (buildingHP > maxPossibleHP)
                        buildingHP = maxPossibleHP;
                }
            }
            else
            {
                float revolt = Time.deltaTime * 5;
                if (buildingHP < 0)
                {
                    buildingHP += revolt;
                    if (buildingHP > 0)
                        buildingHP = 0;
                }
                else if (buildingHP > 0)
                {
                    buildingHP -= revolt;
                    if (buildingHP < 0)
                        buildingHP = 0;
                }
            }
        }
    }

    public void Capture(ref float captureSpeed)
    {
        if(isÑaptured == false)
        {
            
            totalDamage += captureSpeed * Time.deltaTime;
            isÀttacked = true;
        }
    }

    public virtual void GetDamage(float attackDamage)
    {
        if (isÑaptured == true)
        {
            totalDamage += attackDamage;
            isÀttacked = true;
        }
    }

    public float GetBuildingHP()
    {
        return buildingHP;
    }

    public float GetBuildingMaxHP()
    {
        if(currentSide == Side.Player)
        {
            return maxBuildingHP;
        }
        else
        {
            return minBuildingHP;
        }
    }

    protected void OnBuildingSideChanged(Building tower) 
    {
        OnChangeSide?.Invoke(tower);
    }

    public virtual void Upgrade()
    {
        if (buildingLevel != 3)
        {
            particleEffectController.PlayUpgradeEffect();
            settlementGameObjects[buildingLevel - 1].SetActive(false);
            buildingLevel++;
            if (currentSide == Side.Player)
            {
                playerController.buildingsInControll += 1;
            }
            else
            {
                enemyController.buildingsInControll += 1;
            }
            buildingHP = maxBuildingHP * buildingLevel;
            settlementGameObjects[buildingLevel - 1].SetActive(true);
        }
    }

    protected virtual void ÑhangeBuildingLevelToStart()
    {
        AudioManager.instance.PlayEffect(loseControllEffect);
        if (buildingLevel != 1)
        {
            particleEffectController.PlayLoseBuildingEffect();
            settlementGameObjects[buildingLevel-1].SetActive(false);
            settlementGameObjects[0].SetActive(true);
        }
        if(lastSide == Side.Player)
        {
            playerController.buildingsInControll -= buildingLevel;
        }
        else if(lastSide == Side.Enemy)
        {
            enemyController.buildingsInControll -= buildingLevel;
        }
        buildingLevel = 1;
    }
}
