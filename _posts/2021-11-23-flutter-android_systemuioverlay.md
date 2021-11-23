---
title:  "[Flutter] Android SystemUIOverlay 이슈"
date:   2021-11-23 13:00:00 +0530
categories:
  - Flutter
tags:
  - Flutter
  - Dart
---
이번 포스트는 Flutter 2.2.3 버전으로 진행됩니다.

앱을 개발하다보면 앱 특성에 따라서 Status bar, Bottom Navigation bar를 다양한 방식으로 처리를 하게됩니다.
Flutter에서는 service.dart를 통한 SystemChrome.setEnabledSystemUIOverlays를 제공하여, 이를 통해서 다양한 방식으로 처리가 가능합니다.

1. Status, Bottom 미사용 - Full 화면
2. Status, Bottom 모두 사용
3. Status만 사용
4. Bottom만 사용

일반적으로 1, 2번의 경우는 큰 이슈가 발생하지 않지만, 이번에 3번의 경우로 앱을 개발하면서 발생했던 내용을 간략히 공유드리겠습니다.

Flutter에서는 SystemChrome.setEnabledSystemUIOverlays([SystemUiOverlay.top])을 통해서 Status bar만 사용하도록 제공합니다.
하지만, Android에서는 제공된 방법으로 사용시 하기와 같은 이슈가 발생합니다.

### 이슈
1. 화면을 터치해도 Bottom Navigation UI가 나타남.
2. TextFormField 사용시 두번 터치를 해야 키보드가 올라옴(처음 터치시에는 Bottom Navigation UI가 나타남)

### 해결
Android Platform 코드를 작성한 뒤 처리합니다.

* Android Native Java Code
```
import io.flutter.embedding.android.FlutterActivity;
import io.flutter.embedding.engine.FlutterEngine;
import android.view.View;
import android.os.Bundle;
import android.util.Log;
import androidx.annotation.NonNull;
import io.flutter.plugin.common.MethodCall;
import io.flutter.plugin.common.MethodChannel;
import io.flutter.plugin.common.MethodChannel.MethodCallHandler;
import io.flutter.plugin.common.MethodChannel.Result;
import io.flutter.plugins.GeneratedPluginRegistrant;

public class MainActivity extends FlutterActivity {

    private static final String CHANNEL = "com.sample.project/channel";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        hideSystemUI();
    }

    @Override
    public void configureFlutterEngine(@NonNull FlutterEngine flutterEngine) {
        GeneratedPluginRegistrant.registerWith(flutterEngine);
        new MethodChannel(flutterEngine.getDartExecutor().getBinaryMessenger(), CHANNEL)
            .setMethodCallHandler(
            (call, result) -> {
                if (call.method.equals("hideUi")) hideSystemUI();
            }
            );
    }

  @Override
  public void onWindowFocusChanged(boolean hasFocus) {
      super.onWindowFocusChanged(hasFocus);
      if(hasFocus) {
          hideSystemUI();
      }
  }

  protected void hideSystemUI() {
      // Enables regular immersive mode.
      // For "lean back" mode, remove SYSTEM_UI_FLAG_IMMERSIVE.
      // Or for "sticky immersive," replace it with SYSTEM_UI_FLAG_IMMERSIVE_STICKY
      View decorView = getWindow().getDecorView();
      decorView.setSystemUiVisibility(
              View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
              | View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                      | View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                      | View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                      | View.SYSTEM_UI_FLAG_HIDE_NAVIGATION);
  }
}
```
* Android Native Kotlin Code
```

import android.os.Bundle
import android.os.Handler
import androidx.annotation.NonNull
import android.view.View
import android.view.WindowManager
import io.flutter.embedding.android.FlutterActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodCall
import io.flutter.plugin.common.MethodChannel
import io.flutter.plugin.common.MethodChannel.MethodCallHandler
import io.flutter.plugin.common.MethodChannel.Result
import io.flutter.plugin.common.PluginRegistry.Registrar
import io.flutter.plugins.GeneratedPluginRegistrant
import android.util.Log

class MainActivity: FlutterActivity(), MethodCallHandler {

    private static final String CHANNEL = "com.sample.project/channel";
    private final val HIDEUI = "hideUi"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        hideSystemUI()
        Log.e("MainActivity", "onCreate!!!");
    }

    override fun configureFlutterEngine(@NonNull flutterEngine: FlutterEngine) {
        GeneratedPluginRegistrant.registerWith(flutterEngine);

        val binaryMessenger = flutterEngine.getDartExecutor().getBinaryMessenger()
        var channel = MethodChannel(binaryMessenger, CHANNEL)
        channel.setMethodCallHandler(this)
        MainActivity._channel = channel

        Log.e("MainActivity", "configureFlutterEngine!!!");
    }

    override fun onMethodCall(@NonNull call: MethodCall, @NonNull result: Result) {

        Log.e("MainActivity", "onMethodCall!!! " + call.method);
        when (call.method) {
            HIDEUI -> this.hideSystemUI()
            else -> result.notImplemented()
        }
    }

    override fun onWindowFocusChanged(hasFocus: Boolean) {
        super.onWindowFocusChanged(hasFocus);
        if (hasFocus) hideSystemUI()
    }

    private fun hideSystemUI() {
        window.decorView.systemUiVisibility = (View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                or View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                or View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                or View.SYSTEM_UI_FLAG_LAYOUT_STABLE
                or View.SYSTEM_UI_FLAG_HIDE_NAVIGATION)
    }

    companion object {
        var _channel: MethodChannel? = null
    }
}
```

* Flutter Dart Code
```
static Future hideSystemUI() async {
  try {
    var result = await _platform.invokeMethod('hideUi', <String, dynamic>{});
    return result;
  } on PlatformException catch (e) {
    Exception(e);
  }
}
```
```
@override
void initState() {
  super.initState();

  var keyboardVisibilityController = KeyboardVisibilityController();

  _keyboardStreamSubscription =
      keyboardVisibilityController.onChange.listen((visible) {
    if (!visible) {
      UXFunctions.hideSystemUI();
    }
  });
}
```
위와 같이 Android Native (Java or Kotlin)코드를 작성한 뒤 초기 OnCreate, onWindowFocus에서 호출하며, 이후 키보드(Input)를 사용하는 Widget에서 키보드가 내려갈 때 hideSystemUI()를 호출해줍니다.

참고로, 키보드 Detect를 위해서 flutter_keyboard_visibility Dependency를 사용하였습니다.