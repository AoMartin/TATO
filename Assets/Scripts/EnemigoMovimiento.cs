using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class EnemigoMovimiento : MonoBehaviour
{
    public float velocidad = 1.5f;
    Rigidbody2D rigibody;
    public bool mirandoDerecha = false;
    private Vector3 posicionInicial;

    public bool vertical = false;

    void Awake()
    {
        rigibody = GetComponent<Rigidbody2D>();
        posicionInicial = this.transform.position;
    }

    void Start()
    {
        this.transform.position = posicionInicial;
    }
    
    private void FixedUpdate()
    {

        float velocidadActual = velocidad;

   
        if(mirandoDerecha)
        {
            velocidadActual = velocidad;
            if(!vertical) this.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            velocidadActual = -velocidad;
            if(!vertical) this.transform.localEulerAngles = Vector3.zero;
        }

        if(!vertical)
            rigibody.velocity = new Vector2(velocidadActual, rigibody.velocity.y);
        else
            rigibody.velocity = new Vector2(rigibody.velocity.x,velocidadActual);                
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collisioner")
        {
            mirandoDerecha = !mirandoDerecha; 
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            #if UNITY_EDITOR || UNITY_STANDALONE
                MenuDebug.MD.Restart();

            #elif UNITY_ANDROID
                MenuDebug.MD.RestartMobile();
            #endif
        }
    }
}
