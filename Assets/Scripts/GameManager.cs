using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public GameObject curentCharacter;

    private Dictionary<Team, List<GameObject>> teams;
    private Dictionary<Team, Color> teamColors;
    private Team activeTeam;
    private Dictionary<Team, int> characterIdxs;

    private Timer timer;

    private enum Team
    {
        ALPHA,
        BETA
    };

    public static GameObject GetCurrentCharacter()
    {
        //return instance.characters[instance.characterIdx];
        return instance.teams[instance.activeTeam][instance.characterIdxs[instance.activeTeam]];
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
        createTeams(2);
        activeTeam = Team.ALPHA;
        enableCharacter(activeTeam, characterIdxs[activeTeam]);

        this.InvokeRepeating("OnRoundFinished", 10, 10);
    }

    private void createTeams(int players)
    {
        teams = new Dictionary<Team, List<GameObject>>();
        characterIdxs = new Dictionary<Team, int>();

        foreach (Team team in System.Enum.GetValues(typeof(Team)))
        {
            var teamCharacters = new List<GameObject>();
            for (int i = 0; i < players; ++i)
            {
                GameObject character = (GameObject)Instantiate(Resources.Load("CharacterRobotBoy"));
                var characterRenderer = character.GetComponent<Renderer>();
                characterRenderer.material.color = teamColors[team];
                teamCharacters.Add(character);
            }
            teams[team] = teamCharacters;
            characterIdxs[team] = 0;
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            characterIdxs[activeTeam] = (characterIdxs[activeTeam] + 1) % teams[activeTeam].Count;
            enableCharacter(activeTeam, characterIdxs[activeTeam]);
        }
    }

    private void enableCharacter(Team team, int idx)
    {
        curentCharacter = teams[team][idx];
        foreach (Team t in System.Enum.GetValues(typeof(Team)))
        {
            for(int i=0; i<teams[t].Count; i++)
            {
                bool current = (i == idx) && (t == team);
                teams[t][i].GetComponent<Platformer2DUserControl>().enabled = current;
            }
        }
    }

    private void OnRoundFinished()
    {
        activeTeam = activeTeam.Next();
        enableCharacter(activeTeam, characterIdxs[activeTeam]);
    }
}
