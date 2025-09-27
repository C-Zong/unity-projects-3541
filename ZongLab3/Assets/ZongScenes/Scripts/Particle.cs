using UnityEngine;

/**
 *  Handle individual particle behavior
 */
public class Particle
{
  public static float gravity;
  public static float restitution;
  public static float radius;
  public GameObject particle;
  Vector3 velocity;
  enum ParticleColor { RED, ORANGE, YELLOW };
  ParticleColor color;

  public Particle(GameObject par, Vector3 v)
  {
    particle = par;
    velocity = v;
    color = ParticleColor.RED;
  }

  public void Update(float deltaTime)
  {
    Vector3 acceleration = gravity * Vector3.up;
    Vector3 preVelocity = velocity;
    velocity = velocity + acceleration * deltaTime;
    particle.transform.position = particle.transform.position + (velocity + preVelocity) / 2 * deltaTime;
  }

  // Check and handle collision with the floor
  public Particle FloorCollision(float floorY)
  {
    Particle p = null;
    if (particle.transform.position.y - radius < floorY)
    {
      if (color == ParticleColor.RED)
      {
        color = ParticleColor.ORANGE;
        particle.GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
        FloorCollisionResponse(floorY);
      }
      else if (color == ParticleColor.ORANGE)
      {
        color = ParticleColor.YELLOW;
        particle.GetComponent<Renderer>().material.color = Color.yellow;
        FloorCollisionResponse(floorY);
      }
      else
      {
        particle.GetComponent<Renderer>().material.color = Color.red;
        particle.SetActive(false);
        p = this;
      }
    }
    return p;
  }

  // Update particle position and velocity after floor collision
  void FloorCollisionResponse(float floorY)
  {
    Vector3 pos = particle.transform.position;
    pos.y = floorY + radius;
    particle.transform.position = pos;
    velocity.y = -restitution * velocity.y;
  }

  // Check and handle collision with a collider object
  public void ColliderCollision(ColliderType type, GameObject collider, float colliderSize)
  {
    switch (type)
    {
      case ColliderType.SPHERE:
        Vector3 dir = particle.transform.position - collider.transform.position;
        float minDist = radius + colliderSize;
        if (dir.sqrMagnitude < minDist * minDist)
        {
          /** This part of code is adapted from AI. */
          float dist = dir.magnitude;
          Vector3 n = dist > 0f ? dir / dist : Vector3.up;
          float penetration = minDist - dist;
          if (penetration > 0f)
          {
            particle.transform.position += n * penetration;
            float vn = Vector3.Dot(velocity, n);
            if (vn < 0f)
            {
              velocity = velocity - (1f + restitution) * vn * n;
            }
          }
          /** End of adapted code */
        }
        break;
      case ColliderType.CUBE:
        /** This part of code is changed from AI code. */
        Vector3 localPoint = collider.transform.InverseTransformPoint(particle.transform.position);
        Vector3 halfSize = Vector3.one * colliderSize;
        if (IsInsideCube(localPoint, halfSize))
        {
          Vector3 localClosestPoint = new Vector3(
              Mathf.Clamp(localPoint.x, -0.5f, 0.5f),
              Mathf.Clamp(localPoint.y, -0.5f, 0.5f),
              Mathf.Clamp(localPoint.z, -0.5f, 0.5f)
          );
          Vector3 closestPoint = collider.transform.TransformPoint(localClosestPoint);
          Vector3 dirToParticle = particle.transform.position - closestPoint;
          float distToParticle = dirToParticle.magnitude;
          Vector3 n = distToParticle > 0f ? dirToParticle / distToParticle : Vector3.up;
          particle.transform.position = closestPoint + n * radius;
          float vn = Vector3.Dot(velocity, n);
          if (vn < 0f)
          {
            velocity -= (1f + restitution) * vn * n;
          }
        }
        /** End of adapted code */
        break;
    }
  }

  // Check if a point is inside the cube collider
  bool IsInsideCube(Vector3 localPoint, Vector3 halfSize)
  {
    return Mathf.Abs(localPoint.x * halfSize.x * 2) <= halfSize.x + radius &&
           Mathf.Abs(localPoint.y * halfSize.y * 2) <= halfSize.y + radius &&
           Mathf.Abs(localPoint.z * halfSize.z * 2) <= halfSize.z + radius;
  }
}
