using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    private Animator spikeAnim;

    [SerializeField] private bool activeSpike = false;


    public void FuncSpike_On()
    {
        activeSpike = true;
    }
    public void FuncSpike_Off()
    {
        activeSpike = false;
    }
    void Start()
    {
        spikeAnim = GetComponent<Animator>();
    }

    void Update()
    {
        if (activeSpike)
        {
            spikeAnim.SetBool("ActiveSpike", true);
        }
        else
        {
            spikeAnim.SetBool("ActiveSpike", false);
        }
    }
}
