using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest_ArribaAbajo : TatoMoveTest
{
    void Update()
    {
        movem.y = Input.GetAxis("Vertical");
        movem.x = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        transform.position += movem * Time.fixedDeltaTime * velocidad;
    }

}
