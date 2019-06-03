**UI模块常用代码模板**
=======================

## BuildUI UI获取

	self.gameObject                       获取自身gameObject
	goutil.GetButton(go, 'Panel/Btn')   按钮
	goutil.GetImage(go, "Panel/Icon")	Imag

## collection对象获取
 
	for item in ilist(self.collections())
		if item.value.id = testId then
			item.value:xxFunction()
			item.value.isSelect(true)
		else
			item.value.isSelect(fase)
		end
	end

## BindValues 绑定

	self:BindValue(self.Icon, self.viewModel.icon, "overrideSprite")  --绑定按钮
	self:BindValue(self.Btn, closure(self.viewModel.OnBtnClick, self.viewModel))  --绑定按钮事件


## 基本属性绑定


	BindValues:
	self:BindValue(self.TabText, self.viewModel.txtColor, "color") //颜色
	self:BindValue(self.TabText, self.viewModel.txtText, "text") //文字
	self:BindValue(self.Icon, self.viewModel.icon, "overrideSprite") //精灵
	self:BindValue(self.Icon, self.viewModel.iconEnable, "enabled") //组件enable


## 按钮绑定

	self:BindEvent(self.GiftBtn, closure(self.viewModel.OnGiftBtnClick, self.viewModel))
	self:BindEvent(self.GiftBtn, function()
			self.viewModel:OnGiftBtnClick(...)
		end)


## 功能刷新绑定

	self:BindValue(self, function()
		local v = self.viewModel.property()
		--...
		end, 'justNeed')
	或者用闭包方法代替
	self.viewModel.OnFunction = function()
		...
	end
	在viewModel中调用该方法，用于反向调用view中对象

## SetActive显示绑定

	self:BindValue(self.Panel,self.viewModel.showPanel,nil,{bindType = DataBind.BindType.SetActive, invert = false})

## LoadChildPrefab 子UI绑定
	
	self:LoadChildPrefab(
		"xxSubView",
		function(task, prefab, cellCls)
			self:BindValue(
				self.bindObject,
				self.viewModel.xxViewCollections,
				nil,
				{bindType = DataBind.BindType.Collection, mainView = self, cellCls = cellCls, prefab = prefab}
			)
		end
	)

## 重新绑定（新建viewModel重新绑定到旧的view中，用于对象列表复用view情况）

    local idx = 0 //注意：此处idx从0开始
    for item in ilist(self.collections())
		local newVM = XXViewModel.new()
		self.collections.reBinding(idx, newVM)
		idx = idx + 1
	end


