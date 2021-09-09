using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TierraBase))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TierraBase tierraBase = (TierraBase)target;
        if (tierraBase.gameObject.transform.childCount == 0)
        {
            if (GUILayout.Button("Generar Mapa"))
            {
                tierraBase.Generar();
            }
        }
        else
        {
            if (GUILayout.Button("Limpiar Mapa"))
            {
                tierraBase.LimpiarMapa();
            }
            if (GUILayout.Button("Volver a Generar Mapa"))
            {
                tierraBase.LimpiarMapa();
                tierraBase.Generar();
            }
        }

    }
}

[ExecuteInEditMode]
[SelectionBase]
[RequireComponent(typeof(BoxCollider2D))]
public class TierraBase : MonoBehaviour
{
    [Min(0.1f)]
    public float tamanioCelda = 1;
    [Min(0)]
    public int suavizado = 0;

    public Material materialTierra;
    public Material materialParedes;
    [Min(1)]
    public float alturaParedes;

    [SerializeField, HideInInspector] int anchoAproximado;
    [SerializeField, HideInInspector] int altoAproximado;
    [SerializeField, HideInInspector] TierraBloque[] bloquesTierra;

    [SerializeField, HideInInspector] Transform tato;
    [SerializeField, HideInInspector] CircleCollider2D tatu_collider;
    [SerializeField, HideInInspector] int fps_para_excavar = 3;
    [SerializeField, HideInInspector] bool excavando = false;
    [SerializeField, HideInInspector] float medioBloque;

    private void Awake()
    {
        LimpiarMapa();
        Generar();
        
        if (tatu_collider == null) tatu_collider = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>();
        tato = tatu_collider.transform;
    }

    public void LimpiarMapa()
    {
        int childCount = gameObject.transform.childCount;
        for (int i = childCount - 1; i > -1; i--)
        {
            DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
        }
    }

    public void Generar()
    {

        int nodos = Mathf.RoundToInt(altoAproximado / tamanioCelda);
        int cantidadBloques = Mathf.RoundToInt(anchoAproximado / (nodos * tamanioCelda));
        cantidadBloques = anchoAproximado % (nodos * tamanioCelda) == 0 ? cantidadBloques : cantidadBloques + 1;

        float origen_x = transform.position.x + (nodos * tamanioCelda) / 2;
        float origen_y = transform.position.y + (nodos * tamanioCelda) / 2;
        Vector3 origen = new Vector3(origen_x, origen_y, 0f);
        Vector3 offsetX = Vector3.zero;

        bloquesTierra = new TierraBloque[cantidadBloques];
        medioBloque = (nodos * tamanioCelda) / 2;

        for (int i = 0; i < cantidadBloques; i++)
        {
            GameObject nuevoBloque = new GameObject("BloqueTierra");
            offsetX.x = (nodos * tamanioCelda * i);
            nuevoBloque.transform.position = origen + offsetX;
            nuevoBloque.transform.parent = gameObject.transform;

            bloquesTierra[i] = nuevoBloque.AddComponent<TierraBloque>();
            bloquesTierra[i].Inicializar(materialTierra, materialParedes, alturaParedes);
            bloquesTierra[i].GenerarMapa(nodos, nodos, tamanioCelda);
        }
    }

    private void Update()
    {
        if (excavando && Time.frameCount % fps_para_excavar == 0)
        {
            float r1 = (tato.position.x - tatu_collider.radius);
            float r2 = (tato.position.x + tatu_collider.radius);

            foreach (TierraBloque bloque in bloquesTierra)
            {
                if ( //Si cualquiera de los 2 bordes del radio de TATO se encuentra dentro del bloque se lo debe excavar
                     //R1 ESTA ADENTRO DEL BLOQUE
                    (
                           r1 >= (bloque.transform.position.x - medioBloque)
                        && r1 <= (bloque.transform.position.x + medioBloque)
                    )
                    //R2 ESTA ADENTRO DEL BLOQUE
                    || (
                           r2 >= (bloque.transform.position.x - medioBloque)
                        && r2 <= (bloque.transform.position.x + medioBloque)
                    )
                )
                {
                    TierraBloque.Coord tatoCoord = bloque.PuntoToCoord(tato.position, tamanioCelda);
                    bloque.ExcavarCirculo(tatoCoord, Mathf.RoundToInt(tatu_collider.radius / tamanioCelda));

                    //Una vez se ha excavado un circulo se debe regenerar el mesh
                    bloque.RegenerarMapa(0, tamanioCelda);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { excavando = true; }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { excavando = false; }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) return;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        collider.size = new Vector2(Mathf.RoundToInt(collider.size.x), Mathf.RoundToInt(collider.size.y));
        anchoAproximado = (int)collider.size.x;
        altoAproximado = (int)collider.size.y;

        int nodos = Mathf.RoundToInt(altoAproximado / tamanioCelda);

        int cantidadBloques = Mathf.RoundToInt(anchoAproximado / (nodos * tamanioCelda));
        cantidadBloques = anchoAproximado % (nodos * tamanioCelda) == 0 ? cantidadBloques : cantidadBloques + 1;

        anchoAproximado = Mathf.RoundToInt(altoAproximado * cantidadBloques);

        float mapaAncho = anchoAproximado;
        float mapaAlto = altoAproximado;

        Rect rect = new Rect();

        rect.xMin = transform.position.x;
        rect.xMax = transform.position.x + mapaAncho;
        rect.yMin = transform.position.y;
        rect.yMax = transform.position.y + mapaAlto;
        rect.center = transform.position;
        rect.center += new Vector2(rect.size.x / 2, rect.size.y / 2);

        collider.offset = new Vector3(collider.size.x / 2, collider.size.y / 2, 0f);

        Vector3[] verts = new Vector3[]
        {
            new Vector3(rect.xMin, rect.yMin, transform.position.z),
            new Vector3(rect.xMin, rect.yMax, transform.position.z),
            new Vector3(rect.xMax, rect.yMax, transform.position.z),
            new Vector3(rect.xMax, rect.yMin, transform.position.z)
        };
        Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, .35f), Color.yellow);
    }
#endif
}
