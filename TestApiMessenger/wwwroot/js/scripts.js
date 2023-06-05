
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

let allUsers = [];

document.getElementById("loginBtn").addEventListener("click", function () {
    const userName = document.getElementById("userName").value;
    if (userName.length > 0) {
        hubConnection.invoke("Login", userName)
            .then(function () {
                document.getElementById("sendBtn").disabled = false;
                document.getElementById("recipient-name").disabled = false;
                document.getElementById("subject-name").disabled = false;
                document.getElementById("message-text").disabled = false;
                document.getElementById("userName").disabled = true;
                document.getElementById("loginBtn").disabled = true;
                document.getElementById("logOutBtn").disabled = false;
            })
            .catch(function (err) {
                return console.error(err.toString());
            });
    }
});

document.getElementById("logOutBtn").addEventListener("click", function () {
    location.reload();
});

document.getElementById("sendBtn").addEventListener("click", function () {
    const userName = document.getElementById("userName").value;
    const recipient = document.getElementById("recipient-name").value;
    const subject = document.getElementById("subject-name").value;
    const message = document.getElementById("message-text").value;

    hubConnection.invoke("SendPrivateMessage", userName, recipient, subject, message)
        .then(function () {
            document.getElementById("recipient-name").value = "";
            document.getElementById("subject-name").value = "";
            document.getElementById("message-text").value = "";
            
        })
        .catch(function (err) {
            return console.error(err.toString());
        });
});

hubConnection.on("GetAllMsg", function (message) {
    ViewAllMsg(message);
});

function ViewAllMsg(messages) {
    messages.forEach(function (message) {
        // Создаем элемент div
        var div = document.createElement('div');
        div.classList.add('card', 'mb-3', 'w-1');

        // Создаем элемент div для содержимого сообщения
        var bodyDiv = document.createElement('div');
        bodyDiv.classList.add('card-body');

        // Создаем заголовок сообщения
        var titleElement = document.createElement('h5');
        titleElement.classList.add('card-title');
        titleElement.textContent = "Subject: " + message.subject;


        // Создаем подзаголовок сообщения
        var subtitleElement = document.createElement('h6');
        subtitleElement.classList.add('card-subtitle', 'mb-2', 'text-body-secondary');
        subtitleElement.textContent = "Sender: " + message.senderName;

        // Создаем текстовое содержимое сообщения
        var contentElement = document.createElement('p');
        contentElement.classList.add('card-text');
        contentElement.textContent = "Message: " + message.text;

        var contentElementTime = document.createElement('i');
        contentElementTime.classList.add('card-time');
        contentElementTime.textContent = "Time: " + message.timestamp;

        // Добавляем заголовок, подзаголовок и текстовое содержимое в div с содержимым сообщения
        bodyDiv.appendChild(titleElement);
        bodyDiv.appendChild(subtitleElement);
        bodyDiv.appendChild(contentElement);
        bodyDiv.appendChild(contentElementTime);


        // Добавляем div с содержимым сообщения в общий div сообщения
        div.appendChild(bodyDiv);

        const firstElem = document.getElementById("chatroom").firstChild;
        document.getElementById("chatroom").insertBefore(div, firstElem);
    });
}

hubConnection.on("ReceiveMessage", function (messages) {
    let message = messages[0];
    toastr.info("Subject: " + message.subject + "\n" + "Sender: " + message.senderName);
    ViewAllMsg(messages);
});

document.getElementById("recipient-name").addEventListener("focus", function (event) {
    hubConnection.invoke("GetAllUsers")
        .catch(function (err) {
            return console.error(err.toString());
        });
});

hubConnection.on("GetAllUsers", function (users) {
    allUsers = users;
    $(function () {
        $("#recipient-name").autocomplete({
            source: allUsers,
        });
    });
});

hubConnection.start()
    .then(function () {
        document.getElementById("sendBtn").disabled = true;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });


