---
title:  "[Unity3D] Blender와 Unity3D의 연동"
excerpt: "Blender에서 제작한 리소스를 Unity3D에 Import하여 사용합니다."
date:   2022-01-21 09:00:00 +0530
categories:
  - Unity3D
tags:
  - Unity3D
  - C#
toc: true
toc_sticky: true
---
유니티는 다양한 플랫폼을 지원하지만, 각 플랫폼별 빌드 환경, 구성 및 절차가 상이합니다. 
유니티는 기본적으로 File > Build Settings(Shift+Ctrl+B)를 통해서 지정된 플랫폼의 빌드가 가능합니다. 

단, AOS의 경우는 마켓 등록을 위해서 빌드 후 Signing(Keystore)의 절차등의 추가적인 과정이 필요하며, iOS의 경우는 이전 빌드된 Xcode의 프로젝트를 Modification방식으로 처리가 필요할 때도 있습니다.

![Build Settings](/assets/images/unity/unity_build_settings.png)

## 1. 빌드 스크립트 생성
유니티 에디터상의 메뉴를 생성하므로 BuildScript.cs 파일을 Assets/Editor폴더에 생성합니다.
생성된 파일에 아래와 같이 기본 코드를 추가합니다.
```cs
public class BuildScript : MonoBehaviour
{
#if UNITY_EDITOR
    private static readonly string[] _scenes;
    private static readonly string _targetDir = "Build";
    private static readonly string _buildName;

    // AppManager.Instance라는 SingletonGameObject를 최초 씬에 생성해서 AppConfig를 Serialize(Field)로 관리해서 사용합니다.
    static BuildScript()
    {
        PlayerSettings.productName = AppManager.Instance.AppConfig.ProductName;
        PlayerSettings.bundleVersion = AppManager.Instance.AppConfig.VersionName;

        _buildName = string.Format("{0}_{1}_{2}", AppManager.Instance.AppConfig.BuildName, AppManager.Instance.AppConfig.VersionName, AppManager.Instance.AppConfig.VersionCode);
        UnityEngine.Debug.Log($"{PlayerSettings.productName}, {PlayerSettings.bundleVersion}");
        _scenes = FindEnabledEditorScenes();
    }

    // File > Build Settings에 등록된 씬을 가져옵니다.
    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            UnityEngine.Debug.Log(string.Format("FindEnabledEditorScenes: {0}", scene.path)); 
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    // File > Build Settings에 등록된 옵션의 정보를 사용합니다.
    private static BuildOptions GetBuildOptions()
    {
        BuildOptions buildOptions = BuildOptions.None;

        if (EditorUserBuildSettings.symlinkLibraries) buildOptions |= BuildOptions.SymlinkLibraries;
        if (EditorUserBuildSettings.development) buildOptions |= BuildOptions.Development;
        if (EditorUserBuildSettings.connectProfiler) buildOptions |= BuildOptions.ConnectWithProfiler;
        if (EditorUserBuildSettings.allowDebugging) buildOptions |= BuildOptions.AllowDebugging;

        UnityEngine.Debug.Log(string.Format("BuildOptions:{0}", buildOptions));

        return buildOptions;
    }

    // % (ctrl on Windows, cmd on OS X), # (shift), & (alt) (ex.%&#p)
    [MenuItem("RE:CUBE/Build/General #%o")]
    static void Build()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneWindows: BuildWin32(); break;
            case BuildTarget.StandaloneWindows64: BuildWin64(); break;
            case BuildTarget.Android: BuildAndroid(); break;
            case BuildTarget.iOS: BuildIPhone(); break;
            case BuildTarget.WebGL: BuildWebGL(); break;
            default:
                {
                    UnityEngine.Debug.LogError(string.Format("not support build target   :{0}", EditorUserBuildSettings.activeBuildTarget));
                    break;
                }

        }
    }
    
    // 타겟 플랫폼과 얻어온 씬의 정보를 통해 빌드를 진행합니다.
    private static bool GenericBuild(string[] scenes, string targetDir, BuildTarget buildTarget, BuildOptions buildOptions)
    {
        UnityEngine.Debug.Log(string.Format("Generic build target dir:{0}", targetDir));

        bool buildSucceed = true;

        if (buildTarget != EditorUserBuildSettings.activeBuildTarget)
        {
            buildSucceed = false;
            UnityEngine.Debug.Log(string.Format("GenericBuild has stopped:{0}", buildTarget));
            return false;
        }

        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(scenes, targetDir, buildTarget, buildOptions);
        UnityEngine.Debug.Log(string.Format("GenericBuild has succeed:{0}", report.summary));
        
        return buildSucceed;
    }

    private static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
#endif
}
```
## 2. 빌드 메뉴 구성
아래와 같이 MenuItem을 활용하면 스샷처럼 유니티 메뉴가 생성되며, 단축키(#%xxx) 지정도 가능합니다.
![Build Menu](/assets/images/unity/unity_build_menu.png)
```cs
[MenuItem("RE:CUBE/Build/General #%o")]
static void Build()
{
  ...
}
```
## 3. 플랫폼별 스크립트 작성
### 3-1. Android
   ```cs
   static void BuildAndroid()
   {
      BuildOptions buildOptions = GetBuildOptions();
      PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, AppManager.Instance.AppConfig.BundleName);

      PlayerSettings.Android.bundleVersionCode = AppManager.Instance.AppConfig.VersionCode;
      PlayerSettings.Android.keyaliasName = AppManager.Instance.AppConfig.KeyaliasName;
      PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass = AppManager.Instance.AppConfig.KeyaliasPass;
      PlayerSettings.statusBarHidden = AppManager.Instance.HiddenStatusBar;

      string _path = Application.dataPath;
      _path = _path.Substring(0, _path.LastIndexOf('/'));

      PlayerSettings.Android.keystoreName = string.Format("{0}/helper/{1}", _path, AppManager.Instance.AppConfig.KeystoreName);

      UnityEngine.Debug.Log($"BuildAndroid keystoreName: {PlayerSettings.Android.keystoreName}, keyaliasName: {PlayerSettings.Android.keyaliasName}");

      string _buildTargetPath = BuildPath("android");
      CreateDirectory(_buildTargetPath);

      string _fileName = "";
      if (EditorUserBuildSettings.buildAppBundle)
          _fileName = string.Format("{0}/{1}.aab", _buildTargetPath, _buildName);
      else
          _fileName = string.Format("{0}/{1}.apk", _buildTargetPath, _buildName);

      GenericBuild(_scenes, _fileName, BuildTarget.Android, buildOptions);
   }
   ```

### 3-2. iOS
   ```cs
   static void BuildIPhone()
   {
       BuildOptions buildOptions = GetBuildOptions();
       PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, AppManager.Instance.AppConfig.BundleName);

       // 최초 빌드시에는 제거한다.(이 옵션의 의미는 Unity에서 프로젝트를 iOS버전으로 빌드할때 현재 이미 빌드된 프로젝트 파일들이 있을 경우 Replace하지 말고 Merge하라는 의미)
       // XCode 프로젝트를 생성하는 첫번째 빌드시에는 제외시켜야 한다.
       if (PlayerPrefs.GetInt("BuiltHistory") == 1)
       {
           buildOptions |= BuildOptions.AcceptExternalModificationsToPlayer;
       }

       PlayerSettings.iOS.buildNumber = AppManager.Instance.AppConfig.VersionCodeString;
       PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
       PlayerSettings.iOS.targetOSVersionString = AppManager.Instance.AppConfig.IOSTargetVersion;
       PlayerSettings.statusBarHidden = AppManager.Instance.HiddenStatusBar;
       //PlayerSettings.iOS.allowHTTPDownload = true;

       string _buildTargetPath = BuildPath("ios");
       CreateDirectory(_buildTargetPath);

       GenericBuild(_scenes, _buildTargetPath, BuildTarget.iOS, buildOptions);

       PlayerPrefs.SetInt("BuiltHistory", 1);
       PlayerPrefs.Save();
   }

   // iOS의 경우 PostProcess를 통해 Xcode의 속성값 변경등에 활용할 수 있습니다.
   [PostProcessBuildAttribute(1)]
   public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
   {
       if (buildTarget != BuildTarget.iOS) 
           return;
   }
   ```

### 3-3. Web
   ```cs
   static void BuildWebGL()
   {
       BuildOptions buildOptions = GetBuildOptions();
       PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, AppManager.Instance.AppConfig.BundleName);

       // 아래 설정은 프로젝트 Publishing Settings에서 처리
       //PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip; // 가급적 모든 브라우저에서 http, https를 지원하기 위함
       //PlayerSettings.WebGL.memorySize = 1024;

       string _buildTargetPath = BuildPath("webgl");
       CreateDirectory(_buildTargetPath);

       GenericBuild(_scenes, _buildTargetPath, BuildTarget.WebGL, buildOptions);
   }
   ```

### 3-4. Windows
   ```cs
   static void BuildWin32()
   {
       BuildOptions buildOptions = GetBuildOptions();
       PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, AppManager.Instance.AppConfig.BundleName);

       string _fileName = string.Format("{0}.exe", PlayerSettings.productName);

       string _buildTargetPath = BuildPath("win32");
       CreateDirectory(_buildTargetPath);
       GenericBuild(_scenes, _buildTargetPath + "/" + _fileName, BuildTarget.StandaloneWindows, buildOptions);

       RunProcess(_fileName, "win32");
   }

   static void BuildWin64()
   {
       BuildOptions buildOptions = GetBuildOptions();
       PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, AppManager.Instance.AppConfig.BundleName);

       string _fileName = string.Format("{0}.exe", _buildName);

       string _buildTargetPath = BuildPath("win64");
       CreateDirectory(_buildTargetPath);
       GenericBuild(_scenes, _buildTargetPath + "/" + _fileName, BuildTarget.StandaloneWindows64, buildOptions);

       RunProcess(_fileName, "win64");
   }
   ```

### 3-5. 추가 스크립트
   ```cs
   // GenericBuild이후에 추가 스크립트
   // 윈도우 버전의 빌드 후 실행, 빌드 후 특정 bat, sh파일 실행으로 자동화 추가를 할 수 있습니다.
   private static void RunProcess(string fileName, string subPath)
   {
       // Run after the build
       var _process = new Process();
       _process.StartInfo.FileName = string.Format("{0}/{1}", BuildPath(subPath), fileName);
       _process.Start();
   }
   ```

***[Unity3D 빌드 메뉴](https://docs.unity3d.com/ScriptReference/MenuItem.html)***

***해당 스크립트는 예전에 작성한 부분이라 요즘에는 더 좋은 방법이 있을 듯 하지만, 현재도 잘 사용하고 있기에 공유합니다.***🧐 