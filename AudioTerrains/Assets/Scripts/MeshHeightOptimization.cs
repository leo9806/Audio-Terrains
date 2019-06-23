using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHeightOptimization : MonoBehaviour {


    MeshFilter meshFilter;
    Mesh mesh;
    Vector3[] vertices;

    int prevPos;
    int currPos;

    // Use this for initialization
    void Start ()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vertices = new Vector3[mesh.vertices.Length];
        vertices = mesh.vertices;
        prevPos = 0;
        currPos = 0;
    }


    void FlatRectArea()
    {
        
       
            for (int i = 0; i < 50; ++i)
            {
                prevPos = currPos;
                vertices[currPos+i].y = 0;
                
            }
        //currPos += 350;
            
        
        
        mesh.vertices = vertices;
    }
	
	// Update is called once per frame
	void Update () {
        FlatRectArea();
	}
}
