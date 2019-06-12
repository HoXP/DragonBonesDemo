using UnityEngine;
using DragonBones;

/// <summary>
/// 龙骨中介类，主要为了减少xlua生成的cpp代码量。
/// 需要扩展功能时，都往这个类里添加。
/// 使用方法：
/// 在需要使用龙骨动画的游戏物体上挂载这个类即可。
/// </summary>
[RequireComponent(typeof(UnityArmatureComponent))]
public class DragonBonesProxy : MonoBehaviour {
	public OnCallBackString FrameEvent;
	public OnCallBackString LoopCompleteEvent;
	public OnCallBackString CompleteEvent;
	private UnityArmatureComponent dragonBonesCom;

	void Awake() {
		dragonBonesCom = GetComponent<UnityArmatureComponent>();
	}

	void OnEnable() {
		if(dragonBonesCom != null)
		{
            //帧事件回调
			dragonBonesCom.AddDBEventListener(EventObject.FRAME_EVENT,OnFrameEvent);
            //loop播完一次回调
			dragonBonesCom.AddDBEventListener(EventObject.LOOP_COMPLETE,OnLoopCompleteEvent);
            //播放完成回调
			dragonBonesCom.AddDBEventListener(EventObject.COMPLETE,OnCompleteEvent);
		}
	}

	void OnDisable() {
		if(dragonBonesCom != null)
		{
			dragonBonesCom.RemoveDBEventListener(EventObject.FRAME_EVENT,OnFrameEvent);
			dragonBonesCom.RemoveDBEventListener(EventObject.LOOP_COMPLETE,OnLoopCompleteEvent);
			dragonBonesCom.RemoveDBEventListener(EventObject.COMPLETE,OnCompleteEvent);
		}
	}

    //void Update()
    //{
    //    if(Input.GetKeyUp(KeyCode.Alpha1))
    //    {
    //        Play("sayHi", 1, 1);
    //    }
    //}
    public Rect GetRectColider(string name)
    {
        if(dragonBonesCom != null && dragonBonesCom.animation != null)
        {
            Slot slot = dragonBonesCom.armature.GetSlot(name);
            if(slot != null)
            {
                BoundingBoxData boundingBoxData = slot.boundingBoxData;
                if(boundingBoxData != null)
                {
                    var tx = slot.globalTransformMatrix.tx;
                    var ty = slot.globalTransformMatrix.ty;
                    var boundingBoxWidth = boundingBoxData.width;
                    var boundingBoxHeight = boundingBoxData.height;
                    //var leftTopPos = new Vector3(tx - boundingBoxWidth * 0.5f, ty + boundingBoxHeight * 0.5f, 0.0f);
                    //var leftBottomPos = new Vector3(tx - boundingBoxWidth * 0.5f, ty - boundingBoxHeight * 0.5f, 0.0f);
                    //var rightTopPos = new Vector3(tx + boundingBoxWidth * 0.5f, ty + boundingBoxHeight * 0.5f, 0.0f);
                    //var rightBottomPos = new Vector3(tx + boundingBoxWidth * 0.5f, ty - boundingBoxHeight * 0.5f, 0.0f);
                    return new Rect(tx, ty, boundingBoxWidth, boundingBoxHeight);
                }
            }
        }
        return new Rect(0, 0, 0, 0); 
    }

    private void OnCompleteEvent(string type, EventObject eventObject)
    {
		if(CompleteEvent != null)
		{
        	string param = "{" + string.Format("\"eventName\":\"complete\",\"animationName\":\"{0}\"",eventObject.animationState._animationData.name) + "}";
			CompleteEvent.Invoke(param);
		}
    }

    private void OnLoopCompleteEvent(string type, EventObject eventObject)
    {
		if(LoopCompleteEvent != null)
		{
        	string param = "{" + string.Format("\"eventName\":\"loopComplete\",\"animationName\":\"{0}\"",eventObject.animationState._animationData.name) + "}";
			LoopCompleteEvent.Invoke(param);
		}
    }

    private void OnFrameEvent(string type, EventObject eventObject)
    {
        if (FrameEvent != null)
        {
            string param = "{" + string.Format("\"eventName\":\"{0}\",\"eventParams\":\"{1}\",\"animationName\":\"{2}\"",eventObject.name,eventObject.data.GetString(),eventObject.animationState._animationData.name) + "}";
            FrameEvent.Invoke(param);
        }
    }

    /// <summary>
    /// 反注册委托事件
    /// </summary>
    /// <param name="type"></param>
    public void UnsetEvent(int type)
    {
        switch(type)
        {
            case 1:
                FrameEvent = null;
                break;
            case 2:
                CompleteEvent = null;
                break;
            case 3:
                LoopCompleteEvent = null;
                break;
        }
    }

