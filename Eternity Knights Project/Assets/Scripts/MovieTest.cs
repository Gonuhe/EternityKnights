using UnityEngine;

public class MovieTest : MonoBehaviour
{
  public MovieTexture movie;
  
  void Start()
  {
    GetComponent<MeshRenderer>().material.mainTexture=movie;
    GetComponent<AudioSource>().clip=movie.audioClip;
    movie.Play();
    GetComponent<AudioSource>().Play();
  }
}