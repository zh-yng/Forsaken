using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    [SerializeField] private CutsceneManager cutsceneManager;
    [SerializeField] private GameObject aggroArea;

    [Header("UI References")]
    [SerializeField] private GameObject lossScreen;
    [SerializeField] private GameObject winScreen;

    [Header("Control Variables")]
    [SerializeField] private int numStages;
    private int currentStage = 1;
    private bool fightStarted = false;
    private bool isTransitioning = false;
    private bool gameOver = false;
    private BossStateMachine bossStateMachine;
    private PlayerStateMachine playerStateMachine;

    public int CurrentStage {get {return currentStage;} set {currentStage = value;}}
    public int NumStages {get {return numStages;}}
    public bool FightStarted {get {return fightStarted;} set {fightStarted = value;}}
    public bool GameOver {get {return gameOver;} set {gameOver = value;}}
    public bool IsTransitioning {get {return isTransitioning;} set {isTransitioning = value;}}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        bossStateMachine = boss.GetComponent<BossStateMachine>();
        playerStateMachine = player.GetComponent<PlayerStateMachine>();
        SetTimeScale(1f);
    }

    public void BeginBattle()
    {
        IsTransitioning = true;
        fightStarted = true;
        aggroArea.SetActive(false);
    }

    //insert some way to transition here
    public void BeginNextStage()
    {
        currentStage += 1;
        cutsceneManager.PlayCutScene(currentStage);
        Debug.Log("entering next stage");
        IsTransitioning = true;
        bossStateMachine.Health = 100;
        bossStateMachine.Damage *= 2;
        bossStateMachine.MoveSpeed *= 1.5f;
    }

    public void UnlockPlayerAbility(int ability)
    {
        Debug.Log("unlocking");
        playerStateMachine.UnlockAbility(ability);
    }

    public void CheckWinStatus()
    {
        if (currentStage == numStages && bossStateMachine.Health <= 0)
        {
            gameOver = true;
            playerStateMachine.OnDisable();
            cutsceneManager.PlayCutScene(1);
        }
        else if (playerStateMachine.Health <= 0)
        {
            gameOver = true;
            playerStateMachine.OnDisable();
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

}
