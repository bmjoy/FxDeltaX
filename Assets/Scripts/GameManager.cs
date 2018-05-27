using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityStandardAssets._2D;
using Assets;
using UnityEngine.SceneManagement;

public enum PlayerMode
{
    MOVING,
    SHOOTING
};

public enum State
{
    GAME_NOT_STARTED,
    TEAM_PREPARATION,
    GAMEPLAY,
    SHOOT_TIME,
    WAIT_FOR_ROUND_START
};

public enum Team
{
    ALPHA,
    BRAVO
};

public class GameManager : MonoBehaviour {

    public static readonly int POWER_MULTIPLICATOR = 3;

    public static readonly float JUMP_COST = 10;
    public static readonly float MOVE_COST = 0.5f;

    public static GameManager instance = null;
    public GameObject currentCharacter;

    private Dictionary<Team, Queue<GameObject>> teams;
    private Dictionary<Team, Color> teamColors;
    private Dictionary<Team, Color> teamColorsActive;
    private Team activeTeam;

    private Team winnerTeam;

    private Timer timer;
    private int waitForRoundTimeLeft;

    private Vector3 startRoundPos;

    private State state = State.GAME_NOT_STARTED;
    private PlayerMode playerMode = PlayerMode.MOVING;
    private bool characterPlacing = false;
    private bool allowToStartGame = true;

    private static int playersQty = 4;
    private static int serieTime = 3;
    private const int teamsQty = 2;
    private const int waitForRoundStartInitialTime = 10;
    //private  const int startingHp = 100;


    public static GameObject GetCurrentCharacter()
    {
        return instance.currentCharacter;
    }

    public static PlayerMode GetPlayerMode()
    {
        return instance.playerMode;
    }

    public static int GetIdleLeftTime()
    {
        return instance.waitForRoundTimeLeft;
    }

    public static State GetGameState()
    {
        return instance.state;
    }

    public static Team GetWinnerTeam()
    {
        return instance.winnerTeam;
    }

    public void StartNewGame()
    {
        allowToStartGame = true;
    }

    public void EndGame()
    {
        foreach (Team team in System.Enum.GetValues(typeof(Team)))
        {
            if (teams[team].Count != 0)
                winnerTeam = team;
        }

        SceneManager.LoadScene("Menu");
        state = State.GAME_NOT_STARTED;
        allowToStartGame = false;
    }

	// Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void createTeams(int players)
    {
        teams = new Dictionary<Team, Queue<GameObject>>();

        foreach (Team team in System.Enum.GetValues(typeof(Team)))
        {
            teams[team] = new Queue<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.GAME_NOT_STARTED)
            UpdateOnGameNotStarted();
        else if (state == State.TEAM_PREPARATION)
            UpdateOnTeamPreparation();
        else if (state == State.GAMEPLAY)
            UpdateOnGameplay();
        else if (state == State.SHOOT_TIME)
            UpdateOnShootTime();
        else if (state == State.WAIT_FOR_ROUND_START)
            UpdateOnWaitForRoundStart();
    }

    private void UpdateOnGameNotStarted()
    {
        if(allowToStartGame)
        {
            playersQty = GameSettings.instance.GetCharactersQty();
            serieTime = GameSettings.instance.GetRoundTime();

            teamColors = new Dictionary<Team, Color>()
            {
                {Team.ALPHA, new Color(1f, 0.25f, 0.25f)},
                {Team.BRAVO, new Color(0.25f, 0.25f, 1f)}
            };

            teamColorsActive = new Dictionary<Team, Color>()
            {
                {Team.ALPHA, new Color(1f, 0.5f, 0.25f)},
                {Team.BRAVO, new Color(0.25f, 0.7f, 1f)}
            };

            createTeams(teamsQty);
            activeTeam = Team.ALPHA;
            if (timer != null)
                timer.Dispose();
            timer = new Timer(new TimerCallback(OnTimerTick));
            state = State.TEAM_PREPARATION;
        }
    }

