using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrearTierras : MonoBehaviour
{
    public GameObject tierras;
    public Transform tato;
    int veces = 0;

    void Update()
    {
        if(tato.transform.position.x - (30 + (60 * veces)) < 5 && tato.transform.position.x - (30 + (60 * veces)) > 0){
            veces++;
            Instantiate(tierras,new Vector3(tierras.transform.position.x + (60*veces),0.0f,0.0f),Quaternion.identity);
            
        }
    }
}
