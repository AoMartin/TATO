using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuDebug : MonoBehaviour
{
    bool mostrar_menu = false;
    public KeyCode tecla;
    
    EnemigoMovimiento[] enemigos;
    public PhysicsMaterial2D tierra_dura; 

    public GameObject menu;
    public TatoMoveTest_ArribaAbajo arribaAbajo;
    public TatoMoveTest_Constante constante;
    bool es_constante = false;

    public GameObject tierras;
    public GameObject prefab_tierras;

    public TierrasBlandas tierrasBlandas;

    public static MenuDebug MD;
    private void Awake() {
        MD = this;

        enemigos = FindObjectsOfType<EnemigoMovimiento>();

        tato_velocidad.text = constante.velocidad.ToString();
        tato_velocidad2.text = arribaAbajo.velocidad.ToString();
        enemigo_velocidad.text = enemigos[0].velocidad.ToString();
        borde_friccion.text= tierra_dura.friction.ToString();
        borde_rebote.text= tierra_dura.bounciness.ToString();
        freno.text = constante.freno.ToString();
        freno2.text = arribaAbajo.freno.ToString();
        txt_tierras.text = tierrasBlandas.fuerza.ToString();

        if(constante.enabled) modo.text = "Modo: Constante";
        else modo.text = "Modo: ArribaAbajo";
    }

    public TMP_InputField tato_velocidad;
    public TMP_InputField tato_velocidad2;
    public TMP_InputField enemigo_velocidad;
    public TMP_InputField borde_friccion;
    public TMP_InputField borde_rebote;
    public TMP_InputField freno;
    public TMP_InputField freno2;
    public TMP_InputField txt_tierras;
    public TMP_Text modo;

    public void CerrarMenu(){
        menu.SetActive(!menu.gameObject.activeSelf);
        Time.timeScale = menu.gameObject.activeSelf? 0.0f : 1.0f;
    }

    public void Salir(){
        Application.Quit();
    }

    public void Restart(){
        modo.text="Restarteando...";
        arribaAbajo.gameObject.transform.position = Vector3.zero;
        Rigidbody2D rb = arribaAbajo.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0.0f;
        rb.drag = 0;
        tierrasBlandas = null;
        Destroy(tierras);
        tierras = Instantiate(prefab_tierras,Vector3.zero,Quaternion.identity);
        tierrasBlandas = tierras.GetComponent<TierrasBlandas>();
        float ftierras;
        if (float.TryParse(txt_tierras.text, out ftierras))
        {
            tierrasBlandas.fuerza = ftierras;
            tierrasBlandas.UpdateTierras();
        }
        if(constante.enabled) modo.text = "Modo: Constante";
        else modo.text = "Modo: ArribaAbajo";
    }

    public void RestartMobile(){
        modo.text = "RESETEANDO! . . . (esperar)";
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void CambiarModo(){
        constante.enabled = !constante.enabled;
        arribaAbajo.enabled = !arribaAbajo.enabled;

        if(constante.enabled) modo.text = "Modo: Constante";
        else modo.text = "Modo: ArribaAbajo";

        CambiarValores();
    }

    void Update()
    {
        if(Input.GetKeyUp(tecla)) {
            mostrar_menu = !mostrar_menu;
            CerrarMenu();
        }
        
    }

    public void CambiarValores()
    {
        float tatovel;
        if (float.TryParse(tato_velocidad.text, out tatovel))
        {
            constante.velocidad = tatovel;
        }
        float frenotato;
        if (float.TryParse(freno.text, out frenotato))
        {
            constante.freno = frenotato;
        }
        float tatovel2;
        if (float.TryParse(tato_velocidad2.text, out tatovel2))
        {
            arribaAbajo.velocidad = tatovel2;
        }
        float frenotato2;
        if (float.TryParse(freno2.text, out frenotato2))
        {
            arribaAbajo.freno = frenotato2;
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
        float ftierras;
        if (float.TryParse(txt_tierras.text, out ftierras))
        {
            tierrasBlandas.fuerza = ftierras;
            tierrasBlandas.UpdateTierras();
        }
    }

    private void OnGUI() {
        if(!mostrar_menu) return;

        
    }
}
