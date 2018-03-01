using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.WebCam;

public enum Planet
{
	Moon,
	Mars
};

public class MarsMapper : MonoBehaviour
{
	public Shader tileShader;

	public MarsRover Rover;

	public GameObject Mars;
	public GameObject Moon;
	public GameObject MiniRover;

	private Planet current_planet = Planet.Moon;
	
	private static string moon_api =
		"http://moontrek.jpl.nasa.gov/trektiles/Moon/EQ/LRO_WAC_Mosaic_Global_303ppd_v02/1.0.0/default/default028mm/8/";
	private static string mars_api =
		"https://mars.nasa.gov/maps/explore-mars-map/catalog/Mars_Viking_MDIM21_ClrMosaic_global_232m/1.0.0/default/default028mm/9/";

	private HashSet<Vector2> downloaded_tiles;
	

	private List<GameObject> tiles;
	// Use this for initialization
	void Start ()
	{
		downloaded_tiles = new HashSet<Vector2>();
		tiles = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector2 robotPos = Rover.getLatLong();
		Vector3 coords = LatLngToCart(robotPos, 0.5f);
		//Debug.Log(coords);
		switch (current_planet)
		{
				case Planet.Mars:
					MiniRover.transform.position = Mars.transform.position + coords;
					break;
				case Planet.Moon:
					MiniRover.transform.position = Moon.transform.position + coords;
					break;
		}




		
	}


	public void updateTiles(Vector3 roverLocation)
	{
		Vector2 roverTile = new Vector2((int)roverLocation.x/10, (int)roverLocation.z/10);
		for (int x = -2; x < 3; x++)
		{
			for (int y = -2; y < 3; y++)
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
		string url = "";
		switch (current_planet)
		{
			case Planet.Mars:
				url = mars_api + coords.x + "/" + coords.y + ".png";
				break;
			case Planet.Moon:
				url = moon_api + coords.x + "/" + coords.y + ".jpg";
				break;
		}
		
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
		yield return www.SendWebRequest();
		downloaded_tiles.Add(coords);
		Texture2D texture = DownloadHandlerTexture.GetContent(www);
		//Texture2D bumpTexture = getGreyscale(texture);
		GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Plane);
		tile.transform.position = new Vector3(coords.x*10, 0, coords.y*10);
		tile.transform.Rotate(new Vector3(0,90,0));
		tile.layer = 9;
		Renderer rend = tile.GetComponent<Renderer>();
		rend.material = new Material(tileShader);
		rend.material.mainTexture = texture;
		tiles.Add(tile);
		//rend.material.SetTexture("_ParallaxMap", bumpTexture);
	}

	public Texture2D getGreyscale(Texture2D color)
	{
		Texture2D bumpTexture = new Texture2D(color.width, color.height, TextureFormat.RGB24, false, false);
		Color[] pixels = color.GetPixels();
		float max = 0;
		for (int y = 0; y < color.height; y++)
		{
			for (int x = 0; x < color.width; x++)
			{
				Color pixel = pixels[x+y*color.width];
				float lum = pixel.grayscale;
				max = Math.Max(max, lum);
				bumpTexture.SetPixel(x,y, new Color(lum,lum,lum));
			}
		}

		return bumpTexture;
	}
	
	public static Vector2 MapToLatLng(float x, float y){
		double lon = x / (512.0 / 360.0) - 180.0;
		double lat = y / (256.0 / 180.0) - 90.0;
		return new Vector2((float)lat, (float)lon);
	}

	public static Vector3 LatLngToCart(Vector2 latlng, float radius)
	{
		double x = radius * Math.Cos(latlng[0]) * Math.Cos(latlng[1]);
		double y = radius * Math.Cos(latlng[0]) * Math.Sin(latlng[1]);
		double z = radius * Math.Sin(latlng[0]);
		return new Vector3((float)x,(float)y,(float)z);
	}

	public void SetPlanet(Planet planet)
	{
		foreach (GameObject tile in tiles)
		{
			Destroy(tile);
		}
		downloaded_tiles = new HashSet<Vector2>();
		current_planet = planet;
	}
}
