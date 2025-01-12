

  {/*  // Kết nối đến SignalR Hub*/}
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

   {/* // Bắt đầu kết nối với server*/}
    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("Connection failed: ", err));




  {/*  // Lắng nghe sự kiện 'ReceiveMessage' từ SignalR*/}
    connection.on("ReceiveMessage", (user, message) => {
        const chatMessages = document.getElementById("chatMessages");
        const newMessage = document.createElement("div");
        newMessage.textContent = `${user}: ${message}`;
        chatMessages.appendChild(newMessage);
        chatMessages.scrollTop = chatMessages.scrollHeight; // Scroll to bottom of messages
    });

{/*    // Hàm gửi tin nhắn khi nhấn nút Send*/}
    function sendMessage() {
        const user = "User1"; // Thay thế bằng tên người dùng thực tế
        const message = document.getElementById("chatInput").value;
        if (message.trim() !== "") {
            connection.invoke("SendMessage", user, message)
                .then(() => {
                    document.getElementById("chatInput").value = ""; // Xóa ô nhập
                })
                .catch(err => console.error("Send failed: ", err));
        }
    }

 {/*   // Mở cửa sổ chat*/}
    function openChat() {
        document.getElementById("chatPopup").style.display = "block";
    }

    {/*// Đóng cửa sổ chat*/}
    function closeChat() {
        document.getElementById("chatPopup").style.display = "none";
    }
