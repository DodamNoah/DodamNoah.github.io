---
title:  "[Unity3D] Blender와 Unity3D의 연동"
excerpt: "Blender에서 제작한 리소스를 Unity3D에 Import하여 사용합니다."
date:   2022-01-21 09:00:00 +0530
categories:
  - Unity3D
tags:
  - Unity3D
  - C#
  - Blender
toc: true
toc_sticky: true
---
유니티는 다양한 디자인 솔루션(3DMax, Maya, Blender등)을 지원합니다.
이번 포스트에서는 Blender에서 제작된 리소스(FBX+Animation)를 유니티로 Import시에 이슈에 대해서 알아봅니다.

해당 내용은 Blender 3.0과 Unity3D 2018.4.10f1에서 진행합니다.

Blender는 프로젝트 파일(.blender)를 바로 유니티로 Import하여 사용이 가능하였습니다.
하지만 Blender 3.0 버전부터는 Blender에서 FBX를 Export하여 유니티에서 FBX를 Import하여야 합니다.

## 이슈
1. Texture & Material 연동
2. Scale & Transform 지정
3. Animation 통합(Optional)
4. 2D Texture Border 이슈

## 1. Texture & Material 연동
Blender에서 FBX만 Export후에 유니티에 연동을 하면 Texture와 Material의 정보를 잃게됩니다.
Unpack Texture를 통해서 Texture를 추출한 뒤에 유니티에서 Texture와 FBX를 같이 Import하여야 합니다.
Import 후 하기 스샷과 같이 Material를 Extract하여야 Material이 생성되며 Texture가 연결됩니다.
마지막으로 생성된 Material를 통해서 자신에게 맞는 Shader를 지정해 줍니다.
![Export FBX](/assets/images/unity/blender/blender_4.png)
![Unpack Texture](/assets/images/unity/blender/blender_3.png)
![Before Extract Material](/assets/images/unity/blender/unity_material_extract_1.png)
![After Extract Material](/assets/images/unity/blender/unity_material_extract_2.png)

## 2. Scale & Transform 지정
Blender에서 FBX를 기본 속성으로 Export하면 유니티에서 Scale이 100으로 지정됩니다.
하기 스샷과 같이 Transform > Apply Scalings를 All Local(기본)에서 FBX Units Scale로 변경을 하여야 유니티에서 Scale값이 1로 지정됩니다.
또한, Forward를 X Forward로 지정하여야 유니티에서 Y축이 180도가 돌아가지 않은 상태로 처리됩니다.
마지막으로 Apply Transform를 체크하여 Transform들의 값을 기본 값으로 처리할 수 있습니다.
![Export FBX Attribute](/assets/images/unity/blender/blender_5.png)

## 3. Animation 통합(Optional)
간혹 Blender에서 Animation(Clip) 작업을 진행한 뒤 동일 타임라인에 표시(동시 재생)하여도 무관한 Animation(Clip)들이 분할되어 나오는 경우가 있습니다.
이건 기본 옵션인 All Actions이 체크되어 발생하는 현상이며, 이 옵션은 모든 Action을 단일로 Bake하는 기능입니다.
만약 모든 Action을 통합하여 유니티에서 Import시 하나의 Animation Clip으로 처리가 필요할 경우 해당 옵션을 체크 해제하시면 됩니다.
![Export FBX Bake Animation Attribute](/assets/images/unity/blender/blender_2.png)

## 4. 2D Texture Border 이슈
Blender는 3D 제작뿐만이 아닌 2D 제작도 할 경우가 발생합니다. 간혹 아래 스샷과 같이 Import후 텍스쳐의 Border에 라인이 발생하는 이슈가 생길 수 있습니다.
![Import 2D FBX 이슈](/assets/images/unity/blender/result_1.png)
이 경우 유니티의 Import한 텍스쳐의 속성을 Default > Sprite(2D and UI)로 변경하면 이슈를 해결할 수 있습니다.
![Unity3D Texture Attribute](/assets/images/unity/blender/result_1.png)
![Import 2D FBX 해결](/assets/images/unity/blender/result_2.png)

***이번에 Blender3.0과 Unity3D를 연동하면서 겪은 이슈에 대해서 부족한 지식이지만 공유하게되었습니다. 조금이나마 도움이 되었으면 합니다.***🧐 
