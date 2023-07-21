using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera playerCamera;
    [SerializeField] float cameraXBounds = 60;
    [SerializeField] float cameraBottomBounds = -140;
    [SerializeField] float cameraTopBounds = 90;
    
    Vector3 startPoint;
    Vector3 currentPoint;
    Vector3 newCameraPos;
    RaycastHit downButtonHit;
    RaycastHit moveButtonHit;
    [SerializeField] LayerMask moveMask; 

    [SerializeField] Vector2 panSpeed = new Vector2(3f,6);

    private void Awake()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = playerCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out downButtonHit, 100f, moveMask))
            {
                startPoint = downButtonHit.point;
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = playerCamera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out moveButtonHit, 100f, moveMask))
            {
                currentPoint = moveButtonHit.point;
            }
            if ((Vector3.Distance(downButtonHit.point, moveButtonHit.point) >= 0.6f))
            {
                Vector3 direction = startPoint - currentPoint;
                direction.y = 0;
                direction.x *= panSpeed.x;
                direction.z *= panSpeed.y;
                Vector3 currentCameraPos = playerCamera.transform.position;
                newCameraPos = currentCameraPos + (direction * Time.deltaTime);

                if (newCameraPos.x < -cameraXBounds || newCameraPos.x > cameraXBounds)
                {
                    newCameraPos.x = currentCameraPos.x;
                }
                if (newCameraPos.z < cameraBottomBounds || newCameraPos.z > cameraTopBounds)
                {
                    newCameraPos.z = currentCameraPos.z;
                }
                playerCamera.transform.position = newCameraPos;
            }
        }
    }
}
