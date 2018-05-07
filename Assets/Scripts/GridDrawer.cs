using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets._2D;

public class GridDrawer : MonoBehaviour {
    [SerializeField]
    private GameObject lineGeneratorPrefab;

    GameObject newLineGen;

    Vector3 lastPosition;

    List<Vector3> nonMovedGrid;


	// Use this for initialization
	void Start () {
        SpawnLineGenerator();
        nonMovedGrid = MakeGrid(10, 2);
        lastPosition = new Vector3(0, 0);
	}

    private void SpawnLineGenerator()
    {
        newLineGen = Instantiate(lineGeneratorPrefab);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position = GameManager.GetCurrentCharacter().GetComponent<Transform>().position;
        if(lastPosition != position)
        {
            GameObject currentCharacter = GameManager.GetCurrentCharacter();
            LineRenderer lRend = newLineGen.GetComponent<LineRenderer>();

            lRend.widthMultiplier = 0.3f;

           




            DrawGrid(lRend, position);

            lastPosition = position;
        }

	}

    List<Vector3> MakeGrid(int radius, int length)
    {
        List<Vector3> horizontal = new List<Vector3>();
        int coef = length;
        for(int y = -radius; y <= radius; y++)
        {
            horizontal.Add(new Vector3(coef * radius, y * length, 50));
            coef = -coef;
            horizontal.Add(new Vector3(coef * radius, y * length, 50));
        }

        var vertical = horizontal.Select(vec => new Vector3(vec.y, vec.x, vec.z));
        vertical.Reverse();

        return horizontal.Concat(vertical).ToList();
    }

    void DrawGrid(LineRenderer lRend, Vector3 offset)
    {
        lRend.positionCount = nonMovedGrid.Count;
        lRend.SetPositions(nonMovedGrid.Select(vec => vec + offset).ToArray());
    }
}
