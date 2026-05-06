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

    void OnCollisionEnter2D(Collision2D collision)
    {
        activeFloor = true;
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        activeFloor = false;
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
