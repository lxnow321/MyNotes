module('AQ.UI.XXModule', package.seeall)

XXMainView = class('XXMainView', AQ.UI.ViewBase)
XXMainView.name = 'XXMainView'

function XXMainView:GetResourcesPath()
	local resPath = {'prefabs/ui/xxmodule', 'XXMainView'}
	return resPath
end

function XXMainView:GetViewModel()
	return AQ.ViewModel.XXModule.XXMainViewModel
end

function XXMainView:GetRoot()
	return 'POPUP'
end

function XXMainView:BuildUI()
	local go = self.gameObject
end

function XXMainView:BindValues()
	local vm = self.viewModel
end

function XXMainView:BindEvents()
	local vm = self.viewModel
end

function XXMainView:OnOpening()
end

function XXMainView:OpenFinished()
end

function XXMainView:OnClosing()
end

function XXMainView:OnDestroy()
end
