using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
namespace KATVR
{
    public static class KATVR_Basic {
        #region Basic Variable - 基础变量
        public enum LanguageList { Chinese, Engligh };
        public static LanguageList Language;
        public static string LanguageFilePath = Application.dataPath + "/LanguageFile.xml";
        #endregion
    }

    public class KATVR_Global
    {
        public static KATDevice_Walk KDevice_Walk;
    }

}

