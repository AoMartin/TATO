using UnityEngine;
using System.Collections.Generic;
//using System.Linq;

[ExecuteInEditMode]
public class TierraGenerador : MonoBehaviour
{
    //		# + #      Marching Squares
    //  	+ 	+		# : NodoEsquina
    //		# + #		+ : Nodo

    public class Nodo
    {
        public Vector3 posicion;
        public int indiceVertice = -1;

        public Nodo(Vector3 _pos)
        {
            posicion = _pos;
        }
    }

    public class NodoEsquina : Nodo
    {
        public bool activo;
        public Nodo superior, derecho;

        public NodoEsquina(Vector3 _pos, bool _activo, float tamañoCelda) : base(_pos)
        {
            activo = _activo;
            superior = new Nodo(posicion + Vector3.forward * tamañoCelda / 2f);
            derecho = new Nodo(posicion + Vector3.right * tamañoCelda / 2f);
        }
    }

    public class Celda
    {
        public NodoEsquina arribaIzq, arribaDer, abajoDer, abajoIzq;
        public Nodo centroArriba, centroDer, centroAbajo, centroIzq;
        //Suma un 1 binario dependiendo del nodoEsquina activo
        public int configuracion;

        public Celda(NodoEsquina _topLeft, NodoEsquina _topRight, NodoEsquina _bottomRight, NodoEsquina _bottomLeft)
        {
            arribaIzq = _topLeft;
            arribaDer = _topRight;
            abajoDer = _bottomRight;
            abajoIzq = _bottomLeft;

            centroArriba = arribaIzq.derecho;
            centroDer = abajoDer.superior;
            centroAbajo = abajoIzq.derecho;
            centroIzq = abajoIzq.superior;

            if (arribaIzq.activo)	//1000
                configuracion += 8;
            if (arribaDer.activo)	//0100
                configuracion += 4;
            if (abajoDer.activo)	//0010
                configuracion += 2;
            if (abajoIzq.activo)	//0001
                configuracion += 1;
        }

    }

    public class Grilla
    {
        public Celda[,] celdas;

        public Grilla(int[,] mapa, float tamañoCelda)
        {
            int nodoCantidadX = mapa.GetLength(0);
            int nodoCantidadY = mapa.GetLength(1);
            float mapaAncho = nodoCantidadX * tamañoCelda;
            float mapaAlto = nodoCantidadY * tamañoCelda;

            NodoEsquina[,] nodoEsquinas = new NodoEsquina[nodoCantidadX, nodoCantidadY];

            for (int x = 0; x < nodoCantidadX; x++)
            {
                for (int y = 0; y < nodoCantidadY; y++)
                {
                    Vector3 pos = new Vector3((-mapaAncho / 2) + (x * tamañoCelda) + (tamañoCelda / 2), 0, (-mapaAlto / 2) + (y * tamañoCelda) + (tamañoCelda / 2));
                    nodoEsquinas[x, y] = new NodoEsquina(pos, mapa[x, y] == 1, tamañoCelda);
                }
            }

            celdas = new Celda[nodoCantidadX - 1, nodoCantidadY - 1];
            for (int x = 0; x < nodoCantidadX - 1; x++)
            {
                for (int y = 0; y < nodoCantidadY - 1; y++)
                {
                    celdas[x, y] = new Celda(nodoEsquinas[x, y + 1], nodoEsquinas[x + 1, y + 1], nodoEsquinas[x + 1, y], nodoEsquinas[x, y]);
                }
            }
        }
    }

    //Los triangulos se usaran para armar el mesh de la tierra dependiendo de los vertices activos de las celdas
    //Los lados de un traingulo que no sean compartidos por otro triangulo serán usados como bordes - outlines
    struct Triangulo
    {
        public int indiceVerticeA;
        public int indiceVerticeB;
        public int indiceVerticeC;
        int[] vertices;

        public Triangulo(int a, int b, int c)
        {
            indiceVerticeA = a;
            indiceVerticeB = b;
            indiceVerticeC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        //Indexador del array de vertices
        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }

