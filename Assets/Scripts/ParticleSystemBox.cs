using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemBox : MonoBehaviour
{
    public GameObject FireworksAll;

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("player"))
        {
            Explode();
        }
    }
    void Explode()
    {
        GameObject firework = Instantiate(FireworksAll, transform.position, Quaternion.identity);
        firework.GetComponent<ParticleSystem>().Play();
    }
}
