using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadMePython : MonoBehaviour { }
// ���� �ö� �� �����Ⱦ� ���� 
// �Ŀ��� �̿��ؼ� �ö� ����, ollama list�� �ִ��� Ȯ��
// ollama run ollama3:12b  ����
// ollama3:12b�� ���̸� �´� �𵨷� �����ϸ��
// ollama serve ���� 
//�� ����� �����ϸ� Ollama�� ��׶��忡�� API ������ �����ϸ� �⺻������ localhost:11434���� ����մϴ�.
//���� Python �ڵ忡�� �� API�� ��û�� ���� ���� ����� �� �ֽ��ϴ�.
// ���� ��� ������ ���̽� �ڵ� ����
// untiy ���� 
// input field�� ��ɾ� �Է��ϰ� ��ư Ŭ��
// ��� ����: ť�긦 (0,0,0)�� ����������, cube�� (1,2,3)�� �̵�������
// ����: ������
// Json ������Ʈ���� ������ �̸��� ��ġ�� �����Ͽ� ť�긦 �����ϰų� �̵���Ŵ
// Json ������Ʈ���� ���������� �� ����? �ҵ