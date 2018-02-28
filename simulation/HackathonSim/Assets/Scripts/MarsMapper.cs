using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

public class MarsMapper : MonoBehaviour
{
	public Shader tileShader;
	private static string api =
		"https://mars.nasa.gov/maps/explore-mars-map/catalog/Mars_Viking_MDIM21_ClrMosaic_global_232m/1.0.0/default/default028mm/9/";

	private HashSet<Vector2> downloaded_tiles;
	// Use this for initialization
	void Start ()
	{
		downloaded_tiles = new HashSet<Vector2>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void updateTiles(Vector3 roverLocation)
	{
		Vector2 roverTile = new Vector2((int)roverLocation.x/10, (int)roverLocation.z/10);
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				Vector2 tile = roverTile + new Vector2(x,y);
				if (!downloaded_tiles.Contains(tile))
				{
					StartCoroutine(GetTile(tile));
				}
			}
		}
	}
	
	
	public IEnumerator GetTile(Vector2 coords)
	{
		string url = api + coords.x + "/" + coords.y + ".png";
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		yield return www.SendWebRequest();
		downloaded_tiles.Add(coords);
		Texture texture = DownloadHandlerTexture.GetContent(www);
		GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
		tile.transform.position = new Vector3(coords.x*10, 0, coords.y*10);
		tile.transform.Rotate(new Vector3(0,90,0));
		Renderer rend = tile.GetComponent<Renderer>();
		rend.material = new Material(tileShader);
		rend.material.mainTexture = texture;
	}
}
