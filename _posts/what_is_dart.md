# Dart를 공부하자!

모든 플랫폼에는 개발 언어가 존재합니다.
* Cocoa2D > C/C++
* Unity3d > C#
* React Native > JavaScript
* Cordova > HTML5, CSS3 and JavaScript

[Dart](https://dart.dev/)

## 적용 순서
1. 프로젝트의 pubspec.yaml 파일이 있는 위치에 flutter_launcher_icons.yaml 파일 생성합니다.
2. pubspec.yaml에 다음 코드 추가
   ```
   dev_dependencies:
     flutter_launcher_icons: ^0.9.2 # 버전
   ```
3. 다음 코드를 기입(**코드 기입시 첫줄은 꼭!! 유지**)
*하기 코드는 flutter_launcher_icons.yaml 파일 생성없이 pubspec.yaml에 작성해도 무관합니다.*
   ```

     flutter_icons:
       android: true
       ios: true
       image_path: "icon/app_icon.png" # 프로젝트내 아이콘 경로
   ```
4. 터미널 창을 통해서 다음을 실행합니다.
   ```
   flutter pub get
   flutter pub run flutter_launcher_icons:main
   ```
---
### 각 플랫폼 폴더에 해상도별 아이콘이 생성됩니다. 예를 들어, 안드로이드의 경우 android/app/main/res/mipmap-xxx 폴더들에서 ic_launcher.png를 확인할 수 있습니다.