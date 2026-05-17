using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMeunController : MonoBehaviour
{
    [SerializeField] private List<Button> fullMapList;      //��ü �� ����Ʈ
    [SerializeField] private List<Button> currentMapList;   //���� ������ �ִ� �� ����Ʈ (���������� �� ����Ʈ�� ��)
    [SerializeField] private int mapCount = 1;            //���� ������ �� ���� ����
    [SerializeField] private int currentSelection = 0;  //���� ������ ���� �ε���
    [SerializeField] private TitleMenuController titleMenuController;
    [SerializeField] private Image arrowBothLeft;   // ���� ȭ��ǥ �̹���
    [SerializeField] private Image arrowBothRight;  //  ������ ȭ��ǥ �̹���
    private bool isMoved = false; // �ߺ� �Է� ���� �÷���

    Color arrBothGoColor = new Color(1f, 1f, 1f, 1f); // ������ ��� (�ѱ� �� �ִ� ����)
    Color arrBothStopColor = new Color(1f, 1f, 1f, 0.5f); // �������� ���(���ѱ�� ����)

    // �ʱ�ȭ �Լ�
    public void Initialize()
    {
        fullMapList.AddRange(transform.Find("Map").GetComponentsInChildren<Button>(true));
        arrowBothLeft = transform.Find("ArrowBothLeft").GetComponent<Image>();
        arrowBothRight = transform.Find("ArrowBothRight").GetComponent<Image>();
        GetMap();
        titleMenuController = FindAnyObjectByType<TitleMenuController>();
    }

    private void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    // �� ����Ʈ���� mapCount ������ŭ ���� ������ currentMapList�� �߰��ϴ� �Լ�
    private void GetMap()
    {
        currentMapList.Clear(); // ����Ʈ ���

        if (mapCount > fullMapList.Count)
        {
            Debug.LogWarning("mapCount�� fullMapList�� ������ �ʰ��߽��ϴ�. mapCount�� fullMapList�� ������ �����մϴ�.");
            mapCount = fullMapList.Count; // mapCount�� fullMapList�� ������ ����
        }

        mapCount = Mathf.Clamp(mapCount, 1, fullMapList.Count); // mapCount�� fullMapList�� ������ ���� �ʵ��� ����

        for (int i = 0; i < mapCount; i++)
        {
            currentMapList.Add(fullMapList[i]);

            int sceneIndex = i;
            sceneIndex += 1 ; // �������� �� ��ȣ�� 1���� �����Ѵٰ� ���� (0�� Ÿ��Ʋ ��)

            // ������ ������ �Ѱ��ݴϴ�.
            currentMapList[i].onClick.AddListener(() => LoadScene(sceneIndex));
        }

        currentMapList[currentSelection].gameObject.SetActive(true); // ù ��° �� Ȱ��ȭ
        UpdateArrowStates();
    }

    // Ű���� ����/������ �̵� �� ���� ���� ó��
    private void HandleKeyboardNavigation()
    {
        float xInput = Input.GetAxisRaw("Horizontal"); // A/D, ����/������ ����Ű

        // ����/������ �̵� ó��
        if (xInput != 0)
        {
            if (!isMoved)
            {
                if (xInput > 0) // ������ �̵�
                {
                    currentSelection++;
                    if (currentSelection > currentMapList.Count - 1)
                    {
                        currentSelection = currentMapList.Count - 1; // �ε���  ���� ����
                    }
                }
                else if (xInput < 0) // ���ʷ� �̵�
                {
                    currentSelection--;
                    if (currentSelection < 0)
                    {
                        currentSelection = 0; // �ε��� ���� ����
                    }

                }
                isMoved = true;

                MapSetActive(currentSelection);
                UpdateArrowStates();
            }
        }
        else
        {
            isMoved = false; // Ű���� ���� ���� �÷��� ����
        }

        // �Ϲ� ���ͷ� ���� ó��
        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (currentMapList.Count > 0)
            {
                // ���� ���õ� ��ư�� onClick �̺�Ʈ�� �ڵ�� ����!
                currentMapList[currentSelection].onClick.Invoke();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            titleMenuController.ResetMenuSelection(); // Ÿ��Ʋ �޴��� ���ư� �� �ٽ� �޴� ���� �����ϵ��� ����
            this.gameObject.SetActive(false);
        }
    }

    // ���� ���õ� �� ��ư Ȱ��ȭ, �������� ��Ȱ��ȭ
    private void MapSetActive(int currentSelection)
    {
        for (int i = 0; i < currentMapList.Count; i++)
        {
            currentMapList[i].gameObject.SetActive(false);
        }

        currentMapList[currentSelection].gameObject.SetActive(true);
    }

    // ȭ��ǥ ���� ���¿� ���� ���İ� ����
    private void UpdateArrowStates()
    {
        // ���� ó��: ���� ����Ʈ�� �ƹ��͵� ���ų� 1�� ������ ��� ���� �� ���� ó��
        if (currentMapList == null || currentMapList.Count <= 1)
        {
            if (arrowBothLeft != null) arrowBothLeft.color = arrBothStopColor;
            if (arrowBothRight != null) arrowBothRight.color = arrBothStopColor;
            return;
        }

        // 1. ���� ȭ��ǥ: ���� ���� ����(0��)�̸� ������, �ƴϸ� ���ϰ�
        if (arrowBothLeft != null)
        {
            arrowBothLeft.color = (currentSelection == 0) ? arrBothStopColor : arrBothGoColor;
        }

        // 2. ������ ȭ��ǥ: ���� ���� ������(������ �� ��ȣ)�̸� ������, �ƴϸ� ���ϰ�
        if (arrowBothRight != null)
        {
            arrowBothRight.color = (currentSelection == currentMapList.Count - 1) ? arrBothStopColor : arrBothGoColor;
        }
    }


    private void Update()
    {
        HandleKeyboardNavigation();
    }
}
