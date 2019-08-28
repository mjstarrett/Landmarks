using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Xml;
using System.IO;
using KATVR;

[CustomEditor(typeof(KATDevice))]
[ExecuteInEditMode]
public class KATDeviceEditor : Editor {
    #region Language
    string[] word = new string[32];
    #endregion
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        KATDevice KATdevice = (KATDevice)target;
        //wXML();
        LoadLanguageFile(KATdevice.displayLanguage);
        /* 基础选项 */
        KATdevice.displayLanguage = (KATDevice.LanguageList)EditorGUILayout.EnumPopup(word[0], KATdevice.displayLanguage);
        KATdevice.device = (KATDevice.DeviceTypeList)EditorGUILayout.EnumPopup(word[1], KATdevice.device);
        EditorGUILayout.Space();
        /* 通用变量 */
        #region 通用变量
        KATdevice.targetMoveObject = (Transform)EditorGUILayout.ObjectField(word[2], KATdevice.targetMoveObject, typeof(Transform), true);
        KATdevice.targetRotateObject = (Transform)EditorGUILayout.ObjectField(word[16], KATdevice.targetRotateObject, typeof(Transform), true);
        KATdevice.vrCameraRig = (Transform)EditorGUILayout.ObjectField(word[3], KATdevice.vrCameraRig, typeof(Transform), true);
        KATdevice.vrHandset = (Transform)EditorGUILayout.ObjectField(word[4], KATdevice.vrHandset, typeof(Transform), true);
        #endregion
        EditorGUILayout.Space();
        #region For KAT WALK
        if (KATdevice.device == KATDevice.DeviceTypeList.KAT_WALK)
        {
            KATdevice.MovementStyle = (KATDevice.MovementStyleList)EditorGUILayout.EnumPopup(word[6], KATdevice.MovementStyle);
            EditorGUILayout.BeginHorizontal();
            KATdevice.multiply = EditorGUILayout.FloatField(word[5], KATdevice.multiply);
            if (GUILayout.Button(word[15], GUILayout.Width(75))) KATdevice.multiply = 1;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            KATdevice.multiplyBack = EditorGUILayout.FloatField(word[14], KATdevice.multiplyBack);
            if (GUILayout.Button(word[15], GUILayout.Width(75))) KATdevice.multiplyBack = 0.3f;
            EditorGUILayout.EndHorizontal();
            KATdevice.ResetCameraKey = (KeyCode)EditorGUILayout.EnumPopup(word[13], KATdevice.ResetCameraKey);
            EditorGUILayout.Space();
            #region Displayed Variable
            if (KATVR_Global.KDevice_Walk != null)
            {
                EditorGUILayout.Slider(word[7], KATVR_Global.KDevice_Walk.data_DisplayedSpeed, 0, 1);
                EditorGUILayout.Slider(word[8], KATVR_Global.KDevice_Walk.data_bodyYaw, 0, 360);
                if (KATVR_Global.KDevice_Walk.data_isMoving == 1)
                {
                    if (KATVR_Global.KDevice_Walk.data_moveDirection > 0)
                        EditorGUILayout.LabelField(word[9], word[11]);
                    else if (KATVR_Global.KDevice_Walk.data_moveDirection < 0)
                        EditorGUILayout.LabelField(word[9], word[12]);
                }
                else
                {
                    EditorGUILayout.LabelField(word[9], word[10]);
                }
            }
            #endregion
        }
        #endregion



    }

    void OnEnable()
    {
    }
    void LoadLanguageFile(KATDevice.LanguageList Type)
    {
        XmlDocument xml = new XmlDocument();
        XmlReaderSettings set = new XmlReaderSettings();
        set.IgnoreComments = true;
        xml.Load(XmlReader.Create(KATVR.KATVR_Basic.LanguageFilePath, set));
        XmlNodeList list = xml.SelectSingleNode("LanguageFile").ChildNodes;
        switch (Type)
        {
            case KATDevice.LanguageList.简体中文:
                foreach(XmlElement element in list)
                {
                    if (element.GetAttribute("type") == "Chinese")
                    {
                        for (int i = 0; i < 32; i++)
                        {
                           word[i] = element.ChildNodes[i].InnerText;
                        }
                    }
                }
                break;
            case KATDevice.LanguageList.English:
                foreach (XmlElement element in list)
                {
                    if (element.GetAttribute("type") == "English")
                    {
                        for (int i = 0; i < 32; i++)
                        {
                            word[i] = element.ChildNodes[i].InnerText;
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
    void wXML()
    {
        if (!File.Exists(Application.dataPath + "/LanguageFile.xml"))
        {
            XmlElement[] LanguageWords = new XmlElement[32];
            XmlDocument xml = new XmlDocument();
            XmlElement language = xml.CreateElement("LanguageFile");
            XmlElement type = xml.CreateElement("Languagetype");
            type.SetAttribute("type", "Chinese");
            for (int i = 0; i < LanguageWords.Length; i++)
            {
                LanguageWords[i] = xml.CreateElement("word");
                LanguageWords[i].SetAttribute("index", i.ToString());
                LanguageWords[i].InnerText = "输入词汇";
                type.AppendChild(LanguageWords[i]);
            }
            language.AppendChild(type);
            type = xml.CreateElement("Languagetype");
            type.SetAttribute("type", "English");
            for (int i = 0; i < LanguageWords.Length; i++)
            {
                LanguageWords[i] = xml.CreateElement("word");
                LanguageWords[i].SetAttribute("index", i.ToString());
                LanguageWords[i].InnerText = "输入词汇";
                type.AppendChild(LanguageWords[i]);
            }
            language.AppendChild(type);
            xml.AppendChild(language);
            xml.Save(Application.dataPath + "/LanguageFile.xml");
        }

    }


}
