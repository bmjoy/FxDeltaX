using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;
using System.Linq;

public class ExplosionLogic : MonoBehaviour
{
    private enum State
    {
        NotEvenAlive,
        Flying,
        Exploding,
        Dying
    }
    //    GameObject gameObj;
    ExplosionDrawer drawer;
    CircleCollider2D circleCol;
    //PolygonCollider2D collider;
    Vector3 location;
    float radius;
    System.Action toDestroy;

    static readonly Vector3 offset = new Vector3(0, 0, 1);
    /*int TTL = -1;
    float lastX = -1.0f;
    Vector3 offsetY;
    float initDX;
    bool alreadyHit = false;
    State state = State.NotEvenAlive;
    float length = 0.3f;
    float isRight = 0.0f;
    int initTTL = 200;
    bool goDestroy = false;*/
    Dictionary<string, double> values = new Dictionary<string, double>() { { "x", 0f } };
    // Use this for initialization
    void Start()
    {
        //gameObj = new GameObject();
        drawer = gameObject.AddComponent<ExplosionDrawer>();
    }

    public void AddData(Vector3 location, System.Action toDestroy)
    {
        this.location = location;
        this.toDestroy = toDestroy;
        circleCol = gameObject.AddComponent<CircleCollider2D>();
        circleCol.radius = 0.1f;
        radius = 0.1f;
        //rigid = gameObject.AddComponent<Rigidbody2D>();
    }

    void Update()
    {
        circleCol.radius = radius;
        var arr = Radius2PerimeterVec(radius).Select(v => v + location + offset).ToArray();
        drawer.UpdateCircle(arr);
    }

    public void Raise()
    {
        radius += 0.05f;
    }

    public void Lower()
    {
        radius -= 0.05f;
    }

    private Vector3[] Radius2PerimeterVec(float radius)
    {
        List<Vector3> list = new List<Vector3>();
        int parts = (int)Mathf.Pow(radius + 3,2);
        float anglePart = 2 * Mathf.PI / parts;
        for(int i = 0; i < parts; i++)
        {
            float actualAngle = anglePart * i;
            var point = new Vector3(Mathf.Sin(actualAngle) * radius, Mathf.Cos(actualAngle) * radius);
            list.Add(point);
            //filling explosion
            list.Add(Vector3.zero);
            list.Add(point);
        }
        list.Add(list.First());
        return list.ToArray();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnDestroy()
    {
        Destroy(drawer);
    }
}
