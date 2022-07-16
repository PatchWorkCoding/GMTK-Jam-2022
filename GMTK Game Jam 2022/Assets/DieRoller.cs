using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieRoller : MonoBehaviour
{
    public void RollDie(Vector3 _rotation)
    {
        transform.GetChild(0).Rotate(_rotation);
    }

    public int DieFace()
    {
        Vector3[] _dirs = new Vector3[] 
        { 
            transform.GetChild(0).up, 
            -transform.GetChild(0).forward, 
            -transform.GetChild(0).right, 
            transform.GetChild(0).right, 
            transform.GetChild(0).forward, 
            -transform.GetChild(0).up
        };

        for (int i = 0; i < 6; i++)
        {
            if (Vector3.Dot(_dirs[i], Vector3.up) > 0.95f)
            {
                return i + 1;
            }
        }

        return -1;
    }
}
