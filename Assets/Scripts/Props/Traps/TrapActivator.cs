using Core.Health.Interfaces;
using UnityEngine;

namespace Props.Traps
{
	public class TrapActivator : MonoBehaviour
	{
		[SerializeField] private Trap trap;
		

        private void OnTriggerEnter(Collider collider)
		{
			trap.Activate();
		}
	}
}