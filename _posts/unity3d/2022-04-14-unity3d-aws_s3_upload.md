---
title:  "[Unity3D] Unity3D AWS S3 파일/폴더 업로드"
excerpt: "Unity3D Editor에서 파일 및 폴더를 AWS S3에 업로드하는 법"
date:   2022-04-14 10:30:00 +0530
categories:
  - Unity3D
tags:
  - Unity3D
  - C#
  - AWS
  - CloudFront
toc: true
toc_sticky: true
published: false
---
유니티에서 파일/폴더를 아마존 S3에 올린 뒤 무효화(Invalidation)를 진행하여 CloudFront에 바로 반영되는 방법을 알아보도록 하겠습니다.

해당 내용은 Unity3D 2018.4.10f1에서 진행되었습니다.

## 1. 선행 작업
## 2. 파일/폴더 업로드
## 3. 업로드 무효화(Invalidation)

***참고로 위의 내용은 C#으로 진행되었으므로 C# 어플리케이션 개발에도 활용할 수 있습니다.***🧐 