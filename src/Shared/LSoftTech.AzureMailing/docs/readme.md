The purpose of this package is to create an implementation of Azure email sender using Azure Communication Services. See [official documentation](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/email/send-email?tabs=linux%2Cconnection-string%2Csend-email-and-get-status-async%2Csync-client&pivots=platform-azportal) for more information on proper configuration and implementation.

By default, the package reads the connection string from appsettings.json:

```json
...
"ConnectionStrings": {
  "Mailing": "<your-connection-string>"
}
...
```

Usage:
```csharp
var sender = new AzureEmailSender();

var content = new EmailContent("Hello world!");
content.PlainText = "Hello world message!";

var message = new EmailMessage(
  senderAddress: "me@linksoft.cz",
  recipientAddress: "you@linksoft.cz",
  content: content);

await sender.SendEmailAsync(message);
```

This package is a part of larger set of packages for LinkSoft Technologies shared open source repository.

You can find the repository on [GitHub](https://github.com/Linksofteu/LinkSoft).