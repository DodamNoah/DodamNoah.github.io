---
title:  "[Flutter] Flavor를 이용한 빌드 다양성"
excerpt: "앱을 개발하는 과정에서 약간의 속성 변화를 통해서 다양한 결과물(Dev/Stage/Prod)이 필요한 때가 있다."
date:   2021-11-24 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
toc: true
toc_sticky: true
---
동일 프로젝트내의 특정 속성만을 변경하여 네이티브, Dart 영역의 다른 빌드 결과물을 추출하는 방법을 다루려고 합니다. Git을 사용할 경우 Branch를 통해서도 가능한 부분이지만, 동일 속성 및 몇가지 조건 변화를 위해서 여러개의 Branch를 관리하기에는 관리 이슈가 발생할 수 있습니다.  

이번 포스트는 Flutter 2.2.3 버전으로 진행합니다.

# 1. Android  
Flutter 프로젝트의 android/app 폴더내의 build.gradle 수정   

```
apply plugin: 'com.android.application'
apply from: "$flutterRoot/packages/flutter_tools/gradle/flutter.gradle"

**def flavor**

android {
  sourceSets {
      main.java.srcDirs += 'src/main/kotlin'
      // Flavor별로 AndroidManifest.xml를 다르게 지정할 경우에는 하기와 같이 처리합니다.  
      // 지정하지 않을 경우 app/src/main 폴더에 있는 AndroidManifest.xml 파일이 자동으로 지정됩니다.  
      admin {
          manifest.srcFile('src/admin/AndroidManifest.xml')
      }
  }

  // keystore가 다를 경우에만 지정한다.
  signingConfigs {
      branch {
          keyAlias keystoreProperties['keyAlias']
          keyPassword keystoreProperties['keyPassword']
          storeFile file(keystoreProperties['storeFile'])
          storePassword keystoreProperties['storePassword']
      }
      admin {
          keyAlias keystoreProperties['keyAlias']
          keyPassword keystoreProperties['keyPassword']
          storeFile file(keystoreProperties['storeFile'])
          storePassword keystoreProperties['storePassword']
      }
  }

  flavorDimensions "build-type"
  productFlavors {
      branch {
          dimension "build-type"
          signingConfig signingConfigs.branch // signing이 다를 경우에는 각 signing에 맞게 지정  
          resValue "string", "app_name", "Sample"
      }
      admin {
          dimension "build-type"
          applicationIdSuffix ".admin"
          signingConfig signingConfigs.admin
          resValue "string", "app_name", "Sample Admin"
      }
  }
}

// [Tip] 하기와 같이 처리하면 빌드시 추출되는 apk파일명을 Flavor에 따라서 다르게 처리 가능합니다.  
android.applicationVariants.all { variant ->
    variant.outputs.all {
        def appName = "Sample"
        // def buildType = variant.variantData.variantConfiguration.buildType.name
        def buildType = variant.buildType.name
        def newName
        if (buildType == 'debug'){
            newName = "${appName}_${variant.getFlavorName()}-debug-${variant.versionName}_${variant.versionCode}.apk"
        } else {
            newName = "${appName}_${variant.getFlavorName()}-${variant.versionName}_${variant.versionCode}.apk"
        }
        outputFileName = newName
    }
}
```
# 2. iOS  
# 3. Dart(Optional)  
Dart쪽에서도 Flavor에 따라서 다르게 코드를 진행할 경우에는 하기와 같이 처리합니다.  
일반적으로 main.dart에 있는 내용을 main_common.dart로 옮긴 뒤 각 Flavor에 맞는 main_${Flavor명}.dart를 생성해서 처리합니다.  
이후 UI/UX 구성시 Flavor에 따라서 다르게 처리가 필요할 경우 Environment.buildType을 통해서 분기 처리해 줍니다.  

* Environment 클래스  
```
enum BuildType {
  branch,
  admin
}
class Environment {
  //
  static Environment _instance;
  static Environment get instance => _instance;
  final BuildType _buildType;
  static BuildType get buildType => instance._buildType;
  const Environment._internal(this._buildType);
  factory Environment.newInstance(BuildType buildType) {
    assert(buildType != null);
    if (_instance == null) {
      _instance = Environment._internal(buildType);
    }
    return _instance;
  }

  Future run() async {
    await mainCommon(_buildType);
  }
```   
* main_brach.dart  
```
import 'package:hangul_yaho/services/environment.dart';
Future<void> main() async => Environment.newInstance(BuildType.branch).run();
```   
* main_admin.dart  
```
import 'package:hangul_yaho/services/environment.dart';
Future<void> main() async => Environment.newInstance(BuildType.admin).run();
```   
# 4. Build  
```
flutter build apk --flavor ${flavor명} -t lib/main_${flavor명}.dart
>> flutter build apk --flavor admin -t lib/main_admin.dart
```
# 5. Visual Studio Code(Debug)  
비주얼 스튜티오 코드에서 Flavor를 통한 디버깅 빌드의 경우 .vscode/launch.json에 하기와 같이 처리후 사용하면 됩니다.  
```
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Flutter iOS",
      "type": "dart",
      "request": "launch",
      "program": "lib/main.dart"
    },
    {
      "name": "Flutter AOS Branch",
      "type": "dart",
      "request": "launch",
      "program": "lib/main_branch.dart",
      "args" : [
        "--flavor",
        "branch"
      ]
    },
    {
      "name": "Flutter AOS Admin",
      "type": "dart",
      "request": "launch",
      "program": "lib/main_admin.dart",
      "args" : [
        "--flavor",
        "admin"
      ]
    }
  ]
}
```   
[참조](https://medium.com/@dev.juyoung/flutter-flavor%EB%A5%BC-%ED%86%B5%ED%95%9C-%EB%B9%8C%EB%93%9C-%EB%B3%80%ED%98%95-part-1-161be6928c50)
