using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuDebug : MonoBehaviour
{
    bool mostrar_menu = false;
    public KeyCode tecla;
    
    TatoMoveTest tato;
    EnemigoMovimiento[] enemigos;
    public PhysicsMaterial2D tierra_dura; 

    public GameObject menu;
    public TatoMoveTest_ArribaAbajo arribaAbajo;
    public TatoMoveTest_Constante constante;
    bool es_constante = false;

    private void Awake() {
        tato = FindObjectOfType<TatoMoveTest>();
        enemigos = FindObjectsOfType<EnemigoMovimiento>();

        tato_velocidad.text = tato.velocidad.ToString();
        enemigo_velocidad.text = enemigos[0].velocidad.ToString();
        borde_friccion.text= tierra_dura.friction.ToString();
        borde_rebote.text= tierra_dura.bounciness.ToString();
        freno.text = constante.freno.ToString();
    }

    public TMP_InputField tato_velocidad;
    public TMP_InputField enemigo_velocidad;
    public TMP_InputField borde_friccion;
    public TMP_InputField borde_rebote;
    public TMP_InputField freno;

    public void CerrarMenu(){
        menu.SetActive(!menu.gameObject.activeSelf);
    }

    public void Salir(){
        Application.Quit();
    }

    void Update()
    {
        if(Input.GetKeyUp(tecla)) {
            mostrar_menu = !mostrar_menu;
            CerrarMenu();
        }
        Time.timeScale = mostrar_menu? 0.0f : 1.0f;
    }

    public void CambiarValores()
    {
        float tatovel;
        if (float.TryParse(tato_velocidad.text, out tatovel))
        {
            tato.velocidad = tatovel;
        }
        float frenotato;
        if (float.TryParse(freno.text, out frenotato))
        {
            constante.freno = frenotato;
        }
        float enemigovel;
        if (float.TryParse(enemigo_velocidad.text, out enemigovel))
        {
            foreach(EnemigoMovimiento e in enemigos){
                e.velocidad = enemigovel;
            }
        }
        float friccion;
        if (float.TryParse(borde_friccion.text, out friccion))
        {
            tierra_dura.friction = friccion;
        }
        float rebote;
        if (float.TryParse(borde_rebote.text, out rebote))
        {
            tierra_dura.bounciness = rebote;
        }
    }

    private void OnGUI() {
        if(!mostrar_menu) return;

        
    }
}
