---
title:  "Git은 무엇인가?"
excerpt: "개발자로써의 첫걸음으로 Git을 알아보자"
date:   2024-03-20 10:30:00 +0530
author: "김백건"
categories:
  - Mentoring
tags:
  - Mentoring
  - Git
toc: true
toc_sticky: true
---
이 포스트는 나의 사랑스러운 조카 김백건과의 1:1 멘토링의 출발로 가볍게 Git의 중요성과 사용법을 간략히 알아봅니다. 

## 1. Git은 왜 필요한가?
- 프로젝트를 진행하는데 있어서 조직원들과의 협업을 하는데 필수적인 솔루션.
- 프로젝트의 소스 관리 및 개발 히스토리를 제공.

## 2. Repository(저장소) 생성
- 나만의 저장소를 생성해서 프로젝트 관리를 할 수 있음.
- 깃허브는 open source로 다른 사용자들도 내 저장소에 접근할 수 있다.
- 저장소 생성시 ReadMe.md 파일을 생성해서 프로젝트의 히스토리를 기록한다.  
(ReadMe.md는 MarkDown 규격으로 간단한 Tag들을 통해서 문서 작성이 가능하다.)

- .gitignore 파일 관리  
빌드를 통해 생성된 파일 및 협업하고 있는 각 개발자들의 환경에 따라서 생성된 파일은 공유될 필요가 없으며, 그러한 파일 및 폴더 목록을 관리하여 Commit, Push 리스트에서 배제하는 용도로 사용된다.  
참고로, 파일명 앞에 붙은 .은 보통 숨김파일을 의미하다.  
(note: 탐색기에서 shift+우클릭하면 더 많은 보기들이 나옴)

## 3. 브랜치
- 보통 레파지토리를 생성하게 되면 main 브랜치로 생성하게된다. 
- 브랜치는 현재 함께 프로젝트를 진행하는 구성원이라면 각자의 역할에 맞는 여러 개의 브랜치를 생성할 수 있다.
- 일반적으로 main(master) 브랜치는 상용을 목적으로 사용하며, 상용전/후 개발 진행시에는 별도의 브랜치를 생성해서 관리한다.

## 4. Git 설정
- clone을 통해 원격저장소의 생성한 저장소를 가져온다.
```
git clone 원격저장소주소 로컬폴더명
```
- checkout을 통해 원격저장소에 생성된 다수의 브랜치중의 특정 브랜치를 변경/전환한다.  
```
git checkout 브랜치명
```
:warning: WARNING  
checkout을 통해 브랜치를 변경/전환할 경우 꼭 현재 브랜치의 작업 내용을 하기 절차를 통해서 원격 저장소에 반영 후 사용한다.

## 5. Git 사용법
- git pull 이후 충돌이 발생하면 상황에 따라서 별도의 대처가 필요
```bash
$ cd projects // 프로젝트 생성 디렉토리로 이동(Optional)
$ git status // 현 (원격) 저장소 및 (로컬) 저장소(Stage) 상태 확인
$ git add . (or git add filename) // 수정 또는 추가 파일 등록
$ git commit -m 'description' // 수정 또는 추가 파일 Stage에 등록
$ git status // 현 (원격) 저장소 및 로컬(Stage) 상태 확인
$ git pull origin main // (원격) 저장소의 변경 사항 받아오기
$ git push origin main // (로컬) 저장소의 변경 사항 서버에 적용
$ git status // 현 (원격) 저장소 및 로컬(Stage) 상태 확인
```

## 6. 기타
- git commit만 하면 _____ 창이 열리는데 Esc키를 눌러 :wq를 이용해 수정할 수도 있고 :q를 통해 나올 수 있다.
- git commit -m "설명문"을 통해 빠르게 commit 하는 연습을 하자.
- hello.txt를 pull했는데 충돌이 일어낫다 -> 메모장을 킨 후 충돌이 일어난 부분을 확인 후 수정하여 다시 한번 더 merge를 시킨 후 충돌이 해결된이후에 push를 해주자. 
- 파일 수정 - add - commit - pull - push 순서는 반드시 지켜주자!
- git command에 익숙해지기 전까지는 절대로 SourceTree와 같은 git툴을 사용하지 말고 terminal을 통한 직접 명령어로 숙지!!

***"어떤 바보라도 컴퓨터가 이해할 수 있는 코드를 작성할 수 있습니다. 훌륭한 프로그래머는 인간이 이해할 수 있는 코드를 작성합니다." - Martin Fowler***  
***"Any fool can write code that a computer can understand. Good programmers write code that humans can understand."***

