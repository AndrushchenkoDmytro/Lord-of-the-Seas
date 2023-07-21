using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{

    [SerializeField] ParticleSystem UpgradeEffect;
    [SerializeField] ParticleSystem LoseTowerEffect;
    [SerializeField] Building building;

    private void Awake()
    {
        UpgradeEffect = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        LoseTowerEffect = transform.GetChild(1).gameObject.GetComponent<ParticleSystem>();
    }

    public void PlayUpgradeEffect()
    {
        UpgradeEffect.Play();
    }

    public void PlayLoseBuildingEffect()
    {
        LoseTowerEffect.Play();
    }
}
