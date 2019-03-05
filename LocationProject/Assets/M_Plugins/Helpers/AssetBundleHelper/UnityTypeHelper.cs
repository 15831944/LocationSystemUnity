using Unity.Common.Consts;

public static class UnityTypeHelper
    {
    public static UnityType GetUnityType()
        {
#if UNITY_EDITOR && UNITY_ANDROID
        return UnityType.UNITY_EDITOR_ANDROID;
#endif

#if UNITY_ANDROID
        return UnityType.UNITY_ANDROID;
#elif UNITY_IPHONE
            return UnityType.UNITY_IPHONE;
#elif UNITY_EDITOR
        return UnityType.UNITY_EDITOR;
#elif UNITY_WEBGL
        return UnityType.UNITY_WEBGL;
#elif UNITY_STANDALONE_WIN
            return UnityType.UNITY_STANDALONE_WIN;
#elif UNITY_WEBPLAYER
           return UnityType.UNITY_WEBPLAYER;

#endif
    }
}

