**UI模块常用代码模板**
=======================

## 1.BuildUI UI获取

	self.gameObject                       获取自身gameObject
	goutil.GetButton(go, 'Panel/Btn')   按钮
	goutil.GetImage(go, "Panel/Icon")	Imag

## 2.BindValues 绑定

	self:BindValue(self.Icon, self.viewModel.icon, "overrideSprite")  --绑定按钮
	self:BindValue(self.Btn, closure(self.viewModel.OnBtnClick, self.viewModel))  --绑定按钮事件


## 1.文字绑定

	View:

	BuildUI:
	self.GiftDesc = goutil.GetText(self.BottomGroup, 'Content/GiftDesc')

	BindValues:
	self:BindValue(self.TabText, self.viewModel.txtColor, "color") //颜色
	self:BindValue(self.TabText, self.viewModel.txtText, "text") //文字

	ViewModel:

	self.txtColor = self.createProperty(self.TXT_NORMAL_COLOR)
	self.txtText = self.createProperty(tabName)

## 2.按钮绑定

	View:

	BuildUI:
	self.GiftBtn = goutil.GetButton(self.BottomGroup, 'Content/GiftBtn')

	BindEvents:
	self:BindEvent(self.GiftBtn, closure(vm.OnGiftBtnClick, vm))


ViewModel:

## 3.功能刷新绑定

	self:BindValue(self, function()
		local v = self.viewModel.property()
		--...
		end, 'justNeed')

## 4.SetActive显示绑定

	self:BindValue(self.Panel,self.viewModel.showPanel,nil,{bindType = DataBind.BindType.SetActive, invert = false})


## 5.协程使用

    self.co = coroutine.start(function()
		coroutine.wait(0.1) --秒
		self.tabScrollVertical(1 - defaultTabIdx / #self.tabViewModels)
		coroutine.stop(self.co)
		self.co = nil
	end)


## 6.资源加载

    local loader = LoaderService.AsyncLoader("xxx Load 说明")
	local btnIconPath = "资源路径"
	loader:AddTask(
		btnIconPath,
		self.paramData["icon"],
		typeof(UnityEngine.Sprite),
		function(task, icon, err)
			self.icon(icon)
			self.loader = nil
		end
	):AutoUnloadBundle(true):Start()
	self.loader = loader

	disepose调用:
	if self.loader then
		self.loader:Cancel()
		self.loader = nil
	end

## 7.Dotween组件

	self:BindValue(self, function()
		local value = vm.startShowFlagImage() 
		if value then
			local colorDT = AQ.LuaComponent.Add(self.FlashImage.gameObject, AQ.Dotween.Dotween2D.DTColorImage)
			colorDT:SetParams({
				--{color = ...多次dotween},
				{color = UnityEngine.Color.New(1, 1, 1, 0), delay = 0, duration = 1, 
				func = DG.Tweening.Ease.Linear, updateType = DG.Tweening.UpdateType.Normal, isIndependentTimeScale = false ,
				callback = function() 
					--回调方法
				end}
			})
			colorDT:OnEnable() //看是否需要，如果挂在组件的对象是从disable到enable状态，那么可以不用加。组件内的执行时通过OnEnable中调用的，如果SetParams之前已经调用OnEnable，那么不会生效，故需要手动再调用一次OnEnable方法。
		end
	end, 'justNeed')


## Texture2D转Sprite

local tex2D = Texture2D.New()
local sprite = UnityEngine.Sprite.Create(tex2D, UnityEngine.Rect.New(0, 0, tex2D.width, tex2D.height), UnityEngine.Vector2.zero)

注意:第二个参数rect数据是tex2D的纹理范围，如果rect中的宽高大小小于tex2D的实际纹理大小，那么转换出来的sprite只是tex2D的（0，0）位置到传入的宽高大小的范围的纹理。