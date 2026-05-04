using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.2f; // �̵��� �ɸ��� �ð� (��: 0.2��)
    [SerializeField] private float jumpHeight = 0.1f; // ���� ����
    [SerializeField] private Transform tr;
    [SerializeField] private LayerMask targetLay;
    // Ű�� �ѹ��� �Է¹ޱ� ���� ����
    private bool isMoveing;

    Animator animator;

    Vector3 Pos => transform.position;

    private void Start()
    {
        animator = GetComponent<Animator>();
        tr = transform.Find("PlayerSprite");
    }


    IEnumerator Movement(Vector3 dir)
    {
        // �̹��� ��ȯ
        if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);

        // �̵� �Ǵ� ���� Ű�� �Է¹��� �ʰ� true
        isMoveing = true;
        //animator.SetBool("Run", isMoveing);
        // ���� ������ �Ÿ� (�� ��ġ + ������ ����)
        Vector3 targetPos = Pos + dir;

        float elapsedTime = 0f;// ��� �ð� �ʱ�ȭ
        // ��� �ð��� ������ �̵� �ð�(duration)���� ���� ���� �ݺ�
        while (elapsedTime < moveDuration)
        {
            float progress = elapsedTime / moveDuration; // 0���� 1���� �����
            // Lerp(����, ��, ����): ����(0~1)�� ���� ��ġ�� ������
            // ��� �ð��� ��ü �̵� �ð����� ������ ������ ��� (��: 0.1�� / 0.2�� = 0.5)
            // ��� �̵� (X, Z��)
            Vector3 currentPos = Vector3.Lerp(Pos, targetPos, progress);
            // ���� �̵� (Y��): ���� ��� �̿��� 0 -> 1 -> 0���� ����
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            currentPos.y += yOffset;

            transform.position = currentPos;
            // �� ������ �ð��� ������
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ���������� �����ϸ� ��ġ�� ������
        transform.position = targetPos;
        // ���� Ű�� �Է¹ޱ� ���ؼ� false
        isMoveing = false;
        //animator.SetBool("Run", isMoveing);
    }

    bool CanMove(Vector2 dir)
    {
        // 1. ���� ���� ����: �� ��(ĳ����)�� ���� �ʰ� �̵� �������� 0.6f��ŭ �б�
        
        Vector2 rayStart = (Vector2)transform.position + Vector2.up * 0.5f;
        // 오프셋 값 0.5 올림. 추후에 스프라이트 바뀌면 변경 필요할지도.

        // 2. ���� ��� (�Ÿ� 1f)
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dir, 1f);

        // 3. ����� �α� �߰�
        if (hit.collider != null)
        {
            // ���𰡿� �ε����� ��: �ε��� ����� �̸��� ���̾� ���
            Debug.Log($"<color=red>[����]</color> {hit.collider.name} (���̾�: {LayerMask.LayerToName(hit.collider.gameObject.layer)}) : {hit.collider.gameObject.layer}");

        }
        else
        {
            // �ƹ��͵� ���� ��
            Debug.Log("<color=green>[���]</color> ���� ����ֽ��ϴ�. �̵� ����!");
        }

        // 4. ��(Scene) �� �ð�ȭ (������=����, ���=���)
        Debug.DrawRay(rayStart, dir * 1f, hit.collider != null ? Color.red : Color.green, 0.5f);

        // ��� ��ȯ: �ε��� �� ����� true(�̵� ����)
        return hit.collider == null;
    }


    void Update()
    {
        if (!isMoveing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");


            // �밢���� �����ϱ� ���� ���ǹ�
            if (x != 0 && CanMove(new Vector3(x, 0, 0)))
                StartCoroutine(Movement(new Vector3(x, 0, 0)));
            else if (y != 0 && CanMove(new Vector3(0, y, 0)))
                StartCoroutine(Movement(new Vector3(0, y, 0)));

        }
    }
}
