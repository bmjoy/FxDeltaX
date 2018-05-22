using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDrawer : MonoBehaviour
{
    LineRenderer lineRend;
    double angle;
    //Vector3[] bullet;
    Vector3[] circle;
    const float halfBulletLength = 0.5f / 2.0f;
    GameObject gameObj;

    public void UpdateCircle(Vector3[] circle)
    {
        this.circle = circle;
    }

    // Use this for initialization
    void Start()
    {
        gameObj = new GameObject();
        lineRend = gameObj.AddComponent<LineRenderer>();
        lineRend.widthMultiplier = 0.3f;
        lineRend.material = Resources.Load("ProjectileMaterial", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {
        if(circle != null)
        {
            lineRend.positionCount = 0;
            lineRend.positionCount = circle.Length;
            lineRend.SetPositions(circle);
        }
        
    }

    private void OnDestroy()
    {
        Destroy(lineRend);
        Destroy(gameObj);
    }
}
