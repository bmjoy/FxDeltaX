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
    GAMEPLAY
};

public class GameManager : MonoBehaviour {

    private enum Team
    {
        ALPHA,
        BETA
    };

    public static readonly int POWER_MULTIPLICATOR = 100;

    public static GameManager instance = null;
    public GameObject currentCharacter;

    private Dictionary<Team, Queue<GameObject>> teams;
    private Dictionary<Team, Color> teamColors;
    private Team activeTeam;

    private Timer timer;
    private int timeLeft;

    private State state = State.GAME_NOT_STARTED;
    private PlayerMode playerMode = PlayerMode.MOVING;
    private bool characterPlacing = false;
    private bool allowToStartGame = true;

    private static int playersQty = 4;
    private static int serieTime = 3;
    private const int teamsQty = 2;
    //private  const int startingHp = 100;


    public static GameObject GetCurrentCharacter()
    {
        return instance.currentCharacter;
    }

    public static PlayerMode GetPlayerMode()
    {
        return instance.playerMode;
    }

    public static int GetRoundLeftTime()
    {
        return instance.timeLeft;
    }

    public State GetGameState()
    {
        return instance.state;
    }

    public void StartNewGame()
    {
        allowToStartGame = true;
    }

    public void EndGame()
    {
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
    }

    private void UpdateOnGameNotStarted()
    {
        if(allowToStartGame)
        {
            playersQty = GameSettings.instance.GetCharactersQty();
            serieTime = GameSettings.instance.GetRoundTime();

            teamColors = new Dictionary<Team, Color>()
            {
                {Team.ALPHA, Color.red},
                {Team.BETA, Color.blue}
            };

            createTeams(teamsQty);
            activeTeam = Team.ALPHA;
            timer = new Timer(new TimerCallback(OnTimerTick));
            state = State.TEAM_PREPARATION;
        }
    }

    private void UpdateOnTeamPreparation()
    {
        if (!characterPlacing)
        {
            if (areAllTeamsFullyCreated())
            {
                prepareToGameplay();
                return;
            }
            else if(isCurrentTeamFullyCreated())
            {
                activeTeam = activeTeam.Next();
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
            EndGame();
        }
        if (timeLeft == 0)
        {
            currentCharacter.GetComponent<PlatformerCharacter2D>().getAnim().SetFloat("Speed", 0f);
            StartNewRound();
        }
    }

    private void prepareToGameplay()
    {
        state = State.GAMEPLAY;
        Camera.main.GetComponent<CameraController>().Enable(true);
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
            }
        }
    }

    private void UpdateCharacterOnModeChange()
    {
        currentCharacter.GetComponent<Platformer2DUserControl>().enabled = (playerMode == PlayerMode.MOVING);
        currentCharacter.GetComponent<PlatformerCharacter2D>().getAnim().SetFloat("Speed", 0f);
    }

    private void StartNewRound()
    {
        activeTeam = activeTeam.Next();
        if(!teams[activeTeam].Any())
        {
            EndGame();
            return;
        }
        currentCharacter = teams[activeTeam].Dequeue();
        teams[activeTeam].Enqueue(currentCharacter);
        enableCurrentCharacter();
        playerMode = PlayerMode.MOVING;
        currentCharacter.GetComponent<StaminaComponent>().Set(100);
        timer.Change(1000, 1000);
        timeLeft = serieTime;
    }


    public void KillCharacter(GameObject characterToKill)
    {
        if(currentCharacter.Equals(characterToKill))
        {
          //  throw new System.Exception("kiled yourself, need to handle that");
        }
        /*foreach(var teamQueuePair in teams)
        {
            var queue = teamQueuePair.Value;
            if(queue.Contains(characterToKill))
            {
                var newQueue = new Queue<GameObject>();
                foreach(var character in queue)
                {
                    if(!character.Equals(characterToKill))
                    {
                        newQueue.Enqueue(character);
                    }
                }
                
                teams[teamQueuePair.Key] = newQueue;
            }
        }*/
        foreach(var team in new Team[] { Team.ALPHA, Team.BETA})
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

        var characterRenderer = character.GetComponent<Renderer>();
        characterRenderer.material.color = teamColors[team];

        return character;
    }

    private void OnTimerTick(System.Object stateInfo)
    {
        if (timeLeft > 0)
            --timeLeft;
    }
}
