# Setup

## First time

```bash
flutter pub get
```

## Android — разрешить HTTP трафик к бекенду

После `flutter create .` добавить в `android/app/src/main/AndroidManifest.xml`:

```xml
<application
    android:usesCleartextTraffic="true"
    ...>
```

Или добавить network security config для конкретного IP:

```xml
<!-- android/app/src/main/res/xml/network_security_config.xml -->
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <domain-config cleartextTrafficPermitted="true">
        <domain includeSubdomains="false">95.84.137.57</domain>
    </domain-config>
</network-security-config>
```

## iOS — разрешить HTTP

В `ios/Runner/Info.plist`:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```
