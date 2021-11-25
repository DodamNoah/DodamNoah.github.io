---
title:  "[Flutter] 멀티 SDK 설치 및 설정"
excerpt: "Flutter의 다양한 SDK 버전을 동일 PC에 셋팅해서 사용하는 방법을 알아봅니다. 기존 앱의 유지/관리를 위해서 간혹 필요합니다."
date:   2021-11-02 13:00:00 +0530
last_modified_at: 2021-11-23 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
toc: true
toc_sticky: true
---
초기 Flutter를 개발할 당시의 SDK 버전은 1.26.0으로 프로젝트를 진행하였으며, 해당 프로젝트는 Android, iOS, Web 플랫폼에서 서비스를 해야만 했습니다.😱

하지만, Flutter 1x 버전의 경우 Android, iOS는 Stable 버전으로 제공되었지만, Web의 경우 Dev or Master 버전으로만 제공되어 동일 프로젝트를 채널 변경으로 처리할 경우 개발의 너무 불편하였으며, 이를 해결하기 위해서 여러 버전의 SDK를 설치(셋팅)하여 간단히 변경할 수 있도록 처리하였습니다.

다행히 Flutter 2x 버전부터는 Web버전 역시 동일 SDK에서 지원하므로 아래 내용이 불필요할 수 있습니다.😍

```
$ flutter channel dev
...
$ flutter channel stable
```

## 1. Windows 10 + Visual Studio Code
1. [Flutter 다운로드](https://docs.flutter.dev/development/tools/sdk/releases) 필요한 SDK를 다운로드합니다.
2. C:\develop\ 폴더에 SDK 버전별 폴더로 압축을 해제합니다.
![Flutter SDK](/assets/images/win1.png)
3. aliase 폴더에 배치 파일을 버전별로 생성합니다.
![Alias](/assets/images/win2.png)

**[배치 파일 내용]**
```bash
@echo off
C:\develop\flutter_stable\bin\flutter %*
```
4. 컴퓨터 환경 변수에 aliase폴더를 Path로 지정합니다.
![환경 변수 설정](/assets/images/win3.png)
5. Visual Studio Code 환경 설정에 Flutter 경로를 지정합니다.
![Visual Studio Code 설정](/assets/images/win4.png)
6. 사용법
   * 터미널 사용
   ```
   > flutter2x pub get // aliase 파일로 구별
   ```
   * Visual Studio Code SDK 변경
   ```
   Ctrl+Shift+P를 통해서 > Flutter: Change SDK를 통해서 변경
   Visual Studio Code 오른쪽 하단 Flutter 버전을 변경
   ```

## 2. Mac + Visual Studio Code
1. [Flutter 다운로드](https://docs.flutter.dev/development/tools/sdk/releases?tab=macos) 필요한 SDK를 다운로드합니다.
2. Windows 10 방식과 동일하게 원하는 폴더에 압축을 해제합니다.
3. .bash_aliases 파일을 .bashrc이 있는 폴더($HOME)에 생성해서 아래의 코드와 같이 작성합니다.
```
alias flutters='/uangel/development/flutter/bin/flutter'
alias flutterb='/uangel/development/flutter_beta/bin/flutter'
alias flutter2s='/uangel/development/flutter2x/bin/flutter'
```
4. $HOME/.bashrc 파일에 아래와 같이 코드를 작성합니다.
```
if [ -f ~/.bash_aliases ]; then
. ~/.bash_aliases
fi
```
5. 사용법
   * 터미널 사용
   ```
   ~ /.bash_aliases // Visual Studio Code 구동 후 정의한 fltuter(xxx) alias가 안 먹을 경우
   $ flutter2x pub get
   ```
   * Visual Studio Code SDK 변경
   ```
   Ctrl+CMD+P를 통해서 > Flutter: Change SDK를 통해서 변경
   Visual Studio Code 오른쪽 하단 Flutter 버전을 변경
   ```

***Flutter SDK를 변경후에는 가급적! flutter(변경 버전 alias) pub get를 호출해줘야 합니다.***