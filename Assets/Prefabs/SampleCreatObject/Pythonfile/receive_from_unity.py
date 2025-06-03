import socket
import requests
import json
import re

HOST = '0.0.0.0'
PORT = 9000

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind((HOST, PORT))
server.listen(1)
print(f" Unity 명령 수신 대기 중... (포트 {PORT})")

conn, addr = server.accept()
print(f" Unity 연결됨: {addr}")

while True:
    try:
        line = conn.recv(1024).decode("utf-8").strip()
        if not line:
            continue

        print(" Unity로부터 받은 명령:", line)

        prompt = (
            "다음 명령을 JSON 형식으로만 출력하세요.\n"
            "절대 설명 없이 JSON만 출력하세요.\n"
            "출력 예시:\n"
            "{ \"action\": \"move\", \"objectName\": \"Cube\", \"position\": {\"x\": 1, \"y\": 0, \"z\": 1} }\n"
            f"명령: {line}"
        )

        payload = {
            "model": "gemma3:12b",
            "prompt": prompt,
            "stream": False
        }

        res = requests.post("http://localhost:11434/api/generate", json=payload)
        result = res.json()
        model_output = result.get("response", "")

        # 코드블럭 제거
        match = re.search(r"```json\s*(\{.*?\})\s*```", model_output, re.DOTALL)
        json_str = match.group(1) if match else model_output.strip()

        print(" JSON 응답:", json_str)
        conn.sendall((json_str + '\n').encode('utf-8'))

    except Exception as e:
        print(" 에러:", e)
        break

conn.close()
server.close()