using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTower : Building
{
    [SerializeField] private List<Ship> shipTargets;
    private Ship currentTarget;
    private int shipTargetsForAttackCount;
    [SerializeField] private float lightCannonDamage = 15;
    [SerializeField] private int lightCannons = 3;
    [SerializeField] private float lightCannonsRecharge = 4;
    private float timeToRechargeLight;

    IEnumerator currentCoroutine;
    private CannonBallPool cannonBallsPool;
    [SerializeField] private Transform[] TowersParts;
    [SerializeField] private Transform[] tower1Components;
    [SerializeField] Color[] sideColor;
    [SerializeField] AudioClip audioClip; 
    
    private void Awake()
    {
        base.Awake();
        cannonBallsPool = GameObject.Find("/Ñolonies/TowerCannonPool").GetComponent<CannonBallPool>();
        timeToRechargeLight = 4f;
    }

    void FixedUpdate()
    {
        if(currentTarget != null)
        {
            if (isÑaptured == true && currentSide != currentTarget.GetSide())

            {
                if (timeToRechargeLight <= lightCannonsRecharge)
                {
                    timeToRechargeLight += Time.deltaTime;
                }
                else
                {
                    if (shipTargetsForAttackCount > 0)
                    {
                        if (currentTarget != null)
                        {
                            AttackTargetFromLightCanons(0);
                        }
                    }
                }
            }
        }
    }

    public override void GetDamage(float attackDamage)
    {
        if (isÑaptured == true)
        {
            totalDamage += attackDamage * 0.65f;
            isÀttacked = true;
        }
    }

    protected override void GetTotalDamage()
    {
        if (isÀttacked == true)
        {
            buildingHP += totalDamage;
            if (currentSide == Side.Player)
            {
                if (buildingHP <= 0)
                {
                    lastSide = currentSide;
                    currentSide = Side.Neutral;
                    gameObject.tag = "Neutral";
                    buildingHP = 0;
                    isÑaptured = false;
                    ÑhangeBuildingLevelToStart();
                    OnBuildingSideChanged(this);
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
                    OnBuildingSideChanged(this);
                }
            }
            else // side == Side.Neutral
            {
                if (isÑaptured == true)
                {
                    if (buildingHP <= 0)
                    {
                        lastSide = currentSide;
                        isÑaptured = false;
                        buildingHP = 0;
                        ÑhangeBuildingLevelToStart();
                        OnBuildingSideChanged(this);
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
                        ChangeSideColor();
                        OnBuildingSideChanged(this);
                    }
                    else if (buildingHP < minBuildingHP)
                    {
                        lastSide = currentSide;
                        currentSide = Side.Enemy;
                        gameObject.tag = "Enemy";
                        buildingHP = minBuildingHP;
                        isÑaptured = true;
                        ChangeSideColor();
                        OnBuildingSideChanged(this);
                    }
                }
            }
            isDamaged = true;
            stunTime = 0;
            totalDamage = 0;
            isÀttacked = false;
        }
    }

    private void ChangeSideColor()
    {
        if(currentSide == Side.Player)
        {
            for (int i = 0; i < TowersParts.Length; i++)
            {
                MeshRenderer meshRenderer = TowersParts[i].GetChild(0).GetComponent<MeshRenderer>();

                meshRenderer.materials[0].color = sideColor[0];
                meshRenderer.materials[6].color = sideColor[0];
            }
        }
        else if (currentSide == Side.Neutral)
        {
            for (int i = 0; i < TowersParts.Length; i++)
            {
                MeshRenderer meshRenderer = TowersParts[i].GetChild(0).GetComponent<MeshRenderer>();
                meshRenderer.materials[0].color = sideColor[1];
                meshRenderer.materials[6].color = sideColor[1];
            }
        }
        else
        {
            for (int i = 0; i < TowersParts.Length; i++)
            {
                MeshRenderer meshRenderer = TowersParts[i].GetChild(0).GetComponent<MeshRenderer>();
                meshRenderer.materials[0].color = sideColor[2];
                meshRenderer.materials[6].color = sideColor[2];
            }
        }


    }

    public override void Upgrade()
    {
        if (buildingLevel != 3)
        {
            particleEffectController.PlayUpgradeEffect();
            TowersParts[buildingLevel].gameObject.SetActive(true);
            if(buildingLevel == 1)
            {
                for (int i = 0; i < tower1Components.Length; i++)
                {
                    tower1Components[i].gameObject.SetActive(false);
                }
            }
            buildingLevel++;
            buildingHP = maxBuildingHP * buildingLevel;
            lightCannonDamage += 7.5f;
            lightCannons += 1;
            lightCannonsRecharge += 0.25f;

        }
    }

    protected override void ÑhangeBuildingLevelToStart()
    {
        if (buildingLevel != 1)
        {
            particleEffectController.PlayLoseBuildingEffect();
            for (int i = buildingLevel; i > 1; i--)
            {
                TowersParts[i - 1].gameObject.SetActive(false);
            }
            for (int i = 0; i < tower1Components.Length; i++)
            {
                tower1Components[i].gameObject.SetActive(false);
            }

            lightCannonDamage -= 7.5f * buildingLevel;
            lightCannons -= 1 * buildingLevel;
            lightCannonsRecharge -= 0.25f * buildingLevel;
        }
        buildingLevel = 1;
    }



    private void FindShipToAttack()
    {
        Ship tempShip = null;
        float minHP = 2000f;
        foreach (Ship ship in shipTargets)
        {
            float tempHP = ship.shipHP;
            if (minHP > tempHP)
            {
                minHP = tempHP;
                tempShip = ship;
            }
        }
        currentTarget = tempShip;
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

        for (int i = 0; i < lightCannons; i++)
        {
            if (currentTarget != null)
            {
                tempSpawnPos = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.05f, 0.2f), Random.Range(-1f, 1f));
                cannonBallsPool.GetCannonBall(currentTarget.gameObject, tempSpawnPos, lightCannonDamage);
                yield return new WaitForSeconds(0.12f + Random.Range(20, 30) * 0.005f);
            }
            else timeToRechargeLight += Time.deltaTime * i;
        }
    }

    private void RemoveShipFromTargetList(Ship destroyedShip)
    {
        if (currentTarget != null)
        {
            if (currentTarget.GetInstanceID() == destroyedShip.GetInstanceID())
                currentTarget = null;
        }
        if (shipTargets.Remove(destroyedShip) == true)
        {
            shipTargetsForAttackCount--;
            FindShipToAttack();
        }

    }

    public void AddTarget(Ship ship)
    {
        shipTargets.Add(ship);
        ship.OnShipDestroy += RemoveShipFromTargetList;
        shipTargetsForAttackCount++;
        FindShipToAttack();
    }

    public void RemoveTarget(Ship ship)
    {
         if (shipTargets.Remove(ship) == true)
         {
             shipTargetsForAttackCount--;
             ship.OnShipDestroy -= RemoveShipFromTargetList;
             FindShipToAttack();
         }
    }
}
