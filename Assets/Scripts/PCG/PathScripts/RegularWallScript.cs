using UnityEngine;

public class RegularWallScript : MonoBehaviour
{
  
    [SerializeField] private Transform wall;

    private bool entry = false;

    public bool isEntry()
    {
        return entry;
    }

  
}

