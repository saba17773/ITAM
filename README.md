# สิ่งที่ต้องเตรียม

- .NET Core SDK 3.0 ขึ้นไป
- Visual Studio Code (**แนะนำอันนี้** แล้วลง C# Extension เพิ่ม) หรือ Visual Studio ก็ได้

# ลง SDK Version ใหม่แแล้ว C# Extension ใน VS Code พัง

ให้เพิ่ม Environment Variables ให้ **name** ชื่อ `MSBuildSDKsPath` ส่วน **value** ให้เอา path ของ SDK ที่จะใช้ใส่เข้าไป `C:\Program Files\dotnet\sdk\**SDK_VERSION**\Sdks`

# Setup

- Restore backup file database ที่อยู่ใน folder **Database** โดยให้เอา backup ล่าสุดมา restore
- แก้ไข config database ที่ file **appsettings.Development.json**
- Default port คือ 5000 สามารถเปลี่ยนได้ โดยเข้าไปแก้ไขที่ file **launchSettings.json** ใน folder **Properties**
- เข้าไปที่ folder **Web** รันคำสั่ง `dotnet restore` เพื่อ update package
- ทดสอบรันด้วยคำสั่ง `dotnet watch run`

# Build & Publish

Build `dotnet build -c Release -o Build`

Publish `dotnet publish -c Release -o Build`

หรือรันผ่าน `Build.cmd` ก็ได้

# Deploy บน IIS

- Enable IIS บน server
- Download `Runtime & Hosting Bundle` ไปติดตั้งบน server
- Add Site ชี้ไปที่ path ของไฟล์ที่ build มาแล้ว ตั้งชื่อเว็บ และใส่ Port

> ที่ Application pool ตรง .NET Framework Version ให้เลือก No Manage Code
