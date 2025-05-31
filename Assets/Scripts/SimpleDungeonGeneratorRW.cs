using System.Collections.Generic;
using UnityEngine;

public class SimpleDungeonGeneratorRW : MonoBehaviour
{
    public static List<GameObject> corridorFloor = new List<GameObject>();

    [SerializeField] private GameObject floorPrefab;

    private int radius = 10;

    void Start()
    {
        PathGame pathGenerator = new PathGame(radius);

        for (int x = 0; x < radius; x++)
        {
            for (int z = 0; z < radius; z++)
            {
                GameObject floor = Instantiate(floorPrefab,
                    new Vector3(x, 0, z ),
                    Quaternion.identity);

                corridorFloor.Add(floor);
                pathGenerator.AssignTopAndBottom(z, floor);

            }
        }

        pathGenerator.generatePath();

        foreach (var pObj in corridorFloor)
        {
           //Destroy(pObj);
           pObj.SetActive(false);
        }

        foreach (var pObj  in pathGenerator.getPath())
        {
            pObj.SetActive(true);
        }

    }

  
}
