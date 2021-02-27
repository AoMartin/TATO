using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest_ArribaAbajo : TatoMoveTest
{
    public float freno = 1.0f;
    float freno_actual;
    Rigidbody2D rigidbody;
    bool mover;

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movem.y = Input.GetAxis("Vertical");
        //movem.x = Input.GetAxis("Horizontal");
        if(Input.GetButton("Vertical")){
            movem.x = 1.0f;
            freno_actual = 0.0f;
            mover= true;
        }
        else{
            movem.x = rigidbody.velocity.normalized.sqrMagnitude;
            freno_actual += Time.deltaTime * freno * Mathf.InverseLerp(1f,0.0f,Mathf.Abs(transform.position.y));
            mover=false;
        }
       
    }

    private void FixedUpdate()
    {
        transform.position += movem * Time.fixedDeltaTime * velocidad;
        rigidbody.drag = mover? 0 : freno_actual;
    }

}