    /// <summary>
    /// 检测是否有动画
    /// </summary>
    /// <param name="animationName">动画</param>
    /// <returns></returns>
    public bool HasAnimation(string animationName)
    {
        if(!string.IsNullOrEmpty(animationName))
        {
            return dragonBonesCom.animation.animationNames.Exists( animName =>
            {
                return animationName.Equals(animName);
            });
        }
        return false;
    }

    /// <summary>
    /// 播放指定动画
    /// </summary>
    /// <param name="animationName">动画名</param>
    /// <param name="playTimes">循环播放次数。 [-1: 使用动画数据默认值, 0: 无限循环播放, [1~N]: 循环播放 N 次] </param>
    /// <param name="speed"> [(-N~0): 倒转播放, 0: 停止播放, (0~1): 慢速播放, 1: 正常播放, (1~N): 快速播放]</param>
    public void Play(string animationName, int playTimes,float speed)
	{
        if(!HasAnimation(animationName))
        {
            throw new System.Exception(string.Format("找不到动画名为{0}的动画，请找产品切磋！！", animationName));
        }
		if (dragonBonesCom != null && dragonBonesCom.animation != null)
		{
			dragonBonesCom.animation.Play(animationName,playTimes).timeScale = speed;
		}
	}

    /// <summary>
    /// 淡入播放指定的动画。往往用在动画融合上，即同时播放多个动画
    ///这里需要解释的一点就是 DragonBones 骨骼动画在运行时有一个组的概念，我们可以让动画在一个组中播放，当另一个动画被设置为在相同组中播放时，
    ///之前播放的同组动画就会停止，所以我们可以把希望同时播放的动画放在不同的组里。我们可以把开火放到上半身组，跑步放到下半身组，
    /// 这样角色就可以同时开火和跑步了。
    /// </summary>
    /// <param name="animationName">动画名</param>
    /// <param name="group">组的名称，融合动画时，需要把不同的动画放在不同的组里</param>
    /// <param name="playTimes">循环播放次数。 [-1: 使用动画数据默认值, 0: 无限循环播放, [1~N]: 循环播放 N 次]</param>
    /// <param name="speed">[(-N~0): 倒转播放, 0: 停止播放, (0~1): 慢速播放, 1: 正常播放, (1~N): 快速播放]</param>
    public void FadeIn(string animationName,string group, int playTimes,float speed)
	{
		if (dragonBonesCom != null && dragonBonesCom.animation != null)
		{
			DragonBones.AnimationState state = dragonBonesCom.animation.FadeIn(animationName,0.2f,playTimes,0,group);
			state.timeScale = speed;
			state.resetToPose = false;
		}
	}

    /// <summary>
    /// 停止播放指定动画，null停止所有动画播放
    /// </summary>
    /// <param name="animationName">动画名</param>
	public void Stop(string animationName)
	{
		if (dragonBonesCom != null && dragonBonesCom.animation != null)
		{
			dragonBonesCom.animation.Stop(animationName);
		}
	}

    /// <summary>
    /// 更换龙骨动画一个插槽里显示对象的显示，如果看不懂，去了解一下龙骨的一些基本概念
    ///http://developer.egret.com/cn/github/egret-docs/DB/dbPro/interface/mainInterface/index.html
    /// </summary>
    /// <param name="slotName">插槽名字</param>
    /// <param name="displayName">显示对象名字</param>
    /// <returns></returns>
	public bool ReplaceSlotDisplay(string slotName,string displayName)
	{
		ArmatureData armatureData = dragonBonesCom.armature.armatureData;
		if (armatureData == null || armatureData.defaultSkin == null)
		{
			return false;
		}
		Slot slot = dragonBonesCom.armature.GetSlot(slotName);
		if (slot == null)
		{
			return false;
		}
		var displayData = armatureData.defaultSkin.GetDisplay(slotName, displayName);
		if (displayData == null)
		{
			return false;
		}
		UnityFactory.factory.ReplaceDisplay(slot, displayData);
		return true;
	}

    /// <summary>
    /// 横向翻转动画
    /// </summary>
    /// <param name="flipX"></param>
	public void FlipX(bool flipX)
	{
		if (dragonBonesCom != null && dragonBonesCom.armature != null)
			dragonBonesCom.armature.flipX = flipX;
	}

    /// <summary>
    /// 纵向翻转动画
    /// </summary>
    /// <param name="flipY"></param>
	public void FlipY(bool flipY)
	{
		if (dragonBonesCom != null && dragonBonesCom.armature != null)
			dragonBonesCom.armature.flipY = flipY;
	}

	void OnDestroy() {
		FrameEvent = null;
		LoopCompleteEvent = null;
		CompleteEvent = null;
	}

}
