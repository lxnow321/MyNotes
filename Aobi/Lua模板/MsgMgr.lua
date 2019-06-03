module('AQ.Top', package.seeall)

MsgMgr = SingletonClass('MsgMgr')

local instance = MsgMgr
local isMsgHandled = false
local msgs = {}

instance.AddListeners = function()
    RegionNetWorkAgent:addListener(RegionNetWorkAgent.LoadRegionInfoReplyEvent,instance.AddUnionMsgQueue,"LoadRegionInfoReplyInfo")
    RegionNetWorkAgent:addListener(RegionNetWorkAgent.SetRegionReplyEvent,instance.AddUnionMsgQueue,"SetRegionReplyInfo")
end

instance.RemoveListeners = function ()
    RegionNetWorkAgent:removeListener(RegionNetWorkAgent.LoadRegionInfoReplyEvent,instance.AddUnionMsgQueue,"LoadRegionInfoReplyInfo")
    RegionNetWorkAgent:removeListener(RegionNetWorkAgent.SetRegionReplyEvent,instance.AddUnionMsgQueue,"SetRegionReplyInfo")
end

instance.AddUnionMsgQueue = function (key,data)
    table.insert(msgs, {
        key = key,
        data = data
    })
    instance.ReceviceMsgListener()
end

function MsgMgr.ReceviceMsgListener()
    print('ReceviceMsgListener', isMsgHandled)
    if not isMsgHandled then
        instance.HandleMsg()
    end
end

function MsgMgr.HandleMsg()
    local msg = table.remove(msgs, 1)
    if not msg then
        isMsgHandled = false
        return
    end
    local data = debug.getinfo(2)
    local whichFile = data.source
    local whichLine = data.currentline
    print("MsgMgr.HandleMsg",msg.key,whichFile,whichLine)
    isMsgHandled = true
    HandlerMgr.Handle(msg)
end

function MsgMgr.Init() 
	instance.AddListeners()
end

function MsgMgr.Dispose( force )
    if force then 
    	instance.RemoveListeners()
    end
    isMsgHandled = false
    msgs = {}
end

function MsgMgr.SendGetBonusEvent( msg,callback )
    if #msg == 0 then
        callback()
    else
        UIManager.getItemEntry:GetItem( msg,callback )
    end
end