# About
This is an example Unity project demonstrating [VRPhysicsHands](https://github.com/oxters168/VRPhysicsHands)
and [PhysicsGadgets](https://github.com/oxters168/PhysicsGadgets). To learn more about each, visit their respective
github pages. This project was created in Unity 2019.2.19f1.

![Switch](https://i.imgur.com/iTsknER.gif)
![Button](https://i.imgur.com/irIqI4S.gif)
![Joystick](https://i.imgur.com/W71BM3X.gif)
![Lever](https://i.imgur.com/vZY9L3A.gif)

# Installation
You need an Oculus Quest to install. Head over to the releases and download the latest apk. Plug in your Oculus Quest
to your computer. Then you can install through adb commands, or more easily through [SideQuest](https://sidequestvr.com).
On SideQuest there is a button on the top right corner of the window that lets you pick an apk to install to your headset.
Once pressed a popup dialog will appear. Navigate to where you downloaded the apk and choose it. After it's done installing, you'll find 'HandedHands' in your Oculus Quest apps under 'Unknown Sources'.

# How to use this Repository
If you're looking to just use the hands or gadgets in your current project, it's better to bring them in individually rather than through this repository. They don't have all the Unity project files clutter, so they can be brought in like a package. This repository has some dependencies. To be able to open this project in Unity, you will need some extra things after downloading/cloning. It's better to go down the cloning path as there seems to be an [issue](https://github.com/git-lfs/git-lfs/issues/903) with lfs files not downloading when clicking download on github. Here are the steps to take in order to get everything working:

1. First download or clone this repository.

1. Make sure to include the submodules when cloning.

1. If you're downloading then you'll need to download these submodules manually ([VRPhysicsHands](https://github.com/oxters168/VRPhysicsHands), [PhysicsGadgets](https://github.com/oxters168/PhysicsGadgets), [UnityHelpers](https://github.com/oxters168/UnityHelpers)) and extract them in their proper folders inside 'Assets/Submodules/'. UnityHelpers is a bit more tricky since it has another submodule inside it called [geometry3Sharp](https://github.com/gradientspace/geometry3Sharp/tree/79829341d6c225375128c32cd4720dd48f970c6e). Make sure to extract it inside the geometry3Sharp folder inside UnityHelpers.

1. Open the project with Unity by clicking 'Add' in Unity Hub then navigating to the master folder and choosing it. (there are going to be errors, don't worry)

1. Click 'Window > TextMeshPro > Import TMP Essential Resources' then when the import popup appears, click import.

1. Import [Oculus Integration](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022) provided by Oculus from the Asset Store. After it's done importing click yes to update 'Oculus Utilities Plugin', click 'Restart', click 'Upgrade' for the 'Update Spatializer Plugins' pop up, and finally click 'Restart' again.

1. Import [Classic Skybox](https://assetstore.unity.com/packages/2d/textures-materials/sky/classic-skybox-24923) provided by mgsvevo from the Asset Store. (Move it to the Imported folder for my sanity)

1. Open build settings and switch the platform to Android.

1. Go to 'Player Settings':
   1. Under 'Other Settings' make sure the minimum api level is set to 19 (If you can't set this, delete the ProjectSettings.asset file or back it up)
   1. Under 'XR Settings' add Oculus to Virtual Reality SDKs and enable V2 Signing.

1. Open the HandsTest scene and click on OVRCameraRig and make sure that 'Hand Tracking Support' is set to 'Hands Only'.

And that should be it, enjoy!
