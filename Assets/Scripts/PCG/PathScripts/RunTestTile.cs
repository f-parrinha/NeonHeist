using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunTestTile : MonoBehaviour
{

    //[SerializeField] private GameObject startEntry;
    [SerializeField] private Tile startTile;
    [SerializeField] private Transform startPoint;
    //private Transform exitPoint;
    
    //[SerializeField] private LayerMask pcg_Layer;


    void Start()
    {
        Generate();

    }

    private void Generate()
    {

      
        Vector3 tileOffset = startTile.transform.position - startTile.getEntryPoint().position;
        Vector3 rotationToPosition = startPoint.rotation * tileOffset;
        
        Vector3 pos = startPoint.position + rotationToPosition;

        GameObject tile = Instantiate(startTile.gameObject, pos, startPoint.rotation); //review rotation
        
        
        GenerateTiles(tile);
       
       
    }

    private void GenerateTiles(GameObject firstTile)
    {
        
        Tile currentTile = firstTile.GetComponent<Tile>();

        for (int i = 0; i < 15; i++) // how many iteration
        {
            int randomTile = Random.Range(0, currentTile.getTilesCount());
            GameObject nextTileObj = currentTile.getTile(randomTile);
            Tile nextTileScript = nextTileObj.GetComponent<Tile>();

            Quaternion rotation = currentTile.getExitPoint().rotation; 
            // td bem ate aqui
            Debug.Log("current Tile exit pos " + currentTile.getExitPoint().position);

            Vector3 scaledPoint = Vector3.Scale(nextTileScript.getEntryPoint().localPosition, nextTileObj.transform.localScale);
            Debug.Log("actual value of entry " + scaledPoint);
            
            Vector3 tileOffset = -scaledPoint;
            Debug.Log("next tile entry point " + tileOffset);
            
            Vector3 offsetRotation = rotation * tileOffset;
            
            Vector3 pos = currentTile.getExitPoint().position + offsetRotation;


            //Check overlap before instantiate
            /*Collider[] hitColliders = Physics.OverlapBox(
                pos +(rotation * nextTileScript.getBoxCollider().center),
                nextTileScript.getBoxCollider().size/2,
                rotation,
                pcg_Layer
            );
            
            foreach( Collider hitCollider in hitColliders )
            {
                if(hitCollider.gameObject != currentTile.gameObject)
                    return;
            }*/
            //after check
           
            GameObject nextTile = Instantiate(nextTileObj, pos, rotation);

                       
            currentTile = nextTile.GetComponent<Tile>(); 

        }

          
           /* List<Transform> exitPointList = FindChildByName(current.transform, "Exit");
           

            if (exitPointList.Count == 1)
                entryPoint = exitPointList[0];
            else
            {
                int randomEntry = Random.Range(0, exitPointList.Count);
                entryPoint = exitPointList[randomEntry];
            }
           
            
            //Tile tileScript = entryPoint.GetComponent<Tile>();

            Debug.Log("current tile entry pos " + entryPoint.position);

            int randomTile = Random.Range(0, tile.getTilesCount());
            GameObject nextTtile = tileScript.getTile(randomTile);


            List<Transform> exitPointList = FindChildByName(nextTtile.transform, "Exit");

            if (exitPointList.Count == 1)
                exitPoint = exitPointList[0];
            else
            {
                int randomEntry = Random.Range(0, exitPointList.Count);
                exitPoint = exitPointList[randomEntry];
            }
            Debug.Log("next tile exit pos (merge) " + exitPoint.position);

            Vector3 pos = newPos+ entryPoint.position  - exitPoint.position; // tile.transform.position + entryPoint.position; // - mergePoint.position;
            Debug.Log("tile pos (received) " + newPos);
            
            Instantiate(nextTtile, pos,Quaternion.identity);
            Debug.Log("instantiate pos sum " + pos);

            current = nextTtile;
            newPos = pos;
            Debug.Log("changed current to next");


        }*/
    }

    private List<Transform> FindChildByName(Transform parent,string name )
    {
        List<Transform> entryChildren = new List<Transform>();
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
              entryChildren.Add(child);
            }
        }

        return entryChildren;
      }
}
