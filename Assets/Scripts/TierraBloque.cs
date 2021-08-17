using UnityEngine;
using UnityEditor;

public class TierraBloque : MonoBehaviour
{

    struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }
    static Transform tato;
    static CircleCollider2D tatu_collider;
    public static int fps_para_excavar = 3;
    bool excavando = false;

    private void Awake()
    {
        Application.targetFrameRate = 30;
        //groundController = GetComponent<GroundController>();
        if (tatu_collider == null) tatu_collider = GameObject.FindGameObjectWithTag("Player").GetComponent<CircleCollider2D>();
        tato = tatu_collider.transform;
    }

    private void Update()
    {
        if (excavando && Time.frameCount % fps_para_excavar == 0)
        {
            Coord tatoCoord = PuntoToCoord(tato.position);
            //Debug.Log($"{tatoCoord.tileX}:{tatoCoord.tileY}");
            ExcavarCirculo2(tatoCoord, Mathf.RoundToInt(tatu_collider.radius / tamañoCelda));

            //Una vez se ha excavado un circulo se debe regenerar el mesh
            RegenerarMapa();
        }
    }

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


    [Min(1)]
    public int anchoAproximado;
    [Min(1)]
    public int altoAproximado;
    [Min(0.1f)]
    public float tamañoCelda = 1;
	[Min(0)]
	public int suavizado = 0;

    int nodosX;
    int nodosY;
    int[,] mapa;
    Vector3 origenCoordenadas;

