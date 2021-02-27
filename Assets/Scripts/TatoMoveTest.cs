using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest : MonoBehaviour
{
    protected Vector3 movem;
    public float velocidad = 1.0f;
    
    // Update is called once per frame
    void Update()
    {
        movem.y = Input.GetAxis("Vertical");
        movem.x = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate() {
        transform.position += movem * Time.fixedDeltaTime;
    }

}
