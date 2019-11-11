**UI模块常用代码模板**
=======================


## 底板大小/BG

1656 * 960

## SearchIllegalTextureContext.txt修改

GetLongYing:GetLongYing:GetLongYing/GetLongYingMain;GetLongYing/GetLongYingGame;GetLongYing/GetLongYingFastPlan:

预制文件夹：默认检测的文件夹名：用到的其他图集文件夹名（用封号';'隔开）

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


## 基本属性绑定/BindValue

	BindValues:

	self.Name = goutil.GetText(go, 'Name')

	self:BindValue(self.Name, vm.NameText, "text") //文字
	self:BindValue(self.Name, vm.NameColor, "color") //颜色

	self.Img = goutil.GetImage(go, 'Img')
	self:BindValue(self.Img, self.viewModel.Img, "overrideSprite") //精灵
	self:BindValue(self.Img, self.viewModel.ImgEnabled, "enabled") //组件enable
	self:BindValue(self.Img, vm.ImgFillAmount, 'fillAmount') --绑定fillAmount

	self.SanjiaoRect = goutil.GetRectTransform(self.Center, 'SortGroup/Sanjiao')
	self:BindValue(self.SanjiaoRect, vm.SanjiaoRatation, 'localRotation')

	--scrollview窗口移动  Vector2(水平，垂直)
	self:BindValue(self.BigListScrollView, vm.bigListScrollViewPosition, 'normalizedPosition')

	--Slider
	self:BindValue(self.RewardProgressSlider, vm.RewardProgressSliderValue, 'value')




## 基本事件绑定/BindEvent

	self:BindEvent(self.GiftBtn, closure(self.viewModel.OnGiftBtnClick, self.viewModel))
	self:BindEvent(self.GiftBtn, function()
			self.viewModel:OnGiftBtnClick(...)
		end)


## 功能刷新绑定/Function

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

	self:BindValue(self.Panel, vm.showPanel,nil,{bindType = DataBind.BindType.SetActive, invert = false})

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

## 重新绑定/reBinding

	（新建viewModel重新绑定到旧的view中，用于对象列表复用view情况）
    local idx = 0 //注意：此处idx从0开始
    for item in ilist(self.collections())
		local newVM = XXViewModel.new()
		self.collections.reBinding(idx, newVM)
		idx = idx + 1
	end


