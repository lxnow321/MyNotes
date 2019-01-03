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

	self:BindEvent(self.GiftBtn, closure(vm.OnGiftBtnClick, vm))

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
		end, 'jsutNeed')

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
