using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;

public class Condition {
    private bool isAbility;
    private bool isHeat;
    private bool isDead;
    private List<GameObject> animals = new List<GameObject>();

    public Condition(bool _isAbility, bool _isHeat, bool _isDead, GameObject _animal) { 
        isAbility = _isAbility; // �ɷ��� ��� ������ ��������
        isHeat = _isHeat; // ������ �޾��� ��
        isDead = _isDead; // ���� ��������
        animals.Add(_animal); // ������ ID�� ������Ʈ�� ������ ��
    }

    public void Show() {
        Debug.Log(isAbility);
        Debug.Log(isHeat);
        Debug.Log(isDead);
    }
}

public class mainFight : MonoBehaviour
{
    //------------------------Unit Condition & ID Input---------------------------//
    Dictionary <int, Condition> unitCondition = new Dictionary<int, Condition>();
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit = new GameObject[5];
    public GameObject[] enemyUnit  = new GameObject[5];
    public int[] playerUnitID      = new int[5];
    public int[] enemyUnitID       = new int[5];

    //----------------------------Unit Spawn--------------------------------------//
    public Transform[] playerSpawn;
    public Transform[] enemySpawn;
    public GameObject TestUI;
    

    private void Start()
    {
        // ī�޶� ���� ����� 2D �������� ��ȯ
        Camera.main.orthographic = true;
        Instantiate(TestUI, new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("Canvas").transform);

        Enemy       = GameObject.Find("Enemy1").GetComponent<enemy>(); // ��ȭ�ϴ� ��� ������Ʈ�� enemy�� �־��ָ� �˴ϴ�.
        Player      = GameObject.Find("Player").GetComponent<player>();

        playerUnit  = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit   = (GameObject[])Enemy.enemyUnit.Clone();

        // 1 ~ 6���� Transform �ҷ��ͼ� ����� ��.
        playerSpawn = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn  = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        // ID �и�, dictionary�� ����
        InputDictionary();
        SpawnUnit();
    }

    // ���� ���̵� �޾Ƽ� �����ϴ� ����
    void InputDictionary() {
        for (int unitCount = 0; unitCount < 5; unitCount++)
        {
            if (playerUnit[unitCount] != null)
            {
                playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID;

                // �ʱ� ID ���� �ǰ��� ���� ���¸� �־���
                if (playerUnitID[unitCount] <= 10)
                {
                    unitCondition.Add(playerUnitID[unitCount], new Condition(true, false, false, playerUnit[unitCount]));
                }
                else
                {
                    unitCondition.Add(playerUnitID[unitCount], new Condition(false, false, false, playerUnit[unitCount]));
                }
            }

            if (enemyUnit[unitCount] != null)
            {
                enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID;

                if (enemyUnitID[unitCount] <= 1010)
                {
                    unitCondition.Add(enemyUnitID[unitCount], new Condition(true, false, false, enemyUnit[unitCount]));
                    // unitCondition[enemyUnitID[unitCount]].Show(); // ���� �� ���ִ��� �׽�Ʈ
                }
                else
                {
                    unitCondition.Add(enemyUnitID[unitCount], new Condition(false, false, false, enemyUnit[unitCount]));
                }
            }
        }
    }

    void SpawnUnit() { 
        // �Ʊ�, �� unit �� �� ���̰� �� �迭 ������� ��ȯ ����
        for(int i = 0; i < (playerUnit.Length < enemyUnit.Length ? enemyUnit.Length : playerUnit.Length); i++)
        {
            if (playerUnit[i] != null)
                Instantiate(playerUnit[i], playerSpawn[i + 1].position, Quaternion.Euler(0, 90, 0));

            if (enemyUnit[i] != null)
                Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
        }
    }
}
