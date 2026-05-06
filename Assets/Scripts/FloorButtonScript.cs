using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    private Animator floorAnim;
    private BoxCollider2D bc;
    
    [SerializeField] private bool activeFloor;
    
    [SerializeField] private GameObject spikeObject;
    //활성화 할 스파이크 오브젝트 할당

    void Start()
    {
        floorAnim = GetComponent<Animator>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        activeFloor = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        activeFloor = true;
    }


    void Update()
    {
        if (activeFloor)
        {
            floorAnim.SetBool("ActiveFloorButton", true);
            spikeObject.GetComponent<SpikeScript>().FuncSpike_On();
        }
        else
        {
            floorAnim.SetBool("ActiveFloorButton", false);
            spikeObject.GetComponent<SpikeScript>().FuncSpike_Off();
        }
    }
}
