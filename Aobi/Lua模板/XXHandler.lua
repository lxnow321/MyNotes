module('AQ.LightAndDark', package.seeall)

DarkIntertwineEnterGameReplyHandler = class('DarkIntertwineEnterGameReplyHandler')

function DarkIntertwineEnterGameReplyHandler:ctor()
end

function DarkIntertwineEnterGameReplyHandler:Handle(msg)
    -- LightAndDarkService:dispatch(LightAndDarkService.SucUnionDungeonGainBoxBnReplyEvent, msg)
    MsgMgr.HandleMsg()
end