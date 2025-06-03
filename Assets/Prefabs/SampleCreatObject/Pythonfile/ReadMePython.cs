using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadMePython : MonoBehaviour { }
// 현제 올라마 로 구현된어 있음 
// 파워셀 이용해서 올라마 실행, ollama list로 있는지 확인
// ollama run ollama3:12b  실행
// ollama3:12b는 모델이름 맞는 모델로 실행하면됨
// ollama serve 실행 
//이 명령을 실행하면 Ollama가 백그라운드에서 API 서버로 동작하며 기본적으로 localhost:11434에서 대기합니다.
//이제 Python 코드에서 이 API에 요청을 보내 모델을 사용할 수 있습니다.
// 실행 방법 동봉된 파이썬 코드 실행
// untiy 실행 
// input field에 명령어 입력하고 버튼 클릭
// 명령 예시: 큐브를 (0,0,0)에 생성시켜줘, cube를 (1,2,3)에 이동시켜줘
// 제거: 수정중
// Json 오브젝트에서 프리팹 이름과 위치를 추출하여 큐브를 생성하거나 이동시킴
// Json 오브젝트에서 프리팹으로 모델 관리? 할듯