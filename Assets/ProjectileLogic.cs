using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;
using System.Linq;
using Assets;

public class ProjectileLogic : MonoBehaviour
{
    private enum State
    {
        NotEvenAlive,
        DontHitYourself,
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
    public float power;
    int dontHitTTL = 0;
    int flyTTL = 400;
    int explosionTTL = 40;
    Vector3 savedLocation;
    public static int power2explosionTTL = 2;
    bool goDestroy = false;
    Dictionary<string, double> values = new Dictionary<string, double>() { { "x", 0f } };
    // Use this for initialization
    void Start()
    {
        //gameObj = new GameObject();
        drawer = gameObject.AddComponent<ProjectileDrawer>();
    }

    public void AddData(Vector3 initLocation, Text2AST.Equation equation, bool looksRight, float power, System.Action toDestroy)
    {
        this.initLocation = initLocation;
        this.lastLocation = initLocation;
        this.equation = equation;
        lastX = 0.0f;
        // initDX = 0.5f * (looksRight ? 1 : -1);
        offsetY = new Vector3(0, GetY(lastX));
        initDX = 0.5f;
        isRight = looksRight ? 1 : -1;
        goDestroy = true;
        this.toDestroy = toDestroy;
        TTL = dontHitTTL;
        state = State.DontHitYourself;
        this.power = power;
        explosionTTL = power2explosionTTL * (int)power;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.NotEvenAlive:
                break;
            case State.DontHitYourself:
                MoveProjectile();
                if(TTL < 0)
                {
                    circleCol = gameObject.AddComponent<CircleCollider2D>();
                    circleCol.radius = 0.1f;
                    rigid = gameObject.AddComponent<Rigidbody2D>();
                    TTL = flyTTL;
                    state = State.Flying;
                }
                break;
            case State.Flying:
                MoveProjectile();
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
                transform.position = savedLocation;
                break;
            case State.Dying:
                Destroy(rigid);
                Destroy(drawer);
                toDestroy();
                break;
        }

    }

    private void MoveProjectile()
    {
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
    }

    private float GetY(float x)
    {
        return (float)equation.With("x", x).GetValue();
    }

    private void Explode()
    {
        drawer.Clear();
        TTL = explosionTTL;
        state = State.Exploding;
        alreadyHit = true;
        explosion = gameObject.AddComponent<ExplosionLogic>();
        explosion.AddData(lastLocation, power * GameManager.POWER_MULTIPLICATOR, () => { });
        savedLocation = lastLocation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject gameObj = GameManager.GetCurrentCharacter();
        bool isCollidedWithItself = false;
        if(gameObj != null)
        {
            if(gameObj.Equals(collision.collider.gameObject))
            {
                isCollidedWithItself = true;
            }
        }
        if (state == State.Flying && !isCollidedWithItself)
        {
            Debug.Log("hit sth");
            Explode();

            //logic for dealing damage
            //var stamina_left = GameManager.GetCurrentCharacter().GetComponent<PlatformerCharacter2D>().stamina;
            //var stamina_left = GameManager.GetCurrentCharacter().GetComponent<StaminaComponent>().value;

            /*FindObjectsOfType(typeof(GameObject))
                .Cast<GameObject>()
                .Where(gameObj => gameObj.GetComponent<PlatformerCharacter2D>() != null)
                .Where(gameObj => (gameObj.GetComponent<Transform>().position - lastLocation).magnitude < radius)
                .ToList()
                .ForEach(i => i.GetComponent<HpComponent>().Dec(power));*/

            //GameManager.GetCurrentCharacter().GetComponent<PlatformerCharacter2D>().stamina = 0;

            //var co = FindObjectsOfType(typeof(GameManager)).Cast<GameManager>().ToList()[0].GetCurrentCharacter();

            /*.Where(gameObj => gameObj.GetComponent<PlatformerCharacter2D>() != null)
            .Where(i => i.GetComponent<PlatformerCharacter2D>().currentCharacter)
            .ToList()
            .ForEach(i => i.GetComponent<PlatformerCharacter2D>().Hit());*/
        }
    }
}
