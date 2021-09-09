using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest_ArribaAbajo : TatoMoveTest
{
    public float freno = 1.0f;
    float freno_actual;
    Rigidbody2D rigidbody;
    bool mover;
    float screenHalf;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        screenHalf = Screen.width / 2;
    }

    void Update()
    {

#if UNITY_EDITOR || UNITY_STANDALONE

        movem.y = Input.GetAxis("Vertical");
        //movem.x = Input.GetAxis("Horizontal");
        if (Input.GetButton("Vertical"))
        {
            movem.x = 1.0f;
            freno_actual = 0.0f;
            mover = true;
        }
        else
        {
            movem.x = rigidbody.velocity.normalized.sqrMagnitude;
            freno_actual += Time.deltaTime * freno * Mathf.InverseLerp(1f, 0.0f, Mathf.Abs(transform.position.y));
            mover = false;
        }

#elif UNITY_ANDROID

        if (Input.touchCount > 0)
        {
            if(Input.GetTouch(0).position.x > screenHalf){
                movem.y = -1.0f;
            }
            else{
                movem.y = 1.0f;
            }

            movem.x = 1.0f;
            freno_actual = 0.0f;
            mover = true;
        }
        else{
            movem.y = 0.0f;
            movem.x = rigidbody.velocity.normalized.sqrMagnitude;
            freno_actual += Time.deltaTime * freno * Mathf.InverseLerp(1f, 0.0f, Mathf.Abs(transform.position.y));
            mover = false;
        }

#endif

    }

    private void FixedUpdate()
    {
        transform.position += movem * Time.fixedDeltaTime * velocidad;
        rigidbody.drag = mover ? 0 : freno_actual;
    }

}
