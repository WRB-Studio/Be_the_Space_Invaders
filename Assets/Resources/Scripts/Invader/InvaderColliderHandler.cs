using System.Collections;
using UnityEngine;

public class InvaderColliderHandler : MonoBehaviour
{
    Invader invaderScrp;


    public void init(Invader invaderScrpVal)
    {
        invaderScrp = invaderScrpVal;
    }

    /*
    * HP - damage
    * return bool: true = destroyed; false = only HP decreasing
    */
    public bool hit(int damage)
    {
       return invaderScrp.hit(damage);
    }

}
