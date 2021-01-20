using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GroundController : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
	private float widthWorld, heightWorld;
	private int widthPixel, heightPixel;
	private Color transparent; 
	
	//Se usa para ir calculando los valores de Y en el borde del circulo y guardarlos para no tener que recarcularlos
	Dictionary<int,int> coordYinCircle_cache = new Dictionary<int, int>();

	// Start() de GroundController
	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer>(); 
		// sr : variavel global de GroundController, ref para o SpriteRenderer de Ground
		//Texture2D tex = (Texture2D) Resources.Load("ground2");
		// Resources.Load("nome_do_arquivo") carrega um arquivo localizado
		// em Assets/Resources
		Texture2D tex =spriteRenderer.sprite.texture;
		Texture2D tex_clone = (Texture2D) Instantiate(tex);
		// Criamos uma Texture2D clone de tex para nao alterarmos a imagem original 
		spriteRenderer.sprite = Sprite.Create(tex_clone, 
		                          new Rect(0f, 0f, tex_clone.width, tex_clone.height),
		                          new Vector2(0.5f, 0.5f), 100f);
		transparent = new Color(0f, 0f, 0f, 0f);
		InitSpriteDimensions();
	}

	private void InitSpriteDimensions() {
		widthWorld = spriteRenderer.bounds.size.x;
		heightWorld = spriteRenderer.bounds.size.y;
		widthPixel = spriteRenderer.sprite.texture.width;
		heightPixel = spriteRenderer.sprite.texture.height;
	}

	private void CalculateCircleBorderCache(int radio){

		int i,d;
		for ( i = 0; i <= radio; i++)
		{
			// r = Sqrt(x*x + y*y) => y = Sqrt(r*r - x*x)
			d = Mathf.RoundToInt(Mathf.Sqrt(radio * radio - i * i));
			coordYinCircle_cache.Add(i,d);
		}
	}
/*
	private void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.CompareTag("Player")){
			DestroyGround(other.collider.GetComponent<CircleCollider2D>());
		}
	}*/
	private void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.CompareTag("Player")){
			DestroyGround(other as CircleCollider2D);
		}
	}

	public void  DestroyGround( CircleCollider2D collider ){

		Vector2Int center = World2Pixel(collider.bounds.center.x, collider.bounds.center.y);
		int radio = Mathf.RoundToInt(collider.bounds.size.x/2*widthPixel/widthWorld); //TRIGGER VERSION se usa el bounds /2
		radio+=10;

		int px, nx, py, ny, bordeY;
		
		for (int i = 0; i <= radio; i++)
		{
			//Chequea si el cache de coordY no esta vacio para tomar el valor
			if(coordYinCircle_cache.Count > 0){
				bordeY = coordYinCircle_cache[i];
			}
			else{
				CalculateCircleBorderCache(radio);
				bordeY = coordYinCircle_cache[i];
			}
			
			for (int j = 0; j <= bordeY; j++)
			{
				px = center.x + i;
				nx = center.x - i;
				py = center.y + j;
				ny = center.y - j;

				spriteRenderer.sprite.texture.SetPixel(px, py, transparent);
				spriteRenderer.sprite.texture.SetPixel(nx, py, transparent);
				spriteRenderer.sprite.texture.SetPixel(px, ny, transparent);
				spriteRenderer.sprite.texture.SetPixel(nx, ny, transparent);
			}
		}
		spriteRenderer.sprite.texture.Apply();

		//"""Recalcular el collider (por ahora)"""
		Destroy(GetComponent<PolygonCollider2D>());
		PolygonCollider2D newcolli = gameObject.AddComponent<PolygonCollider2D>();
		newcolli.isTrigger = true; //TRIGGER VERSION
	}

	private Vector2Int World2Pixel(float x, float y) {
		Vector2Int vector = new Vector2Int();
		
		float dx = x-transform.position.x;
		vector.x = Mathf.RoundToInt(0.5f*widthPixel+ dx *widthPixel/widthWorld);
		
		float dy = y - transform.position.y;
		vector.y = Mathf.RoundToInt(0.5f * heightPixel + dy * heightPixel / heightWorld);
		
		return vector;
	}
}
