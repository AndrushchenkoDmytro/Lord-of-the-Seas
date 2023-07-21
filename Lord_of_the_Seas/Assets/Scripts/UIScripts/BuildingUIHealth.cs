using UnityEngine;
using TMPro;

public class BuildingUIHealth : MonoBehaviour
{
    static Transform cameraPos;
    [SerializeField] Building building;
    [SerializeField] Transform hpTransform;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Side startSide;
    [SerializeField] Color[] healthColor;
    float textMinPosZ = 0;
    float textMaxPosZ = 0;
    float textMinSize = 1.7f;
    float textMaxSize = 2.2f;
    

    private void Awake()
    {
        if (cameraPos == null)
        {
            cameraPos = Camera.main.transform;
        }
        textMinPosZ = hpTransform.localPosition.z;
        textMaxPosZ = textMinPosZ - 2.18f;
        if (startSide == Side.Enemy)
        {
            hpText.color = healthColor[0];
        }
        else if (startSide == Side.Player)
        {
            hpText.color = healthColor[1];
        }
        else
        {
            hpText.color = healthColor[2];
        }
        building.OnChangeSide += ChangeHealthColor;
    }

    private void Update()
    {
        float distane = Mathf.Abs(cameraPos.position.z - hpTransform.position.z); // 62 - max 
        distane = Mathf.Clamp(distane, 5, 62);
        float per = distane/62; //:80 -6.82 -9
        Vector3 tempPos = hpTransform.localPosition;
        tempPos.z = Mathf.Lerp(textMinPosZ, textMaxPosZ, per);
        hpTransform.localPosition = tempPos;
        hpText.fontSize = Mathf.Lerp(textMinSize, textMaxSize, per);
    }

    private void FixedUpdate()
    {
        int hpText = (int)building.GetBuildingHP();
        if (hpText < 0) hpText = Mathf.Abs(hpText);
        this.hpText.text = hpText.ToString();
    }

    public void ChangeHealthColor(Building building)
    {
        if(building.currentSide == Side.Enemy)
        {
            hpText.color = healthColor[0];
        }
        else if(building.currentSide == Side.Player)
        {
            hpText.color = healthColor[1];
        }
        else
        {
            hpText.color = healthColor[2];
        }
    }

}
