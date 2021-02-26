using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest_Constante : TatoMoveTest
{
    bool mover = false;
    Rigidbody2D rigidbody;
    public float freno = 1.0f;
    float factor_movim = 0.0f;
    //float avanzar;
    float freno_actual = 0.0f;
    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        mover = Input.GetButton("Constante");
        factor_movim = mover? factor_movim + Time.deltaTime : 0.0f;
        //avanzar = Input.GetAxis("Constante");

        if(!mover){
            freno_actual += Time.deltaTime * freno * Mathf.InverseLerp(1f,0.0f,Mathf.Abs(transform.position.y));
        }
        else{
            freno_actual = 0.0f;
        }
    }
    private void FixedUpdate() {
        if(!(Mathf.Abs(rigidbody.velocity.y)<0.005f)){
             transform.position += new Vector3(1.0f, 0.0f,0.0f) * velocidad;
        }
        if(!mover){
            rigidbody.drag = freno_actual;
            return;
        }
        else
            rigidbody.drag = 0;
        transform.position += new Vector3(0.0f, Mathf.Sin(-factor_movim),0.0f) * velocidad;
    }
}
