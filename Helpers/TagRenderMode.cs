// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.

using System.Runtime.CompilerServices;

namespace SGCApp.Helpers
{
#if !SyncfusionFramework3_5
    [TypeForwardedFrom("System.Web.Mvc, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
#endif 
    public enum TagRenderMode
    {
        Normal,
        StartTag,
        EndTag,
        SelfClosing
    }
}