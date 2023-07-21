using UnityEngine;
using UnityEngine.UI;

public class BuildingHealthProgressBar : MonoBehaviour
{
    [SerializeField] Building building;
    [SerializeField] Transform progressBar;
    [SerializeField] Image firstGroundImage;
    [SerializeField] float progressBarScale = 1;
    float buildingMaxHp;
    [SerializeField] Color[] healthColor;

    private void Awake()
    {
        buildingMaxHp = (int)building.GetBuildingMaxHP();
        firstGroundImage = transform.GetChild(1).GetComponent<Image>();
        building.OnChangeSide += ChangeProgressBarColor;
    }

    private void Start()
    {
        ChangeProgressBarColor(building);
    }

    private void FixedUpdate()
    {
        int hpText = (int)building.GetBuildingHP();
        progressBarScale = Mathf.Abs(hpText / (buildingMaxHp * (building.buildingLevel + 1)));
        progressBar.localScale = new Vector3(progressBarScale, 1, 1);
    }

    public void ChangeProgressBarColor(Building building)
    {
        if(building.currentSide == Side.Enemy)
        {
            firstGroundImage.color = healthColor[0];
        }
        else if(building.currentSide == Side.Player)
        {
            firstGroundImage.color = healthColor[1];
        }
        else
        {
            firstGroundImage.color = healthColor[2];
        }
    }
}
