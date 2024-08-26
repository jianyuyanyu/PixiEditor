# DO NOT MODIFY .CS FILES IN THIS DIRECTORY

They are autogenerated from the .proto files. If you want to make changes to them, 
please modify the .proto files and build project.

Protogen is installed on first build, but if you want to install it manually, you can do it by installing protobuf-net.Protogen nuget package.
To install it globally on your machine go to https://www.nuget.org/packages/protobuf-net.Protogen and follow .NET CLI (Global) instructions.

For example `dotnet tool install --global protobuf-net.Protogen --version 3.2.42`

After installing protogen, you can run it by executing `protogen` in the root directory of PixiEditor.Extensions.CommonApi

```bash
protogen --csharp_out=ProtoAutogen --proto_path=DataContracts *.proto
```