using UnityEngine;

public class ClearScreenCameraShakeScript : MonoBehaviour
{
    [SerializeField] private GameObject cameraShakeObj;


    void Awake()
    {
        cameraShakeObj.GetComponent<CameraShakeScript>().CameraShake(.35f);
    }
}
