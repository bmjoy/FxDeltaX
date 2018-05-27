using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;
using UnityEngine;
using System.Linq;
using Assets;

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
    private const float dr = 0.05f;
    float radius;
    System.Action toDestroy;

    static readonly Vector3 offset = new Vector3(0, 0, 1);

    float power;

    List<GameObject> alreadyHit = new List<GameObject>();
    
    Dictionary<string, double> values = new Dictionary<string, double>() { { "x", 0f } };
    // Use this for initialization
    void Start()
    {
        //gameObj = new GameObject();
        drawer = gameObject.AddComponent<ExplosionDrawer>();
    }

    float dmgToDeal()
    {
        return power / radius * dr; // ProjectileLogic.power2explosionTTL;
    }

    public void AddData(Vector3 location, float power, System.Action toDestroy)
    {
        this.power = power;
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
        radius += dr;
    }

    public void Lower()
    {
        radius -= dr;
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
        GameObject collidedWith = collision.gameObject;
        HpComponent hpComponent = collidedWith.GetComponent<HpComponent>();
        if(hpComponent != null)
        {
            if(!alreadyHit.Contains(collidedWith))
            {
                alreadyHit.Add(collidedWith);
                hpComponent.Dec(dmgToDeal());
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy");
        Destroy(drawer);
        Destroy(circleCol.GetComponent<CircleCollider2D>());
        Destroy(circleCol);
    }
}
