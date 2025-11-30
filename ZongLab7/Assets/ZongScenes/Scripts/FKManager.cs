using UnityEngine;

// Manage forward kinematics setup
public class FKManager : MonoBehaviour
{
  // Set up a hierarchy of joints and arms for forward kinematics
  public static GameObject[] SetupFK(GameObject jointPrefab, GameObject armPrefab, Vector3[] jointPositions)
  {
    int count = jointPositions.Length;
    GameObject[] joints = new GameObject[count];

    joints[0] = Instantiate(jointPrefab, jointPositions[0], Quaternion.identity);
    joints[0].name = "Joint_" + 0;

    // Create arms and joints
    for (int i = 0; i < count - 1; i++)
    {
      Vector3 start = jointPositions[i];
      Vector3 end = jointPositions[i + 1];
      Vector3 dir = end - start;
      float length = dir.magnitude;

      GameObject arm = Instantiate(armPrefab, start, Quaternion.identity);
      arm.name = "Arm_" + i;
      arm.transform.parent = joints[i].transform;

      arm.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

      Transform armTransform = arm.transform.Find("Cylinder");
      Vector3 s = armTransform.localScale;
      s.y = length / 2.0f;
      armTransform.localScale = s;
      armTransform.localPosition = new Vector3(0, length / 2.0f, 0);

      joints[i + 1] = Instantiate(jointPrefab, jointPositions[i + 1], Quaternion.identity);
      joints[i + 1].name = "Joint_" + (i + 1);
      joints[i + 1].transform.parent = arm.transform;
      joints[i + 1].transform.localPosition = new Vector3(0, length, 0);
    }
    joints[count - 1].SetActive(false);

    return joints;
  }
}
