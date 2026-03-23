using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject boss;
    [SerializeField] private GameManager manager;

    [Header("Cutscene References")]
    [SerializeField] private GameObject[] cutscenes;
    private BossStateMachine bossStateMachine;
    private PlayerStateMachine playerStateMachine;
    private int currentCutscene;

    void Awake()
    {
        bossStateMachine = boss.GetComponent<BossStateMachine>();
        playerStateMachine = player.GetComponent<PlayerStateMachine>();
    }

    public void PlayCutScene(int index)
    { 
        manager.FightStarted = false;
        cutscenes[index].GetComponent<PlayableDirector>().Play();
        currentCutscene = index;
    }

    public void OnCutsceneStart()
    {
        playerStateMachine.OnDisable();
        if (bossStateMachine.gameObject.activeInHierarchy == true)
        {
            bossStateMachine.IsStunned = false;
            bossStateMachine.JumpToState(new BossStartState(bossStateMachine));  
            manager.FightStarted = false;
        }
        
    }

    public void OnCutsceneEnd()
    {
        if (!manager.GameOver)
        {
            playerStateMachine.OnEnable();
        }
        if (bossStateMachine.gameObject.activeInHierarchy == true)
        {
            bossStateMachine.IsStunned = false;
            manager.FightStarted = true;
        }
        cutscenes[currentCutscene].GetComponent<PlayableDirector>().Stop();
    }

}
