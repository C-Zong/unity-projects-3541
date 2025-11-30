Unity Lab7 - CSE 3541 Coursework

Name: Chenyang Zong 🧑‍💻

Unity version: 6000.2.7f2 🟢
Render Pipeline: Universal

🎮 User Input

Target Sphere Movement
Use the following keys to move the target sphere in 3D space:
A / D → Move Left / Right
Q / E → Move Up / Down
W / S → Move Forward / Back

Camera Controls
Move and rotate the camera while it always looks toward (0, 0, 0):
J / L → Rotate Clockwise / Counter-clockwise around the Y-axis (top-down view)
U / O → Move Camera Up / Down
I / K → Move Camera Away from / Toward the Y-axis

------------------
Unity Editor Part
------------------

Target (Script)
Speed — Movement speed of the target sphere

Main Camera (Script)
Speed — Camera movement speed
Rotation Speed — Speed of rotating the camera around the Y-axis

Robot (Script)
Speed — IK iteration speed
Threshold — Allowed distance tolerance between the end effector and the target before IK stops iterating
Initial Target Position — Initial position of the target sphere
Initial Joint Positions — A list of Vector3 positions used to initialize the arm's joints (ordered from root to end)
Target — Reference to the target object (the sphere).
Joint/Arm Prefab — Prefabs used to instantiate each joint and arm segment.

🏗️ Features Implemented

Planning - submitting a proposal on time [1 point]

Marketing - use the Unity Web Player to make your finished lab playable online [1 point]
https://play.unity.com/en/games/e59d8ca7-65e3-4b58-ac74-c56db23871d2/interactive-3d-robotic-arm-ccd-ik

Implement an inverse kinematics algorithm.

------------------
Details
------------------

FKManager (Script)
Initialize the forward kinematics hierarchy, creating joints and arm segments based on the provided joint positions.

IKManager (Script)
Perform a single iteration of the CCD inverse kinematics algorithm to move the end effector toward the target.

MovementManager (Script)
Expose public parameters in the Inspector and control initialization, user input handling, and per-frame update.

Target/Camera Controller (Script)
Handle user input and update the movement of the target sphere/camera.

📑 Notes on Development

Parts of the code were adapted with the help of AI tools.