using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tierra : MonoBehaviour
{
    Tierra2D groundController;
    static CircleCollider2D tatu_collider;
    public static int fps_para_excavar = 7;
    bool excavando = false;

    private void Awake() {
        groundController = GetComponent<Tierra2D>();
        if(tatu_collider == null) tatu_collider = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>();
    }

    private void Update() {
        if(excavando && Time.frameCount % fps_para_excavar == 0){
            groundController.DestroyGround(tatu_collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")){
            excavando = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){
            excavando = false;
        }
    }
}
