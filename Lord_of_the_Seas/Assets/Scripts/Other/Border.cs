using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField] bool vertical = true;

    [SerializeField] Vector3 point0_End, point1_End;

    LineRenderer lineRender;

    private void Awake()
    {
        lineRender = GetComponent<LineRenderer>();
        lineRender.positionCount = 2;
        lineRender.SetPosition(0, point0_End); lineRender.SetPosition(1, point1_End);
    }

}
