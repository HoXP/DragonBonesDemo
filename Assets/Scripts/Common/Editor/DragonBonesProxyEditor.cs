using UnityEngine;
using UnityEditor;
using DragonBones;
using MiniJSON;
using System.Collections;
using System.Collections.Generic;

internal class DBEffEvent
{
    public string param;
    public int frameIndex;

    public override string ToString()
    {
        return string.Format("param:{0},frameIndex:{1}", param, frameIndex);
    }
}

internal class DBEffData
{
    public string animationName;
    public List<DBEffEvent> eventDataList;

    public override string ToString()
    {
        string arr = "";
        arr += "[";
        foreach (DBEffEvent e in eventDataList)
        {
            arr += "{";
            arr += e.ToString();
            arr += "}";
        }
        arr += "]";
        return string.Format("animationName:{0},{1}", animationName, arr);
    }
}

[CustomEditor(typeof(DragonBonesProxy))]
public class DragonBonesProxyEditor : Editor
{
    //private string effName = "";
    private List<DBEffData> dataList = new List<DBEffData>();

    private int clickIndex = 0;
    private int selectIndex = 0;
    private UnityArmatureComponent comp;
    private static DragonBonesProxyEditor inst;

    public static void Refresh()
    {
        inst.OnEnable();
    }

    private void OnEnable()
    {
        inst = this;
        var db = target as DragonBonesProxy;
        comp = db.GetComponent<UnityArmatureComponent>();
        if (comp == null || comp.unityData == null)
        {
            return;
        }
        dataList.Clear();
        Dictionary<string, object> dicRoot = (Dictionary<string,object>)MiniJSON.Json.Deserialize(comp.unityData.dragonBonesJSON.text);
        if (dicRoot.ContainsKey("armature"))
        {
            List<object> armatureRoot = (List<object>)dicRoot["armature"];
            for (int i = 0; i < armatureRoot.Count; i++)
            {
                Dictionary<string, object> armature = (Dictionary<string, object>)armatureRoot[i]; 
                if (armature.ContainsKey("animation"))
                {
                    List<object> animationRoot = (List<object>)armature["animation"];
                    foreach (object anim in animationRoot)
                    {
                        Dictionary<string, object> animDic = (Dictionary<string, object>)anim;
                        if (animDic.ContainsKey("frame"))
                        {
                            List<object> frameList = (List<object>)animDic["frame"];
                            foreach (object frame in frameList)
                            {
                                Dictionary<string, object> frameDic = (Dictionary<string, object>)frame;
                                DBEffData data = new DBEffData();
                                data.eventDataList = new List<DBEffEvent>();
                                if (frameDic.ContainsKey("events"))
                                {
                                    List<object> eventList = (List<object>)frameDic["events"];
                                    foreach (object evt in eventList)
                                    {
                                        Dictionary<string, object> evtDic = (Dictionary<string, object>)evt;
                                        if ("event_effect" == (string)evtDic["name"])
                                        {
                                            data.animationName = (string)animDic["name"];
                                            List<object> strings = (List<object>)evtDic["strings"];
                                            DBEffEvent e = new DBEffEvent() {param = (string)strings[0],frameIndex = int.Parse(frameDic["duration"].ToString()) };
                                            data.eventDataList.Add(e);
                                            dataList.Add(data);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (GUILayout.Button(dataList[i].animationName,GUILayout.Width(230)))
            {
                clickIndex = i;
                selectIndex = 0;
                comp.animation.GotoAndStopByFrame(dataList[i].animationName, (uint)dataList[i].eventDataList[0].frameIndex);
            }
            if (i == clickIndex)
            {
                for(int j = 0; j < dataList[clickIndex].eventDataList.Count; j++)
                {
                    DrawDetail(j);
                }
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawDetail( int index )
    {
        DBEffEvent e = dataList[clickIndex].eventDataList[index];
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("特效名称",GUILayout.Width(70));
        EditorGUILayout.LabelField(e.param);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("帧", GUILayout.Width(70));
        EditorGUILayout.LabelField(e.frameIndex.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.ToggleLeft("当前", index == selectIndex,GUILayout.Width(100));

        if(GUILayout.Button("摆Pos", GUILayout.Width(70)))
        {
            selectIndex = index;
            comp.animation.GotoAndStopByFrame(dataList[clickIndex].animationName, (uint)e.frameIndex);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.EndVertical();
    }
}