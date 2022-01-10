---
title:  "[Flutter] Tips"
excerpt: "Flutter를 개발하면서 소소한 도움이 되는 팁들에 대해서 공유합니다."
date:   2021-12-01 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
toc: true
toc_sticky: true
---

# 1. 공통
## 1. 페이지 이동시 값 전달
Flutter에서는 Navigator.of(context).pushNamed(...)를 통해서 페이지를 이동하며 이전 페이지로 돌아올 때 Natigator.pop(...)를 사용합니다.  
만약 A > B > A로 페이지 이동시 B에서의 특정 값을 A에서 받아야 할 경우 아래와 같이 처리합니다.  
```
// A > B 호출
var result = await Navigator.of(context).pushNamed(context, B);
// B에서 처리
Navigator.pop(context, parameters); // parameters에 원하는 값을 넘겨주면 var result로 받아서 처리가 가능합니다.
```
# 2. Android
## 1. Flavor & Variant를 통한 Output 이름 지정  
android/app/build.gradle에 하기와 같이 코드를 삽입하면 빌드 후 projectDir/build/app/outputs/apk/에 기본 정보가 들어간 apk를 만들 수 있습니다.(AppName_market-1.0.0_10.apk)
```bash
flutter {
    source '../..'
}
...
android.applicationVariants.all { variant ->
    variant.outputs.all {
        def appName = "AppName"
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
# 3. iOS
# 4. Web