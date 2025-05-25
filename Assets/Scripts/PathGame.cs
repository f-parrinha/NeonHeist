using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathGame 
{
    private List<GameObject> pathList = new List<GameObject>();
    private List <GameObject> topFloor = new List<GameObject>();
    private List<GameObject> bottomFloor = new List<GameObject>();
     
    private int currentFloorIndex;
    private int radius;

    private bool hasReachedX ;
    private bool hasReachedZ;

    private GameObject startfloor;
    private GameObject endfloor;

    public PathGame(int radius)
    { 
        this.radius = radius; 
    }

    public List<GameObject> getPath ()
    {
        return pathList;
    }

    public void AssignTopAndBottom(int z, GameObject floor)
    {
       if(z==0) topFloor.Add(floor);

       if(z==radius-1) bottomFloor.Add(floor);
    }

    private bool AssignAndCheckEndingAndStartingFloor ()
    {
        int xIndex = Random.Range(0, topFloor.Count-1);
        int zIndex = Random.Range(0, bottomFloor.Count-1);

        startfloor = topFloor[xIndex];
        endfloor = bottomFloor[zIndex];

        return startfloor !=null && endfloor != null;
    }

    public void generatePath()
    {
       if(AssignAndCheckEndingAndStartingFloor())
       {

            GameObject curr = startfloor;

            for (int i = 0; i < 5; i++) //want to move left 2 times
            {
                MoveLeft(ref curr);
            }

            var safetyBreakX = 0;

            while(!hasReachedX)
            {
                safetyBreakX++;
                if (safetyBreakX > 100) break;

                if (curr.transform.position.x > endfloor.transform.position.x)
                    MoveDown(ref curr);
                else if (curr.transform.position.x < endfloor.transform.position.x)
                    MoveUp(ref curr);
                else
                    hasReachedX = true; ;


            }

            var safetyBreakZ = 0;

            while (!hasReachedZ)
            {
                safetyBreakZ++;
                if (safetyBreakZ > 100) break;

                if (curr.transform.position.z > endfloor.transform.position.z)
                    MoveRight(ref curr);
                else if (curr.transform.position.z < endfloor.transform.position.z)
                    MoveLeft(ref curr);
                else
                    hasReachedZ = true; ;


            }

            pathList.Add(endfloor);
        }
    }

    private void MoveLeft(ref GameObject current)
    {
        pathList.Add(current);
        currentFloorIndex = SimpleDungeonGeneratorRW.corridorFloor.IndexOf(current);

        currentFloorIndex++;
        current = SimpleDungeonGeneratorRW.corridorFloor[currentFloorIndex];

    }

    private void MoveRight(ref GameObject current)
    {
        pathList.Add(current);
        currentFloorIndex = SimpleDungeonGeneratorRW.corridorFloor.IndexOf(current);

        currentFloorIndex--;
        current = SimpleDungeonGeneratorRW.corridorFloor[currentFloorIndex];


    }

    private void MoveUp(ref GameObject current)
    {
        pathList.Add(current);
        currentFloorIndex = SimpleDungeonGeneratorRW.corridorFloor.IndexOf(current);
        int n = currentFloorIndex + radius;

        current = SimpleDungeonGeneratorRW.corridorFloor[n];
    }

    private void MoveDown(ref GameObject current)
    {
        pathList.Add(current);
        currentFloorIndex = SimpleDungeonGeneratorRW.corridorFloor.IndexOf(current);
        int n = currentFloorIndex - radius;

        current = SimpleDungeonGeneratorRW.corridorFloor[n];
    }

    /*public static HashSet<Vector3Int> SimpleRandomWalk(Vector3Int startpoint, int walkLenght)
    {
         HashSet<Vector3Int> path = new HashSet<Vector3Int>();

         path.Add(startpoint);
         var previousPos = startpoint;

         for (int i = 0; i < walkLenght; i++)
         {
             var newPos = previousPos + Direction3D.getRandomDirection();
             path.Add(newPos);
             previousPos = newPos;
         }

         return path;


    }
 }

 public static class Direction3D
 {
     public static List<Vector3Int> directionList = new List<Vector3Int>
     {
         new Vector3Int(0, 0, 1), //forward
         new Vector3Int(0, 0, -1), //backwards
         new Vector3Int(1, 0, 0), //left
         new Vector3Int(-1, 0, 0), //right

     };

     public static Vector3Int getRandomDirection ()
     {
         return directionList[Random.Range(0, directionList.Count)];
     }*/
}
