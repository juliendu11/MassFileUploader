# MassFileUploader

[C#].NET Core

Upload files to multiple networks at once with only username and password

- [x] Uptobox login & uploading  
- [ ] 1Fichier login & uploading  
- [ ] MEGA login & uploading  
- [ ] Dropbox login & uploading  
- [ ] Uploaded login & uploading  
- [ ] Turbobit login & uploading

# How to use ?

Use builder to get instance:

```c#
 massUploaderBuilder = MassUploader.Core.Builder.MassUploaderBuilder.CreateBuilder()
                .Uptobox("email", "password")
                .Build();
```

## Login and upload

```c#
Console.WriteLine("------LOGIN------");
Console.WriteLine("\r\n");
var login = await massUploaderBuilder.LoginNetwork();
foreach (var net in login)
  Console.WriteLine($"[{net.Key}]Result: {net.Value.Success}, message: {net.Value.Message}");

Console.WriteLine("\r\n");
Console.WriteLine("------UPLOAD------");
Console.WriteLine("\r\n");
var upload = await massUploaderBuilder.Upload.UploadNetwork(@"filePath");
foreach (var net in upload)
  Console.WriteLine($"[{net.Key}]Result: {net.Value.Success}, message: {net.Value.Message}");
```
##### Note: All methods are asynchronous.  
##### Note: For LoginNetwork () and UploadNetwork () the tasks are done in parallel and wait until all are finished (parallel connection of uptobox, mega then waiting for the result)

## Property Changed

You can subscribe to an event to get live information on network status

```c#
massUploaderBuilder = MassUploader.Core.Builder.MassUploaderBuilder.CreateBuilder()
                .Uptobox("email", "password")
                .Build();
massUploaderBuilder.UptoboxSession.PropertyChanged += UptoboxSession_PropertyChanged;
```

```c#
private static void UptoboxSession_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine("Uptbox status: ",massUploaderBuilder.UptoboxSession.NetworkStatus);
        }
```

Available value:

- Disabled  
- Enabled  
- Logged  
- LoginError  
- Uploaded  
- UploadingError
