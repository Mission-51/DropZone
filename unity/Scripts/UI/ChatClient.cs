/*using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class ChatClient : MonoBehaviour
{
    private WebSocket webSocket;
    private const string SERVER_URL = "wss://j11d110.p.ssafy.io/ws";  // 수정된 서버 URL

    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private TextMeshProUGUI chatDisplayText;
    [SerializeField] private string roomId = "defaultRoom";
    [SerializeField] private ScrollRect chatScrollRect;  // ScrollRect 추가
    private string accessToken;  // JWT 토큰
    

    private LobbyManager lobbyManager;
    public enum MessageType
    {
        enter,
        chat,
        leave
    }
    [System.Serializable]
    public class ChatMessageDto
    {
        public string roomId;
        public string sender;
        public string message;
        public MessageType type;  // 메시지 타입을 enum으로 변경
    }


    void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        if (lobbyManager == null)
        {
            Debug.LogError("LobbyManager not found in the scene!");
            return;
        }
        
        LobbyManager.OnNicknameReady += InitializeChatWithNickname;
        UnityMainThreadDispatcher.Instance();
    }


    // Coroutine으로 닉네임을 불러온 후 서버 연결
    private IEnumerator InitializeChat()
    {
        yield return null; // 코루틴이 비동기적으로 작동할 수 있도록 대기
        ConnectToServer(); // 서버 연결

        InitializeUI(); // UI 초기화
    }

    


    void InitializeUI()
    {
        
        sendButton.onClick.AddListener(OnSendButtonClick);

        
        messageInputField.onSubmit.AddListener(OnInputFieldSubmit);
    }

    void ConnectToServer()
    {
        webSocket = new WebSocket(SERVER_URL);

        webSocket.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        webSocket.OnOpen += (wsSender, e) =>
        {
            Debug.Log("Connection opened");

            SendStompConnect();
            SendJoinMessage();  // 채팅방 입장 시 입장 메시지 전송
        };

        webSocket.OnMessage += (wsSender, e) =>
        {
            try
            {
                Debug.Log("Message received: " + e.Data);
                ProcessMessage(e.Data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error processing message: " + ex.Message);
            }
        };

        webSocket.OnError += (wsSender, e) =>
        {
            Debug.LogError("Error: " + e.Message);
        };

        webSocket.OnClose += (wsSender, e) =>
        {
            Debug.Log("Connection closed");
            SendLeaveMessage();  // 채팅방 나갈 때 나가기 메시지 전송
            
        };

        webSocket.Connect();
    }

    void SendStompConnect()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        string token = "Bearer " + accessToken;
        string connectFrame = $"CONNECT\naccept-version:1.2\nAuthorization:{token}\n\n\0";
        webSocket.Send(connectFrame);

        SendStompSubscribe();  // 구독 프레임 전송
    }

    void SendStompSubscribe()
    {
        string subscribeFrame = $"SUBSCRIBE\ndestination:/sub/room/{roomId}\nid:sub-0\n\n\0";
        Debug.Log("Sending subscribe frame: " + subscribeFrame);
        webSocket.Send(subscribeFrame);
    }

    void SendJoinMessage()
    {
        
        string currentNick = lobbyManager.GetNick();  // 매번 최신 닉네임을 가져옴
        Debug.Log("Join message for: " + currentNick);
        SendSystemMessage($"{currentNick}님이 입장하셨습니다.", MessageType.enter);

        
    }

    // 나가기 메시지 전송
    void SendLeaveMessage()
    {
        string currentNick = lobbyManager.GetNick();  // 매번 최신 닉네임을 가져옴
        Debug.Log("Leave message for: " + currentNick);
        SendSystemMessage($"{currentNick}님이 나가셨습니다.", MessageType.leave);
    }

    // 입장/나가기 메시지를 전송하는 함수
    void SendSystemMessage(string message, MessageType messageType)
    {
        Debug.Log("Sending system message: " + message);
        if (webSocket.ReadyState == WebSocketState.Open)
        {
            ChatMessageDto systemMessage = new ChatMessageDto
            {
                roomId = roomId,
                sender = "",  // 시스템 메시지는 sender가 없음
                message = message,
                type = messageType  // 메시지 타입에 따라 enter 또는 leave로 설정
            };

            string json = JsonConvert.SerializeObject(systemMessage);
            Debug.Log("Serialized system message: " + json); // 직렬화된 JSON 확인
            string stompFrame = $"SEND\ndestination:/pub/chat/message\ncontent-type:application/json\n\n{json}\0";
            webSocket.Send(stompFrame);
        }
    }

    void SendChatMessage(string roomId, string message)
    {
        string currentNick = lobbyManager.GetNick();  // 매번 최신 닉네임을 가져옴
        if (webSocket.ReadyState == WebSocketState.Open)
        {
            ChatMessageDto chatMessage = new ChatMessageDto
            {
                roomId = roomId,
                sender = currentNick,  // 최신 닉네임 사용
                message = message,
                type = MessageType.chat  // 일반 메시지 타입으로 설정
            };

            string json = JsonConvert.SerializeObject(chatMessage);
            string stompFrame = $"SEND\ndestination:/pub/chat/message\ncontent-type:application/json\n\n{json}\0";

            Debug.Log("Sending STOMP frame: " + stompFrame);
            webSocket.Send(stompFrame);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Cannot send message.");
        }
    }

    void ProcessMessage(string message)
    {
        Debug.Log("Received STOMP frame: " + message);

        int index = message.IndexOf("\n\n");
        if (index != -1)
        {
            // 메시지 본문을 추출
            string body = message.Substring(index + 2).Trim('\0');

            // JSON 파싱
            ChatMessageDto receivedMessage = JsonConvert.DeserializeObject<ChatMessageDto>(body);

            if (receivedMessage != null)
            {
                Debug.Log("Parsed message: " + receivedMessage.message + " | Type: " + receivedMessage.type);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    DisplayMessage(receivedMessage);
                });
            }
            else
            {
                Debug.LogWarning("Failed to parse received message as ChatMessageDto.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid STOMP message format: " + message);
        }
    }

    void DisplayMessage(ChatMessageDto message)
    {
        string formattedMessage;

        // 메시지 타입에 따라 다른 방식으로 처리
        if (message.type == MessageType.enter)
        {
            // 입장 메시지 처리 - 초록색
            formattedMessage = $"<color=#00FF00>{message.message}</color>\n";  // 초록색으로 메시지 표시
            Debug.Log("System message received: " + formattedMessage);
        }
        else if (message.type == MessageType.chat)
        {
            // 일반 채팅 메시지 처리 - 기본 색상
            formattedMessage = $"{message.sender} : {message.message}\n";
            Debug.Log("Chat message received from " + message.sender + ": " + message.message);
        }
        else if (message.type == MessageType.leave)
        {
            // 나가기 메시지 처리 - 빨간색
            formattedMessage = $"<color=#FF0000>{message.message}</color>\n";  // 빨간색으로 메시지 표시
            Debug.Log("Leave message received: " + formattedMessage);
        }
        else
        {
            Debug.LogWarning("Unknown message type: " + message.type);
            return; // 잘못된 타입이 있을 경우 처리하지 않음
        }

        chatDisplayText.text += formattedMessage;
    }

    public void OnSendButtonClick()
    {
        SendCurrentMessage();
    }

    void OnInputFieldSubmit(string text)
    {
        SendCurrentMessage();
    }

    void SendCurrentMessage()
    {
        if (!string.IsNullOrEmpty(messageInputField.text))
        {
            SendChatMessage(roomId, messageInputField.text);
            messageInputField.text = "";

            // 메시지 전송 후 바로 커서를 활성화하여 다시 입력할 수 있도록 설정
            messageInputField.ActivateInputField();
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        LobbyManager.OnNicknameReady -= InitializeChatWithNickname;
        if (webSocket != null && webSocket.ReadyState == WebSocketState.Open)
        {
            SendLeaveMessage();  // 웹소켓 닫기 전에 나가기 메시지 전송
            webSocket.Close();
        }
    }
    private void InitializeChatWithNickname(string nickname)
    {
        Debug.Log("닉네임을 확인하고 채팅 연결 시작: " + nickname);
        
        StartCoroutine(InitializeChat());
    }
}*/

