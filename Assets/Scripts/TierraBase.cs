using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(BoxCollider2D))]
public class TierraBase : MonoBehaviour
{
    public Material materialTierra;
    public Material materialParedes;
    public float alturaParedes;

    TierraBloque[] bloquesTierra;
}
