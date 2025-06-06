package com.xsolla.sdk.unity.Example.androidProxies;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

import com.xsolla.android.payments.XPayments;
import com.xsolla.android.payments.data.AccessToken;
import com.xsolla.android.payments.callback.BrowserCallback;
import com.xsolla.android.payments.ui.ActivityType;

public class PaymentsProxyActivity extends Activity {

    private static final String ARG_TOKEN = "token";
    private static final String ARG_SANDBOX = "isSandbox";
    private static final String ARG_REDIRECT_HOST = "redirect_host";
    private static final String ARG_REDIRECT_SCHEME = "redirect_scheme";
    private static final String ARG_PAYSTATION_VERSION = "paystation_version";
    private static final String ARG_ACTIVITY_TYPE = "activity_type";
    private static final int RC_PAY_STATION = 1;
    
    private static BrowserCallback browserCallback;
    private static final String TAG = "PaymentsProxyActivity";

    public static void perform(Activity activity, String token, boolean isSandbox, String redirectScheme, String redirectHost, int paystationVersion, BrowserCallback callback, String activityType) {
        browserCallback = callback;
        
        Intent intent = new Intent(activity, PaymentsProxyActivity.class);
        intent.putExtra(ARG_TOKEN, token);
        intent.putExtra(ARG_SANDBOX, isSandbox);
        intent.putExtra(ARG_REDIRECT_HOST, redirectHost);
        intent.putExtra(ARG_REDIRECT_SCHEME, redirectScheme);
        intent.putExtra(ARG_PAYSTATION_VERSION, paystationVersion);
        intent.putExtra(ARG_ACTIVITY_TYPE, activityType);
        activity.startActivity(intent);
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (savedInstanceState != null) {
            finish();
            return;
        }

        Intent intent = getIntent();
        if (intent == null) {
            Log.e(TAG, "Intent is null. Exiting.");
            finish();
            return;
        }
        
        String token = intent.getStringExtra(ARG_TOKEN);
        if (token == null) {
            Log.e(TAG, "Missing token extra. Exiting.");
            finish();
            return;
        }
        
        boolean isSandbox = intent.getBooleanExtra(ARG_SANDBOX, false);        
        String redirectScheme = intent.getStringExtra(ARG_REDIRECT_SCHEME);
        String redirectHost = intent.getStringExtra(ARG_REDIRECT_HOST);
        int paystationVersionNumber = intent.getIntExtra(ARG_PAYSTATION_VERSION, 4);
        String activityType = intent.getStringExtra(ARG_ACTIVITY_TYPE);

        XPayments.PayStationVersion paystationVersion = XPayments.PayStationVersion.V4;
        if (paystationVersionNumber == 3)
            paystationVersion = XPayments.PayStationVersion.V3;

        XPayments.IntentBuilder builder = XPayments.createIntentBuilder(this)
                .accessToken(new AccessToken(token))
                .isSandbox(isSandbox)
                .payStationVersion(paystationVersion);
                
        if (redirectScheme != null)
            builder.setRedirectUriScheme(redirectScheme);

        if (redirectHost != null)
            builder.setRedirectUriHost(redirectHost);
            
        if (activityType != null)
        {
            switch (activityType)
            {
                case "WebView":
                    builder.setActivityType(ActivityType.WEB_VIEW);
                    break;
                case "CustomTab":
                    builder.setActivityType(ActivityType.CUSTOM_TABS);
                    break;
                case "TrustedWebActivity":
                    builder.setActivityType(ActivityType.TRUSTED_WEB_ACTIVITY);
                    break;
                default:
                    Log.w(TAG, "Unknown activityType: " + activityType + ", defaulting to CUSTOM_TABS.");
                    builder.setActivityType(ActivityType.CUSTOM_TABS);
                    break;
            }
        }

        Log.d(TAG, "Launching PayStation activity.");
        startActivityForResult(builder.build(), RC_PAY_STATION);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        
        boolean isManually = true;
        
        if (data != null) {
            try {
                XPayments.Result result = XPayments.Result.fromResultIntent(data);
                XPayments.Status status = result.getStatus();
                isManually = (status == XPayments.Status.CANCELLED);
                Log.d(TAG, "PayStation closed with status: " + status);
            } catch (Exception e) {
                Log.e(TAG, "Error parsing result from PayStation", e);
            }
        } else {
            Log.w(TAG, "No result data returned from PayStation.");
        }
    
        if (browserCallback != null) {
            browserCallback.onBrowserClosed(isManually);
        } else {
            Log.w(TAG, "browserCallback is null.");
        }
    
        finish();
    }
}