using System;
using System.Collections;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class ChatClient : MonoBehaviour
{
    private WebSocket webSocket;
    private const string SERVER_URL = "wss://j11d110.p.ssafy.io/ws"; // 수정된 서버 URL

    [SerializeField]
    private TMP_InputField messageInputField;

    [SerializeField]
    private Button sendButton;

    [SerializeField]
    private TextMeshProUGUI chatDisplayText;

    [SerializeField]
    private string roomId = "defaultRoom";

    [SerializeField]
    private ScrollRect chatScrollRect; // ScrollRect 추가
    private string accessToken; // JWT 토큰

    private LobbyManager lobbyManager;

    public enum MessageType
    {
        enter,
        chat,
        leave
    }

    [System.Serializable]
    public class ChatMessageDto
    {
        public string roomId;
        public string sender;
        public string message;
        public MessageType type; // 메시지 타입을 enum으로 변경
    }

    void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        if (lobbyManager == null)
        {
            Debug.LogError("LobbyManager not found in the scene!");
            return;
        }

        LobbyManager.OnNicknameReady += InitializeChatWithNickname;
        UnityMainThreadDispatcher.Instance();
    }

    // Coroutine으로 닉네임을 불러온 후 서버 연결
    private IEnumerator InitializeChat()
    {
        yield return null; // 코루틴이 비동기적으로 작동할 수 있도록 대기
        ConnectToServer(); // 서버 연결

        InitializeUI(); // UI 초기화
    }

    // 닉네임을 가져올 때까지 대기하는 Coroutine


    void InitializeUI()
    {
        sendButton.onClick.AddListener(OnSendButtonClick);

        messageInputField.onSubmit.AddListener(OnInputFieldSubmit);
    }

    void ConnectToServer()
    {
        webSocket = new WebSocket(SERVER_URL);

        webSocket.SslConfiguration.ServerCertificateValidationCallback = (
            sender,
            certificate,
            chain,
            sslPolicyErrors
        ) => true;

        webSocket.OnOpen += (wsSender, e) =>
        {
            Debug.Log("Connection opened");

            SendStompConnect();
            SendJoinMessage(); // 채팅방 입장 시 입장 메시지 전송
        };

        webSocket.OnMessage += (wsSender, e) =>
        {
            try
            {
                Debug.Log("Message received: " + e.Data);
                ProcessMessage(e.Data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error processing message: " + ex.Message);
            }
        };

        webSocket.OnError += (wsSender, e) =>
        {
            Debug.LogError("Error: " + e.Message);
        };

        webSocket.OnClose += (wsSender, e) =>
        {
            Debug.Log("Connection closed");
            SendLeaveMessage(); // 채팅방 나갈 때 나가기 메시지 전송
        };

        webSocket.Connect();
    }

    void SendStompConnect()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        string token = "Bearer " + accessToken;
        string connectFrame = $"CONNECT\naccept-version:1.2\nAuthorization:{token}\n\n\0";
        webSocket.Send(connectFrame);

        SendStompSubscribe(); // 구독 프레임 전송
    }

    void SendStompSubscribe()
    {
        string subscribeFrame = $"SUBSCRIBE\ndestination:/sub/room/{roomId}\nid:sub-0\n\n\0";
        Debug.Log("Sending subscribe frame: " + subscribeFrame);
        webSocket.Send(subscribeFrame);
    }

    void SendJoinMessage()
    {
        string currentNick = lobbyManager.GetNick(); // 매번 최신 닉네임을 가져옴
        Debug.Log("Join message for: " + currentNick);
        SendSystemMessage($"{currentNick}님이 채팅채널에 입장하셨습니다.", MessageType.enter);
    }

    // 나가기 메시지 전송
    void SendLeaveMessage()
    {
        string currentNick = lobbyManager.GetNick(); // 매번 최신 닉네임을 가져옴
        Debug.Log("Leave message for: " + currentNick);
        SendSystemMessage($"{currentNick}님이 채팅채널에서 떠났습니다.", MessageType.leave);
    }

    // 입장/나가기 메시지를 전송하는 함수
    void SendSystemMessage(string message, MessageType messageType)
    {
        Debug.Log("Sending system message: " + message);
        if (webSocket.ReadyState == WebSocketState.Open)
        {
            ChatMessageDto systemMessage = new ChatMessageDto
            {
                roomId = roomId,
                sender = "", // 시스템 메시지는 sender가 없음
                message = message,
                type = messageType // 메시지 타입에 따라 enter 또는 leave로 설정
            };

            string json = JsonConvert.SerializeObject(systemMessage);
            Debug.Log("Serialized system message: " + json); // 직렬화된 JSON 확인
            string stompFrame =
                $"SEND\ndestination:/pub/chat/message\ncontent-type:application/json\n\n{json}\0";
            webSocket.Send(stompFrame);
        }
    }

    void SendChatMessage(string roomId, string message)
    {
        string currentNick = lobbyManager.GetNick(); // 매번 최신 닉네임을 가져옴
        if (webSocket.ReadyState == WebSocketState.Open)
        {
            ChatMessageDto chatMessage = new ChatMessageDto
            {
                roomId = roomId,
                sender = currentNick, // 최신 닉네임 사용
                message = message,
                type = MessageType.chat // 일반 메시지 타입으로 설정
            };

            string json = JsonConvert.SerializeObject(chatMessage);
            string stompFrame =
                $"SEND\ndestination:/pub/chat/message\ncontent-type:application/json\n\n{json}\0";

            Debug.Log("Sending STOMP frame: " + stompFrame);
            webSocket.Send(stompFrame);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Cannot send message.");
        }
    }

    void ProcessMessage(string message)
    {
        Debug.Log("Received STOMP frame: " + message);

        int index = message.IndexOf("\n\n");
        if (index != -1)
        {
            // 메시지 본문을 추출
            string body = message.Substring(index + 2).Trim('\0');

            // JSON 파싱
            ChatMessageDto receivedMessage = JsonConvert.DeserializeObject<ChatMessageDto>(body);

            if (receivedMessage != null)
            {
                Debug.Log(
                    "Parsed message: "
                        + receivedMessage.message
                        + " | Type: "
                        + receivedMessage.type
                );

                UnityMainThreadDispatcher
                    .Instance()
                    .Enqueue(() =>
                    {
                        DisplayMessage(receivedMessage);
                    });
            }
            else
            {
                Debug.LogWarning("Failed to parse received message as ChatMessageDto.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid STOMP message format: " + message);
        }
    }

    void DisplayMessage(ChatMessageDto message)
    {
        string formattedMessage;

        // 메시지 타입에 따라 다른 방식으로 처리
        if (message.type == MessageType.enter)
        {
            // 입장 메시지 처리 - 초록색
            formattedMessage = $"<color=#00FF00>{message.message}</color>\n"; // 초록색으로 메시지 표시
            Debug.Log("System message received: " + formattedMessage);
        }
        else if (message.type == MessageType.chat)
        {
            // 일반 채팅 메시지 처리 - 기본 색상
            formattedMessage = $"{message.sender} : {message.message}\n";
            Debug.Log("Chat message received from " + message.sender + ": " + message.message);
        }
        else if (message.type == MessageType.leave)
        {
            // 나가기 메시지 처리 - 빨간색
            formattedMessage = $"<color=#FF0000>{message.message}</color>\n"; // 빨간색으로 메시지 표시
            Debug.Log("Leave message received: " + formattedMessage);
        }
        else
        {
            Debug.LogWarning("Unknown message type: " + message.type);
            return; // 잘못된 타입이 있을 경우 처리하지 않음
        }

        chatDisplayText.text += formattedMessage;
    }

    public void OnSendButtonClick()
    {
        SendCurrentMessage();
    }

    void OnInputFieldSubmit(string text)
    {
        SendCurrentMessage();
    }

    void SendCurrentMessage()
    {
        if (!string.IsNullOrEmpty(messageInputField.text))
        {
            SendChatMessage(roomId, messageInputField.text);
            messageInputField.text = "";

            // 메시지 전송 후 바로 커서를 활성화하여 다시 입력할 수 있도록 설정
            messageInputField.ActivateInputField();
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        LobbyManager.OnNicknameReady -= InitializeChatWithNickname;
        if (webSocket != null && webSocket.ReadyState == WebSocketState.Open)
        {
            SendLeaveMessage(); // 웹소켓 닫기 전에 나가기 메시지 전송
            webSocket.Close();
        }
    }

    private void InitializeChatWithNickname(string nickname)
    {
        Debug.Log("닉네임을 확인하고 채팅 연결 시작: " + nickname);

        StartCoroutine(InitializeChat());
    }
}
