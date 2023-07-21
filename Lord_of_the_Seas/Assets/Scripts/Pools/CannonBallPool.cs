using System.Collections.Generic;
using UnityEngine;

public class CannonBallPool : MonoBehaviour
{
    [SerializeField] private GameObject cannonBallPrefab;
    
    [SerializeField] private List<GameObject> cannonBallsList = new List<GameObject>();

    GameObject cannonBallGameObject;
    CannonBall cannonBall;

    private void Awake()
    {
        Vector3 Pos = new Vector3(0, 50, 0);
        for (int i = 0; i < 25; i++)
        {
            cannonBallGameObject = Instantiate(cannonBallPrefab, Pos, Quaternion.identity);
            cannonBallsList.Add(cannonBallGameObject);
            cannonBallGameObject.SetActive(false);
        }
    }

    public GameObject GetCannonBall(GameObject target, Vector3 spawnPosition, float damage )
    {
        if (cannonBallsList.Count > 0)
        {
            cannonBallGameObject = cannonBallsList[0];
            cannonBallsList.Remove(cannonBallGameObject);
        }
        else
        {
            cannonBallGameObject = Instantiate(cannonBallPrefab, spawnPosition,Quaternion.identity);
        }
        cannonBall = cannonBallGameObject.GetComponent<CannonBall>();
        cannonBall.SetProperties(target, ref spawnPosition, damage);
        cannonBall.OnCannonBallDestroy += ReturnFireBall;

        return cannonBallGameObject;
    } 

    private void ReturnFireBall(GameObject cannonBall)
    {
        cannonBall.GetComponent<CannonBall>().OnCannonBallDestroy -= ReturnFireBall;
        cannonBallsList.Add(cannonBall);
    }
}


