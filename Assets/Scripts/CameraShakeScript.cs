using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeScript : MonoBehaviour
{

    private CinemachineImpulseSource impluseSource;
    void Awake()
    {
        impluseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void CameraShake(float force = 1f)
    {
        if (impluseSource != null)
        {
            impluseSource.GenerateImpulse(force);
        }
    }
}
