using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

	//Singleton Pattern
	public static AudioManager Instance;

	[Header("Zone Sounds")]
	[SerializeField] private AudioClip defaultMusic;
	[SerializeField] private float maxMusicVolume = 0.2f;

	private float maxSoundEffectVolume = 0.25f;
	private AudioSource musicAudioSource;
	private AudioSource soundEffectSource;
	private AudioClip nextMusic;


	//Singleton
	private AudioManager() { }

	private void Awake()
	{
		//enforce singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		musicAudioSource = GetComponents<AudioSource>()[0];
		soundEffectSource = GetComponents<AudioSource>()[1];

		soundEffectSource.volume = maxSoundEffectVolume;
		soundEffectSource.mute = true;

		musicAudioSource.clip = defaultMusic;
		musicAudioSource.volume = maxMusicVolume;
		musicAudioSource.loop = true;
		musicAudioSource.Stop();


		StartCoroutine(StartInitialSounds());
	}

	public void ChangeZoneSoundsToDefault()
	{
		if (musicAudioSource.clip != defaultMusic)
		{
			ChangeEnvironmentMusic(defaultMusic);
		}
	}

	public void ChangeEnvironmentMusic(AudioClip newMusic)
	{
		if (musicAudioSource.clip != newMusic)
		{
			nextMusic = newMusic;
			StartCoroutine(SwapClip(musicAudioSource, newMusic, maxMusicVolume));
		}
	}

	public void PausePlayOneShotResumeMusic(AudioClip music)
	{
		if (music == musicAudioSource.clip)
		{
			return;
		}

		StartCoroutine(PausePlayOneShotResume(musicAudioSource, music, maxMusicVolume));
	}

	public void PlaySoundEffect(AudioClip clip, bool oneShot = false)
	{
		if (oneShot)
		{
			soundEffectSource.PlayOneShot(clip);
			return;
		}

		if (soundEffectSource.isPlaying)
		{
			return;
		}

		soundEffectSource.clip = clip;
		soundEffectSource.Play();
	}

	private IEnumerator StartInitialSounds()
	{
		if(musicAudioSource.clip != defaultMusic)
		{
			musicAudioSource.clip = defaultMusic;
		}

		yield return new WaitForSeconds(0.525f);

		if (!musicAudioSource.isPlaying)
		{
			musicAudioSource.Play();
		}

		soundEffectSource.mute = false;
	}

	private IEnumerator PausePlayOneShotResume(AudioSource source, AudioClip clip, float maxVol)
	{
		AudioClip oldClip = source.clip;
		float time = source.time;
		source.loop = false;
		source.time = 0f;
		source.clip = clip;
		source.Play();

		yield return new WaitForSeconds(clip.length);

		source.volume = 0;
		source.clip = oldClip;
		source.loop = true;
		source.time = Mathf.Min(time, oldClip.length - 0.01f);
		source.Play();
		StartCoroutine(FadeIn(source, maxVol));
	}

	private IEnumerator SwapClip(AudioSource source, AudioClip newClip, float maxVol)
	{


		yield return StartCoroutine(FadeOut(source));

		if (newClip == nextMusic)
		{
			source.clip = newClip;
			source.time = 0f;
		}

		source.Play();

		StartCoroutine(FadeIn(source, maxVol));
	}

	private IEnumerator FadeIn(AudioSource source, float maxVol)
	{
		int i = 0;
		while (source.volume < maxVol && i <= 40)
		{
			i++;
			source.volume += 0.025f;
			yield return new WaitForSeconds(0.1f);
		}
		source.volume = maxVol;
	}

	private IEnumerator FadeOut(AudioSource source)
	{
		int i = 0;
		while (source.volume > 0 && i <= 40)
		{
			i++;
			source.volume -= 0.025f;
			yield return new WaitForSeconds(0.1f);
		}
		source.volume = 0;
	}
}