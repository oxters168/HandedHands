# About
This is an example Unity project demonstrating [VRPhysicsHands](https://github.com/oxters168/VRPhysicsHands)
and [PhysicsGadgets](https://github.com/oxters168/PhysicsGadgets). To learn more about each, visit their respective
github pages. This project was created in Unity 2019.2.19f1.

# Installation
You need an Oculus Quest to install. Head over to the releases and download the latest apk. Plug in your Oculus Quest
to your computer. Then you can install through adb commands, or more easily through [SideQuest](https://sidequestvr.com).
On SideQuest there is a button on the top right corner of the window that lets you pick an apk to install to your headset.
Once pressed a popup dialog will appear. Navigate to where you downloaded the apk and choose it. Once installed you can now
find 'HandedHands' in your Oculus Quest apps under 'Unknown Sources'.

# How to use this Repository
If you're looking to just use the hands or gadgets in your current project, it's better to bring them in individually rather than through this repository. They don't have all the Unity project files clutter, so they can be brought in like a package. This repository has some dependencies. To be able to open this project in Unity, you will need some extra things after downloading/cloning. Here are the steps to take in order to get everything working:

- First download or clone this repository
- Make sure to include the submodules when cloning
- If you're downloading then you'll need to also download these submodules (
[VRPhysicsHands](https://github.com/oxters168/VRPhysicsHands), 
[PhysicsGadgets](https://github.com/oxters168/PhysicsGadgets), 
[UnityHelpers](https://github.com/oxters168/UnityHelpers)
) manually and extract them in their proper folders inside 'Assets/Submodules/'.
UnityHelpers is a bit more tricky since it has another submodule inside it called 
[geometry3Sharp](https://github.com/gradientspace/geometry3Sharp/tree/79829341d6c225375128c32cd4720dd48f970c6e).
Make sure to extract it inside the geometry3Sharp folder inside UnityHelpers.
- Open the project with Unity by clicking 'Add' in Unity Hub then navigating to the master folder and choosing it.
(there are going to be errors, don't worry)
- Click 'Window > TextMeshPro > Import TMP Essential Resources' then when the import popup appears, click import.
- Import Oculus Integration provided by Oculus from the Asset Store. After it's done importing click yes to update 'Oculus Utilities Plugin'
if it asks you to, click 'Restart' if it asks, click 'Upgrade' for the 'Update Spatializer Plugins' pop up, and finally click
'Restart' again.
- Import Classic Skybox provided by MGSVEVO from the Asset Store. (Move it to the Imported folder for my sanity)
- Open player settings and switch the platform to Android.
- Go into the player settings and under XR settings add Oculus to Virtual Reality SDKs and enable V2 Signing.
- Right click on Assets and select 'Reimport All'
- Open the HandsTest scene and that's it!

That's how it should be done at least, but I still haven't been able to get it to work. I'll try to get it working
and update here. Try these steps at your own risk.
