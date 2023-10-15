using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class Condition {
    private bool isAbility;
    private bool isHeat;
    private bool isDead;
    public List<GameObject> animals = new List<GameObject>();

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

    public GameObject UnitReturn()
    {
        return animals[0];
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

    //----------------------------Unit Spawn & UI---------------------------------//
    public Transform[] playerSpawn;
    public Transform[] enemySpawn;
    public RectTransform[] playerUiSpawn;
    public RectTransform[] enemyUiSpawn;
    public GameObject UnitConditionUi;

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn = 0;

    private void Start()
    {
        // ī�޶� ���� ����� 2D �������� ��ȯ
        Camera.main.orthographic = true;

        Enemy         = GameObject.Find("Enemy1").GetComponent<enemy>(); // ��ȭ�ϴ� ��� ������Ʈ�� enemy�� �־��ָ� �˴ϴ�.
        Player        = GameObject.Find("Player").GetComponent<player>();

        playerUnit    = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit     = (GameObject[])Enemy.enemyUnit.Clone();

        // 1 ~ 6���� Transform �ҷ��ͼ� ����� ��.
        playerSpawn   = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn    = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        playerUiSpawn = GameObject.Find("PlayerUiPosition").GetComponentsInChildren<RectTransform>();
        enemyUiSpawn  = GameObject.Find("EnemyUiPosition").GetComponentsInChildren<RectTransform>();
        // ID �и�, dictionary�� ����
        SpawnUnit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnitAttackStart();
        }
    }

    // ���� ���̵� �޾Ƽ� �����ϴ� ����
    void InputDictionary(int unitCount, GameObject addAnimal) {
        if (addAnimal.GetComponent<animalID>().UnitID < 1000)
        {
            playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID;

            // �ʱ� ID ���� �ǰ��� ���� ���¸� �־���
            if (playerUnitID[unitCount] <= 10)
            {
                unitCondition.Add(playerUnitID[unitCount], new Condition(true, false, false, addAnimal));
            }
            else
            {
                unitCondition.Add(playerUnitID[unitCount], new Condition(false, false, false, addAnimal));
            }
        }

        else if (addAnimal.GetComponent<animalID>().UnitID > 1000)
        {
            enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID;

            if (enemyUnitID[unitCount] <= 1010)
            {
                unitCondition.Add(enemyUnitID[unitCount], new Condition(true, false, false, addAnimal));
                // unitCondition[enemyUnitID[unitCount]].Show(); // ���� �� ���ִ��� �׽�Ʈ
            }
            else
            {
                unitCondition.Add(enemyUnitID[unitCount], new Condition(false, false, false, addAnimal));
            }
        }
    }

    // ���� ü�°� ���ݷ� ǥ�ÿ� Unit ��ȯ
    void SpawnUnit() { 
        // �Ʊ�, �� unit �� �� ���̰� �� �迭 ������� ��ȯ ����
        for(int i = 0; i < (playerUnit.Length < enemyUnit.Length ? enemyUnit.Length : playerUnit.Length); i++)
        {
            if (playerUnit[i] != null) {
                GameObject addAnimal = Instantiate(playerUnit[i], playerSpawn[i + 1].position, Quaternion.Euler(0, 90, 0));
                InputDictionary(i, addAnimal);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
            }

            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                InputDictionary(i, addAnimal);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, GameObject.Find("Canvas").transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
            }
        }
    }

    void UnitAttackStart()
    {
        Debug.Log(unitCondition[enemyUnitID[0]].UnitReturn());
        unitCondition[enemyUnitID[0]].UnitReturn().GetComponent<Animator>().SetBool("isAttack", true);
        //unitCondition[enemyUnitID[0]].UnitReturn().GetComponent<Animator>().SetBool("isAttack", false);
    }
}
