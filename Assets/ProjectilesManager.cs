using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;
using Assets;

public class ProjectilesManager : MonoBehaviour {
    //List<ProjectileLogic> projectiles;
    //List<ProjectileLogic> projectiles;


    public static ProjectilesManager instance;


    // Use this for initialization
    void Start () {
        instance = this;	
	}

    public ProjectileLogic projectile;

    public static void AddProjectile(Vector3 initLocation, Text2AST.Equation equation, bool isFacingRight, float power)
    {
        if(instance.gameObject.GetComponent<ProjectileLogic>() == null)
        {
            instance.projectile = instance.gameObject.AddComponent<ProjectileLogic>();
            instance.projectile.AddData(initLocation, equation, isFacingRight, power, ()=>instance.DestroyLogicAndDontFly());
        }
    }

    void DestroyLogicAndDontFly()
    {
        Destroy(projectile);
        projectile = null;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("enter pressed");
            GameObject currentCharacter = GameManager.instance.currentCharacter;
            if (GameManager.GetPlayerMode() != PlayerMode.SHOOTING)
                return;

            Text2AST.Equation equation = EquationScript.instance.equation;
            if(equation != null)
            {
                AddProjectile(
                    currentCharacter.GetComponent<Transform>().position,
                    equation,
                    currentCharacter.GetComponent<PlatformerCharacter2D>().m_FacingRight,
                    currentCharacter.GetComponent<StaminaComponent>().value / 2.0f);

                currentCharacter.GetComponent<StaminaComponent>().Set(0);
            }
        }
	}
}
