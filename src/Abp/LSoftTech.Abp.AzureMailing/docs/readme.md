This package wraps the shared Azure mailing implementation into an Abp.io framework module.

Add a connection string to your application.json:
```json
...
"ConnectionStrings": {
  "Mailing": "<your-connection-string>"
}
...
```

Usage:
```csharp
[DependsOn(typeof(AzureMailingModule))]
public class YourAbpModule()
{
...
```

This package is a part of larger set of packages for LinkSoft Technologies shared open source repository.

You can find the repository on [GitHub](https://github.com/Linksofteu/LinkSoft).