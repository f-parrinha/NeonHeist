using UnityEngine;

namespace Core.Utilities
{
    public class AudioUtils
    {
        public const float BASE_PITCH = 1f;


        /// <summary>
        /// Create an audio source with AudioClip. It also plays the AudioClip
        /// <para> The AudioSource is not loopable </para>
        /// </summary>
        /// <param name="position"> position to spawn at </param>
        /// <param name="clip"> audio clip to be played </param>
        /// <param name="destroyTimer"> time for self destruction </param>
        /// <param name="spatialBlend"> from 2D to 3D</param>
        /// <returns> AudioSource with AudioClip </returns>
        public static AudioSource CreateAudio(Vector3 position, AudioClip clip, float destroyTimer = 1, float pitch = BASE_PITCH, float spatialBlend = 1)
        {
            GameObject obj = new GameObject($"{clip.name}");
            GameObject.Destroy(obj, destroyTimer);
            obj.transform.position = position;

            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = spatialBlend;
            source.pitch = pitch;
            source.loop = false;
            source.Play();
            return source;
        }

        /// <summary>
        /// Create an audio source with no AudioClip
        /// <para> The AudioSource is not loopable </para>
        /// </summary>
        /// <param name="position"> position to spawn at </param>
        /// <param name="destroyTimer"> time for self destruction </param>
        /// <param name="spatialBlend"> from 2D to 3D</param>
        /// <returns> new AudioSource with no AudioClip </returns>
        public static AudioSource CreateAudioWithoutClip(Vector3 position, float destroyTimer = 1, float pitch = BASE_PITCH, float spatialBlend = 1)
        {
            GameObject obj = new GameObject($"Generic AudioSource");
            GameObject.Destroy(obj, destroyTimer);
            obj.transform.position = position;

            AudioSource source = obj.AddComponent<AudioSource>();
            source.spatialBlend = spatialBlend;
            source.pitch = pitch;
            source.loop = false;
            return source;
        }
    }
}