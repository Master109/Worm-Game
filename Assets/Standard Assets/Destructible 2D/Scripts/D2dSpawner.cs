using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;

namespace Destructible2D
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(D2dSpawner))]
	public class D2dSpawner_Editor : D2dEditor<D2dSpawner>
	{
		protected override void OnInspector()
		{
			DrawDefault("delay", "This allows you to control the minimum amount of time between prefab creation in seconds.");

			Separator();
			
			BeginError(Any(t => t.Prefab == null));
				DrawDefault("prefab", "If you want a prefab to spawn at the impact point, set it here.");
			EndError();
		}
	}
}
#endif

namespace Destructible2D
{
	/// <summary>This component allows you to spawn the specified prefab by manually calling the Spawn method.</summary>
	[HelpURL(D2dHelper.HelpUrlPrefix + "D2dSpawner")]
	[AddComponentMenu(D2dHelper.ComponentMenuPrefix + "Spawner")]
	public class D2dSpawner : MonoBehaviour
	{
		/// <summary>This allows you to control the minimum amount of time between prefab creation in seconds.</summary>
		public float Delay { set { delay = value; } get { return delay; } } [SerializeField] private float delay = 1.0f;

		/// <summary>If you want a prefab to spawn at the impact point, set it here.</summary>
		public GameObject Prefab { set { prefab = value; } get { return prefab; } } [SerializeField] private GameObject prefab;

		protected virtual void Update()
		{
			if (delay > 0.0f)
			{
				delay -= Time.deltaTime;

				if (delay <= 0.0f)
				{
					delay = 0.0f;

					Spawn();
				}
			}
		}

		public void Spawn()
		{
			if (prefab != null)
			{
				var clone = Instantiate(prefab, transform.position, transform.rotation);

				clone.SetActive(true);
			}
		}
	}
}