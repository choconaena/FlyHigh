using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Condition {
    public List<bool> isAbility     = new List<bool>();
    public List<bool> isHeat        = new List<bool>();
    public List<bool> isDead        = new List<bool>();
    public List<GameObject> animals = new List<GameObject>();

    public Condition(bool _isAbility, bool _isHeat, bool _isDead, GameObject _animal) { 
        isAbility.Add(_isAbility); // �ɷ��� ��� ������ ��������
        isHeat.Add(_isHeat); // ������ �޾��� ��
        isDead.Add(_isDead); // ���� ��������
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

    public bool isAbilityReturn()
    {
        return isAbility[0];
    }

    public bool isHeatReturn()
    {
        return isHeat[0];
    }

    public bool isDeadReturn()
    {
        return isDead[0];
    }
}

public class mainFight : MonoBehaviour
{
    //------------------------Unit Condition & ID Input---------------------------//
    Dictionary <float, Condition> unitCondition = new Dictionary<float, Condition>();
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit = new GameObject[5];
    public GameObject[] enemyUnit  = new GameObject[5];
    public float[] playerUnitID      = new float[5];
    public float[] enemyUnitID       = new float[5];

    //----------------------------Unit Spawn & UI---------------------------------//
    public Transform[] playerSpawn;
    public Transform[] enemySpawn;
    public RectTransform[] playerUiSpawn;
    public RectTransform[] enemyUiSpawn;
    public GameObject UnitConditionUi;
    private GameObject playerBox; // player animals �θ� ������Ʈ
    private GameObject enemyBox; // enemy animals �θ� ������Ʈ
    private GameObject playerUiBox; // palyer ui �θ� ������Ʈ
    private GameObject enemyUiBox; // enemy ui �θ� ������Ʈ

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn     = 0;
    public int playerUnitCount = 0;
    public int enemyUnitCount  = 0;
    public float fightSpeed;

    private void Start()
    {
        // ī�޶� ���� ����� 2D �������� ��ȯ
        Camera.main.orthographic = true;

        Enemy         = GameObject.Find("Enemy1").GetComponent<enemy>(); // ��ȭ�ϴ� ��� ������Ʈ�� enemy�� �־��ָ� �˴ϴ�.
        Player        = GameObject.Find("Player").GetComponent<player>();

        playerBox     = GameObject.Find("PlayerAnimals");
        enemyBox      = GameObject.Find("EnemyAnimals");

        playerUiBox   = GameObject.Find("PlayerUiBox");
        enemyUiBox    = GameObject.Find("EnemyUiBox");

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
    void InputDictionary(int unitCount, GameObject addAnimal, float idValue) {
        if (addAnimal.GetComponent<animalID>().UnitID < 1000)
        {
            Debug.Log(idValue);
            playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID + idValue;

            if (!(unitCondition.ContainsKey(playerUnitID[unitCount])))
            {
                // �ʱ� ID ���� �ǰ��� ���� ���¸� �־���
                if (playerUnitID[unitCount] <= 10)
                {
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                }
                else
                {
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(false, false, false, addAnimal));
                }
            }
            else
            {
                Debug.Log("�ߺ��Դϴ�.");
                InputDictionary(unitCount, addAnimal, idValue + 0.1f);
            }
        }

        else if (addAnimal.GetComponent<animalID>().UnitID > 1000)
        {
            Debug.Log(idValue);
            enemyUnitID[unitCount] = enemyUnit[unitCount].GetComponent<animalID>().UnitID + idValue;

            if (!(unitCondition.ContainsKey(enemyUnitID[unitCount])))
            {
                if (enemyUnitID[unitCount] <= 1010)
                {
                    unitCondition.Add((enemyUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                    // unitCondition[enemyUnitID[unitCount]].Show(); // ���� �� ���ִ��� �׽�Ʈ
                }
                else
                {
                    unitCondition.Add((enemyUnitID[unitCount]), new Condition(false, false, false, addAnimal));
                }
            }
            else
            {
                Debug.Log("�ߺ��Դϴ�.");
                InputDictionary(unitCount, addAnimal, (idValue + 0.1f));
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
                addAnimal.transform.parent = playerBox.transform;
                InputDictionary(i, addAnimal,0);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, playerUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
            }

            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                addAnimal.transform.parent = enemyBox.transform;
                InputDictionary(i, addAnimal,0);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, enemyUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
            }
        }
    }

    void UnitAttackStart()
    {
        //--------------------���� �ǽ�---------------------//
        Condition playerCondition = unitCondition[playerUnitID[playerUnitCount]];
        Condition enemyCondition = unitCondition[enemyUnitID[enemyUnitCount]];

        playerCondition.UnitReturn().transform.DOMoveX(-0.4f, fightSpeed).SetEase(Ease.OutQuint);
        enemyCondition.UnitReturn().transform.DOMoveX(0.4f, fightSpeed).SetEase(Ease.OutQuint);

        playerCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");
        enemyCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");

        animalID playerID = playerCondition.UnitReturn().GetComponent<animalID>();
        animalID enmeyID = enemyCondition.UnitReturn().GetComponent<animalID>();
        
        playerID.Heart -= enmeyID.Attack;
        enmeyID.Heart -= playerID.Attack;

        playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + playerID.Heart;
        enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + enmeyID.Heart;

        if(playerID.Heart <= 0)
        {
            Debug.Log("player Dead");
            playerCondition.isDead[0] = true;
            playerCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 2));
            Debug.Log("player heat : " + playerCondition.isHeat);
            Debug.Log("player dead : " + playerCondition.isDead);
        }
        else
        {
            Debug.Log("player Heat");
            playerCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 0)); // 0 player
        }

        if(enmeyID.Heart <= 0)
        {
            Debug.Log("enemy Dead");
            enemyCondition.isDead[0] = true;
            enemyCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 3));
        }
        else
        {
            Debug.Log("enemy Heat");
            enemyCondition.isHeat[0] = true;
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 1)); // 1 enemy
            Debug.Log("enemy heat : " + enemyCondition.isHeat);
            Debug.Log("enemy dead : " + enemyCondition.isDead);
        }
        //--------------------���� ����---------------------//
    }

    void ControllConditionGame(int checkNum) // 0 player , 1 enemy
    {
        //------------����� Ȯ�� �� ���� ����-------------//
        // ���� ���� �о�� position ����
        if (unitCondition[playerUnitID[playerUnitCount]].isDead[0] && checkNum == 0)
        {
            playerBox.transform.DOMoveX(playerUnitCount, 1f).SetEase(Ease.OutSine);
            playerUiBox.GetComponent<RectTransform>().DOAnchorPosX(75 * playerUnitCount, 1f).SetEase(Ease.OutSine);
        }

        if(unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] && checkNum == 1)
        {
            enemyBox.transform.DOMoveX(-1*enemyUnitCount, 1f).SetEase(Ease.OutSine);
            enemyUiBox.GetComponent<RectTransform>().DOAnchorPosX(-75 * enemyUnitCount, 1f).SetEase(Ease.OutSine);
        }

        //--------isHeat & isDead Ȯ�� �� �ɷ� ���-------//
        //------------------------------------------------//
        if (unitCondition[playerUnitID[playerUnitCount]].isDead[0] && checkNum == 0)
        {
            unitCondition[playerUnitID[playerUnitCount]].isHeat.RemoveAt(0);
            unitCondition[playerUnitID[playerUnitCount]].isDead.RemoveAt(0);
            unitCondition[playerUnitID[playerUnitCount]].animals.RemoveAt(0);
            ++playerUnitCount;
        }

        if (unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] && checkNum == 1)
        {
            unitCondition[enemyUnitID[enemyUnitCount]].isHeat.RemoveAt(0);
            unitCondition[enemyUnitID[enemyUnitCount]].isDead.RemoveAt(0);
            unitCondition[enemyUnitID[enemyUnitCount]].animals.RemoveAt(0);
            ++enemyUnitCount;
        }
    }

    IEnumerator StartAttackAnim(GameObject animalObj, int checkNum) // checkNum 0 == player, 1 == enemy, 2 == playerDestroy, 3 == enemyDestroy
    {
        yield return new WaitForSeconds(0.01f); // ������
        yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        
        if(checkNum == 0)
        {
            animalObj.transform.DOMoveX(-1, fightSpeed).SetEase(Ease.OutQuint);
        }
        else if(checkNum == 1)
        {
            animalObj.transform.DOMoveX(1, fightSpeed).SetEase(Ease.OutQuint);
        }
        else if(checkNum == 2)
        {
            animalObj.GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(animalObj);
            playerUiBox.transform.GetChild(playerUnitCount).gameObject.SetActive(false);
            ControllConditionGame(0);
        }
        else if(checkNum == 3)
        {
            animalObj.GetComponent<Animator>().SetTrigger("isDead");
            yield return new WaitForSeconds(0.01f);
            yield return new WaitForSeconds(animalObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
            Destroy(animalObj);
            enemyUiBox.transform.GetChild(enemyUnitCount).gameObject.SetActive(false);
            ControllConditionGame(1);
        }
    }
}