    private void UpdateOnTeamPreparation()
    {
        if (!characterPlacing)
        {
            activeTeam = activeTeam.Next();
            if (areAllTeamsFullyCreated())
            {
                StateToWaitForRoundStart();
                return;
            }

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            var character = CreateCharacter(mousePos, activeTeam);
   
            teams[activeTeam].Enqueue(character);
            characterPlacing = true;
            currentCharacter = character;
        }
        else
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            currentCharacter.transform.position = mousePos;
            if (Input.GetKeyDown(KeyCode.Mouse0))
                characterPlacing = false;
        }
    }

    private void UpdateOnGameplay()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            playerMode = playerMode.Next();
            UpdateCharacterOnModeChange();
            Debug.Log("Player mode changed to " + playerMode.ToString());
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Sudoku");
            teams[activeTeam].Clear();
            EndGame();
        }

        if (currentCharacter == null)
        {
            StateToWaitForRoundStart();
            return;
        }

        float stamina = currentCharacter.GetComponent<StaminaComponent>().value;
        if (stamina <= 0)
        {
            StateToShootTime();
        }
    }

    private void UpdateOnShootTime()
    {
        var projectile = ProjectilesManager.instance.projectile;
        if (projectile == null)
        {
            StateToWaitForRoundStart();
        }
    }

    private void UpdateOnWaitForRoundStart()
    {
        if (waitForRoundTimeLeft <= 0 || currentCharacter.transform.position != startRoundPos)
        {
            StateToGameplay();
        }
    }

    private bool isCurrentTeamFullyCreated()
    {
        return teams[activeTeam].Count == playersQty;
    }

    private bool areAllTeamsFullyCreated()
    {
        bool teamsFull = true;
        foreach (var item in teams)
            teamsFull &= (item.Value.Count == playersQty);

        return teamsFull;
    }

    private void enableCurrentCharacter()
    {
        foreach (Team t in System.Enum.GetValues(typeof(Team)))
        {
            foreach(GameObject gameObj in teams[t])
            {
                gameObj.GetComponent<Platformer2DUserControl>().enabled = currentCharacter.Equals(gameObj);
                gameObj.GetComponent<Renderer>().material.color = currentCharacter.Equals(gameObj) ? teamColorsActive[t] : teamColors[t];
            }
        }
    }

    private void UpdateCharacterOnModeChange()
    {
        currentCharacter.GetComponent<Platformer2DUserControl>().enabled = (playerMode == PlayerMode.MOVING);
        stopAnimationForCurrentCharacter();
    }

    private void StateToGameplay()
    {
        state = State.GAMEPLAY;
        waitForRoundTimeLeft = 0;
        Camera.main.GetComponent<CameraController>().Enable(true);
        StartNewRound();
    }

    private void StateToWaitForRoundStart()
    {
        state = State.WAIT_FOR_ROUND_START;
        timer.Change(1000, 1000);
        waitForRoundTimeLeft = waitForRoundStartInitialTime;
        stopAnimationForCurrentCharacter();

        activeTeam = activeTeam.Next();
        currentCharacter = teams[activeTeam].Dequeue();
        teams[activeTeam].Enqueue(currentCharacter);
        currentCharacter.GetComponent<StaminaComponent>().Set(100);
        startRoundPos = currentCharacter.transform.position;
        enableCurrentCharacter();
        playerMode = PlayerMode.MOVING;
    }

    private void StateToShootTime()
    {
        state = State.SHOOT_TIME;
    }

    private void stopAnimationForCurrentCharacter()
    {
        if(currentCharacter != null)
        {
            currentCharacter.GetComponent<PlatformerCharacter2D>().getAnim().SetFloat("Speed", 0f);
        }
    }

    private void StartNewRound()
    {
        string previousEquation = currentCharacter.GetComponent<EquationScriptComponent>().GetString();
        GameObject.Find("UIManager").GetComponent<UIManager>().equationInput.text = previousEquation;
        timer.Change(1000, 1000);
    }


    public void KillCharacter(GameObject characterToKill)
    {
        foreach (Team team in System.Enum.GetValues(typeof(Team)))
        {
            var newQueue = new Queue<GameObject>();
            while(teams[team].Any())
            {
                var character = teams[team].Dequeue();
                if(!character.Equals(characterToKill))
                {
                    newQueue.Enqueue(character);
                }
                else
                {
                    Destroy(character);
                }
            }
            teams[team] = newQueue;
        }

        foreach (Team t in System.Enum.GetValues(typeof(Team)))
        {
            if (teams[t].Count == 0)
            {
                EndGame();
                return;
            }
        }
    }

    private GameObject CreateCharacter(Vector3 whereToCreate, Team team)
    {
        GameObject character = (GameObject)Instantiate(Resources.Load("CharacterRobotBoy"), whereToCreate, transform.rotation);
        character.GetComponent<Platformer2DUserControl>().enabled = false;
        var hpComp = character.AddComponent<Assets.HpComponent>();
        hpComp.SetInit(100);
        var staminaComp = character.AddComponent<Assets.StaminaComponent>();
        staminaComp.SetInit(100);
        staminaComp.Set(0);

        character.AddComponent<Assets.EquationScriptComponent>();
        character.AddComponent<Assets.FallingHitter>();

        character.GetComponent<PlatformerCharacter2D>().IfJumped(() => character.GetComponent<StaminaComponent>().Dec(JUMP_COST));
        character.GetComponent<PlatformerCharacter2D>().IfMoved(() => character.GetComponent<StaminaComponent>().Dec(MOVE_COST)); 

        var characterRenderer = character.GetComponent<Renderer>();
        characterRenderer.material.color = teamColors[team];

        return character;
    }

    private void OnTimerTick(System.Object stateInfo)
    {
        if (waitForRoundTimeLeft > 0)
        {
            --waitForRoundTimeLeft;
        }
    }
}
