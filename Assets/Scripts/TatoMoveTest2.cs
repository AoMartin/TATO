using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest2 : MonoBehaviour
{
    Vector3 movem;
    public float speed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        movem.y = Input.GetAxis("Vertical");
        movem.x = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate() {
        transform.position += movem * Time.fixedDeltaTime * speed;
    }

}
