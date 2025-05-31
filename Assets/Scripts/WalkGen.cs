using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class WalkGen : MonoBehaviour
{
    [SerializeField] public GameObject wall;
    [SerializeField] public GameObject entrance;
    [SerializeField] public GameObject floor;
    
    private static List<GameObject> entryOptions = new List<GameObject>();
   

    void Start()
    {
        //add start option in the array
        for (int i = 0; i < wall.transform.childCount; i++)
        {
            GameObject child = wall.transform.GetChild(i).gameObject;
           
        }

        //choose 1 entry point
        int entryPoint  = Random.Range(1, entryOptions.Count);

        //get the pos of the wall, replace with entrance
        Vector3 pos = wall.transform.GetChild(entryPoint).position;
        Instantiate(entrance, pos, Quaternion.identity);
        Destroy(wall.transform.GetChild(entryPoint).gameObject);


        //instantiate corridor

        Vector3 floorPos = new Vector3(pos.x +0.5f, 0, pos.z);
        Instantiate(floor, floorPos, Quaternion.identity);

        
        for (int i =0;  i < 5; i++)
        {
            Vector3 newfloorPos = new Vector3(floorPos.x+ 1f, 0, floorPos.z);
            Instantiate(floor, newfloorPos, Quaternion.identity);
            floorPos = newfloorPos;
        }

        //Room
        int roomArea = 5;
        for(int x=0; x< roomArea+1; x++)
        {
            for(int z=1; z< roomArea; z++)
            {
                Vector3 newfloorPos = new Vector3(floorPos.x + ( 1*x), 0, floorPos.z + (z*1));
                Instantiate(floor, newfloorPos, Quaternion.identity);
                floorPos = newfloorPos;
            }
        }
       
    }
   

  

}
