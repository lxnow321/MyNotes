module('AQ.XXX', package.seeall)
-- module('AQ.PMCatch', package.seeall)

MsgMgr = SingletonClass('MsgMgr')
local instance = MsgMgr

local handlers

local Register
local HandleMsg
local AddOneListener
local AddListeners
local RemoveOneListener
local RemoveListeners

Register = function(msgType, handler)
	handlers[msgType] = handler
end

HandleMsg = function(msgType, ...)
	if handlers and handlers[msgType] then
		handlers[msgType]:Handle(...)
	end
end

AddOneListener = function(dispatcher, eventName)
	dispatcher:addListener(eventName, HandleMsg, eventName)
end

AddListeners = function()
	--AddOneListener(PMCatchNetWorkAgent, PMCatchNetWorkAgent.EVENT_PM_CATCH_JOIN_REPLY)
end

RemoveOneListener = function(dispatcher, eventName)
	dispatcher:removeListener(eventName, HandleMsg, eventName)
end

RemoveListeners = function()
	--RemoveOneListener(PMCatchNetWorkAgent, PMCatchNetWorkAgent.EVENT_PM_CATCH_JOIN_REPLY)
end

function MsgMgr.OnLogin()
	handlers = {}
    --Register(PMCatchNetWorkAgent.EVENT_PM_CATCH_JOIN_REPLY, PMCatchReplyHandler.New())
    
	AddListeners()
end

function MsgMgr.OnLogout()
	RemoveListeners()
	handlers= nil
end