using UnityEngine;

// Manage the movement of the robotic arm towards a target point
public class MovementManager : MonoBehaviour
{
    public float speed = 1.0f;
    public float threshold = 0.01f;
    public Vector3 initialTargetPosition;
    public Vector3[] initialJointPositions;
    public GameObject target;
    public GameObject jointPrefab;
    public GameObject armPrefab;

    GameObject[] joints;
    IKManager ikManager;
    float timeElapsed = 0.0f;

    // Set up target point and robotic arm
    void Start()
    {
        target.transform.position = initialTargetPosition;
        joints = FKManager.SetupFK(jointPrefab, armPrefab, initialJointPositions);
        joints[0].transform.parent = this.transform;

        ikManager = new IKManager(target.transform.position, joints);
    }

    // Call IK solver periodically to move the arm towards the target
    void Update()
    {
        timeElapsed += Time.deltaTime * speed;

        if (timeElapsed > 1.0f)
        {
            timeElapsed = 0.0f;

            Vector3 targetPos = target.transform.position;
            Vector3 endPos = joints[joints.Length - 1].transform.position;
            if (Vector3.Distance(endPos, targetPos) > threshold)
            {
                ikManager.moveArmsTowardsTarget(targetPos);
            }
        }
    }
}
