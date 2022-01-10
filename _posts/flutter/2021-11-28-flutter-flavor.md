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

1. Android
2. iOS
3. Dart