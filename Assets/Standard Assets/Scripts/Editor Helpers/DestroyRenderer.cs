using UnityEngine;

namespace IRLFriend
{
	public class DestroyRenderer : MonoBehaviour
	{
		public virtual void Start ()
		{
			if (Application.isPlaying)
			{
				Destroy(GetComponent<Renderer>());
				Destroy(this);
			}
		}
	}
}