# RiggingSpider
<img src="https://raw.githubusercontent.com/MDJ0126/RiggingSpider/main/Image/1.gif" width="500px" height="auto">
Unity의 Animation Rigging 패키지를 이용한 학습 프로젝트입니다.

## 이동 원리

<img src="https://raw.githubusercontent.com/MDJ0126/RiggingSpider/main/Image/4.gif" width="500px" height="auto">

1. 각 다리마다 IK 컴포넌트를 연결하였으며, Target을 월드좌표에 고정합니다.
2. 다리가 일정 범위를 벗어나면, Target을 움직입니다.
3. Target이 이동할 때는 삼각함수 Sin을 이용하여 자연스럽게 곡선으로 움직입니다.
4. Target이 움직이므로 다리가 따로옵니다.

## 다리 움직임 표현

<img src="https://raw.githubusercontent.com/MDJ0126/RiggingSpider/main/Image/sin.png" width="500px" height="auto">

삼각함수 "Sin * PI" 을 통한 개념으로, 이동할 때 0f~1f로 Lerp를 통해서 애니메이션을 매프레임마다 위치를 이동합니다.

## 인스펙터 조절

<img src="https://raw.githubusercontent.com/MDJ0126/RiggingSpider/main/Image/inspector.png" width="500px" height="auto">

레이케스트에 관련된 옵션들은 컴포넌트에서 조절이 가능합니다.

## 이슈

1. 다리 이동 범위가 짧아서 다음 예측 이동 거리를 못 찾을 경우, 지형을 떠나 공중 이동됨
<img src="https://raw.githubusercontent.com/MDJ0126/RiggingSpider/main/Image/issue1.gif" width="500px" height="auto">

2. 지그재그 패턴을 적용하지 않았음
<img src="https://raw.githubusercontent.com/MDJ0126/RiggingSpider/main/Image/issue2.gif" width="500px" height="auto">

### 사용된 패키지
1. Animation Rigging (Built-in)
2. Spider orange. (https://assetstore.unity.com/packages/3d/characters/robots/spider-orange-181154)

### 유니티 엔진 버전
Unity 2022.3.16f1 (플래폼 PC)
