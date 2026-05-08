using UnityEngine;
using System.Collections.Generic;

public class TestUndo : MonoBehaviour
{
    [SerializeField] private List<Vector3> playerVec;


    public void SetPlayerVectorAdd(Vector3 playerVector)
    {
        playerVec.Add(playerVector);
    }
    
}
