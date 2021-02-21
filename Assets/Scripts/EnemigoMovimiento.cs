using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
{
    public float velocidad = 1.5f;
    Rigidbody2D rigibody;
    public bool mirandoDerecha = false;
    private Vector3 posicionInicial;
    RaycastHit2D tocaElSuelo;
  

    void Awake()
    {
        rigibody = GetComponent<Rigidbody2D>();
        posicionInicial = this.transform.position;
    }

    void Start()
    {
        this.transform.position = posicionInicial;
    }


    // Update is called once per frame
    void Update()
    {
       
    }
    
    private void FixedUpdate()
    {

        float velocidadActual = velocidad;

   
        if(mirandoDerecha)
        {
            velocidadActual = velocidad;
            this.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            velocidadActual = -velocidad;
            this.transform.localEulerAngles = Vector3.zero;
        }

        rigibody.velocity = new Vector2(velocidadActual, rigibody.velocity.y);
                                   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collisioner")
        {
            mirandoDerecha = !mirandoDerecha; 
        }
    }



}
