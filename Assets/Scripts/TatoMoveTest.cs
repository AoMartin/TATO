using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest : MonoBehaviour
{
    float movem;

    // Update is called once per frame
    void Update()
    {
        movem = Input.GetAxis("Vertical");
    }

    private void FixedUpdate() {
        transform.position += Vector3.up * movem * Time.fixedDeltaTime;
    }
}