        public bool Contains(int indiceVertice)
        {
            return indiceVertice == indiceVerticeA || indiceVertice == indiceVerticeB || indiceVertice == indiceVerticeC;
        }
    }

    [SerializeField, HideInInspector] Grilla grilla;
    [SerializeField, HideInInspector] MeshFilter tierra;
    [SerializeField, HideInInspector] MeshFilter paredes;
    [SerializeField, HideInInspector] float alturaParedes = 2;
 
    [SerializeField, HideInInspector] List<Vector3> vertices;
    [SerializeField, HideInInspector] List<int> triangulos;
 
    [SerializeField, HideInInspector] Dictionary<int, List<Triangulo>> trianguloDiccionario = new Dictionary<int, List<Triangulo>>();
    [SerializeField, HideInInspector] List<List<int>> outlines = new List<List<int>>();
    [SerializeField, HideInInspector] HashSet<int> verticesChequeados = new HashSet<int>();

    public void NewTierraGenerador(MeshFilter tierraFilter, MeshFilter paredesFilter, float alturaParedes)
    {
        this.tierra = tierraFilter;
        this.paredes = paredesFilter;
        this.alturaParedes = alturaParedes;
    }

    public void GenerarTierraMesh(int[,] mapa, float tamañoCelda)
    {
        trianguloDiccionario.Clear();
        outlines.Clear();
        verticesChequeados.Clear();

        grilla = new Grilla(mapa, tamañoCelda);
        vertices = new List<Vector3>();
        triangulos = new List<int>();

        for (int x = 0; x < grilla.celdas.GetLength(0); x++)
        {
            for (int y = 0; y < grilla.celdas.GetLength(1); y++)
            {
                TriangularCelda(grilla.celdas[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        tierra.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangulos.ToArray();
        mesh.RecalculateNormals();

        CalcularTierraUVs(mesh, mapa, tamañoCelda);
        GenerarParedesMesh();
    }

    //TODO arreglar esto
    void CalcularTierraUVs(Mesh mesh, int[,] mapa, float tamañoCelda)
    {
        int tileAmountX = 2;
        int tileAmountY = 3;
        Vector2[] uvs = new Vector2[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            float porcentajeX = Mathf.InverseLerp(-mapa.GetLength(0) / 2 * tamañoCelda, mapa.GetLength(0) / 2 * tamañoCelda, vertices[i].x) * tileAmountX;
            float porcentajeY = Mathf.InverseLerp(-mapa.GetLength(0) / 2 * tamañoCelda, mapa.GetLength(0) / 2 * tamañoCelda, vertices[i].z) * tileAmountY;
            //uvs[i] = new Vector2(porcentajeX, porcentajeY);
            uvs[i].x = porcentajeX;
            uvs[i].y = porcentajeY;
        }
        mesh.uv = uvs;
    }

    void GenerarParedesMesh()
    {
        CalcularBordesDelMesh();

        List<Vector3> verticesParedes = new List<Vector3>();
        List<int> triangulosParedes = new List<int>();
        Mesh paredesMesh = new Mesh();

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int indiceInicial = verticesParedes.Count;
                verticesParedes.Add(vertices[outline[i]]); 								        // izquierda
                verticesParedes.Add(vertices[outline[i + 1]]);								    // derecha
                verticesParedes.Add(vertices[outline[i]] - Vector3.up * alturaParedes); 		// abajo izquierda
                verticesParedes.Add(vertices[outline[i + 1]] - Vector3.up * alturaParedes); 	// abajo derecha

                triangulosParedes.Add(indiceInicial + 0);
                triangulosParedes.Add(indiceInicial + 2);
                triangulosParedes.Add(indiceInicial + 3);

                triangulosParedes.Add(indiceInicial + 3);
                triangulosParedes.Add(indiceInicial + 1);
                triangulosParedes.Add(indiceInicial + 0);
            }
        }
        paredesMesh.vertices = verticesParedes.ToArray();
        paredesMesh.triangles = triangulosParedes.ToArray();
        //paredesMesh.triangles = paredesMesh.triangles.Reverse().ToArray();
        paredes.mesh = paredesMesh;
    }

    //En base a los nodos activos, como se deberan armar los triangulos para el mesh de la celda actual
    //Se recorre la configuracion en sentido horario desde arriba y de derecha a izquierda
    void TriangularCelda(Celda celda)
    {
        switch (celda.configuracion)
        {
            case 0:
                break;

            // 1 punto:
            case 1:
                MeshDesdePuntos(celda.centroIzq, celda.centroAbajo, celda.abajoIzq);
                break;
            case 2:
                MeshDesdePuntos(celda.abajoDer, celda.centroAbajo, celda.centroDer);
                break;
            case 4:
                MeshDesdePuntos(celda.arribaDer, celda.centroDer, celda.centroArriba);
                break;
            case 8:
                MeshDesdePuntos(celda.arribaIzq, celda.centroArriba, celda.centroIzq);
                break;

            // 2 puntos:
            case 3:
                MeshDesdePuntos(celda.centroDer, celda.abajoDer, celda.abajoIzq, celda.centroIzq);
                break;
            case 6:
                MeshDesdePuntos(celda.centroArriba, celda.arribaDer, celda.abajoDer, celda.centroAbajo);
                break;
            case 9:
                MeshDesdePuntos(celda.arribaIzq, celda.centroArriba, celda.centroAbajo, celda.abajoIzq);
                break;
            case 12:
                MeshDesdePuntos(celda.arribaIzq, celda.arribaDer, celda.centroDer, celda.centroIzq);
                break;
            case 5:
                MeshDesdePuntos(celda.centroArriba, celda.arribaDer, celda.centroDer, celda.centroAbajo, celda.abajoIzq, celda.centroIzq);
                break;
            case 10:
                MeshDesdePuntos(celda.arribaIzq, celda.centroArriba, celda.centroDer, celda.abajoDer, celda.centroAbajo, celda.centroIzq);
                break;

            // 3 puntos:
            case 7:
                MeshDesdePuntos(celda.centroArriba, celda.arribaDer, celda.abajoDer, celda.abajoIzq, celda.centroIzq);
                break;
            case 11:
                MeshDesdePuntos(celda.arribaIzq, celda.centroArriba, celda.centroDer, celda.abajoDer, celda.abajoIzq);
                break;
            case 13:
                MeshDesdePuntos(celda.arribaIzq, celda.arribaDer, celda.centroDer, celda.centroAbajo, celda.abajoIzq);
                break;
            case 14:
                MeshDesdePuntos(celda.arribaIzq, celda.arribaDer, celda.abajoDer, celda.centroAbajo, celda.centroIzq);
                break;

            // 4 puntos:
            case 15:
                MeshDesdePuntos(celda.arribaIzq, celda.arribaDer, celda.abajoDer, celda.abajoIzq);
                break;
        }
    }

    void MeshDesdePuntos(params Nodo[] puntos)
    {
        AsignarVertices(puntos);

        if (puntos.Length >= 3)
            CrearTriangulo(puntos[0], puntos[1], puntos[2]);
        if (puntos.Length >= 4)
            CrearTriangulo(puntos[0], puntos[2], puntos[3]);
        if (puntos.Length >= 5)
            CrearTriangulo(puntos[0], puntos[3], puntos[4]);
        if (puntos.Length >= 6)
            CrearTriangulo(puntos[0], puntos[4], puntos[5]);
    }

    void AsignarVertices(Nodo[] puntos)
    {
        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i].indiceVertice == -1)
            {
                puntos[i].indiceVertice = vertices.Count;
                vertices.Add(puntos[i].posicion);
            }
        }
    }

    void CrearTriangulo(Nodo a, Nodo b, Nodo c)
    {
        triangulos.Add(a.indiceVertice);
        triangulos.Add(b.indiceVertice);
        triangulos.Add(c.indiceVertice);

        //Para luego calcular outlines aca y lo que sigue del diccionario
        Triangulo triangulo = new Triangulo(a.indiceVertice, b.indiceVertice, c.indiceVertice);
        AgregarTrianguloAlDiccionario(triangulo.indiceVerticeA, triangulo);
        AgregarTrianguloAlDiccionario(triangulo.indiceVerticeB, triangulo);
        AgregarTrianguloAlDiccionario(triangulo.indiceVerticeC, triangulo);
    }

    //*- OUTLINES
    //Arma el diccionario para guardar los triangulos en base a un indice
    void AgregarTrianguloAlDiccionario(int indiceVerticeKey, Triangulo triangulo)
    {
        if (trianguloDiccionario.ContainsKey(indiceVerticeKey))
        {
            trianguloDiccionario[indiceVerticeKey].Add(triangulo);
        }
        else
        {
            List<Triangulo> listaTriangulos = new List<Triangulo>();
            listaTriangulos.Add(triangulo);
            trianguloDiccionario.Add(indiceVerticeKey, listaTriangulos);
        }
    }

    void CalcularBordesDelMesh()
    {
        for (int verticeIndice = 0; verticeIndice < vertices.Count; verticeIndice++)
        {
            if (!verticesChequeados.Contains(verticeIndice))
            {
                int newOutlineVertex = ObtenerVerticeBordeConectado(verticeIndice);
                if (newOutlineVertex != -1)
                {
                    verticesChequeados.Add(verticeIndice);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(verticeIndice);
                    outlines.Add(newOutline);
                    SeguirOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(verticeIndice);
                }
            }
        }
        SimplificarBordesDelMesh();
    }

    void SimplificarBordesDelMesh()
    {
        for (int indiceOutline = 0; indiceOutline < outlines.Count; indiceOutline++)
        {
            List<int> outlineSimplificada = new List<int>();
            Vector3 direccionVieja = Vector3.zero;
            for (int i = 0; i < outlines[indiceOutline].Count; i++)
            {
                Vector3 p1 = vertices[outlines[indiceOutline][i]];
                Vector3 p2 = vertices[outlines[indiceOutline][(i + 1) % outlines[indiceOutline].Count]];
                Vector3 dir = p1 - p2;
                if (dir != direccionVieja)
                {
                    direccionVieja = dir;
                    outlineSimplificada.Add(outlines[indiceOutline][i]);
                }
            }
            outlines[indiceOutline] = outlineSimplificada;
        }
    }

    void SeguirOutline(int indiceVertice, int inidiceOutline)
    {
        outlines[inidiceOutline].Add(indiceVertice);
        verticesChequeados.Add(indiceVertice);
        int siguienteIndiceVertice = ObtenerVerticeBordeConectado(indiceVertice);

        if (siguienteIndiceVertice != -1)
        {
            SeguirOutline(siguienteIndiceVertice, inidiceOutline);
        }
    }

    int ObtenerVerticeBordeConectado(int indiceVertice)
    {
        List<Triangulo> triangulosContienenVertice = trianguloDiccionario[indiceVertice];

        for (int i = 0; i < triangulosContienenVertice.Count; i++)
        {
            Triangulo triangulo = triangulosContienenVertice[i];

            for (int j = 0; j < 3; j++)
            {
                int verticeB = triangulo[j];
                if (verticeB != indiceVertice && !verticesChequeados.Contains(verticeB))
                {
                    if (IsOutlineEdge(indiceVertice, verticeB))
                    {
                        return verticeB;
                    }
                }
            }
        }
        return -1;
    }

    bool IsOutlineEdge(int verticeA, int verticeB)
    {
        List<Triangulo> triangulosContienenVerticeA = trianguloDiccionario[verticeA];
        int triangulosCompartidos = 0;

        for (int i = 0; i < triangulosContienenVerticeA.Count; i++)
        {
            if (triangulosContienenVerticeA[i].Contains(verticeB))
            {
                triangulosCompartidos++;
                if (triangulosCompartidos > 1)
                {
                    break;
                }
            }
        }
        return triangulosCompartidos == 1;
    }
}
