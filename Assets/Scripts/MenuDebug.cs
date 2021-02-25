using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDebug : MonoBehaviour
{
    bool mostrar_menu = false;
    public KeyCode tecla;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(tecla)) mostrar_menu = !mostrar_menu;
    }

    private void OnGUI() {
        if(!mostrar_menu) return;

        
    }
}
