# CityEngine-ArCore-Unity
## CityEngine Augmented Reality template for Unity ARCore
###### Solution for Visualizing CityEngine Design Scenario's via augmented reality on consumer mobile devices.

**Supported Devices**
- [Android Smartphones and Tablets](https://developers.google.com/ar/discover/supported-devices)
- _iOS Smartphones and Tablet support planned for future release_

**[Unity](https://unity3d.com/get-unity/download) Compatibility:**
- Release 2018.2.6 with Android Build Support

[How-To Video](https://youtu.be/-hGFLRV4bM8)
[![How-To Video](https://img.youtube.com/vi/-hGFLRV4bM8/0.jpg)](https://www.youtube.com/watch?v=-hGFLRV4bM8)

Installation Methods:

Option 1.) [Latest CityEngine AR Unity Package](https://esri.box.com/v/CityEngineAR) **Traditional Users**
- Required Dependencies:
- [Java Development Kit (JDK)](http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html)
- Android SDK 7.0 (API Level 24) or later installed via [Android Studio](https://developer.android.com/studio/)
- Android Device with [Developer Options](https://developer.android.com/studio/debug/dev-options#enable) enabled. 
- ARCore [Instant Preview](https://developers.google.com/ar/develop/unity/instant-preview)

Option 2.) Download Latest GitHub Repo. **Developers Only**
- Require Dependencies:
- [GitLFS](https://git-lfs.github.com/) **To Successfully Download all files**
- [Git](https://git-scm.com/)
- [Java Development Kit (JDK)](http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html)
- Android SDK 7.0 (API Level 24) or later installed via [Android Studio](https://developer.android.com/studio/)
- Android Device with [Developer Options](https://developer.android.com/studio/debug/dev-options#enable) enabled. 
- ARCore [Instant Preview](https://developers.google.com/ar/develop/unity/instant-preview)

Known Limitations/Issues:
- ArCore Instant Preview only supported on **Windows (discrete GPU's only)** and **macOS** development machines. On windows machines with an integrated GPU, the Unity Editor's video feed is not mirrored back to your phone.
- macOS Development machines should use Metal for rendering instead of OpenGL in Unity. **Player Settings > Settings for PC, Mac and Linux Standalone > Other Settings > Metal Editor Support**
