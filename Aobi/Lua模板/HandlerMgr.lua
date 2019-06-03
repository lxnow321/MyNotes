module('AQ.Top',package.seeall)

HandlerMgr = SingletonClass('HandlerMgr')

local instance = HandlerMgr
instance.HandlerTable = {}

function HandlerMgr.Handle( data )
	local handler = instance.HandlerTable[data.key]
	handler:Handle(data.data)
end

function HandlerMgr.Init()
	instance.HandlerTable["LoadRegionInfoReplyInfo"] = LoadRegionInfoReplyHandler.New()
    instance.HandlerTable["SetRegionReplyInfo"] = SetRegionReplyHandler.New()
end

function HandlerMgr.Dispose()
end

function HandlerMgr.GetDirHandler( handlerInfoName )
	return instance.HandlerTable[handlerInfoName]
end