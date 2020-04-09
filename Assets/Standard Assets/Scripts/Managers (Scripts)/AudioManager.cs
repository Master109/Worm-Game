using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;

namespace Worms
{
	public class AudioManager : SingletonMonoBehaviour<AudioManager>
	{
		public float Volume
		{
			get
			{
				return SaveAndLoadManager.GetValue<float>("Volume", 1);
			}
			set
			{
				SaveAndLoadManager.SetValue("Volume", value);
			}
		}
		public bool Mute
		{
			get
			{
				return SaveAndLoadManager.GetValue<bool>("Mute", false);
			}
			set
			{
				SaveAndLoadManager.SetValue("Mute", value);
			}
		}
		public SoundEffect soundEffectPrefab;
		public static SoundEffect[] soundEffects = new SoundEffect[0];

		public override void Awake ()
		{
			base.Awake ();
			UpdateAudioListener ();
			soundEffects = new SoundEffect[0];
		}

		public virtual void UpdateAudioListener ()
		{
			AudioListener.pause = Mute;
			AudioListener.volume = Volume;
		}

		public virtual void ToggleMute ()
		{
			if (GameManager.GetSingleton<AudioManager>() != this)
			{
				GameManager.GetSingleton<AudioManager>().ToggleMute ();
				return;
			}
			Mute = !Mute;
			UpdateAudioListener ();
		}
		
		public virtual SoundEffect PlaySoundEffect (SoundEffect.Settings settings, Vector2 position = new Vector2())
		{
			SoundEffect output = GameManager.GetSingleton<ObjectPool>().SpawnComponent<SoundEffect>(soundEffectPrefab.prefabIndex, position);
			output.audioSource.clip = settings.clip;
			output.audioSource.volume = settings.volume;
			output.audioSource.pitch = settings.pitch;
			output.audioSource.Play();
			GameManager.GetSingleton<ObjectPool>().DelayDespawn (output.prefabIndex, output.gameObject, output.trs, settings.clip.length);
			soundEffects = soundEffects.Add(output);
			return output;
		}
	}
}