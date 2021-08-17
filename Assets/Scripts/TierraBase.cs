using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TierraBase))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TierraBase tierraBase = (TierraBase)target;
        if(tierraBase.gameObject.transform.childCount == 0){
            if(GUILayout.Button("Generar Mapa")){
                tierraBase.Generar();
            }
        }else{
            if(GUILayout.Button("Limpiar Mapa")){
                int childCount = tierraBase.gameObject.transform.childCount;
                for(int i = childCount-1; i > -1; i--){
                    DestroyImmediate(tierraBase.gameObject.transform.GetChild(i).gameObject);
                }
            }
            if(GUILayout.Button("Volver a Generar Mapa")){
                int childCount = tierraBase.gameObject.transform.childCount;
                for(int i = childCount-1; i > -1; i--){
                    DestroyImmediate(tierraBase.gameObject.transform.GetChild(i).gameObject);
                }
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
    
    int anchoAproximado;
    int altoAproximado;
    
    [Min(0.1f)]
    public float tamanioCelda = 1;
	[Min(0)]
	public int suavizado = 0;

    public Material materialTierra;
    public Material materialParedes;
    [Min(1)]
    public float alturaParedes;

    TierraBloque[] bloquesTierra;
    int bloquesOffset = 2;

    static Transform tato;
    static CircleCollider2D tatu_collider;
    public static int fps_para_excavar = 3;
    bool excavando = false;

    private void Awake()
    {
        //Application.targetFrameRate = 30;
        if (tatu_collider == null) tatu_collider = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>();
        tato = tatu_collider.transform;
    }

    public void Generar(){
        float origen_x = transform.position.x + altoAproximado /2 +1;
        float origen_y = transform.position.y + altoAproximado /2;
        Vector3 origen = new Vector3(origen_x - bloquesOffset/2, origen_y, 0f);
        Vector3 offsetX = Vector3.zero;

        //int nodosX = Mathf.RoundToInt(anchoAproximado / tamanioCelda);
        //int nodosY = Mathf.RoundToInt(altoAproximado / tamanioCelda);

        int cantidadBloques = Mathf.RoundToInt(anchoAproximado / altoAproximado);
        cantidadBloques = anchoAproximado % altoAproximado == 0 ? cantidadBloques : cantidadBloques +1;

        bloquesTierra = new TierraBloque[cantidadBloques]; 
        for(int i = 0; i < cantidadBloques;i++){
            GameObject nuevoBloque = new GameObject("BloqueTierra");
            offsetX.x = (altoAproximado * i);
            nuevoBloque.transform.position = origen + offsetX;
            nuevoBloque.transform.parent = gameObject.transform;

            bloquesTierra[i] = nuevoBloque.AddComponent<TierraBloque>();
            bloquesTierra[i].Inicializar(materialTierra,materialParedes,alturaParedes);
            bloquesTierra[i].GenerarMapa(altoAproximado + bloquesOffset, altoAproximado + bloquesOffset, tamanioCelda);
        }
    }

/*
    private void Update()
    {
        if (excavando && Time.frameCount % fps_para_excavar == 0)
        {
            TierraBloque.Coord tatoCoord = PuntoToCoord(tato.position);
            //Debug.Log($"{tatoCoord.tileX}:{tatoCoord.tileY}");
            ExcavarCirculo2(tatoCoord, Mathf.RoundToInt(tatu_collider.radius / tamañoCelda));

            //Una vez se ha excavado un circulo se debe regenerar el mesh
            RegenerarMapa();
        }
    }*/

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            excavando = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            excavando = false;
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) return;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        //El alto del collider minimo debe ser igual al tamanio de la celda
        float colliderAltoRedondeado = Mathf.RoundToInt(collider.size.y);
        float altoCeldaNormalizado = colliderAltoRedondeado < tamanioCelda ? tamanioCelda : colliderAltoRedondeado;

        collider.size = new Vector3 (Mathf.RoundToInt(collider.size.x), altoCeldaNormalizado, 0f);

        if(anchoAproximado < altoAproximado){
            Debug.LogWarning("El ancho minimo debe ser igual al alto!");
            anchoAproximado = altoAproximado;
            collider.size = new Vector3 (anchoAproximado, altoCeldaNormalizado, 0f);
        }

        anchoAproximado = (int) collider.size.x;
        altoAproximado = (int) collider.size.y;

        int cantidadBloques = Mathf.RoundToInt(anchoAproximado / altoAproximado);
        cantidadBloques = anchoAproximado % altoAproximado == 0 ? cantidadBloques : cantidadBloques +1;

        anchoAproximado = altoAproximado * cantidadBloques;
/*
        int nodosXgz = Mathf.RoundToInt(anchoAproximado / tamanioCelda);
        int nodosYgz = Mathf.RoundToInt(altoAproximado / tamanioCelda);
        float mapaAncho = nodosXgz * tamanioCelda;
        float mapaAlto = nodosYgz * tamanioCelda;
*/
        float mapaAncho = anchoAproximado;
        float mapaAlto = altoAproximado;

        Rect rect = new Rect();
/*
        rect.xMin = -mapaAncho / 2 + tamanioCelda / 2 + bloquesOffset/2;
        rect.xMax = mapaAncho / 2 - tamanioCelda / 2 + bloquesOffset/2;
        rect.yMin = -mapaAlto / 2 + tamanioCelda / 2;
        rect.yMax = mapaAlto / 2 - tamanioCelda / 2;
*/
        rect.xMin = transform.position.x;
        rect.xMax = transform.position.x + mapaAncho;
        rect.yMin = transform.position.y;
        rect.yMax = transform.position.y + mapaAlto;
        rect.center = transform.position;
        rect.center += new Vector2(rect.size.x/2,rect.size.y/2);

        collider.offset = new Vector3(collider.size.x/2,collider.size.y/2,0f);
        //collider.size = rect.size;

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
