---
title:  "개발자란 무엇인가?"
excerpt: "그냥 알아보자"
date:   2024-03-27 10:30:00 +0530
author: "김백건"
categories:
  - Mentoring
tags:
  - Mentoring
  - Git
toc: true
toc_sticky: true
---
이 포스트는 개발자로써의 가져야 할 자세에 대해서 두서없이 정리한 포스트입니다.

## 1. 훌륭한 개발자란?
- 오픈 마인드를 갖자!
- 항상 새로움을 추구하자!
- 현재 도출한 결과가 최선이 아닐 수 있다!
- 누구든 나의 스승도 제자도 될 수 있다!

## 2. DodamNoah 블로그의 활용
- 해당 블로그를 통해서 Markdown Languange & Git 실습을 진행  
- Git은 윈도우 CMD가 아닌 Git bash 터미널을 통해서 진행. 리눅스 명령어를 같이 습득  
- Visual Studio Code를 통해서 진행하며, Visual Studio Code의 Extension을 활용  

## 3. 상속 개념(Inheritance)
- 모든 언어(Java, C#, JS, Dart등)에서 제공하는 개념으로 항상 프로그래밍을 할 때 고려  
- Abstract, Interface는 왜 중요하며 차이점은 몰까?  
```java  
enum ConsumerType {
  vip,
  family
}

public abstract class Consumer {
  // CType() 함수와 같이 구현부가 없는 함수를 정의할 경우에는 abstract를 사용해야 한다.  
  // 일반 class에서는 불가하다.  
  public abstract ConsumerType CType();
  public void price() { }
}

public class VipConsumer extends Consumer {
  public int vipCount;

  @Override
  public ConsumerType CType() { return ConsumerType.vip; }

  @Override
  public void price() { }
}

public class FamilyConsumer extends Consumer {
  public int familyCount;

  @Override
  public ConsumerType CType() { return ConsumerType.family; }

  @override
  public void price() { }
}

// 일반적으로 사용법 1의 경우를 많이 사용한다. 단, 구조상에 따라서 적합한 방식을 채택하여 사용하자.  
public static void main(String[] args) {
  // 사용법 1  
  // 하나의 객체를 선언 상황에 맞게 사용할 경우  
  Consumer consumer;
  consumer = new VipConsumer(); // or new FamilyConsumer()  

  // Consumer가 어떤 객체인지 구별하기 위한 방법(enum 사용)  
  // Object 비교를 통해서도 가능하다.  
  if (consumer.CType() == ConsumerType.vip) {
    consumer.price(); // O  
    print(consumer.vipCount); // X (접근 불가)  
    VipConsumer vipConsumer = (VipConsumer)consumer; // 객체를 명시적으로 지정  
    print(consumer.vipCount);
  }
  else if (consumer.CType() == ConsumerType.family) {

  }
  // 사용법 2  
  // 개별 객체를 미리 선언해서 사용할 경우  
  VipConsumer vipConsumer = new VipConsumer();
  FamilyConsumer familyConsumer = new FamilyConsumer();
}  
```  
## 4. Enum 이란?
- 예전의 Enum은 상수를 문자화해서 사용하는 용도였으나, 오늘날 Enum을 지원하는 대부분의 언어에서는 Enum이 많이 발전하여 생성자 및 함수도 가질 수 있다. 꼭 자신이 Class인것처럼....
- [Java Enum 참고 사이트](https://www.baeldung.com/a-guide-to-java-enums)

## 5. 기타
- 개발자는 기술도 중요하지만 늘 새로움을 추구하고자 하는 마음가짐이 필요하다.

***"코드 수를 기준으로 프로그램 진척도를 측정하는 것은, 무게로 비행기 제작 진척도를 측정하는 것과 같다." - Bill Gates***  

