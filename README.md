# UnityTCPSocketExample
Example Unity chat project using Client-Server TCP Sockets to send chat messages between multiple clients.

![alt text](https://raw.githubusercontent.com/JamesVeug/UnityTCPSocketExample/master/ReadmeA.PNG)

# What does this do?
- Clients can connect to the server
- The server listens for new clients
- Connected clients on the server are given a basic name
- Clients send messages to the server
- Server dispatches messages to all connected clients
- Clients display messages received from the server
- Clients can use commands
- - !disconnect (disconnectes user from the server)
- - !ping (displays how long requests are taking)

# What do the different text colors mean?
- Red is for logs on the server (User has Connect/Disconnected... etc)
- Grey is for logs on the client
- Green is for messages from the server to the client

# How to setup for users not on your local network
- #1 Open ports on router (default is 8052) - https://portforward.com/
- #2 Disable windows firewall
- #3 Get IP - https://www.whatismyip.com/
- #4 Start project
- #5 Start Server
- #6 Get external user to connect to your IP from #3 and port from #1
