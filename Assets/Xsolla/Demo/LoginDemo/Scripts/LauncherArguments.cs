﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;

public class LauncherArguments : MonoSingleton<LauncherArguments>
{
    const string LauncherTokenArgName = "xsolla-login-token";

    bool invalidated = false;

    public string GetToken()
	{
		if(invalidated) {
            return "";
		}
        List<string> args = Environment.GetCommandLineArgs().ToList();
        args.ForEach(a => Debug.Log("Application argument: " + a));

        for (int i = 0; i < args.Count; i++) {
			if(args[i].Contains(LauncherTokenArgName)) {
				if((i + 1) < args.Count) {
                    return args[i + 1];
                }
			}
		}

        return "";
    }

	public void InvalidateTokenArguments()
	{
        invalidated = true;
    }
}
