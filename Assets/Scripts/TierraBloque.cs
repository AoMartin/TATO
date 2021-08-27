using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TierraBloque : MonoBehaviour
{

    public struct Coord
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    [SerializeField, HideInInspector] int nodosX;
    [SerializeField, HideInInspector] int nodosY;
    [SerializeField, HideInInspector] int[,] mapa;
    [SerializeField, HideInInspector] Vector3 origenCoordenadas;
    [SerializeField, HideInInspector] TierraGenerador meshGen;

    //Se usa para crear el generador del mapa de nodos, los mesh y asignarlos
    public void Inicializar(Material mat_tierra, Material mat_paredes,float alturaParedes){
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
            tierraRenderer.material = mat_tierra;
            
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
            paredesRenderer.material = mat_paredes;

            meshGen = gameObject.AddComponent<TierraGenerador>();
            meshGen.NewTierraGenerador(tierraFilter,paredesFilter,alturaParedes);
        }
    }

    public void GenerarMapa(int nX, int nY, float tamanioCelda)
    {
        nodosX = nX+1;
        nodosY = nY+1;
        mapa = new int[nodosX, nodosY];
        
        float mapaAncho = nodosX * tamanioCelda;
        float mapaAlto = nodosY * tamanioCelda;
        float bordeIzquierdo = (-mapaAncho / 2) + (tamanioCelda / 2);
        float bordeInferior =( -mapaAlto / 2) + (tamanioCelda / 2);
        origenCoordenadas = new Vector3(bordeIzquierdo, bordeInferior, 0.0f);

        RellenarMapa();
        meshGen.GenerarTierraMesh(mapa, tamanioCelda);
    }

    public void RegenerarMapa(int suavizado, float tamanioCelda)
    {
		//TODO suavizar comentado
        SuavizarMapa(suavizado);
        meshGen.GenerarTierraMesh(mapa, tamanioCelda);
    }

    void RellenarMapa()
    {
        for (int x = 0; x < nodosX; x++)
        {
            for (int y = 0; y < nodosY; y++)
            {
                mapa[x, y] = 1;
            }
        }
    }

    public Coord PuntoToCoord(Vector3 punto, float tamanioCelda)
    {
        int x = Mathf.FloorToInt((punto - transform.TransformPoint(origenCoordenadas)).x / tamanioCelda);
        int y = Mathf.FloorToInt((punto - transform.TransformPoint(origenCoordenadas)).y / tamanioCelda);
        return new Coord(x, y);
    }

    public Vector3 CoordToWorldPoint(Coord tile, float tamanioCelda)
    {
        return new Vector3(-nodosX / 2 + tamanioCelda / 2 + tile.tileX, 2, -nodosY / 2 + tamanioCelda / 2 + tile.tileY);
    }

    //Se usa para calcular la circunferencia de tierra excavada
    public void ExcavarCirculo(Coord c, int r)
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

    public void ExcavarCirculo2(Coord center, int radio)
    {
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
}
