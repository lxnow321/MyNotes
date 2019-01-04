module("AQ.XX", package.seeall)

XXService = SingletonClass("XXService")
local instance = XXService

function XXService.Init()
	
end

function XXService.OnLogin()
	-- xxMgr.OnLogin()
end

function XXService.OnLogout()
	--xxMgr.Dispose()
end