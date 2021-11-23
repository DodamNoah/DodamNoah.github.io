---
title:  "[Flutter] Flutter 설치"
date:   2021-11-01 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
---
이제부터가 시작이다!👏  
이번 포스트에서는 Flutter를 이용한 앱 개발을 위한 다음 과정을 공유하도록 하겠습니다.  
모든 과정은 MacOS, Windows 10 환경에서 진행하도록 하겠습니다.
1. Visual Studio Code 다운로드 & 설치
2. Flutter SDK 다운로드 & 설정
3. 플랫폼 설정
4. Visual Studio Code Plugin 설치(*Optional*)
5. 개발 환경 설정
6. 프로젝트 생성 및 빌드

## 1. Visual Studio Code 다운로드 & 설치
[Visual Studio Code 다운로드](https://code.visualstudio.com/download)  
위의 다운로드 페이지를 통해서 본인 OS에 맞는 버전을 다운로드 및 설치합니다.  
## 2. Flutter SDK 다운로드 & 설정
[Flutter SDK 다운로드](https://flutter.dev/docs/get-started/install)  
위의 다운로드 페이지를 통해서 본인 OS에 맞는 버전을 다운로드합니다.
### 1. Windows 10 환경  
하기 내용에 대한 자세한 가이드는 [Windows install](https://flutter.dev/docs/get-started/install/windows)에서 확인 바랍니다.

### 2. MacOS 환경  
하기 내용에 대한 자세한 가이드는 [macOS install](https://flutter.dev/docs/get-started/install/macos)에서 확인 바랍니다.
* 터미널을 실행해서 아래와 같이 특정(원하는) 경로의 압축을 풀어줍니다.
```
$ cd ~/development
$ unzip ~/Downloads/flutter_macos_xxx-stable.zip
$ mv flutter_macos_xxx-stable flutter2x
``` 
* 압축을 풀어준 경로를 등록합니다.
```
$ export PATH="$PATH:'pwd'/flutter2x/bin
```
🤩 *Flutter를 통해 앱을 개발하다보면 개발 시점에는 최신 SDK를 통해서 개발을 진행하지만, 서비스 이후 앱을 유지/관리하는 과정에서 신규 앱을 추가 개발할 경우 SDK의 버전이 올라가 기존 앱에 대한 대응이 어려울 수 있으므로 멀티 SDK를 설치/유지해야 하는 경우가 발생합니다. 추후 멀티 SDK에 대한 내용을 다루겠지만, 사전에 Flutter의 압축 해제시 flutter가 아닌 flutter2x, flutter2.5x, flutter2x, flutter2x_beta와 같이 폴더 이름 지정을 권장합니다.*
* Flutter 설치 확인를 확인합니다.
```
$ flutter2x doctor -v
```
_이슈 예시(Android 빌드를 위해서 Android SDK 설치 필요. 참고 3.플랫폼 설정)_
_이후 flutter2x가 아닌 flutter로 설명하도록 하겠습니다._
```
[-] Android toolchain - develop for Android devices
    • Android SDK at /Users/obiwan/Library/Android/sdk
    ✗ Android SDK is missing command line tools; download from https://goo.gl/XxQghQ
    • Try re-installing or updating your Android SDK,
      visit https://flutter.dev/setup/#android-setup for detailed instructions.
```
_정상 예시(Android, Xcode, Chrome의 경우는 개발 플랫폼 대응에 따른 Optional)_
```
Doctor summary (to see all details, run flutter doctor -v):
[✓] Flutter (Channel stable, 2.2.3, on macOS 11.0.1 20B50 darwin-x64, locale ko-KR)
[✓] Android toolchain - develop for Android devices (Android SDK version 29.0.2)
[✓] Xcode - develop for iOS and macOS
[✓] Chrome - develop for the web
[✓] Android Studio (version 4.0)
[✓] VS Code (version 1.61.1)
[✓] Connected device (2 available)
• No issues found!
```

## 3. 플랫폼 설정
### 1. iOS 설정  
* [Xcode](https://developer.apple.com/xcode/)를 다운로드하여 설치합니다.
```
$ sude xcode-select --switch /Applicatons/Xcode.app/Contents/Developer
$ sudo xcodebuild -runFirstLaunch
$ sudo xcodebuild -license // Xcode license 동의(1회)
```  
🤩 *여러 버전의 Xcode를 사용하기 위해서는 Applications폴더내의 Xcode 버전별 폴더를 생성 후 Xcode.app의 이름을 변경하여 이동시킵니다.(예. Applicatoins/Xcode13_3/Xcode13_3.app)*
* Homebrew 설치 및 설정
Homebrew는 macOS용 패키지 관리자이며, 터미널 명령어를 통해서 필요한 프로그램의 설치, 삭제, 업데이트를 쉽게 관리할 수 있습니다.
[Homebrew](https://brew.sh/index_ko)에서 Homebrew 설치하기 스크립트를 복사한 뒤 터미널에서 복사 후 RETURN합니다.
"Password"에는 Mac 로그인 비빌번호를 사용합니다.
```
$ /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```  
* Cocoapods 설치 및 설정
Cocoapods는 Swift, Objective-C로 개발된 Xcode용 Dependency(라이브러리)입니다.
[Cocoapods](https://cocoapods.org/) Install 가이드처럼 생성한 Flutter 프로젝트 디렉토리의 ios/폴더로 이동
```
$ sudo gem install cocoapods // 보통 Flutter 프로젝트에 Dependencies를 추가했을 경우만 필요하며, Flutter ios build를 통해서 자동으로 수행됩니다.
$ pod setup
```

### 2. Android 설정
* [Android Studio](https://developer.android.com/studio)에서 다운로드하여 설치합니다.
```
$ flutter doctor --android-licenses // Android 라이센스 동의(1회)
```
* Android Studio 설치를 통해 ANDROID_SDK_ROOT 설정이 필요합니다.
* Flutter는 Android Studio 설치없이 Android SDK, Java 설치만으로도 개발이 가능합니다.
```
$ export ANDROID_SDK_ROOT=/Users/dodam/Library/Android
$ export PATH=$PATH:$ANDROID_SDK_ROOT/sdk/platform-tools
```
* Android Studio 설치를 통해 Java 8 설치 및 JAVA_HOME 설정이 필요합니다.
* Flutter doctor 명령어를 통해 Android toolchain, Xcode, Android Studio가 정상 연결되었는지 확인합니다.
```
$ flutter doctor -v
```
😊 *Android Studio 2.2 이상을 설치 후 'Android Studio 설치 마법사'를 통해서 ANDROID_SDK_ROOT, JAVA_HOME 진행이 가능합니다.*

## 4. Visual Studio Code Plugin 설치(*Optional*)
Visual Studio Code를 통해서 Flutter를 개발시 다음의 Visual Studio Code Extension을 설치, 사용하면 편리하기에 공유합니다.
* Flutter Commands
* Pubspec Assist
* Flutter Widget Snippets
* Awesome Flutter Snippets
* Advanced-new-file
* Bracket Pair Colorizer
* Material Icon Theme / Material Theme Icons
* Todo Tree
* Dart data Class Generator
* Dart Getter And Setter
* Yaml

## 5. 개발 환경 설정

## 6. 프로젝트 생성 및 빌드
* 터미널(도스 프롬프트)를 통한 프로젝트 생성
_Android Studio에서도 프로젝트 생성 가능합니다._
```
$ cd projects // 프로젝트 생성 디렉토리로 이동(Optional)
$ flutter create --org com.yourdomain.packagename -a kotlin(or java) -i swift(or objc) your_app_name
$ cd your_app_name
$ flutter run
```
🧐 *-a, -i를 지정하지 않을 경우 Android는 Kotlin, iOS의 경우 swift로 지정됩니다.*