"D:\Unity3D\Editor\Unity.exe" -batchmode -quit -projectPath %~dp0 -executeMethod BuildUnityPlayer.PerformBuild -androidSdkPath "$ANDROID_HOME" -buildOutput "D:\Unity3D\Unity_Projects\AlphaBT\alphaBT.apk" -buildTarget Android -logFile "D:\Unity3D\Unity_Projects\AlphaBT\log.txt"
pause


"D:\Unity3D\Editor\Unity.exe" -batchmode -quit -projectPath %~dp0 -executeMethod BuildUnityPlayer.PerformBuild -androidSdkPath "$ANDROID_HOME" -buildOutput "D:\Unity3D\Unity_Projects\AlphaBT\alphaBT.apk" -buildTarget Android -logFile "D:\Unity3D\Unity_Projects\AlphaBT\log.txt"
pause