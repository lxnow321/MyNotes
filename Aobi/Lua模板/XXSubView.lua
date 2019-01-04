module('AQ.UI.XXModule',package.seeall)

XXSubView = class('XXSubView', AQ.UI.ListViewBase)
XXSubView.name = 'XXSubView'

function XXSubView:GetResourcesPath()
	local resPath = {'prefabs/ui/xxmodule', 'XXSubView'}
	return resPath
end

function XXSubView:Awake() 
	local go = self.gameObject
end

function XXSubView:BuildUI() 
    local go = self.gameObject
end

function XXSubView:BindValues()
    local vm = self.viewModel
end

function XXSubView:BindEvents()
    local vm = self.viewModel
end

function XXSubView:OnDestroy()
end

function XXSubView:OnEnable()
end

function XXSubView:OnDisable()
end