## 协程使用/Coroutine

    self.co = coroutine.start(function()
		coroutine.wait(0.1) --秒
		self.tabScrollVertical(1 - defaultTabIdx / #self.tabViewModels)
		coroutine.stop(self.co)
		self.co = nil
	end)


## 资源加载/Loader

    local loader = LoaderService.AsyncLoader("xxx Load 说明")
	loader:AddTask(
		'资源路径',
		'资源名称',
		typeof(UnityEngine.Sprite), --typeof(UnityEngine.GameObject)
		function(task, icon, err)
			if not goutil.IsNil(icon) then
				self.icon(icon) -- instGo = Unityengine.GameObject.Instantiate(go)
			end
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
		self.loader:UnloadAllBundles() --看情况加，一般不需要
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


## 事件注册/Event

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
    UIManager:Open('ShopItemDetailView', dataList, '奖励预览', '框内描述', false)

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

如果直接取导表的奖励显示，则需要重新组合bonuse参数：注意：需要传列表，切参数对应
bonus = {num=1,id=800001,type=1}
UIManager.getItemEntry:ShowNormalItemArrTips({{materialType = bonus.type, id = bonus.id, quantity = bonus.num}})



## 确认弹窗

    UIManager.dialogEntry:ShowConfirmDialog('该名玩家已被你指点')

--[[
	msg:内容
	confirmCallback:确认回调
	confirmName:确认按钮上的字，默认“确认”，可为空
	toggleName:勾选上的字，默认“今日不再显示”，可为空
	toggleCallback:勾选后点击确定的回调
]]

    UIManager.dialogEntry:ShowConfirmDialog(msg,confirmCallback,confirmName,toggleName,toggleCallback)

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

	local DoXingbiFill = function()
		if MaterialService.XingBiIsEnough(xingbiCost) then
			--星币足够
			--TODO:DO
		else
			--星币不足
			UIManager.dialogEntry:ShowConfirmCancelDialog(
				string.format('星币不足，是否前往充值?'),
				function()
					ChargeService.OpenViewById(ChargeService.UI_ID_XINGBI)
				end
			)
		end
	end

	if not RosefinchService.IgnoreXingbiFillDialoge then
		UIManager.dialogEntry:ShowConfirmCancelDialog(
			string.format('是否消耗%s星币翻开卡牌', xingbiCost),
			function()
				DoXingbiFill()
			end,
			nil,
			'确定',
			'取消',
			'本次登陆不再提醒',
			function()
				RosefinchService.IgnoreXingbiFillDialoge = true
			end
		)
	else
		DoXingbiFill()
	end

## 导表解析

	local unionTaskConfig = require("Commons/Config/uniontask/UnionTaskConfig")
	instance.unionTaskConfig = AQ.ConfigUtil.Convert(unionTaskConfig,{"Id"})

通过instance.unionTaskConfig[Id]进行获取

	local unionTaskConfig = require("Commons/Config/uniontask/UnionTaskConfig")
	instance.unionDungeonBonusConfig = AQ.ConfigUtil.Convert(unionDungeonBonusConfig,{"Level", "BonusId"})
	

解析多个主键时，需要组合key进行获取
local key = string.format('%d_%d', Level, BonusId)
instance.unionDungeonBonusConfig[key]


## 图片打灰/修改灰色材质球/Color


	self:BindValue(self.OnlineFlag, vm.onlineImgMaterial, 'material')

	instance.GRAY_COLOR =  Color.New(100/255,100/255,100/255,1)
	instance.GARY_MATERIAL = AQ.UIEffectUtil.CreateGray(instance.GRAY_COLOR)

	绑定Image的的material属性，修改material属性即可
	self.onlineImgMaterial(GARY_MATERIAL)


## 无线滚动/ScrollView

	self.cellsContentScroll = AQ.InfiniteScrollView.Get(scrollGo)
	self:LoadChildPrefab(
		'MessageBoxCellView',
		function(task, prefab, cellCls)
			self:BindValue(
				self.cellsContentScroll,
				vm.cellCollection,
				nil,
				{
					bindType = DataBind.BindType.ScrollRectCollection,
					mainView = self,
					cellCls = cellCls,
					prefab = prefab,
					params = {DataBind.ScrollDir.Vertical, 190, 245, -12, -20, 5}
					--width,height,widthSpacing,heightSpacing,perlineNum
					--宽，高，宽空隙，高空隙,每行个数
				}
			)
		end
	)
 
 注意：

* InfiniteScrollView组件是自动加上去的，Prefab上的Content不需要加ContentSizeFitter以及LayoutGroup等。
* InfiniteScrollVie组件永远是将子UI左贴齐的，所以调整ScrollView窗口大小时注意调整好


## 事件监听/addListener

	--对应Service.Init()方法中，必须扩展NotifyDispatcher，否则不具备监听特性
	NotifyDispatcher.extend(instance)

	--监听方法主体
    HouseService:addListener(HouseService.QueryAllBoxMessageReplyEvent, self.OnQueryAllBoxMessageReplyEvent, self)

	HouseService:removeListener(HouseService.QueryAllBoxMessageReplyEvent, self.OnQueryAllBoxMessageReplyEvent, self)


## InputFiled实现限制文字数量

	BindUI:
	self.MsgInputField = goutil.GetInputField(self.Panel, 'MsgInputField')

	BindValues:
	vm.MsgInputField = self.MsgInputField

	BindEvent:
	self:BindEvent(self.MsgInputField, closure(vm.OnMsgInputFieldValueChanged, vm))

	ViewModel:
	function MessageBoxOperationViewModel:OnMsgInputFieldValueChanged(value)
		local err, content = PlayerService.CheckNameInputField(value)
		local validContent = err and content or value
		if StringUtil.UTF8lenLong(validContent) > self.MsgLength then
			validContent = StringUtil.InterceptLong(validContent, self.MsgLength)
		end
		self.MsgInputField.text = validContent
	end

## 时间格式/time/os.date

os.date('时间：%Y.%m.%d %H:%M:%S', self.time)
os.date(format, time)
format以'!'开头 表示世界协调时间



os.date('*t') 返回一个table，包含year,month,day,hour,min,sec,wday,yday,isdst字段

如果format不是'*t'，则格式字符串与 C方法的strftime规则相同，参考：http://www.cplusplus.com/reference/ctime/strftime/

os.date() 与 os.date('%c')相同，返回一个时间字符串 如：03/23/19 14:46:20 （表示2019.3.23）


--剩余时间格式 xx:xx:xx
leftTime = 1563520241
local hour = math.floor(leftTime / 3600)
local min = math.floor((leftTime - hour * 3600) / 60)
local sec = leftTime - hour * 3600 - min * 60
str = string.format('%02d:%02d:%02d', hour, min, sec)


## Timer/定时器/一般用法

	Timer.New(
		function()
			--回调
		end,
		0.01, --执行间隔时间
		-1, -- -1表示循环，>0 表示执行次数
		true  --scale是否受时间scale影响
	)

例：

>循环执行

self.timer = Timer.New(function()
end,
1, -1, true)

> 执行一次

self.timer = Timer.New(function()
end,
1, 1, true)

self.timer:Start() 

self.timer:Stop()

## Timer/倒计时
function GetLongYingGameViewModel:StartCDTimer()
	self:StopCDTimer()
	self.cdStartTime = PlayerService.GetSystemSecondTime() + self.cd
	self.CdTimeText(string.format('%ds', self.cd))
	self.CdTimer =
		Timer.New(
		function()
			self.cd = self.cdStartTime - PlayerService.GetSystemSecondTime() --防止切后台后定时器停止导致时间没有校准
			if self.cd <= 0 then
				self.cd = 0
				self:StopCDTimer()
			end
			self.CdTimeText(string.format('%ds', self.cd))
		end,
		0.5,
		-1,
		true
	)
	self.CdTimer:Start()
end

function GetLongYingGameViewModel:StopCDTimer()
	if self.CdTimer then
		self.CdTimer:Stop()
		self.CdTimer = nil
	end
end


## GetParentViewModelByName

local growUpGuideMainViewModel = self:GetParentViewModelByName("GrowUpGuideMainViewModel")


## 判断材料或物品是否充足/CheckMaterialIsEnough/星币不足

MaterialService.CheckMaterialIsEnough(materialType, id, num)

--星币不足
if MaterialService.XingBiIsEnough(xingbiCost) then
	--星币足够
	--DO
else
	--星币不足
	UIManager.dialogEntry:ShowConfirmCancelDialog(
		string.format('星币不足，是否前往充值?'),
		function()
			ChargeService.OpenViewById(ChargeService.UI_ID_XINGBI)
		end
	)
end


## 跳转充值界面

ChargeService.UI_ID_XINGBI = 1 --星币
ChargeService.UI_ID_BLUE_STONE = 2  --蓝宝石
ChargeService.UI_ID_MONTH_CHARD = 3  --月卡

ChargeService.OpenViewById(ChargeService.UI_ID_XINGBI) --星币充值


## pm头像/亚比头像/宠物头像

local modelId = SkinService.ConvertPetModelId(raceId)
AssetLoaderService.PetIcon(
	AssetLoaderService.PET_ICON_90_90,
	modelId,
	function(icon)
		if not goutil.IsNil(icon) then
			self.PMIcon(icon)
		end
	end
)

## PM/亚比模型加载

* AssetLoaderService.PetGO  接口加载
* PetPackage 亚比背包的分布加载

* SkinService.LoadSkinPrefab 加载模型

	local skinId = AQ.PetPackage.PetPackageConfigSetting.GetDefaultSkinId(raceId)

	local prefabName = AQ.PetPackage.PetPackageConfigSetting.GetModelPrefabName(skinId)
	local animName = AQ.PetPackage.PetPackageConfigSetting.GetModelAnimName(skinId)


## Dotween使用

if self.heightTween then
	self.heightTween:Kill(true)
	self.heightTween = nil
end
self.heightTween = DG.Tweening.DOTween.To(a, b, targetHeight, 0.3)
self.heightTween:OnUpdate(function()
	self.subTabSizeDelta(Vector2(CELL_SIZE.x, currentHeight))
	self.sizeDelta(Vector2(BACKGROUND_SIZE.x, BACKGROUND_SIZE.y +  currentHeight))
end)

self.heightTween:OnComplete(function()
	self.subTabSizeDelta(Vector2(CELL_SIZE.x, currentHeight))
	self.sizeDelta(Vector2(BACKGROUND_SIZE.x, BACKGROUND_SIZE.y +  currentHeight))
	self.heightTween = nil
	self.reActivateParentPT({})
end)




if self.ScrollTween then
	self.ScrollTween:Kill(true)
	self.ScrollTween = nil
end

self.ScrollTween =
	DG.Tweening.DOTween.To(
	DG.Tweening.Core.DOSetter_float(
		function(value)
			--POSVector2.x = value
			--self.LevelCellScrollRect.normalizedPosition = POSVector2
			--直接用value干事情呀
		end
	),
	startPosX, --起始值
	endPosX, --最终值
	0.3  --时间间隔
):SetEase(DG.Tweening.Ease.OutCubic):OnComplete(
	function()
		self.ScrollTween = nil
	end
)


## DOTWEEN/渐变

self:BindValue(
	self,
	function()
		local idx = vm.showFadeinIdx()
		local go = self.RoleGos[idx]
		if not goutil.IsNil(go) then
			local image = goutil.GetImage(go, '')
			image.color = UnityEngine.Color.New(1, 1, 1, 0)
			self.lastTween = image:DOFade(1, 3)
			self.lastTween:OnComplete(
				function()
					self.lastTween = nil
				end
			)
		end
	end,
	'justNeed'
)

## ToggleGroup
可在Toggle按钮父节点（其他也可以）挂在一个ToggleGroup脚本组件，然后将该节点拖拽至Toggle控件中的Group即可。同一个ToggleGroup只会有一个Toggle组件被选中


## 拖拽/移动

准备：
1. 拖拽图标上需要挂在 UIDragTrigger 和 UIClickTrigger脚本
2. 父界面需要增加一个用于拖拽显示的图标（跟随鼠标移动）

View:

function MountActivationPMCellView:BindEvents()
	local vm = self.viewModel

	--点击按下事件
	self:BindEvent(
		self.PMIconClickTrigger,
		function(eventData)
			print('MountActivationPMCellView PointerDown', self.gameObject)
			self.clickDown = true
			self.startDragging = false

			vm:SetMovingPmPosition(self.PMIconRectTransform, eventData)
			vm:ShowMovingPm(true)
		end,
		eventtype.PointerDown
	)

	--点击弹起事件
	self:BindEvent(
		self.PMIconClickTrigger,
		function(eventData)
			print('MountActivationPMCellView PointerUp', self.gameObject)
			if not self.clickDown or self.startDragging then
				return
			end

			vm:ShowMovingPm(false)
		end,
		eventtype.PointerUp
	)

	--开始拖拽事件
	self:BindEvent(
		self.PMIconDragTrigger,
		function(eventData)
			print('MountActivationPMCellView BeginDrag', self.gameObject, self.clickDown)
			if not self.clickDown then
				return
			end
			self.startDragging = true
			vm:SetMovingPmPosition(self.PMIconRectTransform, eventData)
		end,
		eventtype.BeginDrag,
		nil,
		nil,
		true
	)

	--拖拽中事件
	self:BindEvent(
		self.PMIconDragTrigger,
		function(eventData)
			print('MountActivationPMCellView Drag', self.gameObject, self.clickDown)
			if not self.clickDown then
				return
			end
			vm:SetMovingPmPosition(self.PMIconRectTransform, eventData)
		end,
		eventtype.Drag,
		nil,
		nil,
		true
	)

	--结束拖拽事件
	self:BindEvent(
		self.PMIconDragTrigger,
		function(eventData)
			print('MountActivationPMCellView EndDrag', self.gameObject, self.clickDown)
			if not self.clickDown then
				return
			end
			vm:EndDragHandler(eventData)
			vm:ShowMovingPm(false)
			self.clickDown = false
		end,
		eventtype.EndDrag,
		nil,
		nil,
		true
	)
end


ViewModel:

--设置移动位置

	function MountActivationPMCellViewModel:SetMovingPmPosition(diRectTransform, eventData)
		local success, position =
			UnityEngine.RectTransformUtility.ScreenPointToWorldPointInRectangle(
			diRectTransform,
			eventData.position,
			eventData.pressEventCamera,
			nil
		)
		if success then
			self.pParentViewModel:MoveMovingPmUI(position)
		end
	end

--显示拖拽图标

	function MountActivationPMCellViewModel:ShowMovingPm(show)
		if show then
			self.pParentViewModel:ShowMovingPm(true, self.pmRaceId)
		else
			self.pParentViewModel:ShowMovingPm(false)
		end
	end

--结束拖拽

	function MountActivationPMCellViewModel:EndDragHandler(eventData)
		local success, petInfoGO = self.CheckIsMountActivationCell(eventData.pointerCurrentRaycast.gameObject)
		if success then
			local view = AQ.LuaComponent.Get(petInfoGO, AQ.UI.Mount.MountActivationCellView)
			if view then
				local receivedVM = view.viewModel
				if receivedVM then
					self.pParentViewModel:PutPM(receivedVM:GetIdx(), self.pmId, self.pmRaceId)
				end
			end
		end
	end

--判断是否是拖拽目标

	function MountActivationPMCellViewModel.CheckIsMountActivationCell(receiveGO)
		if not receiveGO or receiveGO.name ~= 'PMIcon' then
			return false
		end

		local parent = receiveGO.transform.parent
		if not parent and not parent.parent then
			return false
		end

		return true, parent.parent.gameObject
	end

## 亚比头像/种类图标/亚比图鉴

	--亚比头像
	local modelId = SkinService.ConvertPetModelId(raceId, skinId)
	AssetLoaderService.PetIcon(
		AssetLoaderService.PET_ICON_90_90,
		modelId,
		function(icon)
			if not goutil.IsNil(icon) then
				self.PMIcon(icon)
			end
		end
	)

	--亚比种类图标
	local typeId = CommonSetting.PMSpiritConfig[pmRaceId].Type0
	或者
	local typeId = pmInfo.pmType
	AssetLoaderService.TypeIcon(
		AssetLoaderService.TYPE_ICON_36_36,
		typeId,
		function(icon)
			if not goutil.IsNil(icon) then
				self.FamilyIcon(icon)
			end
		end
	)

	--亚比图鉴
	local modelId = SkinService.ConvertPetModelId(pmRaceId, pmSkinId)
	AssetLoaderService.PetIcon(
		AssetLoaderService.PET_ICON_144_208,
		modelId,
		function(icon)
			if not goutil.IsNil(icon) then
				self.MountImage(icon)
			end
		end
	)


## PlayerPrefs

PlayerPrefs.GetInt(KEY, 0)
PlayerPrefs.SetInt(KEY, 1)


## 显示货币

UIManager.coinEntry:ShowCoin(self.gameObject, self.__cname)


## 剧情播放

--模块名
CutsceneService

--播文字剧情
CutsceneService.PlayChatByFile(
	'剧情文件名',
	nil,
	false,
	function()
		--播完后处理
	end
)

## 5点刷新事件

PlayerService:addListener(PlayerService.EVENT_HOUR_5_SERVER_UPDATE, self.OnHour5UpdateEvent, self)

PlayerService:removeListener(PlayerService.EVENT_HOUR_5_SERVER_UPDATE, self.OnHour5UpdateEvent, self)


## 亚比克制/对阵克制/亚比类型

if pmType then
	UIManager:Open('RestraintInfoPanelView', pmType)
end


## app锁屏/切后台

游戏app锁屏返回可监听AppPauseEvent事件
AppPauseEvent:Add(self.OnAppPause, self)


## 跨周时间显示/活动配置时间

--返回下周周一5点时间
function EliteTestMainViewModel:_CheckWeekEndTime()
	local curTime = PlayerService.GetSystemSecondTime()
	local curWday = os.date('%w', curTime)
	local endTimeTable = os.date('*t', curTime)
	endTimeTable.hour = 5
	endTimeTable.min = 0
	endTimeTable.sec = 0
	if curWday ~= 1 or hour >= 5 then
		--跨过了当周的周一5点
		endTimeTable.day = endTimeTable.day + (8 - curWday)
	end

	local endtime = os.time(endTimeTable) or 0
	return endtime
end

--返回配置的activity_switch结束时间
function EliteTestMainViewModel:_CheckWeekEndTime()
	if self.groupId then
		local activitySwitchId = AQ.WelfareActivity.OperationTaskConfigSetting.GetOperationGroupOnlineId(self.groupId)
		local activitySwitchConfig = AQ.ActivitySvc.ActivitySetting.ActivitySwitchConfig[activitySwitchId]
		local activityTimeConfig = activitySwitchConfig.OnlineTimeIntervals and activitySwitchConfig.OnlineTimeIntervals[1]
		if activityTimeConfig then
			return os.time(activityTimeConfig.endTime)
		end
	end
	return PlayerService.GetSystemSecondTime()
end


## 取位数据/BitUtil

--[[检测某一位的值是否为1，是1返回true，是0返回false
 * @param value 需要检测的数字
 * @param bitIndex 需要检测的下标
 * @return 某一位的值是1返回true，是0返回false
--]]
BitUtil.CheckBitValue(value,bitIndex)

例：local isOne = BitUtil.CheckBitValue(bit,idx - 1) == 1 --取第一位的值，

--[["或"操作,设置某一位的值为1.（即是 1 | 某位）
 * @param value 需要检测的数字
 * @param bitIndex 范围[0~30]
 * @return 返回"或"操作后的新值；-2：下标越位即是 bitIndex > 30了；-1：本来oldValue 的bitIndex位是1导致设置不成功，
--]]
BitUtil.GetNewBitValue(oldValue,bitIndex)

例：bit = BitUtil.GetNewBitValue(bit), idx - 1) --将第一位的值置1

## 活动规则/说明面板

按钮图名称：wenhao
UIManager:Open('CommonRuleView', '标题', '说明')