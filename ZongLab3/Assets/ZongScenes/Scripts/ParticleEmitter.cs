using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/**
 *  Control the particle emitter, particles, and colliders
 */
public class ParticleEmitter : MonoBehaviour
{
    /**
     *  Particle Variables
     */
    public Material particleMaterial;
    public float particleRadius = 0.1f;
    public float restitution = 0.2f;
    IList<Particle> particles = new List<Particle>();
    Queue<Particle> deadParticlesQueue = new Queue<Particle>();

    /**
     *  Emitter Variables
     */
    public float emitterRadius = 1.0f;
    public int emissionRate = 10;
    // Both emitter movement speed and initial particle speed range
    public float initialSpeedRange = 1.0f;
    public GameObject emitter;
    Vector3 position;
    Vector3 moveInput;
    Vector2 rotationInput;

    /**
     *  Environment Variables
     */
    public float gravity = -9.8f;
    public float floorY = -3.0f;
    public float colliderHalfSize = 2.0f;
    public float colliderHeight = 0.0f;
    public GameObject[] colliderObjects;
    ColliderType colliderType = ColliderType.SPHERE;

    void Start()
    {
        position = new Vector3(0, 6, 0);
        emitter.transform.position = position + new Vector3(0, 1.5f, 0);
        InitializeColliders();
        InitializeParticle();
    }

    void Update()
    {
        UpdateEmitterPosition();
        UpdateColliderRotation();
        GenerateParticles();
        foreach (Particle p in particles)
        {
            p.Update(Time.deltaTime);
        }
        UpdateParticleCollisions();
    }

    // Initialize collider positions and sizes
    void InitializeColliders()
    {
        foreach (GameObject obj in colliderObjects)
        {
            obj.transform.position = new Vector3(0, colliderHeight, 0);
            obj.transform.localScale = Vector3.one * colliderHalfSize * 2;
            if (obj != colliderObjects[0])
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
            GetComponent<PlayerInput>().actions["RotateCollider"].Disable();
        }
    }

    // Initialize particle properties
    void InitializeParticle()
    {
        Particle.gravity = gravity;
        Particle.restitution = restitution;
        Particle.radius = particleRadius;
    }

    // Generate new particles at the emitter position
    void GenerateParticles()
    {
        for (int i = 0; i < emissionRate; i++)
        {
            GameObject aSphere;
            if (deadParticlesQueue.Count > 0)
            {
                aSphere = deadParticlesQueue.Dequeue().particle;
                aSphere.SetActive(true);
            }
            else
            {
                aSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                aSphere.transform.parent = transform;
                aSphere.GetComponent<Renderer>().material = particleMaterial;
            }
            float theta = Random.Range(0f, Mathf.PI * 2f);
            float r = emitterRadius * Mathf.Sqrt(Random.value);
            float x = position.x + r * Mathf.Cos(theta);
            float z = position.z + r * Mathf.Sin(theta);
            aSphere.transform.position = new Vector3(x, Random.Range(position.y - 0.1f, position.y), z);
            aSphere.transform.localScale = Vector3.one * particleRadius * 2f;
            Particle p = new Particle(aSphere, new Vector3(Random.Range(-initialSpeedRange, initialSpeedRange), 0, Random.Range(-initialSpeedRange, initialSpeedRange)));
            particles.Add(p);
        }
    }

    // Update particle collisions with floor and collider objects
    void UpdateParticleCollisions()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            Particle p = particles[i];
            if (p.particle.transform.position.y - particleRadius < floorY)
            {
                Particle particle = p.FloorCollision(floorY);
                if (particle != null)
                {
                    particles.RemoveAt(i);
                    i--;
                    deadParticlesQueue.Enqueue(particle);
                }
            }
            else
            {
                p.ColliderCollision(colliderType, colliderObjects[(int)colliderType], colliderHalfSize);
            }
        }
    }

    // Emitter movement input handlers
    void OnMove(InputValue move)
    {
        Vector2 movement = move.Get<Vector2>();
        moveInput.x = movement.x;
        moveInput.z = movement.y;
    }

    // Emitter vertical movement input handler
    void OnUpDown(InputValue move)
    {
        float vertical = move.Get<float>();
        moveInput.y = vertical;
    }

    // Update emitter position based on input
    void UpdateEmitterPosition()
    {
        position += moveInput * initialSpeedRange * Time.deltaTime;
        emitter.transform.position = position + new Vector3(0, 1.5f, 0);
    }

    // Change collider type input handler
    void OnChangeCollider()
    {
        int previousType = (int)colliderType;
        int type = ((int)colliderType + 1) % System.Enum.GetNames(typeof(ColliderType)).Length;
        colliderObjects[previousType].SetActive(false);
        colliderObjects[type].SetActive(true);
        colliderType = (ColliderType)type;
        if (type == 0)
        {
            GetComponent<PlayerInput>().actions["RotateCollider"].Disable();
        }
        else
        {
            GetComponent<PlayerInput>().actions["RotateCollider"].Enable();
        }
    }

    // Rotate collider input handler
    void OnRotateCollider(InputValue rotate)
    {
        rotationInput = rotate.Get<Vector2>();
    }

    // Update collider rotation based on input
    void UpdateColliderRotation()
    {
        colliderObjects[(int)colliderType].transform.Rotate(Vector3.up, rotationInput.x * 100 * Time.deltaTime, Space.World);
        colliderObjects[(int)colliderType].transform.Rotate(Vector3.right, rotationInput.y * 100 * Time.deltaTime, Space.World);
    }
}
