using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    // Start is called before the first frame update
    // [SerializeField] String TreeTag;
    [SerializeField] Terrain terrain;
    void Start()
    {
        print("Starting");
        // print("Checking terrain");
        TreeInstance[] trees = terrain.terrainData.treeInstances;

        foreach (TreeInstance thisTree in trees)
        {
            // print("Checking trees");
            // if (terrain.transform.childCount > thisTree.prototypeIndex)
            // {
            print("Here!");
            GameObject currentTree = terrain.terrainData.treePrototypes[thisTree.prototypeIndex].prefab.gameObject;
            // GameObject currentTree = terrain.transform.GetChild(thisTree.prototypeIndex).gameObject;
            // String currentTag = thisTerrain.terrainData.treePrototypes[thisTree.prototypeIndex].prefab.tag;
            print(currentTree.gameObject.tag);
            if (currentTree.gameObject.tag != "Untagged")
            {
                currentTree.gameObject.tag = "Untagged";
            }
            // }
        }

    }
}
