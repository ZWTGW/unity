using UnityEngine;
using System.Collections;


// szmerek bajerek do wizualizacji dzwieku w Menu, wyglada fajnie i tyle
public class Bajerek : MonoBehaviour
{
	private AudioSource dzwiek;
	public float[] s = new float[1024];
	
	void Awake ()
	{
		this.dzwiek = GetComponent<AudioSource>();
	}
	
	void Start()
	{
		
	}
	
	void Update ()
	{
		dzwiek.GetSpectrumData(s,0,FFTWindow.Triangle);
	}

	void OnGUI() {
		int w = Screen.width;
		int h = Screen.height;
		float r = (float)w / 16;
		float poz = 0;

		// dla tego dubstepa bierzemy tylko poczatek spektrum bo to nie jest normalna muzyka :D

		for(int i = 0; i < 16; i++) {
			poz = (h - 15) - ((s[i] * 5) * (float)h); // * 5 = też tuningujemy pod konkretna muzyczke ;)
			GUI.Box(new Rect(i * r, poz, r, h), "");
		}

	}
}