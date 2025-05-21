# EnerGeo: Wave Energy VR Simulator
EnerGeo is a virtual reality (VR) simulation project focused on selecting optimal locations for wave power plants along the coast of South Korea.

This project was initiated as part of the Capstone Design course (Virtual Reality Project, Spring Semester 2025) at Joongbu University.

<br/>

EnerGeo는 대한민국 연안을 대상으로 파력 발전소의 최적 입지를 선정하기 위한 Virtual Reality (VR) 시뮬레이션 프로젝트입니다.

이 프로젝트는 중부대학교의 2025학년도 1학기 가상현실프로젝트(캡스톤디자인) 강의의 일환으로 시작되었습니다.

<br/>
<br/>

## Team Members / 팀 구성원
| <img src="https://avatars.githubusercontent.com/u/49934162?v=4" width="120"/> | <img src="https://avatars.githubusercontent.com/u/36768979?v=4" width="120"/> | <img src="https://avatars.githubusercontent.com/u/136365917?v=4" width="120"/> | <img src="https://avatars.githubusercontent.com/u/162388950?v=4" width="120"/> |
|:--:|:--:|:--:|:--:|
| **[CorgiMuzi](https://github.com/CorgiMuzi)** <br> *Team Leader* | **[JeongHwan Cho](https://github.com/chojh1027)** <br> *Developer* | **[rhtjdwns00](https://github.com/rhtjdwns00)** <br> *Developer* | **[habook](https://github.com/habook)** <br> *3D Artist* |


<br/>
<br/>

## Purpose / 프로젝트 목적
This project aims to support effective decision-making by visualizing expert opinions on site selection for renewable energy facilities.

By making complex evaluations intuitive and accessible, professionals from various fields can better understand the data and collaborate more efficiently.

<br/>

본 프로젝트는 재생에너지 발전소의 입지 선정에 대한 전문가들의 의견을 시각화함으로써,

서로 다른 분야의 전문가들이 정보를 직관적으로 이해하고, 효과적인 의사결정을 내릴 수 있도록 지원하는 것을 목적으로 합니다.

<br/>
<br/>

## Overview / 개요
EnerGeo combines geospatial simulation with an intuitive user interface powered by a Large Language Model (LLM) backend.

Users can interact with the simulation via natural voice conversations, enabling seamless control and data exploration within the VR environment.

<br/>

EnerGeo는 Large Language Model (LLM) 기반 음성 인터페이스와 지리공간 시뮬레이션을 결합한 시스템입니다.

사용자는 자연어 기반 음성 명령을 통해 시뮬레이션을 직관적으로 조작하고 데이터를 탐색할 수 있습니다.

<br/>
<br/>

## Key Features / 주요 기능
### LLM-based Voice Interface
자연어 음성을 통해 시뮬레이션 제어

### Geospatial VR Environment
실제 지형 및 환경 조건을 반영한 VR 시뮬레이션 공간

### Data-driven Site Evaluation
파고, 발전 가능량, 법적 제약 조건 등을 기반으로 입지 평가

### Modular System Architecture
향후 해상풍력 및 태양광 발전 요소와 연동 가능한 모듈형 설계

<br/>
<br/>

## Commit Message Rules / 커밋 메시지 규칙
### 커밋 메시지 템플릿
```
<커밋 유형>[선택사항: 코드 범위나 모듈 명]: 간단한 변경 사항 설명

[선택사항: 자세한 변경 사항 설명]

[선택사항: 부가 정보 설명]
```

### 커밋 유형 목록
| 커밋 유형| 설명                                            |
| -------- | ------------------------------------------------|
| feat     | 새로운 기능 추가                                 |
| fix      | 버그 수정                                        |
| docs     | 문서 변경 (README, 주석 등)                      |
| style    | 코드 포맷팅, 세미콜론 누락 등 기능에 영향 없는 변경 |
| refactor | 코드 리팩토링 (기능 변화 없음)                    |
| test     | 테스트 코드 추가 또는 수정                        |
| chore    | 빌드 업무, 패키지 매니저 설정 등 기타 변경         |

### 기타 규칙
1. 제목은 50자 이하로 간결하게 작성합니다.
3. 영어로 작성할 경우, 제목 첫 글자는 소문자로 시작하고, 끝에 마침표를 찍지 않습니다.
5. 변경 이유나 추가 설명이 필요한 경우 본문에 자유롭게 기술하세요.
6. 부가 정보(푸터)에는 이슈 번호나 브레이킹 체인지가 있다면 명시합니다.

### 예시
```
feat(weapon): add support for two-handed weapons

- Blend idle and run animations based on the equipped weapon types.
- Change weapon1 and weapon2's type from one-handed to two-handed weapon.
- Add long spear for new two-handed weapon using different anime pose.

Fixes: #7
Resolves: #6
```

더 자세한 규칙은 [Conventional Commits v1.0.0](https://www.conventionalcommits.org/en/v1.0.0/#specification)을 따릅니다.
