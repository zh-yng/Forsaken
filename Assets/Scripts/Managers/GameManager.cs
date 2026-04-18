using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    #region Serializable Fields
    [Header("Object References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject eva;
    [SerializeField] private GameObject boss;
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GameObject aggroArea;
    
    [Header("UI References")]
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject decisionScreen;
    [SerializeField] private GameObject healthbar;


    [Header("Control Variables")]
    [SerializeField] private int numStages;
    
    #endregion
    
    #region Game Control Variables
    private int currentStage = 1;
    private static bool fightStarted = false;
    private bool isTransitioning = false;
    private bool gameOver = false;
    #endregion

    #region Object References
    private BossStateMachine bossStateMachine;
    private PlayerStateMachine playerStateMachine;
    private SaveData saveData;
    #endregion

    #region Getters and Setters
    public int CurrentStage {get {return currentStage;} set {currentStage = value;}}
    public int NumStages {get {return numStages;}}
    public bool FightStarted {get {return fightStarted;} set {fightStarted = value;}}
    public bool GameOver {get {return gameOver;} set {gameOver = value;}}
    public bool IsTransitioning {get {return isTransitioning;} set {isTransitioning = value;}}
    public 
    #endregion
    
    #region Level Initialization
    void Start()
    {
        fightStarted = false;
        bossStateMachine = boss.GetComponent<BossStateMachine>();
        playerStateMachine = player.GetComponent<PlayerStateMachine>();
        bossStateMachine.BossDeath += CheckWinStatus;
        SetTimeScale(1f);
        LoadData();
    }
    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
    public void BeginBattle()
    {
        Time.timeScale = 1f;
        IsTransitioning = true;
        fightStarted = true;
        decisionScreen.SetActive(false);
    }
    public void BeginNextStage()
    {
        currentStage += 1;
        cutsceneManager.PlayCutScene(currentStage);
        IsTransitioning = true;
        bossStateMachine.Health = 100;
        bossStateMachine.Damage *= 2;
        bossStateMachine.MoveSpeed *= 1.5f;
    }
    public void EndChase()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

    #endregion

    #region Player Access

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuManager.gamePaused)
            {
                menuManager.ResumeGame();
            } else
            {
                menuManager.PauseGame();
            }
        }
    }
    public void PlayerParry()
    {
        if (bossStateMachine != null && bossStateMachine.gameObject.activeInHierarchy)
        {
            bossStateMachine.Stun(playerStateMachine.ParryCooldown, playerStateMachine.ParrySlowDownAmount);
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<StateMachine>().Stun(playerStateMachine.ParryCooldown, playerStateMachine.ParrySlowDownAmount);
        }
    }
    public void UnlockPlayerAbility(int ability)
    {
        playerStateMachine.UnlockAbility(ability);
    }

    public void DisablePlayer()
    {
        playerStateMachine.OnDisable();
    }
    public void EnablePlayer()
    {
        playerStateMachine.OnEnable();
    }
    #endregion

    #region Multiple Endings
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
    public void MakeDecision()
    {
        Time.timeScale = 0.3f;
        aggroArea.SetActive(false);
        decisionScreen.SetActive(true);
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
    #endregion

    #region Save System
    public void SaveGame(string spotID){
        saveData.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        saveData.shootUnlocked = playerStateMachine.ShootUnlocked;
        saveData.canDash = playerStateMachine.CanDash;
        saveData.lastSaveSpotID = spotID;
        SaveManager.Save(saveData);
    }
    private void LoadData()
    {
        saveData = SaveManager.Load();
        SetAllDifficulties(saveData.difficulty);
        if (saveData.shootUnlocked) UnlockPlayerAbility(2);
        if (saveData.canDash) UnlockPlayerAbility(3);
        if (string.IsNullOrEmpty(saveData.lastSaveSpotID)) return;
        GameObject[] saveSpots = GameObject.FindGameObjectsWithTag("SavePoint");
        foreach (GameObject saveSpot in saveSpots) {
            SaveSpot spot = saveSpot.GetComponent<SaveSpot>();
            if (spot.SpotID.Equals(saveData.lastSaveSpotID)) {
                playerStateMachine.transform.position = new Vector3(spot.transform.position.x, playerStateMachine.transform.position.y, playerStateMachine.transform.position.z);
                eva.transform.position = new Vector3(spot.transform.position.x - 1f, playerStateMachine.transform.position.y, playerStateMachine.transform.position.z);
                playerStateMachine.Grounded = true;
                eva.transform.position = new Vector3(spot.transform.position.x - 1f, eva.transform.position.y, eva.transform.position.z);
                break;
            }
        }
    }

    private void SetAllDifficulties(Difficulty difficulty)
    {
        //search for all IDataPersistence objects, including inactive ones
        IEnumerable<ISetDifficulty> objectsWithDifficulties = FindObjectsByType<MonoBehaviour>(0).OfType<ISetDifficulty>();

        foreach (ISetDifficulty setDifficulty in objectsWithDifficulties)
        {
            setDifficulty.HandleDifficulty(difficulty);
        }
    }

    #endregion

}
