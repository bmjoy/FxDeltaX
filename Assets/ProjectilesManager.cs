using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class ProjectilesManager : MonoBehaviour {
    //List<ProjectileLogic> projectiles;
    //List<ProjectileLogic> projectiles;


    public static ProjectilesManager instance;

    

    // Use this for initialization
    void Start () {
        instance = this;	
	}

    public static void AddProjectile(Vector3 initLocation, Text2AST.Equation equation, bool isFacingRight)
    {
        ProjectileLogic logic = instance.gameObject.AddComponent<ProjectileLogic>();
        logic.AddData(initLocation, equation, isFacingRight, ()=>Destroy(logic));
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("enter pressed");
            Text2AST.Equation equation = EquationScript.instance.equation;
            if(equation != null)
            {
                GameObject currentCharacter = GameManager.instance.curentCharacter;
                AddProjectile(
                    currentCharacter.GetComponent<Transform>().position,
                    equation,
                    currentCharacter.GetComponent<PlatformerCharacter2D>().m_FacingRight);
            }
        }
	}
}
