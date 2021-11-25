---
title:  "[Flutter] Tips & Troubleshooting"
excerpt: "Flutter를 개발하면서 겪었던 상황들에 대한 해결법을 공유합니다.(빌드 오류, 화면 전환 이슈, 비디오 플레이어 이슈, 앱 아이콘등)"
date:   2021-11-24 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
toc: true
toc_sticky: true
---
크로스 플랫폼으로 앱을 개발하더라도 각각의 플랫폼에서 발생하는 이슈 사항들이 생기게 됩니다.
항시 발생하는 부분이면 오히려 해결하기 쉬울 수 있지만, 다양한 개발 환경에서 접하지 못한 이슈가 발생할 경우 난관에 처할 수 있을 듯 합니다.
이번 포스트는 제가 경험했던 이슈들은 히스토리 차원에서 기록 및 공유할 계획입니다.

# 1. 공통
## 1. 빌드 후 용량이 갑자기 커졌을 경우
Flutter는 빌드 후 캐쉬로 인해서 어느 순간 갑자기 결과물(Apk등)의 용량이 커질 경우가 발생합니다.  
```bash
$ flutter clean
$ flutter pub get
$ flutter build apk or flutter build ios // release
```
## 2. 페이지 이동시 값 전달
Flutter에서는 Navigator.of(context).pushNamed(...)를 통해서 페이지를 이동하며 이전 페이지로 돌아올 때 Natigator.pop(...)를 사용합니다.  
만약 A > B > A로 페이지 이동시 B에서의 특정 값을 A에서 받아야 할 경우 아래와 같이 처리합니다.  
```
// A > B 호출
var result = await Navigator.of(context).pushNamed(context, B);
// B에서 처리
Navigator.pop(context, parameters); // parameters에 원하는 값을 넘겨주면 var result로 받아서 처리가 가능합니다.
```