## 协程使用

    self.co = coroutine.start(function()
		coroutine.wait(0.1) --秒
		self.tabScrollVertical(1 - defaultTabIdx / #self.tabViewModels)
		coroutine.stop(self.co)
		self.co = nil
	end)


## 资源加载

    local loader = LoaderService.AsyncLoader("xxx Load 说明")
	local btnIconPath = "资源路径"
	loader:AddTask(
		btnIconPath,
		self.paramData["icon"],
		typeof(UnityEngine.Sprite), --typeof(UnityEngine.GameObject)
		function(task, icon, err)
			self.icon(icon) -- instGo = Unityengine.GameObject.Instantiate(go)

			self.loader = nil
		end
	):AutoUnloadBundle(true):Start()
	self.loader = loader


	--加载bundle，再加载里面的单个资源
	loader:AddTask(
        'texture/uiatlas/dynamic/union',
        function(task, bundle, err)
            if bundle then
                bundle:LoadAssetAsync(
                    'huangguan_' .. self.rank,
                    typeof(UnityEngine.Sprite),
                    function(sprite, err)
                        if sprite then
                            self.huangguanSprite(sprite)
                        end
                    end
                )
			end
		end
	):AutoUnloadBundle(true):Start()

	disepose调用:
	if self.loader then
		self.loader:UnloadAllBundles()
		self.loader:Cancel()
		self.loader = nil
	end

## Dotween组件

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


## 事件注册

SceneService:addListener(SceneService.OnSceneLoadedEvent, instance.OnSceneLoaded)
SceneService:removeListener(SceneService.OnSceneLoadedEvent, instance.OnSceneLoaded)

--根据回调方法决定是否需要传参
UnionService:addListener(UnionService.SucGetPointMemberListReplyEvent, self.OnSucGetPointMemberListReplyEvent, self)
UnionService:removeListener(UnionService.SucGetPointMemberListReplyEvent,self.OnSucGetPointMemberListReplyEvent,self)


## 奖励预览/可能获取

    local dataList = {}
    for _, v in ipairs(bonusConfig.Bonus) do
        local data = {
            materialType = v.type, --类型
            id = v.id, --物品id
            num = v.num --物品数量
        }
        table.insert(dataList, data)
    end
    UIManager:Open('ShopItemDetailView', dataList, '可能获得', '奖励预览', true)

## 宝箱领取

    local bounds = {}
    for _, v in ipairs(data) do
        table.insert(
            bounds,
            {
                materialType = v.type, --类型
                id = v.id, --物品id
                quantity = v.num --物品数量
            }
        )
    end

    UIManager:Open(
        'OpenGiftView',
        1,
        bounds,
        function()
            --确定后回调
        end
    )

## 显示tips

    UIManager.tipsEntry:ShowArrTips(
        {
            string.format('你已经为该玩家的亚比经验池增加%d经验'),
            string.format('你的战队贡献增加了%d点')
        }
    )

	UIManager.tipsEntry:ShowArrTips({string.format('信息')})


## 显示物品获得tips

	UIManager.getItemEntry:ShowNormalItemArrTips(msg.bonuses)

将服务器给到的UserMaterials（bonuses）列表数据给传入即可，可以直接用protobuff获取的数据传入即可。ShowNormalItemArrTips内部已经重新深拷贝了一个table作为tips所需的数据。



## 确认弹窗

    UIManager.dialogEntry:ShowConfirmDialog('该名玩家已被你指点')

--[[
	msg:内容
	confirmCallback:确认回调
	confirmName:确认按钮上的字，默认“确认”，可为空
	toggleName:勾选上的字，默认“今日不再显示”，可为空
	toggleCallback:勾选后点击确定的回调
]]

    UIManager.dialogEntry:ShowConfirmDialog( msg,confirmCallback,confirmName,toggleName,toggleCallback)

## 确认取消弹窗

--[[
	msg:内容
	confirmCallback:确认回调
	cancelCallback:取消回调
	confirmName:确认按钮上的字，默认“确认”，可为空
	cancelName:取消按钮上的字，默认“取消”，可为空
	toggleName:勾选上的字，默认“今日不再显示”，可为空
	toggleCallback:勾选后点击确定的回调
]]

    UIManager.dialogEntry:ShowConfirmCancelDialog( msg,confirmCallback,cancelCallback,confirmName,cancelName,toggleName,toggleCallback )

## 导表解析

	local unionTaskConfig = require("Commons/Config/uniontask/UnionTaskConfig")
	instance.unionTaskConfig = AQ.ConfigUtil.Convert(unionTaskConfig,{"Id"})

通过instance.unionTaskConfig[Id]进行获取

	local unionTaskConfig = require("Commons/Config/uniontask/UnionTaskConfig")
	instance.unionDungeonBonusConfig = AQ.ConfigUtil.Convert(unionDungeonBonusConfig,{"Level", "BonusId"})
	

解析多个主键时，需要组合key进行获取
local key = string.format('%d_%d', Level, BonusId)
instance.unionDungeonBonusConfig[key]


## 图片打灰/修改灰色材质球


self:BindValue(self.OnlineFlag, vm.onlineImgMaterial, 'material')

instance.GRAY_COLOR =  Color.New(100/255,100/255,100/255,1)
instance.GARY_MATERIAL = AQ.UIEffectUtil.CreateGray(instance.GRAY_COLOR)

绑定Image的的material属性，修改material属性即可
self.onlineImgMaterial(GARY_MATERIAL)