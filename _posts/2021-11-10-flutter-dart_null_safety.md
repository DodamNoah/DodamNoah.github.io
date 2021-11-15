---
title:  "[Flutter] Dart에서 Null-Safety와 기존 Legacy 방식 혼합"
date:   2021-11-10 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
---
Flutter SDK 버전이 지속적으로 업데이트 되면서 Dart 언어 역시 계속 진화를 하고 있습니다.
Flutter 2x 출시와 같이 Dart 언어의 Null-Safety에 대한 큰 변화도 포함되었으며, 이번 포스트에서는 Null-Safety에 대한 이슈 사항에 대해서 간단히 정리하도록 하겠습니다.

## Null-Safety 참고 사이트
1. [Dart Null-Safety의 이해](https://dart.dev/null-safety/understanding-null-safety)
2. [Dart Sound Null-Safety 대처](https://dart.dev/null-safety)
3. [Dart Null-Safety 마이그레이션](https://dart.dev/null-safety/migration-guide)

## 이슈 사항
Flutter 2x부터는 Null-Safety를 지원하기 때문에 하기와 같이 pubspec.yaml 파일의 sdk 버전을 지정할 경우 Null-Safety를 사용할 수 있습니다.
```
environment:
  sdk: ">=2.12.0 <3.0.0"
```
단, 기존 프로젝트에 대한 Flutter, Dart 버전의 업그레이드 및 신규 프로젝트 진행시 아래와 같은 오류를 볼 수 있습니다. 또한, 아직 대응이 되지 않은 Dependency를 사용할 경우 발생합니다.
```
Error: Cannot run with sound null safety, because the following dependencies
don't support null safety:

 - package:flutter_swiper
 - package:flutter_page_indicator
 - package:transformer_page_view

For solutions, see https://dart.dev/go/unsound-null-safety
2


FAILURE: Build failed with an exception.

* Where:
Script '/System/Volumes/Data/development/flutter/packages/flutter_tools/gradle/flutter.gradle' line: 1035

* What went wrong:
Execution failed for task ':app:compileFlutterBuildDebug'.
> Process 'command '/System/Volumes/Data/development/flutter/bin/flutter'' finished with non-zero exit value 1

* Try:
Run with --stacktrace option to get the stack trace. Run with --info or --debug option to get more log output. Run with --scan to get full insights.

* Get more help at https://help.gradle.org

BUILD FAILED in 1m 25s
Exception: Gradle task assembleDebug failed with exit code 1
```

## 해결 방법
- 개별 소스 처리  
이 방법은 소스 파일별로 처리하는 방식으로 Null-Safety가 적용되지 않은 소스 파일 첫 줄에 코멘트를 추가합니다.
```
//@dart=2.8
```
- 프로젝트 빌드 설정 추가(F5를 통한 디버깅 실행 시)  
Visual Studio Code로 진행할 경우 프로젝트내의 .vscode/settings.json 파일에 추가 옵션을 지정합니다.
```
  "dart.flutterRunAdditionalArgs": [
    "--no-sound-null-safety"
  ],
  "dart.vmAdditionalArgs": [
      "--no-sound-null-safety"
  ]
```
- 빌드 옵션 추가  
터미널에서 최종 결과물 출력을 위한 처리시에 추가 옵션을 지정합니다.
```
flutter run --release --no-sound-null-safety
flutter build apk --no-sound-null-safety
```

### 위의 방법은 기존 프로젝트에 대한 대응 및 Legacy Dependencies를 위한 방법으로 Dependecies 사용을 위해서는 불가피하게 필요할 수 있으나, 자신이 소스의 경우 가급적 Null-Safety를 고려해서 개발하는게 추후에도 더 좋지 않을까 생각듭니다.🧐