using UnityEngine;

// Manage inverse kinematics for a robotic arm
public class IKManager
{
    Vector3 targetPos;
    GameObject[] joints;
    int currentIndex;
    float threshold = 0.01f;

    public IKManager(Vector3 targetPosition, GameObject[] jointObjects)
    {
        targetPos = targetPosition;
        joints = jointObjects;
        currentIndex = jointObjects.Length - 2;
    }

    // Move the arms to point towards the target position (CCD algorithm)
    public void moveArmsTowardsTarget(Vector3 targetPosition)
    {
        if (currentIndex < 0 || Vector3.Distance(targetPos, targetPosition) >= threshold)
        {
            currentIndex = joints.Length - 2;
        }
        targetPos = targetPosition;

        Vector3 toTarget = targetPos - joints[currentIndex].transform.position;
        Vector3 toEnd = joints[joints.Length - 1].transform.position - joints[currentIndex].transform.position;
        Quaternion rot = Quaternion.FromToRotation(toEnd, toTarget);
        joints[currentIndex].transform.rotation = rot * joints[currentIndex].transform.rotation;

        currentIndex--;
    }
}
