using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ThreadingUtilities
{
	public class CoroutineWithData
	{
		public Coroutine coroutine;
		public object result;
		public IEnumerator target;
		public MonoBehaviour owner;

		public CoroutineWithData ()
		{
		}

		public CoroutineWithData (MonoBehaviour owner, IEnumerator target)
		{
			this.target = target;
			this.owner = owner;
			this.coroutine = owner.StartCoroutine(Run ());
		}
	 
		public virtual IEnumerator Run ()
		{
			while (target.MoveNext())
			{
				result = target.Current;
				yield return result;
			}
	    }
	}

	public class CoroutineWithData<T> : CoroutineWithData
	{
	}

	public class WaitWhile : CustomYieldInstruction
	{
		Func<bool> predicate;
		public override bool keepWaiting
		{
			get
			{
				return predicate();
			}
		}
		public WaitWhile (Func<bool> predicate)
		{
			this.predicate = predicate;
		}
	}

	public class WaitForReturnedValueOfType : CustomYieldInstruction
	{
		CoroutineWithData cd;
		Type type;
		public override bool keepWaiting
		{
			get
			{
				return cd.result.GetType() != type;
			}
		}
		public WaitForReturnedValueOfType (CoroutineWithData cd, Type type)
		{
			this.cd = cd;
			this.type = type;
		}
	}
	
	public class DoActionAfterCoroutineReturnsValue : CoroutineWithData
	{
		public Action action;

		public DoActionAfterCoroutineReturnsValue (Action action, MonoBehaviour owner, IEnumerator target) : base (owner, target)
		{
			this.action = action;
		}

		public override IEnumerator Run ()
		{
			while (target.MoveNext())
			{
				result = target.Current;
				yield return result;
				action();
			}
		}
	}
	
	public class DoActionAfterCoroutineReturnsValueOfType<T> : CoroutineWithData
	{
		public Action<T> action;

		public DoActionAfterCoroutineReturnsValueOfType (Action<T> action, MonoBehaviour owner, IEnumerator target) : base (owner, target)
		{
			this.action = action;
		}

		public override IEnumerator Run ()
		{
			while (target.MoveNext())
			{
				result = target.Current;
				yield return result;
				if (result is T)
					action((T) result);
			}
		}
	}
	
	// public class PassInValueToFuncAfterCoroutineReturnsValueOfType<T> : CoroutineWithData
	// {
	// 	public Func<T> function;

	// 	public PassInValueToFuncAfterCoroutineReturnsValueOfType (CoroutineWithData cd, Func<T> function) : base(cd.owner, cd.target)
	// 	{
	// 		this.function = function;
	// 	}

	// 	public override IEnumerator Run ()
	// 	{
	// 		while (target.MoveNext())
	// 		{
	// 			result = target.Current;
	// 			yield return result;
	// 			if (result is T)
	// 		}
	// 	}
	// }
}