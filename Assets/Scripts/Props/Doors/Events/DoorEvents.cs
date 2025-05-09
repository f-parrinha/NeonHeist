using UnityEngine;

namespace World.Props.Events
{


    public class DoorEvents : MonoBehaviour
    {

        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;
        [SerializeField] private AudioSource audioSource;

        public void OnOpen()
        {

            audioSource.Stop();
            audioSource.clip = openSound;
            audioSource.Play();
        }

        public void OnClose()
        {

            audioSource.Stop();
            audioSource.clip = closeSound;
            audioSource.Play();
        }
    }
}