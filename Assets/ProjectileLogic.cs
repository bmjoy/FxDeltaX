using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;
using System.Linq;

public class ProjectileLogic : MonoBehaviour
{
//    GameObject gameObj;
    ProjectileDrawer drawer;
    CircleCollider2D circleCol;
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
    float length = 0.3f;
    float isRight = 0.0f;
    int initTTL = 200;
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
    }

    // Update is called once per frame
    void Update()
    {
        //missing synchronization with time
        if (TTL > 0)
        {
            if(TTL == initTTL - 5)
            {
                //collider = gameObject.AddComponent<PolygonCollider2D>();
                //collider.edgeRadius = 0.1f;
                //collider.
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
            float angle = Mathf.Atan2(dy, isRight*dx);
            //lastLocation += new Vector3(isRight*dx, dy);
            lastLocation = new Vector3(isRight * lastX, GetY(lastX)) + initLocation - offsetY;
            drawer.UpdateLocationAndAngle(lastLocation, angle);
            gameObject.GetComponent<Transform>().position = lastLocation;
        }
        else
        {
            if(goDestroy)
            {
                Destroy(rigid);
                Destroy(drawer);
                toDestroy();
            }
        }
    }

    private float GetY(float x)
    {
        return (float)equation.With("x", x).GetValue();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!alreadyHit)
        {
            Debug.Log("hit sth");
            TTL = 0;
            alreadyHit = true;

            //logic for dealing damage
            //float radius = 10;
            /*FindObjectsOfType(typeof(GameObject))
                .Cast<GameObject>()
                .Where(gameObj => gameObj.GetComponent<PlatformerCharacter2D>() != null)
                .Where(gameObj => (gameObj.GetComponent<Transform>().position - lastLocation).magnitude < radius)
                .ToList()
                .ForEach()*/
        }
    }
}
