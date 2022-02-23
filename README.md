# Awesome.Net.WritableOptions

Use to update option values into json file

## Usage

### Install-Package

```powershell
Install-Package Awesome.Net.WritableOptions
```

### Configure writable options

Let's start with the simplest configuration

```csharp
services.ConfigureWritableOptions<MyOptions>(configurationRoot, "MySectionName");
```

or use custom json file

```csharp
services.ConfigureWritableOptions<MyOptions>(configurationRoot, "MySectionName", "Resources/appsettings.custom.json");
```

or use custom json serializer options

```csharp
services.ConfigureWritableOptions<MyOptions>(
    configurationRoot, 
    "MySectionName", 
    defaultSerializerOptions: new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    });
```

## Update option values into json file

```csharp
private readonly IWritableOptions<MyOptions> _options;

public MyClass(IWritableOptions<MyOptions> options)
{
    _options = options;
}
```

```csharp
_options.Update(opt => {
    opt.Field1 = "value1";
    opt.Field2 = "value2";
});
```

No Reload

```csharp
_options.Update(opt => {
    opt.Field1 = "value1";
    opt.Field2 = "value2";
}, false);
```

If a certain configuration, specific json serialization options need to be used.

```csharp
var customSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    Converters = { new JsonStringEnumConverter() }
};
_options.Update(opt => {
    opt.Field1 = "value1";
    opt.Field2 = "value2";
}, serializerOptions: customSerializerOptions);
```

See more:
[``How to update values into appsetting.json?``](https://stackoverflow.com/a/45986656)

## Others

### JsonFileHelper

#### Methods

```csharp
public class JsonFileHelper
{
    public static Func<JsonSerializerOptions> DefaultSerializerOptions;

    public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, Action<T> updateAction = null, JsonSerializerOptions serializerOptions = null) where T : class, new();

    public static void AddOrUpdateSection<T>(string jsonFilePath, string sectionName, T value, JsonSerializerOptions serializerOptions = null);

    public static bool TryGet<T>(string jsonFilePath, string sectionName, out T value, JsonSerializerOptions serializerOptions = null);
}
```

#### `AddOrUpdateSection`

Use the default JSON serialization option update some configuration items

```csharp
JsonFileHelper.AddOrUpdateSection(
    jsonFilePath: "appsettings.json", 
    sectionName: "MySectionName", 
    value: new MyOptions {
        opt.Field1 = "value1";
        opt.Field2 = "value2";
    });
```

Or use the default JSON serialization option update the configuration section

```csharp
JsonFileHelper.AddOrUpdateSection<MyOptions>(
    jsonFilePath: "Resources/appsettings.custom.json", 
    sectionName: "MySectionName", 
    updateAction: opt => {
        opt.Field1 = "value1";
        opt.Field2 = "value2";
    });
```

Or use the custom JSON serialization option update the configuration section

```csharp
JsonFileHelper.AddOrUpdateSection<MyOptions>(
    jsonFilePath: "Resources/appsettings.custom.json", 
    sectionName: "MySectionName", 
    updateAction: opt => {
        opt.Field1 = "value1";
        opt.Field2 = "value2";
    },
    serializerOptions: new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() }
    });
```

#### `TryGet`

Use the default JSON serialization options

```csharp
if(JsonFileHelper.TryGet(jsonFilePath, sectionName, out MyOptions value))
{
    ...
}
```

Or use the custom JSON serialization options

```csharp
var customSerializerOptions = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    Converters = { new JsonStringEnumConverter() }
};
if(JsonFileHelper.TryGet(jsonFilePath, sectionName, out MyOptions value, customSerializerOptions))
{
    ...
}
```
