---
title:  "[Unity3D] Unity3D AR 색칠증강 콘텐츠 제작"
excerpt: "Unity3D에서 2D마커 색칠증강 콘텐츠를 제작하는 방법을 설명합니다."
date:   2022-02-07 17:30:00 +0530
categories:
  - Unity3D
tags:
  - Unity3D
  - C#
  - AR(Augmented Reality)
toc: true
toc_sticky: true
---
유니티와 Maxst 마커를 통한 색칠증강 콘텐츠 제작 방법을 기술합니다.  
이번 포스트는 Unity 2018.4.10f1 환경에서 진행되었으며, 히스토리 차원에서 작성되었습니다.  

***해당 내용은 사적인 부분으로 다른 분들의 일반 정보 사용에 도움이 되지는 않을 듯 합니다.***

색칠 증강을 위한 메쉬 생성은 UCLAGameLab 라이브러리를 사용하였습니다.  
[UCLAGameLab 라이브러리](https://github.com/uclagamelab/MeshCreator)

## 1.개요
여기서 기술하는 2D 색칠증강은 사용자가 제공된 종이(전체 또는 특정 영역)에 색칠을 한 뒤 영역을 AR 카메라로 비춰 텍스쳐로 생성 후 미리 만들어 놓은 오브젝트에 텍스쳐를 적용하는 구현 방식입니다.  

## 2. 2D 색칠증강 영역 정보
색칠증강에 사용하는 마커에서 전체 또는 특정 영역(텍스쳐 생성 영역)의 정보를 얻어옵니다.  
Coloring 씬을 선택해서 진행합니다.  
1. Marker의 Prefab(tale220551)를 올려놓는다.(오른쪽 마우스로 Unpack all)
    * **(중요) 원본 Prefab에 영향을 주지 않기 위함!!!**
2. UIImageTarget_ 중에 색칠 증강이 있는 항목에 다음 GameObject 추가
    * DummyRegionCapture (Reset)
        * RegionCapture
3. Image Target Behavior 스크립트의 Advanced에 Width(1), Height(1), Scale(10) > Maxst 등록 Scale 기입 후 Change Size
4. DummyRegionCapture에 3D Plane 추가
    * 해당 Plane의 Left, Right, Top, Bottom 영역을 조정해서 영역을 지정
    * Transform 값을 복사해서 RegionCapture에 복사
5. 영역이 여러개면 위의 RegionCapture를 여러개 생성해서 처리한다.  
<video width="100%" height="100%" controls="controls">
  <source src="/assets/images/movies/unity/ar_2d_coloring_transform.mp4" type="video/mp4">
</video>

## 3. 콘텐츠 제작
색칠증강을 통해서 생성한 텍스쳐를 적용해서 실질적으로 마커에 반응할 리소스(Prefab)을 제작합니다.  
1. GameObject를 만든다.
2. Mesh Creator Data 스크립트를 Add Component 한다.
3. 사용할 이미지를 Mesh Texture에 적용한다.
4. Mesh Materials > Auto Generate Material?를 체크한다.
5. Update Mesh
6. Shader > Mobile/Unlit (Supports Lightmap) 적용
7. Mesh Create Data 스크립트 제거
8. RC_Get_Texture 스크립트를 Add Component 한다.
9. 메인에 붙인 Modify Animation Play의 Add Region Capture를 클릭해서 Region Capture Info를 갱신한다.  
<video width="100%" height="100%" controls="controls">
  <source src="/assets/images/movies/unity/ar_2d_coloring_contents.mp4" type="video/mp4">
</video>

***해당 내용은 사적인 부분으로 다른 분들의 일반 정보 사용에 도움이 되지는 않을 듯 합니다.***🧐 
