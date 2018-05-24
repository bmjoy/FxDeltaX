﻿using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public enum PlayerMode
{
    MOVING,
    SHOOTING
};

public class GameManager : MonoBehaviour {

    private enum State
    {
        TEAM_PREPARATION,
        GAMEPLAY
    };

    private enum Team
    {
        ALPHA,
        BETA
    };

    public static GameManager instance = null;
    public GameObject currentCharacter;

    private Dictionary<Team, List<GameObject>> teams;
    private Dictionary<Team, Color> teamColors;
    private Team activeTeam;
    private Dictionary<Team, int> characterIdxs;

    private Timer timer;
    private int timeLeft;

    private State state = State.TEAM_PREPARATION;
    private PlayerMode playerMode = PlayerMode.MOVING;
    private bool characterPlacing = false;

    private const int playersQty = 2;
    private const int teamsQty = 2;
    private const int serieTime = 3;
    private const int startingHp = 100;


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

	// Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        teamColors = new Dictionary<Team, Color>()
        {
            {Team.ALPHA, new Color(1,0,0)},
            {Team.BETA, new Color(0,0,1)}
        };
        createTeams(teamsQty);
        activeTeam = Team.ALPHA;
        timer = new Timer(new TimerCallback(OnTimerTick));
    }

    private void createTeams(int players)
    {
        teams = new Dictionary<Team, List<GameObject>>();
        characterIdxs = new Dictionary<Team, int>();

        foreach (Team team in System.Enum.GetValues(typeof(Team)))
        {
            teams[team] = new List<GameObject>();
            characterIdxs[team] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.TEAM_PREPARATION)
            UpdateOnTeamPreparation();
        else if (state == State.GAMEPLAY)
            UpdateOnGameplay();
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

            GameObject character = (GameObject)Instantiate(Resources.Load("CharacterRobotBoy"), mousePos, transform.rotation);
            character.GetComponent<Platformer2DUserControl>().enabled = false;
   
            var characterRenderer = character.GetComponent<Renderer>();
            characterRenderer.material.color = teamColors[activeTeam];

            teams[activeTeam].Add(character);
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
        if (timeLeft == 0)
            StartNewSerie();
    }

    private void prepareToGameplay()
    {
        state = State.GAMEPLAY;
        Camera.main.GetComponent<CameraController>().Enable(true);
        enableCharacter(activeTeam, characterIdxs[activeTeam]);
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

    private void enableCharacter(Team team, int idx)
    {
        currentCharacter = teams[team][idx];
        Debug.Log("Current player hp: " + teams[team][idx].GetComponent<PlatformerCharacter2D>().showHp().ToString());
        foreach (Team t in System.Enum.GetValues(typeof(Team)))
        {
            for(int i=0; i<teams[t].Count; i++)
            {
                bool current = (i == idx) && (t == team);
                teams[t][i].GetComponent<Platformer2DUserControl>().enabled = current;
            }
        }
    }

    private void UpdateCharacterOnModeChange()
    {
        teams[activeTeam][characterIdxs[activeTeam]].GetComponent<Platformer2DUserControl>().enabled = (playerMode == PlayerMode.MOVING);
    }

    private void StartNewSerie()
    {
        characterIdxs[activeTeam] = (characterIdxs[activeTeam] + 1) % teams[activeTeam].Count;
        activeTeam = activeTeam.Next();
        enableCharacter(activeTeam, characterIdxs[activeTeam]);
        playerMode = PlayerMode.MOVING;
        timer.Change(1000, 1000);
        timeLeft = serieTime;
    }

    private void OnTimerTick(System.Object stateInfo)
    {
        if (timeLeft > 0)
            --timeLeft;
    }
}
