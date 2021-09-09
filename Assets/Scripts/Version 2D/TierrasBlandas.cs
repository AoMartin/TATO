using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierrasBlandas : MonoBehaviour
{
    public float fuerza = 3.0f;

    private void Awake() {
        AreaEffector2D[] effectors = GetComponentsInChildren<AreaEffector2D>();
        for(int i = 0; i < effectors.Length; i++){
            effectors[i].forceMagnitude = fuerza;
        }
    }

    public void UpdateTierras(){
        AreaEffector2D[] effectors = GetComponentsInChildren<AreaEffector2D>();
        for(int i = 0; i < effectors.Length; i++){
            effectors[i].forceMagnitude = fuerza;
        }
    }
}