## 2. Dependency 사용시 유의 사항
Flutter에서는 다양한 [Dependency](https://pub.dev/)를 제공하며, Dependency에 따라서 지원하는 플랫폼도 다양합니다.  
따라서, Dependency 사용시에는 꼭 Readme를 숙지하여 플랫폼별 추가 설정이 없는지 확인을 하셔야 합니다.

# 2. Android
## 1. Notch(SafeArea) 디바이스에서의 Status bar 처리
SystemChrome.setEnabledSystemUIOverlays([])로 지정을 하여도 Notch영역이 검은색(사용 불가)으로 나오는 경우에 아래와 같이 처리를 해 줍니다.  
**android > app > src > main > res > values > styles.xml**에 하기 코드를 추가합니다.
```
<item name="android:windowLayoutInDisplayCutoutMode">shortEdges</item>
```
## 2. 풀 화면 사용
Flutter에서 제공하는 SystemChrome.setEnabledSystemUIOverlays([]); API가 정상 동작을 하지 않을 경우에는 MainActivity에 다음의 코드를 추가합니다.  
또한, 키보드(Input Field)를 사용할 경우에는 키보드가 Hide될 때 한번 더 호출해줍니다.
```java
override fun onCreate(savedInstanceState: Bundle?) {
    super.onCreate(savedInstanceState)
    fullScreen()
}
override fun onWindowFocusChanged(hasFocus: Boolean)
{​​​​​​
​   super.onWindowFocusChanged(hasFocus);
   if (hasFocus) fullScreen()
}​​​​​​​​​​​​​​
private fun fullScreen() {​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​
​   window.decorView.systemUiVisibility = (View.SYSTEM_UI_FLAG_IMMERSIVE
     or View.SYSTEM_UI_FLAG_LAYOUT_STABLE
     or View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
     or View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
     or View.SYSTEM_UI_FLAG_HIDE_NAVIGATION
     or View.SYSTEM_UI_FLAG_FULLSCREEN)
}
```

# 3. iOS
## 1. 빌드시 Exception: CocoaPods not installed or not in valid state 발생시
* 방법 #1
```
터미널에서 프로젝트 > ios 폴더로 이동
pod install
IDE(Visual Studio Code) 재 시작
```
* 방법 #2
```
터미널에서 프로젝트 > ios 폴더로 이동
sudo gem uninstall cocoapods
sudo gem install cocoapods -v 1.9.0 // 하위 버전
pod setup
sudo gem install cocoapods (1.10.1)
IDE(Visual Studio Code) 재 시작
```

## 2. 화면 전환 이슈
특정 디바이스(iPhone7)에서 Permission.request를 통해서 특정 권한(카메라, 앨범)에 대한 승인/요청 팝업 이후 ImagePicker를 통해서 카메라, 앨번으로 이동을 하면 Only Landscape모드가 Portrait로 변경되어 앱으로 다시 돌아올 때 Portrait 상태가 됩니다.
* 방법
Permission.request전에 isUndetermined를 체크하여 권한 승인 요청 팝업이 뜬 뒤에는 딜레이(2s) + CircleProgress를 처리한 뒤 ImagePicker를 통해서 카메라, 앨범으로 이동하도록 처리합니다.
```
var undetermined = await Permission.camera.isUndetermined;
await Permission.camera.request();
var granted = await Permission.camera.isGranted;
if (!granted) {
} else {
  setState(() => _imageProgress = true); // CircleProgress 위젯 활성화
  Platform.isAndroid
      ? _profileFromCamera()
      : Future.delayed(
          Duration(
              milliseconds: undetermined ? 2000 : 0), // 2초 딜레이 후 진행
          () => _profileFromCamera());
}
```
* 예상 원인
권한 승인 요청으로 넘어갈 때 또는 권한 승인 요청창이 Portrait로 인식되어 그 상태로 카메라, 앨범에 전달되면 카메라, 앨범 역시 Portrait로 전환되고 이후 앱으로 다시 돌아올 때 Orientation이 돌아오지 않는 걸로 예상 됩니다.(Flutter에서 제공하는 SystemChrome.setPreferredOrientations를 통해서 다시 설정으로 하여도 적용 안 됨)
* 기타
Xcode에 케이블을 연결하고 디바이스에 직접 삽입하여 실행시에는 정상 동작합니다.(Debug, Release 모두) 
단, TestFlight or Enterprise(InHouse)등의 Archive를 통해서 앱을 설치, 진행시에는 이슈 발생

## 3. 비디오 플레이어(video_player) 이슈
Flutter에서는 일반적으로 video_player dependency를 많이 사용합니다. 단, iOS 저사양(?) 단말(아이패드 4세대)에서는 다음의 이슈가 발생합니다.
```
flutter: Failed to load video: Operation Stopped
```
간헐적으로 영상내의 사운드는 재생이 되지만, 화면이 렌더링되지 않습니다.
(video_player 2.1.1-2021/04/19 버전까지 미해결)
만약 저사양 단말에서도 서비스를 제공하기 원할 경우 FijkPlayer dependency의 사용도 고려해볼만 합니다.(단, 예뮬 지원은 하지 않습니다.)

## 4. 앱 아이콘
앱 스토어 아이콘은 투명 레이어(1024x1024)를 포함할 수 없습니다. 아카이브중에 하기와 같이 오류가 발생할 경우에는 다음 링크내의 파일을 Alpha로 다시 저장하는 방법을 사용해보세요.
![Archive 오류](/assets/images/ios_icon_issue.png)
[해결 방법 링크](https://stackoverflow.com/questions/46585809/error-itms-90717-invalid-app-store-icon)

# 4. Web
웹의 경우는 사용자가 다양하게 브라우저의 크기를 조절할 수 있으므로 Responsive한 UI구성이 무엇보다 중요합니다.  
responsive_framework는 해상도에 따라서 UI의 다양한 구성을 지원하며, flutter_screenutil의 경우 대표 해상도를 기준으로 확대/축소 방식을 지원합니다.  

해상도 대응 방식은 개발 초기에 프로젝트에 따라서 신중히 검토 후 진행을 하셔야 합니다. 프로젝트가 어느 정도 진행된 상태에서 변경이 쉽지는 않더군요.😱  

* [responsive_framework](https://pub.dev/packages/responsive_framework) - Responsive한 웹/윈도우에 권장
* [flutter_screenutil](https://pub.dev/packages/flutter_screenutil) - 모바일 플랫폼에 권장

# 5. Windows
## 1. 배포시 유의 사항
개발을 진행하는 PC의 경우는 하기 3개의 DLL이 다양한 경로를 통해서 설치되어 문제가 없을 수 있지마, 사용자들마다 환경이 다르기 때문에 배포시에는 추가해 주셔야 합니다.  
* msvcp140.dll
* vcruntime140.dll
* vcruntime140_1.dll  

만약 빌드시에 자동으로 Debug/Release폴더에 복사를 원할 경우는 아래의 절차를 사용해 보세요.
1. 프로젝트 > windows에 libs폴더를 만들어서 필수 DLL를 추가합니다.
2. windows폴더에 있는 CMakeLists.txt의 제일 하단에 하기 코드를 추가합니다.
```
add_custom_target(FLUTTER_WIN_DLL ALL)
add_custom_command(
   TARGET FLUTTER_WIN_DLL POST_BUILD
   COMMAND ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​CMAKE_COMMAND}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​ -E copy ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​CMAKE_SOURCE_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​/libs/msvcp140.dll ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​INSTALL_BUNDLE_LIB_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​/msvcp140.dll
   COMMAND ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​CMAKE_COMMAND}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​ -E copy ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​CMAKE_SOURCE_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​/libs/vcruntime140.dll ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​INSTALL_BUNDLE_LIB_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​/vcruntime140.dll
   COMMAND ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​CMAKE_COMMAND}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​ -E copy ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​CMAKE_SOURCE_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​/libs/vcruntime140_1.dll ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​INSTALL_BUNDLE_LIB_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​/vcruntime140_1.dll
   WORKING_DIRECTORY ${​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​PROJECT_BUILD_DIR}​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​​
)
```