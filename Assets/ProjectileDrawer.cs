using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDrawer : MonoBehaviour
{
    LineRenderer lineRend;
    double angle;
    Vector3[] bullet;
    const float halfBulletLength = 0.5f / 2.0f;
    GameObject gameObj;

    public void UpdateLocationAndAngle(Vector3 location, float angle)
    {
        Vector3 movement = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * halfBulletLength;
        bullet = new Vector3[] { location - movement, location + movement };
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
        if(bullet != null)
        {
            lineRend.SetPositions(bullet);
            lineRend.positionCount = 2;
        }
    }

    private void OnDestroy()
    {
        Destroy(lineRend);
        Destroy(gameObj);
    }
}
