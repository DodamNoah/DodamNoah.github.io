---
title:  "[Flutter] 앱 아이콘 생성 및 적용"
excerpt: "일반적으로 앱을 만들면 아이콘을 해상도 사이즈별로 다양하게 만들어야 합니다.😱 이번 Post에서는 유용한 Dependency를 통해서 간단히 처리하는 방법을 공유드리겠습니다."
date:   2021-11-10 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
---
Flutter에는 단일 사이즈(권장 512x512 or 256x256) 이미지를 통해서 다양한 플랫폼의 필수 아이콘을 자동으로 생성해 주는 Dependency를 제공합니다.😍

[flutter_launcher_icons(Dependency)](https://pub.dev/packages/flutter_launcher_icons)

적용 순서
프로젝트의 pubspec.yaml 파일이 있는 위치에 flutter_launcher_icons.yaml 파일 생성합니다.
pubspec.yaml에 다음 코드 추가합니다.
```
dev_dependencies:
  flutter_launcher_icons: ^0.9.2 # 버전
```
다음 코드를 기입(**코드 기입시 첫줄은 꼭!! 유지**)
하기 코드는 flutter_launcher_icons.yaml 파일 생성없이 pubspec.yaml에 작성해도 무관합니다.
```

  flutter_icons:
    android: true
    ios: true
    image_path: "icon/app_icon.png"
```
터미널 창을 통해서 다음을 실행합니다.
```
flutter pub get
flutter pub run flutter_launcher_icons:main
```
---
### 각 플랫폼 폴더에 해상도별 아이콘이 생성됩니다. 예를 들어, 안드로이드의 경우 android/app/main/res/mipmap-xxx 폴더들에서 ic_launcher.png를 확인할 수 있습니다.

