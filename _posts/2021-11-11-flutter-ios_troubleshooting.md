---
title:  "[Flutter] iOS Troubleshooting"
excerpt: "iOS 플랫폼에서 발생할 수 있는 상황에 대해서 알아봅니다.(빌드 오류, 화면 전환 이슈, 비디오 플레이어 이슈, 앱 아이콘)"
date:   2021-11-11 13:00:00 +0530
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