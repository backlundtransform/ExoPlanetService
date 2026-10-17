package com.varoid.exoplanethunter;

import com.getcapacitor.BridgeActivity;
import android.webkit.WebSettings;
import android.os.Bundle;

public class MainActivity extends BridgeActivity {

@Override
public void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);

    WebSettings settings = getBridge().getWebView().getSettings();
    settings.setJavaScriptEnabled(true);
    settings.setUseWideViewPort(true);
    settings.setDomStorageEnabled(true);
    settings.setAllowUniversalAccessFromFileURLs(true);
    settings.setAllowFileAccessFromFileURLs(true);
}

}


