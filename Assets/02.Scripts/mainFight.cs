using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    Dictionary <float, Condition> unitCondition = new Dictionary<float, Condition>(); // ���� �޾Ҵ���, �ɷ»�� ��������, �׾�����, �ش� ������Ʈ�� Key������ ã�� ���
    public player Player;
    public enemy Enemy;
    public GameObject[] playerUnit   = new GameObject[5]; // player ��ũ��Ʈ���� unit ����
    public GameObject[] enemyUnit    = new GameObject[5]; // enemy ��ũ��Ʈ���� unit ����
    public float[] playerUnitID      = new float[5]; // player unit id ���� id�� �̾Ƽ� �迭 ����
    public float[] enemyUnitID       = new float[5]; // enemy unit id ���� id�� �̾Ƽ� �迭 ����

    //----------------------------Unit Spawn & UI---------------------------------//
    public Transform[] playerSpawn; // player�� unit���� ��ȯ�ϴ� ��ġ �� ����
    public Transform[] enemySpawn; // enemy�� unit���� ��ȯ�ϴ� ��ġ �� ����
    public RectTransform[] playerUiSpawn; // player unit���� condition�� ��Ÿ���� ui��ġ ����
    public RectTransform[] enemyUiSpawn; // enemy unit���� condition�� ��Ÿ���� ui��ġ ����
    public GameObject UnitConditionUi; // AT�� HP ��Ÿ���� Canvas UI
    private GameObject playerBox; // player animals �θ� ������Ʈ
    private GameObject enemyBox; // enemy animals �θ� ������Ʈ
    private GameObject playerUiBox; // palyer ui �θ� ������Ʈ
    private GameObject enemyUiBox; // enemy ui �θ� ������Ʈ

    //-------------------------Game Condition Value-------------------------------//
    public int currentTurn     = 0; // ������ turn ��, unit���� �ѹ��� �ε�ġ�� 1������ �����Ѵ�.
    public int playerUnitCount = 0; // player unit���� �׾��� ��� 1�� �����Ѵ�. ���� �� ������ �ο����ϴ��� ������ �� ���
    public int enemyUnitCount  = 0; // enemy unit���� �׾��� ��� 1�� �����Ѵ�. ���� �� ������ �ο����ϴ��� ������ �� ���
    public int playerUnitRemain;
    public int enemyUnitRemain;
    public float fightSpeed; // unit���� �ο�� �ӵ��� ����. ���� ���� ���� ���� �����մϴ�.
    public GameObject playerWinPanel;
    public GameObject enemyWinPanel;

    //-------------------------------Ability value--------------------------------//
    public int playerUseAbility  = 0; // player �ɷ� ��� ī��Ʈ
    public int enemyUseAbility   = 0; // enemy �ɷ� ��� ī��Ʈ
    public int playerAttackCount = 0; // player Attack ���� �� ������ ������ ���� ī��Ʈ ����
    public int enemyAttackCount  = 0; // enemy Attack ���� �� ������ ������ ���� ī��Ʈ ����

    //---------------------------------Sound value--------------------------------//
    public AudioSource BGM_Sound;
    public AudioSource SFX_Sound;
    public AudioClip unitAttackSound;
    public AudioClip playerDefeatSound;
    public AudioClip playerWinSound;

    private void Start()
    {
        // ī�޶� ���� ����� 2D �������� ��ȯ
        Camera.main.orthographic = true;
        //------------------find correct obj into values----------------------//
        Enemy          = GameObject.Find("Enemy1").GetComponent<enemy>(); // ��ȭ�ϴ� ��� ������Ʈ�� enemy�� �־��ָ� �˴ϴ�.
        Player         = GameObject.Find("Player").GetComponent<player>();

        playerBox      = GameObject.Find("PlayerAnimals");
        enemyBox       = GameObject.Find("EnemyAnimals");

        playerUiBox    = GameObject.Find("PlayerUiBox");
        enemyUiBox     = GameObject.Find("EnemyUiBox");

        playerUnit     = (GameObject[])Player.palyerUnit.Clone();
        enemyUnit      = (GameObject[])Enemy.enemyUnit.Clone();

        playerWinPanel = GameObject.Find("PlayerWinPanel");
        playerWinPanel.SetActive(false); // active -> false  / can't see this panel
        enemyWinPanel  = GameObject.Find("EnemyWinPanel");
        enemyWinPanel.SetActive(false); // active -> false  / can't see this panel

        // 1 ~ 6���� Transform �ҷ��ͼ� ����� ��. 0�� ����ִ� �θ������Ʈ ����
        playerSpawn    = GameObject.Find("PlayerSpawnPoint").GetComponentsInChildren<Transform>();
        enemySpawn     = GameObject.Find("EnemySpawnPoint").GetComponentsInChildren<Transform>();

        playerUiSpawn  = GameObject.Find("PlayerUiPosition").GetComponentsInChildren<RectTransform>();
        enemyUiSpawn   = GameObject.Find("EnemyUiPosition").GetComponentsInChildren<RectTransform>();

        // sound source ����
        BGM_Sound = GameObject.Find("BGM_Manager").GetComponent<AudioSource>();
        SFX_Sound = GameObject.Find("SFX_Manager").GetComponent<AudioSource>();
        // ID �и�, dictionary�� ����
        SpawnUnit();
    }

    private void Update()
    {
        // ���� ����
        if (Input.GetKeyDown(KeyCode.W))
        {
            UnitAttackStart();
        }
    }

    // ���� ���̵� �޾Ƽ� �����ϴ� ����
    // unitCount == �迭 index, addAnimal == ��ȯ�� ������Ʈ, idValue == ���� ID���� ������Ʈ ���ٽ� ����ID + 0.1�� ����
    void InputDictionary(int unitCount, GameObject addAnimal, float idValue) {
        if (addAnimal.GetComponent<animalID>().UnitID < 1000) // player unit ����
        {
            playerUnitID[unitCount] = playerUnit[unitCount].GetComponent<animalID>().UnitID + idValue; // id �ѹ� ����

            if (!(unitCondition.ContainsKey(playerUnitID[unitCount]))) // ���� dictionary�� key���� ������ ID ���� �ִ� �� Ȯ�� ������ True
            {
                // �ʱ� ID ���� �ǰ��� ���� ���¸� �־���
                if (playerUnitID[unitCount] <= 10)
                {
                    // ���� ���۽� �ٷ� �ɷ��� ���Ǵ� unit�� �־��ֱ�
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(true, false, false, addAnimal));
                }
                else
                {
                    // ���� ���۽� �ٷ� �ɷ��� ���ȵǴ� unit �־��ֱ�
                    unitCondition.Add((playerUnitID[unitCount]), new Condition(false, false, false, addAnimal));
                }
            }
            else
            {
                // �ߺ��� ��� idValue �� 0.1���� �����ְ� ���
                InputDictionary(unitCount, addAnimal, idValue + 0.1f);
            }
        }
        // ���� �ϸƻ�����
        else if (addAnimal.GetComponent<animalID>().UnitID > 1000) // enemy unit ����
        {
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
                InputDictionary(unitCount, addAnimal, idValue + 0.1f);
            }
        }
    }

    // ���� ü�°� ���ݷ� ǥ�ÿ� Unit ��ȯ
    void SpawnUnit() { 
        // �Ʊ�, �� unit �� �� ���̰� �� �迭 ������� ��ȯ ����
        for(int i = 0; i < (playerUnit.Length < enemyUnit.Length ? enemyUnit.Length : playerUnit.Length); i++)
        {
            if (playerUnit[i] != null) {
                // prefabs�� ���̷�Űâ ������Ʈ�� ��ȯ
                GameObject addAnimal = Instantiate(playerUnit[i], playerSpawn[i + 1].position, Quaternion.Euler(0, 90, 0));
                // playerBox ������Ʈ�� �ڽ����� �־���
                addAnimal.transform.parent = playerBox.transform;
                InputDictionary(i, addAnimal, 0);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, playerUiSpawn[i + 1].transform.position, Quaternion.identity, playerUiBox.transform);
                // text ���� �۾�
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + playerUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + playerUnit[i].GetComponent<animalID>().Attack;
                playerUnitRemain++;
            }

            // �� �ڵ�� �ϸƻ���.
            if (enemyUnit[i] != null) {
                GameObject addAnimal = Instantiate(enemyUnit[i], enemySpawn[i + 1].position, Quaternion.Euler(0, 90 * -1, 0));
                addAnimal.transform.parent = enemyBox.transform;
                InputDictionary(i, addAnimal, 0);
                // UI Condition ��ȯ
                GameObject setCondition = Instantiate(UnitConditionUi, enemyUiSpawn[i + 1].transform.position, Quaternion.identity, enemyUiBox.transform);
                setCondition.transform.Find("heart").transform.Find("HP").GetComponent<Text>().text  = "" + enemyUnit[i].GetComponent<animalID>().Heart;
                setCondition.transform.Find("attack").transform.Find("AT").GetComponent<Text>().text = "" + enemyUnit[i].GetComponent<animalID>().Attack;
                enemyUnitRemain++;            
            }
        }
    }

    void UnitAttackStart()
    {
        //--------------------���� �ǽ�---------------------//
        // SFX_Sound�� ���� ���� ���� �ֱ�
        SFX_Sound.clip = unitAttackSound;
        // dictionary���� �� ���� �տ��� �ִ� unit�� Condition Class ��������
        Condition playerCondition = unitCondition[playerUnitID[playerUnitCount]];
        Condition enemyCondition = unitCondition[enemyUnitID[enemyUnitCount]];

        // Class�� ����ִ� ������Ʈ�� ��ġ�� ���� ��ġ��
        playerCondition.UnitReturn().transform.DOMoveX(-0.4f, fightSpeed).SetEase(Ease.OutQuint);
        enemyCondition.UnitReturn().transform.DOMoveX(0.4f, fightSpeed).SetEase(Ease.OutQuint);

        // Attack animator ���� Trigger������ �޾� �ѹ��� ����
        playerCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");
        enemyCondition.UnitReturn().GetComponent<Animator>().SetTrigger("isAttack");

        // animalID���� ������ AT, HT, ID ����
        animalID playerID = playerCondition.UnitReturn().GetComponent<animalID>();
        animalID enmeyID = enemyCondition.UnitReturn().GetComponent<animalID>();
        
        // ��� AT���� HP�� ����
        playerID.Heart -= enmeyID.Attack;
        enmeyID.Heart -= playerID.Attack;
        
        // ���� ���� �־��ֱ�
        SFX_Sound.Play();
        // HP Text ���� �� �� ����
        playerUiBox.transform.GetChild(playerUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + playerID.Heart;
        enemyUiBox.transform.GetChild(enemyUnitCount).transform.Find("heart").transform.Find("HP").GetComponent<Text>().text = "" + enmeyID.Heart;

        // player Unit�� �������� ��
        if(playerID.Heart <= 0)
        {
            playerAttackCount = 0;
            playerCondition.isDead[0] = true;
            playerCondition.isHeat[0] = true;
            playerUnitRemain--;
            AbilityUseCheck(playerUnitID[playerUnitCount]);
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 2));
        }
        // player Unit�� ������ ���ϰ� Ÿ�� �޾��� ��
        else
        {
            enemyAttackCount++;
            playerCondition.isHeat[0] = true;
            AbilityUseCheck(playerUnitID[playerUnitCount]);
            StartCoroutine(StartAttackAnim(playerCondition.UnitReturn(), 0)); // 0 player
        }

        // enemy Unit�� �������� ��
        if(enmeyID.Heart <= 0)
        {
            enemyAttackCount = 0;
            enemyCondition.isDead[0] = true;
            enemyCondition.isHeat[0] = true;
            enemyUnitRemain--;
            AbilityUseCheck(enemyUnitID[enemyUnitCount]);
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 3));
        }
        // enemy Unit�� ������ ���ϰ� Ÿ�� �޾��� ��
        else
        {
            playerAttackCount++;
            enemyCondition.isHeat[0] = true;
            AbilityUseCheck(enemyUnitID[enemyUnitCount]);
            StartCoroutine(StartAttackAnim(enemyCondition.UnitReturn(), 1)); // 1 enemy
        }
        currentTurn++;
        Debug.Log("���� ���� : " + currentTurn);
        //--------------------���� ����---------------------//
    }

    void ControllConditionGame(int checkNum) // 0 player , 1 enemy
    {
        //------------����� Ȯ�� �� ���� ����-------------//
        // ���� ���� �о�� position ����
        if (unitCondition[playerUnitID[playerUnitCount]].isDead[0] && checkNum == 0)
        {
            playerBox.transform.DOMoveX(playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
            playerUiBox.GetComponent<RectTransform>().DOAnchorPosX(75 * playerUnitCount, fightSpeed).SetEase(Ease.OutSine);
        }

        if(unitCondition[enemyUnitID[enemyUnitCount]].isDead[0] && checkNum == 1)
        {
            enemyBox.transform.DOMoveX(-1*enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
            enemyUiBox.GetComponent<RectTransform>().DOAnchorPosX(-75 * enemyUnitCount, fightSpeed).SetEase(Ease.OutSine);
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
        EndFightCheck();
    }

    // ���� �� Paneló��
    void EndFightCheck()
    {
        if(playerUnitRemain == 0) // player���� �й� �� ���� ���ÿ� �������� ��� �й�� ó��
        {
            enemyWinPanel.SetActive(true);
            BGM_Sound.Stop();
            SFX_Sound.clip = playerDefeatSound;
            SFX_Sound.Play();
        }
        else if(enemyUnitRemain == 0) // enemy���� �й�
        {
            playerWinPanel.SetActive(true);
            BGM_Sound.Stop();
            SFX_Sound.clip = playerWinSound;
            SFX_Sound.Play();
        }
    }

    // Attack �ǽ��Ҷ����� �Լ� ����
    void AbilityUseCheck(float ID)
    {   
        // attack animal field where detective
        // player
        if(ID > 0 && ID <= 100) // ���� ������ unit�� �ɷ� ���� Ȯ�� ����
        {
            if(ID > 10 && ID <= 20)
            {
                unitCondition[ID].isAbility[0] = true;
                Debug.Log("Ability Set true : " + ID);
            }
            // �ܺ� unit�� �ɷ� ��� �ʿ� ���� ID ��
            for (int current = 0; current < playerUnitID.Length; current++)
            {
                float currentID = playerUnitID[current];
                if (currentID > 0 && currentID <= 10 && playerUnitRemain == 1)
                {
                    unitCondition[currentID].isAbility[0] = true;
                    Debug.Log("Ability Set true : " + currentID);
                }
                else if (currentID > 40 && currentID <= 50 && unitCondition[ID].isDead[0] == true)
                {
                    if (currentID == 41 && (playerUnitCount == 2 || playerUnitCount == 4))
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                    else if (currentID == 42 || currentID == 43)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                }
                else if (currentID > 50 && currentID <= 70 && unitCondition[ID].isHeat[0] == true)
                {
                    unitCondition[currentID].isAbility[0] = true;
                    Debug.Log("Ability Set true : " + currentID);
                }
                else if(currentID > 80 && currentID <= 90 && enemyAttackCount == 3)
                {
                    unitCondition[currentID].isAbility[0] = true;
                    Debug.Log("Ability Set true : " + currentID);
                }
            }
        }
        // enemy
        else if(ID > 1000 && ID <= 1100)
        {
            if (ID > 1010 && ID <= 1020)
            {
                unitCondition[ID].isAbility[0] = true;
                Debug.Log("Ability Set true : " + ID);
            }
            for (int current = 0; current < enemyUnitID.Length; current++)
            {
                float currentID = enemyUnitID[current];
                if (currentID > 1000 && currentID <= 1010 && enemyUnitRemain == 1)
                {
                    unitCondition[currentID].isAbility[0] = true;
                    Debug.Log("Ability Set true : " + currentID);
                }
                else if (currentID > 1040 && currentID <= 1050 && unitCondition[ID].isDead[0] == true)
                {
                    if (currentID == 1041 && (enemyUnitCount == 2 || enemyUnitCount == 4))
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                    else if (currentID == 1042 || currentID == 1043)
                    {
                        unitCondition[currentID].isAbility[0] = true;
                        Debug.Log("Ability Set true : " + currentID);
                    }
                }
                else if (currentID > 1050 && currentID <= 1070 && unitCondition[ID].isHeat[0] == true)
                {
                    unitCondition[currentID].isAbility[0] = true;
                    Debug.Log("Ability Set true : " + currentID);
                }
                else if (currentID > 1080 && currentID <= 1090 && playerAttackCount == 3)
                {
                    unitCondition[currentID].isAbility[0] = true;
                    Debug.Log("Ability Set true : " + currentID);
                }
            }
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
