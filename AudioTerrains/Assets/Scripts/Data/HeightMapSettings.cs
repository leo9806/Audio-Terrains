using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData {

    public NoiseSettings noiseSettings;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public int width;
    public int height;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }


    protected override void OnValidate() {
        noiseSettings.ValidateValues();
		base.OnValidate ();
	}

}
