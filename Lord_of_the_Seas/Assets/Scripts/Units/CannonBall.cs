using System.Collections;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private GameObject target;
    [SerializeField] private Collider targetHitBox;
    [SerializeField] private float heightY = 5f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private MeshRenderer meshRenderer;
    private float damage = 5;

    IDamageble damageble;

    private IEnumerator currentCoroutine;

    public event System.Action<GameObject> OnCannonBallDestroy;

    public void SetProperties(GameObject target, ref Vector3 spawnPos, float damage)
    {
        this.target = target;
        transform.position = spawnPos;
        this.damage = damage;
        meshRenderer.enabled = true;

        if (target.layer == LayerMask.NameToLayer("City"))
        {
            targetHitBox = this.target.GetComponent<BoxCollider>();
            gameObject.SetActive(true);
            currentCoroutine = MoveToBuilding();
            StartCoroutine(currentCoroutine);
        }
        else
        {
            targetHitBox = this.target.GetComponentInChildren<MeshCollider>();
            gameObject.SetActive(true);
            currentCoroutine = MoveToShip();
            StartCoroutine(currentCoroutine);
        }
    }

    IEnumerator MoveToShip()
    {
        float timePassed = 0;
        float heightT, height;
        Vector3 endPositionOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-1.4f, 1.4f));
        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position;

        float distanceRatio = 5 / Vector3.Distance(startPosition, endPosition);

        while (timePassed < 1)
        {
            if (target != null)
            {
                endPosition = target.transform.position + endPositionOffset;
            }

            heightT = animationCurve.Evaluate(timePassed);

            height = Mathf.Lerp(0, heightY, heightT);

            transform.position = Vector3.Lerp(startPosition, endPosition, timePassed) + new Vector3(0f, height, 0f);


            timePassed += Time.deltaTime * distanceRatio * speed;
            yield return null;
        }
    }
    IEnumerator MoveToBuilding()
    {
        float timePassed = 0;
        float heightT, height;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = target.transform.position + new Vector3(Random.Range(-3.7f, 3.6f), -0.9f, Random.Range(-3.6f, 3.5f));

        float distanceRatio = 5 / Vector3.Distance(startPosition, endPosition);

        while (timePassed < 1)
        {
            heightT = animationCurve.Evaluate(timePassed);

            height = Mathf.Lerp(0, heightY, heightT);

            transform.position = Vector3.LerpUnclamped(startPosition, endPosition, timePassed) + new Vector3(0f, height, 0f);


            timePassed += Time.deltaTime * distanceRatio * speed;
            yield return null;
        }

        yield return new WaitForSeconds(3);
        DeactivateCannonBall();
    }

    IEnumerator CannonBallExplosion()
    {
        particleSystem.Play();
        yield return new WaitForEndOfFrame();
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(1);

        DeactivateCannonBall();
    }

    IEnumerator MoveUnderWater()
    {
        yield return new WaitForSeconds(0.2f);
        DeactivateCannonBall();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null)
        {
            if (other == targetHitBox)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("City"))
                {
                    damageble = target.GetComponent<Building>();
                    damageble.GetDamage(damage);
                    target = null;
                    
                }
                else 
                {
                    damageble = target.GetComponent<Ship>();
                    damageble.GetDamage(damage);
                    target = null;
                    StopCoroutine(currentCoroutine);
                    currentCoroutine = MoveUnderWater();
                    StartCoroutine(currentCoroutine);
                }
          
            }
        }
        else
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Towers") || other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = CannonBallExplosion();
                StartCoroutine(currentCoroutine);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = MoveUnderWater();
                StartCoroutine(currentCoroutine);
            }
        }
    }

    private void DeactivateCannonBall()
    {
        StopCoroutine(currentCoroutine);
        OnCannonBallDestroy?.Invoke(gameObject);
        gameObject.SetActive(false);
    }

    public void PauseCurrentCoroutine()
    {
        StartCoroutine(currentCoroutine);
    }

    public void ResumeCurrentCoroutine()
    {
        StopCoroutine(currentCoroutine);
    }
}
