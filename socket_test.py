import socket
import time

host = ""
port = 8080

try:
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.connect((host, port))

    while True:
        try:
            data = ','.join(map(str, int_list))

            sock.sendall(data.encode("utf-8"))

            response = sock.recv(1024).decode("utf-8")
            print(response)
        except ConnectionResetError:
            print("The client disconnected.")

        except Exception as e:
            print("An error occured:", e)

        time.sleep(1)
except ConnectionRefusedError:
    print("Connection refused. Make sure the server is running.")

finally:
    sock.close()
