using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;
using System.Linq;

public class ProjectileLogic : MonoBehaviour
{
    private enum State
    {
        NotEvenAlive,
        Flying,
        Exploding,
        Dying
    }
    //    GameObject gameObj;
    ProjectileDrawer drawer;
    CircleCollider2D circleCol;
    ExplosionLogic explosion;
    Rigidbody2D rigid;
    //PolygonCollider2D collider;
    Text2AST.Equation equation;
    Vector3 initLocation;
    Vector3 lastLocation;
    System.Action toDestroy;
    int TTL = -1;
    float lastX = -1.0f;
    Vector3 offsetY;
    float initDX;
    bool alreadyHit = false;
    State state = State.NotEvenAlive;
    float length = 0.3f;
    float isRight = 0.0f;
    int initTTL = 200;
    int explosionTTL = 40;
    bool goDestroy = false;
    Dictionary<string, double> values = new Dictionary<string, double>() { { "x", 0f } };
    // Use this for initialization
    void Start()
    {
        //gameObj = new GameObject();
        drawer = gameObject.AddComponent<ProjectileDrawer>();
    }

    public void AddData(Vector3 initLocation, Text2AST.Equation equation, bool looksRight, System.Action toDestroy)
    {
        this.initLocation = initLocation;
        this.lastLocation = initLocation;
        this.equation = equation;
        TTL = initTTL;
        lastX = 0.0f;
        // initDX = 0.5f * (looksRight ? 1 : -1);
        offsetY = new Vector3(0, GetY(lastX));
        initDX = 0.5f;
        isRight = looksRight ? 1 : -1;
        goDestroy = true;
        this.toDestroy = toDestroy;
        state = State.Flying;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.NotEvenAlive:
                break;
            case State.Flying:
                if (TTL == initTTL - 10)
                {
                    circleCol = gameObject.AddComponent<CircleCollider2D>();
                    circleCol.radius = 0.1f;
                    rigid = gameObject.AddComponent<Rigidbody2D>();
                }
                float dx = initDX;
                float lastY = GetY(lastX);
                float nextY = GetY(lastX + dx);
                while (new Vector2(dx, nextY - lastY).magnitude > length)
                {
                    dx = 0.9f * dx;
                    nextY = GetY(lastX + dx);
                }
                lastX += dx;
                TTL--;
                float dy = nextY - lastY;
                float angle = Mathf.Atan2(dy, isRight * dx);
                lastLocation = new Vector3(isRight * lastX, GetY(lastX)) + initLocation - offsetY;
                drawer.UpdateLocationAndAngle(lastLocation, angle);
                gameObject.GetComponent<Transform>().position = lastLocation;
                if (TTL < 0)
                {
                    Explode();
                }
                break;
            case State.Exploding:
                if(TTL >= explosionTTL/2)
                {
                    explosion.Raise();
                }
                else
                {
                    explosion.Lower();
                }
                TTL--;
                if (TTL < 1)
                {
                    state = State.Dying;
                    Destroy(explosion);
                }
                break;
            case State.Dying:
                Destroy(rigid);
                Destroy(drawer);
                toDestroy();
                break;
        }

    }

    private float GetY(float x)
    {
        return (float)equation.With("x", x).GetValue();
    }

    private void Explode()
    {
        TTL = explosionTTL;
        state = State.Exploding;
        alreadyHit = true;
        explosion = gameObject.AddComponent<ExplosionLogic>();
        explosion.AddData(lastLocation, () => { });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state == State.Flying)
        {
            Debug.Log("hit sth");
            Explode();
        }

        //logic for dealing damage
        var stamina_left = GameManager.GetCurrentCharacter().GetComponent<PlatformerCharacter2D>().stamina;

        float radius = 1;
        FindObjectsOfType(typeof(GameObject))
            .Cast<GameObject>()
            .Where(gameObj => gameObj.GetComponent<PlatformerCharacter2D>() != null)
            .Where(gameObj => (gameObj.GetComponent<Transform>().position - lastLocation).magnitude < radius)
            .ToList()
            .ForEach(i => i.GetComponent<PlatformerCharacter2D>().Hit(stamina_left));

        GameManager.GetCurrentCharacter().GetComponent<PlatformerCharacter2D>().stamina = 0;

        //var co = FindObjectsOfType(typeof(GameManager)).Cast<GameManager>().ToList()[0].GetCurrentCharacter();
        
            /*.Where(gameObj => gameObj.GetComponent<PlatformerCharacter2D>() != null)
            .Where(i => i.GetComponent<PlatformerCharacter2D>().currentCharacter)
            .ToList()
            .ForEach(i => i.GetComponent<PlatformerCharacter2D>().Hit());*/
    }
}
