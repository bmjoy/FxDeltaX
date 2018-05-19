using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using System.Linq;

public class EquationScript : MonoBehaviour {
    [SerializeField]
    private GameObject lineGeneratorPrefab;

    GameObject newLineGen;

    Vector3 lastPosition;

    List<Vector3> createdTrajectory;

    public Text2AST.Equation equation;

    Text2AST.Variables variables;

    string equationString = "";

    public static EquationScript instance;


	// Use this for initialization
    public void ActualizeEquation(string newEquationString)
    {
        if(this.equationString != newEquationString)
        {
            try
            {
                equation = new Text2AST.Equation(newEquationString, variables);
                List<Vector3> newlyCreatedTrajectory = new List<Vector3>();
                float offset = (float)equation.With("x", 0).GetValue();
                for(double x = 0; x < 30; x += 0.1)
                {
                    newlyCreatedTrajectory.Add(new Vector3((float)x, (float)equation.With("x", x).GetValue() - offset, 49));
                }
                createdTrajectory = newlyCreatedTrajectory;
            }
            catch(Text2AST.WrongEquationException)
            {
                Debug.Log("Wrong written equation");
                equation = null;
            }
            catch (Text2AST.BadDefinedException)
            {
                Debug.Log("Bad defined equation");
                equation = null;
            }
            catch(System.Exception ex)
            {
                Debug.Log("Unknown exception");
                Debug.Log(ex.Message);
                equation = null;
            }
            equationString = newEquationString;
        }
    }
	void Start () {
        instance = this;
        SpawnLineGenerator();
        variables = new Text2AST.Variables(new string[] { "x" });
	}
	
    private void SpawnLineGenerator()
    {
        newLineGen = Instantiate(lineGeneratorPrefab);
    }
	// Update is called once per frame
	void Update () {
        if(equation != null)
        {
            //Debug.Log(equation.GetValue());
            DrawTrajectory();
        }
	}

    void DrawTrajectory()
    {
        bool isRight = GameManager.GetCurrentCharacter().GetComponent<PlatformerCharacter2D>().m_FacingRight;
        Vector3 location = GameManager.GetCurrentCharacter().GetComponent<Transform>().position;

        LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

        lRend.widthMultiplier = 0.3f;

        lRend.positionCount = createdTrajectory.Count;
        lRend.SetPositions(createdTrajectory.Select(vec => new Vector3(isRight ? vec.x : -vec.x, vec.y, vec.z) + location).ToArray());
    }
}
