const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveMessage", function (user, message) {
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(err => console.error("Błąd połączenia z serwerem:", err.toString()));

document.getElementById("sendButton").addEventListener("click", function () {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    if (user.trim() !== "" && message.trim() !== "") {
        connection.invoke("SendMessage", user, message)
            .catch(err => console.error("Błąd wysyłania wiadomości:", err.toString()));
        document.getElementById("messageInput").value = "";
    }
});
