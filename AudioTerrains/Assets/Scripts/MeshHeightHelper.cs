using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshHeightHelper : MonoBehaviour {

    MeshFilter meshFilter;
    Mesh mesh;
    Vector3[] vertices;
    float[] noiseHeightValues;
    List<float> soundHeightValues;
    List<float> populatedSoundValues;

    public float timer;
    bool check;

    
	// Use this for initialization
	void Start ()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vertices = new Vector3[mesh.vertices.Length];
        noiseHeightValues = new float[mesh.vertices.Length];
        soundHeightValues = new List<float>();
        populatedSoundValues = new List<float>();
        vertices = mesh.vertices;
        soundHeightValues = GameObject.Find("AudioAnalyser").GetComponent<Testing>().GetDominantValues();

        GetNoiseHeight();
        //PopulateSoundValues();
    }

    public void GetNoiseHeight()
    {
        for (int i = 0; i < vertices.Length; ++i)
        {
            noiseHeightValues[i] = vertices[i].y;
            vertices[i].y = 0;
        }
    }

    bool CheckHeightValueCount(List<float> values)
    {
        if (values.Count < noiseHeightValues.Length)
        {
            check = false;
        }
        if (values.Count == noiseHeightValues.Length)
        {
            check = true;
        }

        return check;
    }

    void PopulateSoundValues()
    {
        int desiredLength = noiseHeightValues.Length;
        //float difference = desiredLength - soundHeightValues.Count;
        int numOfLists = (desiredLength / soundHeightValues.Count) + 1;
        List<List<float>> tempLists = new List<List<float>>();
        // populating the list
        for (int i = 0; i < numOfLists; ++i)
        {
            tempLists.Add(new List<float>());
        }


        for (int i = 0; i < numOfLists; ++i)
        {


            if (i == 0)
            {
                for (int l = 0; l < soundHeightValues.Count; ++l)
                {
                    tempLists[i].Add(soundHeightValues[l]);
                }
            }
            else if (i == 1)
            {
                for (int l = 0; l < soundHeightValues.Count; ++l)
                {
                    if (l + 1 < soundHeightValues.Count)
                        tempLists[i].Add(Mathf.Lerp(soundHeightValues[l], soundHeightValues[l + 1], .5f));
                }
            }
            else
            {
                for (int l = 0; l < tempLists[i - 1].Count; ++l)
                {
                    tempLists[i].Add(Mathf.Lerp(tempLists[i - 2][l], tempLists[i-1][l], .5f));
                }
            }
        }

        SortSoundValues(tempLists);
    }

    void SortSoundValues(List<List<float>> list)
    {
        for (int l = 0; l < list[0].Count - 1; ++l)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                populatedSoundValues.Add(list[i][l]);
            }
        }
    }

    void HeightManipulation()
    {
        
        for (int i = 0; i < vertices.Length; ++i)
        {
            if (populatedSoundValues.Count == vertices.Length)
                vertices[i].y += (noiseHeightValues[i] + populatedSoundValues[i]) * 0.0012f;
            else
                vertices[i].y += noiseHeightValues[i] * 0.0012f;
        }
        mesh.vertices = vertices;
    }

    void Update()
    {
        // multiplying by 1000 because timer is in miliseconds
        timer -= Time.deltaTime;

        if (timer > 0)
        {
            HeightManipulation();

        }

    }
}