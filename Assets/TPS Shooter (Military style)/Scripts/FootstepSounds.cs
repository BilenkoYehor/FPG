using UnityEngine;

namespace TPSShooter
{

    [RequireComponent(typeof(AudioSource))]
    public class FootstepSounds : MonoBehaviour
    {
        public LayerMask layers;
        public TextureType[] textureTypes;

        AudioSource audioSource;

        // Use this for initialization
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void PlayFootstepSound()
        {
            RaycastHit hit;
            Vector3 start = transform.position + transform.up;
            Vector3 dir = Vector3.down;

            if (Physics.Raycast(start, dir, out hit, 1.3f, layers))
            {
                if (hit.collider.GetComponent<MeshRenderer>())
                {
                    PlayMeshSound(hit.collider.GetComponent<MeshRenderer>());
                }
                else if (hit.collider.GetComponent<Terrain>())
                {
                    PlayTerrainSound(hit.collider.GetComponent<Terrain>(), hit.point);
                }
            }
        }

        /// <summary>
        /// This method called from animations via AnimationEvents
        /// </summary>
        void PlayMeshSound(MeshRenderer r)
        {
            if (textureTypes.Length > 0)
            {
                foreach (TextureType type in textureTypes)
                {

                    if (type.footstepSounds.Length == 0)
                    {
                        return;
                    }

                    foreach (Texture tex in type.textures)
                    {
                        if (r.material.mainTexture == tex)
                        {
                            PlaySound(audioSource, type.footstepSounds[Random.Range(0, type.footstepSounds.Length)], true, 1, 1.05f);
                        }
                    }
                }
            }
        }

        void PlayTerrainSound(Terrain t, Vector3 hitPoint)
        {
            if (textureTypes.Length > 0)
            {

                int textureIndex = TerrainSurface.GetMainTexture(hitPoint);

                foreach (TextureType type in textureTypes)
                {

                    if (type.footstepSounds.Length == 0)
                    {
                        return;
                    }

                    foreach (Texture tex in type.textures)
                    {
                        if (t.terrainData.splatPrototypes[textureIndex].texture == tex)
                        {
                            PlaySound(audioSource, type.footstepSounds[Random.Range(0, type.footstepSounds.Length)], true, 1, 1.2f);
                        }
                    }
                }
            }
        }

        void PlaySound(AudioSource audioS, AudioClip clip, bool randomizePitch = false, float randomPitchMin = 1, float randomPitchMax = 1)
        {

            audioS.clip = clip;

            if (randomizePitch == true)
            {
                audioS.pitch = Random.Range(randomPitchMin, randomPitchMax);
            }

            audioS.Play();
        }
    }

    [System.Serializable]
    public class TextureType
    {
        public string name;
        public Texture[] textures;
        public AudioClip[] footstepSounds;
    }

}