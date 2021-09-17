using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tato_Control : MonoBehaviour
{
    
    Rigidbody2D rigidbody;
    bool mover;
    Vector3 movem;

    [Header("Movimiento")]
    public float freno = 1.0f;
    float freno_actual;
    public float potenciaExcavar = 1.0f;
    public float aceleracion = 1.0f;
    public float maximaVelocidad = 5.0f;
    
    public float velocidad = 1.0f;
    float mitadPantalla;

    [Header("Animacion")]
    public GameObject modelo3dTato;
    public Animator animator;
    public bool rotarAlMoverse = false;
    public float velocidadRotacion = 1.0f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        mitadPantalla = Screen.width / 2;
    }

    void Update()
    {

#if UNITY_EDITOR || UNITY_STANDALONE

        movem.y = Input.GetAxis("Vertical") * potenciaExcavar;
        //movem.x = Input.GetAxis("Horizontal");
        if (Input.GetButton("Vertical"))
        {
            //if(!mover) movem.x = 1.0f;
            if(movem.x < maximaVelocidad)
                movem.x += aceleracion * Time.deltaTime;
            else
                movem.x = maximaVelocidad;

            freno_actual = 0.0f;
            mover = true;
        }
        else
        {
            //movem.x = 1.0f;
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
        Animar();
        if(rotarAlMoverse) RotarAlMover();
    }

    void Animar()
    {
        if(rigidbody.velocity.sqrMagnitude > 0.0f)
        {
            animator.SetBool("corriendo", true);
        }
        else
        {
            animator.SetBool("corriendo", false);
        }
    }

    void RotarAlMover()
    {
        if (movem != Vector3.zero)
        {
            /*float angle = Mathf.Atan2(rigidbody.velocity.y,rigidbody.velocity.x) * Mathf.Rad2Deg;
            modelo3dTato.transform.rotation *= Quaternion.AngleAxis(angle * velocidadRotacion  * Time.deltaTime, Vector3.forward);*//*
            Quaternion rotation = Quaternion.LookRotation(rigidbody.velocity,transform.right);
            modelo3dTato.transform.rotation = Quaternion.Slerp(modelo3dTato.transform.rotation, rotation, velocidadRotacion * Time.deltaTime);*/

            Vector3 dir = -rigidbody.velocity;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle , modelo3dTato.transform.forward);
            modelo3dTato.transform.rotation = Quaternion.Slerp(modelo3dTato.transform.rotation, q, velocidadRotacion * Time.deltaTime);
        }
    }
    
    private void FixedUpdate()
    {
        transform.position += movem * Time.fixedDeltaTime * velocidad;
        rigidbody.drag = mover ? 0 : freno_actual;
    }

}
