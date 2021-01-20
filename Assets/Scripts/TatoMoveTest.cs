using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TatoMoveTest : MonoBehaviour
{
    Vector3 movem;

    // Update is called once per frame
    void Update()
    {
        movem.y = Input.GetAxis("Vertical");
        movem.x = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate() {
        transform.position += movem * Time.fixedDeltaTime;
    }

    private void OnDrawGizmos() {
        //UnityEditor.Handles.DrawWireDisc(transform.position,Vector3.forward,GetComponent<CircleCollider2D>().radius*2);
        UnityEditor.Handles.DrawWireDisc(transform.position,Vector3.forward,GetComponent<CircleCollider2D>().radius+.1f);
    }
}
