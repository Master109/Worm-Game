﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Worms;
using Extensions;

[Serializable]
public class Timer
{
	public float duration;
	public float timeRemaining;
	float timeElapsed;
	public float TimeElapsed
	{
		get
		{
			return timeElapsed;
		}
	}
	public bool loop;
	public delegate void OnFinished (params object[] args);
	public event OnFinished onFinished;
	public object[] args;
	[HideInInspector]
	public bool pauseIfCan;
	Coroutine timerRoutine;
	public bool realtime;
	public bool canBePaused = true;
	public FinishAction finishAction;
	public static Timer[] runningInstances = new Timer[0];

	public virtual void Start ()
	{
		if (timerRoutine == null)
			timerRoutine = GameManager.GetSingleton<GameManager>().StartCoroutine(TimerRoutine ());
	}

	public virtual void Stop ()
	{
		if (timerRoutine != null)
		{
			GameManager.GetSingleton<GameManager>().StopCoroutine(timerRoutine);
			timerRoutine = null;
			runningInstances = runningInstances.Remove(this);
		}
	}

	public virtual IEnumerator TimerRoutine ()
	{
		runningInstances = runningInstances.Add(this);
		bool justEnded;
		while (true)
		{
			justEnded = false;
			if (!canBePaused || !pauseIfCan)
			{
				if (realtime)
				{
					timeRemaining -= Time.unscaledDeltaTime;
					timeElapsed += Time.unscaledDeltaTime;
				}
				else
				{
					timeRemaining -= Time.deltaTime;
					timeElapsed += Time.deltaTime;
				}
			}
			while (timeRemaining <= 0)
			{
				yield return null;
				if (onFinished != null)
					onFinished (args);
				if (loop)
					timeRemaining += duration;
				else if (finishAction == FinishAction.Stop)
					Stop ();
				else if (finishAction == FinishAction.Reset)
					Reset ();
				justEnded = true;
			}
			if (!justEnded)
				yield return null;
		}
	}

	public virtual void Reset ()
	{
		Stop ();
		timeRemaining = duration;
		timeElapsed = 0;
	}

	public enum FinishAction
	{
		Nothing,
		Stop,
		Reset
	}
}
