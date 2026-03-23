using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject eva;
    [SerializeField] private GameObject boss;
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private GameObject aggroArea;
    [SerializeField] private AudioSource[] songs;
    
    [Header("UI References")]
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject decisionScreen;
    [SerializeField] private GameObject nextSceenScreen;
    [SerializeField] private GameObject healthbar;


    [Header("Control Variables")]
    [SerializeField] private int numStages;
    private int currentStage = 1;
    private static bool fightStarted = false;
    private bool isTransitioning = false;
    private bool gameOver = false;
    private BossStateMachine bossStateMachine;
    private PlayerStateMachine playerStateMachine;
    private SaveData saveData;

    public int CurrentStage {get {return currentStage;} set {currentStage = value;}}
    public int NumStages {get {return numStages;}}
    public bool FightStarted {get {return fightStarted;} set {fightStarted = value;}}
    public bool GameOver {get {return gameOver;} set {gameOver = value;}}
    public bool IsTransitioning {get {return isTransitioning;} set {isTransitioning = value;}}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fightStarted = false;
        bossStateMachine = boss.GetComponent<BossStateMachine>();
        playerStateMachine = player.GetComponent<PlayerStateMachine>();
        SetTimeScale(1f);

        saveData = SaveManager.Load();
        if (saveData.shootUnlocked) UnlockPlayerAbility(2);
        if (saveData.canDash) UnlockPlayerAbility(3);
        if (string.IsNullOrEmpty(saveData.lastSaveSpotID)) return;
        GameObject[] saveSpots = GameObject.FindGameObjectsWithTag("SavePoint");
        Debug.Log(saveSpots.Length);
        foreach (GameObject saveSpot in saveSpots) {
            Debug.Log("iterating through save spots");
            SaveSpot spot = saveSpot.GetComponent<SaveSpot>();
            if (spot.SpotID.Equals(saveData.lastSaveSpotID)) {
                Debug.Log("found a match!");
                Debug.Log("loading save");
                playerStateMachine.transform.position = new Vector3(spot.transform.position.x, playerStateMachine.transform.position.y, playerStateMachine.transform.position.z);
                eva.transform.position = new Vector3(spot.transform.position.x - 1f, playerStateMachine.transform.position.y, playerStateMachine.transform.position.z);
                playerStateMachine.Grounded = true;
                eva.transform.position = new Vector3(spot.transform.position.x - 1f, eva.transform.position.y, eva.transform.position.z);
                break;
            }
        }
    }


    public void MakeDecision()
    {
        Time.timeScale = 0.3f;
        aggroArea.SetActive(false);
        decisionScreen.SetActive(true);
    }

    public void BeginBattle()
    {
        Debug.Log("beginning battle");
        songs[currentStage - 1].Play();
        Time.timeScale = 1f;
        IsTransitioning = true;
        fightStarted = true;
        decisionScreen.SetActive(false);
    }

    public void AbandonEnding()
    {
        Time.timeScale = 1f;
        decisionScreen.SetActive(false);
        gameOver = true;
        playerStateMachine.OnDisable();
        cutsceneManager.PlayCutScene(0);
    }

    public void EndGame()
    {
        gameOver = true;
        playerStateMachine.gameObject.SetActive(false);
        if (bossStateMachine.gameObject.activeInHierarchy)
        {
           bossStateMachine.gameObject.SetActive(false); 
        }
        
        healthbar.SetActive(false);
    }

    //insert some way to transition here
    public void BeginNextStage()
    {
        songs[currentStage - 1].Stop();
        currentStage += 1;
        cutsceneManager.PlayCutScene(currentStage);
        Debug.Log("entering next stage");
        IsTransitioning = true;
        bossStateMachine.Health = 100;
        bossStateMachine.Damage *= 2;
        bossStateMachine.MoveSpeed *= 1.5f;
        songs[currentStage - 1].Play();
    }

    public void UnlockPlayerAbility(int ability)
    {
        Debug.Log("unlocking");
        playerStateMachine.UnlockAbility(ability);
    }

    public void PlayerParry()
    {
        bossStateMachine.JumpToState(new BossStunState(bossStateMachine));
    }
    public void CheckWinStatus()
    {
        if (bossStateMachine.gameObject.activeInHierarchy && currentStage == numStages && bossStateMachine.Health <= 0)
        {
            gameOver = true;
            playerStateMachine.OnDisable();
            fightStarted = false;
            //bossStateMachine.JumpToState(new BossStartState(bossStateMachine));
            cutsceneManager.PlayCutScene(1);
        }
        else if (playerStateMachine.Health <= 0)
        {
            gameOver = true;
            playerStateMachine.OnDisable();
            fightStarted = false;
            //bossStateMachine.JumpToState(new BossStartState(bossStateMachine));
            cutsceneManager.PlayCutScene(0);
        } else {
            BeginNextStage();
        }
    }
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    //move to Menu Manager eventually
    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveGame(string spotID){
        saveData.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        saveData.shootUnlocked = playerStateMachine.ShootUnlocked;
        saveData.canDash = playerStateMachine.CanDash;
        saveData.lastSaveSpotID = spotID;
        SaveManager.Save(saveData);
    }
    public void OpenSceneMenu()
    {
        playerStateMachine.OnDisable();
        nextSceenScreen.SetActive(true);
    }
    public void CloseSceneMenu()
    {
        playerStateMachine.OnEnable();
        nextSceenScreen.SetActive(false);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void EndChase()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

}
