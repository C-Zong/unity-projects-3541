using UnityEngine;

public class CollisionEffects : MonoBehaviour
{
  public AudioClip clip;
  private AudioSource source;
  public Material[] skyboxes;
  private int currentIndex = 0;

  // The following code was adapted with the help of an AI tool.
  // Prompt asked: "How to play a sound effect on collision and how to switch skybox in Unity script"
  // AI suggested adding an AudioSource in Start() and using RenderSettings.skybox for skybox switching.
  void Start()
  {
    source = gameObject.AddComponent<AudioSource>();
    RenderSettings.skybox = skyboxes[0];
  }

  private void OnCollisionEnter(Collision collision)
  {
    source.PlayOneShot(clip);
    currentIndex = (currentIndex + 1) % skyboxes.Length;
    RenderSettings.skybox = skyboxes[currentIndex];
  }
}
