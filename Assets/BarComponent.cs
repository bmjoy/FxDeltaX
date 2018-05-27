using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BarComponent : MonoBehaviour {

	// Use this for initialization
    public void Inc(float dv)
    {
        UpdateValue(this.value + dv);
    }

    public virtual void Dec(float dv)
    {
        UpdateValue(this.value - dv);
    }

    public void Set(float newValue)
    {
        float dv = newValue - value;
        if(dv > 0)
        {
            Inc(dv);
        }
        else
        {
            Dec(-dv);
        }
        //UpdateValue(newValue);
    }

    protected abstract void WhenBelowOrEqualZero();

    protected abstract Material GetMaterial();

    protected static Material GetColoredMaterial(Color color)
    {
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = color;
        return material;
    }

    protected abstract Vector3 GetOffset();
  

    public void SetInit(float value)
    {
        this.value = value;
        this.totalValue = value;
        lastLocation = ToDrawLocation();
        is_initiated = true;
    }

	void Start () {
        gameObj = new GameObject();
        renderer = gameObj.AddComponent<LineRenderer>();
        renderer.material = GetMaterial();
        renderer.widthMultiplier = 0.3f;
        handleTimeoutOnStamina();
	}

    protected abstract void handleTimeoutOnStamina();


    // Update is called once per frame
    void Update () {
		if(is_initiated)
        {
            Vector3 newLocation = ToDrawLocation();
            if((newLocation != lastLocation) || forceRender) //considers eps
            {
                forceRender = false;
                lastLocation = newLocation;
                var startBar = lastLocation - halfVectorLength;
                var endBar = startBar + vectorLength * value / totalValue;
                if(value < 0)
                {
                    endBar = startBar;
                }
                renderer.SetPosition(0, startBar);
                renderer.SetPosition(1, endBar);
                renderer.positionCount = 2;
            }
        }
	}

    private void OnDestroy()
    {
        Destroy(renderer);
        Destroy(gameObj);
    }

    private bool is_initiated = false;
    public float value;
    public float totalValue;
    private LineRenderer renderer;
    private GameObject gameObj;
    private bool forceRender;
    private int timer = 1;

    private void UpdateValue(float newValue)
    {
        value = newValue;
        forceRender = true;
        if(value > totalValue)
        {
            value = totalValue;
        }
        if(value <= float.Epsilon)
        {
            WhenBelowOrEqualZero();
        }
    }

    readonly static Vector3 vectorLength = new Vector3(3, 0);
    readonly static Vector3 halfVectorLength = vectorLength / 2.0f;

    private Vector3 lastLocation;

    private Vector3 ToDrawLocation()
    {
        return GetComponent<Transform>().position + GetOffset();
    }
}
