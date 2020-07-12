using UnityEngine;
using System.Collections;

/**
 * Classe permettant de manipuler la musique principale du jeu (musique de fond)
 * 
 * Ne peut etre utilisé dans Awake.
 **/
public class MainMusicManager : MonoBehaviour
{

  public static MainMusicManager instance;


  private AudioSource _mainMusicSource;

  private AudioClip _clip;

  private AudioClip _previousClip;
  private int _previousClipSamplesTime;

  protected void Awake()
  {
    instance = this;
  }

	// Use this for initialization
  protected void Start()
  {
    _mainMusicSource = gameObject.GetComponent<AudioSource> ();
  }
	
	// Update is called once per frame
  void Update()
  {
  }

  /**
   * musicName est le path depuis le dossier "Audio/Music"
   */
  public bool LoadMusic(string musicName)
  {
    AudioClip temp = Resources.Load("Audio/Music/"+musicName) as AudioClip;
    _previousClip = _mainMusicSource.clip;
    _previousClipSamplesTime = _mainMusicSource.timeSamples;
    if (temp != null)
    {
      _mainMusicSource.clip = temp;
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool Play()
  {
    if (_mainMusicSource.clip != null)
    {
      _mainMusicSource.Play();
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool isPlaying()
  {
    return _mainMusicSource.isPlaying;
  }

  public bool LoadAndPlayMusic(string musicName)
  {
    if(LoadMusic (musicName))
      return Play ();
    else
      return false;
  }

  public void Pause()
  {
    _mainMusicSource.Pause();
  }

  /**
   * Permet de reprendre la chanson précédement jouée au meme endroit (typiquement pour reprendre un chanson après un combat)
   **/
  public bool ResumePrevious()
  {
    if(_previousClip != null && _previousClipSamplesTime >0 && _previousClipSamplesTime < _previousClip.samples)
    {
      _mainMusicSource.clip = _previousClip;
      _mainMusicSource.timeSamples = _previousClipSamplesTime;
      return Play();
    }
    else
      return false;
  }

  /**
   * Permet de lire un bruitage une seule fois. Peut jouer plusieurs bruitages en parallèles, mais ne doit etre utilisé que pour les
   * bruitages non localisés (c'est-à-dire issus du héros, comme une attaque, un pouvoir ou un bruitage de voix), en effet la source
   * est la caméra. Si la source doit etre différente, créer l'AudioSource directement sur l'objet concerné.
   **/
  //TODO Je ne sais pas si la co-routine est la meilleur façon de détruire l'audiosource après.
  public bool PlaySoundEffect(string soundEffectName)
  {
    AudioSource soundEffectSource = gameObject.AddComponent<AudioSource>();

    AudioClip soundEffect = Resources.Load("Audio/SoundEffect/"+soundEffectName) as AudioClip;

    if(soundEffect != null)
    {
      soundEffectSource.clip = soundEffect;
      soundEffectSource.Play();
      StartCoroutine(DestroySoundEffectSource(soundEffect.length+1, soundEffectSource));//+1seconde est par sécurité.
      return true;
    }
    else
    {
      return false;
    }
  }

  private IEnumerator DestroySoundEffectSource(float delay, AudioSource audioSource)
  {
    yield return new WaitForSeconds(delay);
    Destroy(audioSource);
  }


}
