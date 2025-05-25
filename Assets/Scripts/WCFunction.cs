using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class WCFunction : MonoBehaviour
{
    [SerializeField] public GameObject objzero;
    [SerializeField] public  GameObject objone;

    void Start()
    {
        GameObject inst;
        int prevRotation = Random.Range(0, 1);

        if (prevRotation == 1)
        {
            inst = Instantiate(objone);
        }
        else
            inst = Instantiate(objzero);


        for (int i = 0; i < 2; i++)
        {
            int prev = prevRotation;
          
            for (int j = 1; j < 2; j++)
            {
                if (prevRotation ==1)
                {

                    Vector3 pos = new Vector3(inst.transform.position.x +i ,0, inst.transform.position.z +j);
                    inst= Instantiate(objzero, pos, Quaternion.identity);
                    
                }

                if (prevRotation == 0)
                {
                    Vector3 pos = new Vector3(inst.transform.position.x +i, 0, inst.transform.position.z + j);

                    Instantiate(objone, pos, Quaternion.identity);
                   
                }


            }
            
        }

       

    }
   

  

}
