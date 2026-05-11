using UnityEngine;

public class Push : MonoBehaviour
{
    private GameObject[] Obstacles;
    private GameObject[] ObjToPush;

    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Obstacles");
        ObjToPush = GameObject.FindGameObjectsWithTag("ObjToPush");
    }
    void Updated()
    {
    }
    public bool Move(Vector2 direction)
    {
        if (ObjToBlocked(transform.position, direction))
            return false;
        else
        {
            transform.Translate(direction);
            return true;
        }
    }
    public bool ObjToBlocked(Vector3 position, Vector2 direction)
    {
        Vector2 newpos = new Vector2(position.x, position.y) + direction;
        foreach (var obj in Obstacles)//함정찾기
        {
            if (obj.transform.position.x == newpos.x && obj.transform.position.y == newpos.y)
                return true;
        }
        foreach (var objToPush in ObjToPush)//움직여야할 물체 박스같은것들
        {
            if (objToPush.transform.position.x == newpos.x && objToPush.transform.position.y == newpos.y)
                return true;
        }
        foreach (var key in ObjToPush)//열쇄 이동
        {
            if (key.transform.position.x == newpos.x && key.transform.position.y == newpos.y)
                return true;
            //else

            //비활성화
        }
        return false;
    }
}