/*
    void Start()
    {
        GenerarMapa();
    }
*/

    /*
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Coord posicionMouse = PuntoToCoord(Input.mousePosition.x,Input.mousePosition.y);
                ExcavarCirculo(posicionMouse,3);

                //Una vez se ha excavado un circulo se debe regenerar el mesh
                RegenerarMapa();
            }
        }*/

    TierraGenerador meshGen;

    //Se usa para crear el generador del mapa de nodos, los mesh y asignarlos
    void Inicializar(Material mat_tierra, Material mat_paredes,float alturaParedes){
        if(null == meshGen){

            GameObject tierra = new GameObject("tierra");
            tierra.transform.position = transform.position;
            tierra.transform.rotation = Quaternion.Euler(-90f,0f,0f);
            tierra.transform.parent = gameObject.transform;

            MeshFilter tierraFilter = tierra.AddComponent<MeshFilter>();
            MeshRenderer tierraRenderer = tierra.AddComponent<MeshRenderer>();
            tierraRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            tierraRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            tierraRenderer.receiveShadows = false;
            tierraRenderer.motionVectorGenerationMode = UnityEngine.MotionVectorGenerationMode.ForceNoMotion;
            
            GameObject paredes = new GameObject("paredes");
            paredes.transform.position = transform.position;
            paredes.transform.rotation = Quaternion.Euler(-90f,0f,0f);
            paredes.transform.parent = gameObject.transform;

            MeshFilter paredesFilter = paredes.AddComponent<MeshFilter>();
            MeshRenderer paredesRenderer = paredes.AddComponent<MeshRenderer>();
            paredesRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            paredesRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            paredesRenderer.receiveShadows = false;
            paredesRenderer.motionVectorGenerationMode = UnityEngine.MotionVectorGenerationMode.ForceNoMotion;

            meshGen = new TierraGenerador(tierraFilter,paredesFilter,alturaParedes);
        }
    }

    public void GenerarMapa()
    {
        nodosX = Mathf.RoundToInt(anchoAproximado / tamañoCelda);
        nodosY = Mathf.RoundToInt(altoAproximado / tamañoCelda);
        mapa = new int[nodosX, nodosY];

        float mapaAncho = nodosX * tamañoCelda;
        float mapaAlto = nodosY * tamañoCelda;
        float bordeIzquierdo = -mapaAncho / 2 + tamañoCelda / 2;
        float bordeInferior = -mapaAlto / 2 + tamañoCelda / 2;
        origenCoordenadas = new Vector3(bordeIzquierdo, bordeInferior, 0.0f);

        RellenarMapa();
        //SuavizarMapa();

        meshGen.GenerarTierraMesh(mapa, tamañoCelda);
    }

    public void RegenerarMapa()
    {
		//TODO suavizar comentado
        SuavizarMapa(suavizado);
        meshGen.GenerarTierraMesh(mapa, tamañoCelda);
    }

    void RellenarMapa()
    {
        for (int x = 0; x < nodosX; x++)
        {
            for (int y = 0; y < nodosY; y++)
            {
                // 1 - Relleno, 0 - Vacio //Si es parte del borde del mapa va vacio
                if(y==0 || y== nodosY-1 || x==0 || x==nodosX-1 )
                    mapa[x, y] = 0;
                else 
                    mapa[x, y] = 1;
            }
        }
    }

    Coord PuntoToCoord(Vector3 punto)
    {
        int x = Mathf.FloorToInt((punto - transform.TransformPoint(origenCoordenadas)).x / tamañoCelda);
        int y = Mathf.FloorToInt((punto - transform.TransformPoint(origenCoordenadas)).y / tamañoCelda);
        return new Coord(x, y);
    }

    Vector3 CoordToWorldPoint(Coord tile)
    {
        return new Vector3(-nodosX / 2 + tamañoCelda / 2 + tile.tileX, 2, -nodosY / 2 + tamañoCelda / 2 + tile.tileY);
    }

    //Se usa para calcular la circunferencia de tierra excavada
    void ExcavarCirculo(Coord c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int dibujarX = c.tileX + x;
                    int dibujarY = c.tileY + y;
                    if (EstaDentroDelMapa(dibujarX, dibujarY))
                    {
                        mapa[dibujarX, dibujarY] = 0;
                    }
                }
            }
        }
    }

    void ExcavarCirculo2(Coord center, int radio)
    {
		//TODO este radio pa q?
        //radio += 10;

        int px, nx, py, ny, bordeY;

        for (int i = 0; i <= radio; i++)
        {
            for (i = 0; i <= radio; i++)
            {
                // r = Sqrt(x*x + y*y) => y = Sqrt(r*r - x*x)
                bordeY = Mathf.RoundToInt(Mathf.Sqrt(radio * radio - i * i));
                for (int j = 0; j <= bordeY; j++)
                {
                    px = center.tileX + i;
                    nx = center.tileX - i;
                    py = center.tileY + j;
                    ny = center.tileY - j;

					if (EstaDentroDelMapa(px, py))
                    	mapa[px, py] = 0;
					if (EstaDentroDelMapa(nx, py))
                    	mapa[nx, py] = 0;
					if (EstaDentroDelMapa(px, ny))
                    	mapa[px, ny] = 0;
					if (EstaDentroDelMapa(nx, ny))
                    	mapa[nx, ny] = 0;
                }
            }
        }
    }

    //Verifica si el tile actual se encuentra dentro del mapa
    bool EstaDentroDelMapa(int x, int y)
    {
        return x >= 0 && x < nodosX && y >= 0 && y < nodosY;
    }


    //Se utiliza para suavizar la randomizacion en Cellular Automata
    void SuavizarMapa(int iteraciones = 0)
    {
        while (iteraciones > 0)
        {
            for (int x = 0; x < nodosX; x++)
            {
                for (int y = 0; y < nodosY; y++)
                {
                    int cuadrosRellenosVecinos = ContarRellenoCercanos(x, y);

                    if (cuadrosRellenosVecinos > 4)
                        mapa[x, y] = 1;
                    else if (cuadrosRellenosVecinos < 4)
                        mapa[x, y] = 0;
                }
            }
            iteraciones--;
        }
    }

    //Cuanta la cantidad de casilleros rellenos vacios
    int ContarRellenoCercanos(int grillaX, int grillaY)
    {
        int cantidadRellenos = 0;
        for (int vecinoX = grillaX - 1; vecinoX <= grillaX + 1; vecinoX++)
        {
            for (int vecinoY = grillaY - 1; vecinoY <= grillaY + 1; vecinoY++)
            {
                if (EstaDentroDelMapa(vecinoX, vecinoY))
                {
                    if (vecinoX != grillaX || vecinoY != grillaY)
                    {
                        cantidadRellenos += mapa[vecinoX, vecinoY];
                    }
                }
                else
                {
                    cantidadRellenos++;
                }
            }
        }
        return cantidadRellenos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (EditorApplication.isPlaying) return;

        int nodosXgz = Mathf.RoundToInt(anchoAproximado / tamañoCelda);
        int nodosYgz = Mathf.RoundToInt(altoAproximado / tamañoCelda);
        float mapaAncho = nodosXgz * tamañoCelda;
        float mapaAlto = nodosYgz * tamañoCelda;

        Rect rect = new Rect();

        rect.xMin = -mapaAncho / 2 + tamañoCelda / 2;
        rect.xMax = mapaAncho / 2 - tamañoCelda / 2;
        rect.yMin = -mapaAlto / 2 + tamañoCelda / 2;
        rect.yMax = mapaAlto / 2 - tamañoCelda / 2;
        rect.center = transform.position;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = rect.size;
        collider.offset = Vector3.zero;


        Vector3[] verts = new Vector3[]
        {
            new Vector3(rect.xMin, rect.yMin, transform.position.z),
            new Vector3(rect.xMin, rect.yMax, transform.position.z),
            new Vector3(rect.xMax, rect.yMax, transform.position.z),
            new Vector3(rect.xMax, rect.yMin, transform.position.z)
        };
        Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, .3f), Color.cyan);
    }
#endif
}
