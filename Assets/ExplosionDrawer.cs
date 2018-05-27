using System;
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
   // GameObject gameObj;
    GameObject explosionPrefab;

    public void UpdateCircle(Vector3[] circle)
    {
        this.circle = circle;
    }

    // Use this for initialization
    void Start()
    {
      //  gameObj = new GameObject();
      /*  lineRend = gameObj.AddComponent<LineRenderer>();
        lineRend.widthMultiplier = 0.3f;
        lineRend.material = Resources.Load("ProjectileMaterial", typeof(Material)) as Material;*/


        //int i = (int)(UnityEngine.Random.value * 10) + 1;
        explosionPrefab = (GameObject)Instantiate(Resources.Load("Fx Explosion Pack/Prefebs/Exploson1"));
        explosionPrefab.transform.position = ProjectilesManager.instance.projectile.transform.position;
        ParticleSystem part = explosionPrefab.GetComponentInChildren<ParticleSystem>();
        part.startSize = ProjectilesManager.instance.projectile.power / 4;
    }


    // Update is called once per frame
    void Update()
    {
        /*if(circle != null)
        {
            lineRend.positionCount = 0;
            lineRend.positionCount = circle.Length;
            lineRend.SetPositions(circle);
        }*/
        
    }

    private void OnDestroy()
    {
       // Destroy(lineRend);
        Destroy(explosionPrefab);
        //Destroy(gameObj;
    }
}
